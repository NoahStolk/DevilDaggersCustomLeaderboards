using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Enums;
using DevilDaggersCustomLeaderboards.Memory;
using DevilDaggersCustomLeaderboards.Network;
using DevilDaggersCustomLeaderboards.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace DevilDaggersCustomLeaderboards;

public class RecorderService
{
	private const int _mainLoopSleepMilliseconds = 50;

	private readonly NetworkService _networkService;
	private readonly ScannerService _scannerService;
	private readonly UploadService _uploadService;
	private readonly ILogger<RecorderService> _logger;

	private bool _isRecording = true;
	private long _marker;
	private int _selectedIndex;
	private GetUploadSuccess? _uploadSuccess;
	private MainBlock _finalRecordedMainBlock;

	public RecorderService(NetworkService networkService, ScannerService scannerService, UploadService uploadService, ILogger<RecorderService> logger)
	{
		_networkService = networkService;
		_scannerService = scannerService;
		_uploadService = uploadService;
		_logger = logger;
	}

	public async Task Record()
	{
		_marker = await _networkService.GetMarker();

		Console.Clear();
		while (true)
		{
			await HandleInput();

			try
			{
				await ExecuteMainLoop();
			}
			catch (Win32Exception) // TODO: Refactor and wrap around native code only.
			{
				// Ignore exceptions when Devil Daggers is closed.
			}
		}
	}

	private async Task ExecuteMainLoop()
	{
		_scannerService.FindWindow();
		if (_scannerService.Process == null)
		{
			_scannerService.IsInitialized = false;
			Cmd.WriteLine("Devil Daggers not found. Make sure the game is running. Retrying in a second...");
			await Task.Delay(TimeSpan.FromSeconds(1));
			Console.Clear();
			return;
		}

		_scannerService.Initialize(_marker);
		if (!_scannerService.IsInitialized)
		{
			Cmd.WriteLine("Could not find memory block starting address. Retrying in a second...");
			await Task.Delay(TimeSpan.FromSeconds(1));
			Console.Clear();
			return;
		}

		_scannerService.Scan();

		if (!_isRecording)
		{
			if (_scannerService.MainBlock.Time == _scannerService.MainBlockPrevious.Time || _scannerService.MainBlock.Status == (int)GameStatus.LocalReplay)
				return;

			Console.Clear();
			_isRecording = true;
			_uploadSuccess = null;
		}

		GuiUtils.WriteRecording(_scannerService.Process, _scannerService.MainBlock, _scannerService.MainBlockPrevious);

		await Task.Delay(TimeSpan.FromMilliseconds(_mainLoopSleepMilliseconds));
		Console.SetCursorPosition(0, 0);

		bool justDied = !_scannerService.MainBlock.IsPlayerAlive && _scannerService.MainBlockPrevious.IsPlayerAlive;
		bool uploadRun = justDied && (_scannerService.MainBlock.GameMode == 0 || _scannerService.MainBlock.TimeAttackOrRaceFinished);
		if (!uploadRun)
			return;

		if (!_scannerService.MainBlock.StatsLoaded)
		{
			Console.Clear();
			Cmd.WriteLine("Waiting for stats to be loaded...");

			await Task.Delay(TimeSpan.FromSeconds(0.5));
			return;
		}

		if (_scannerService.MainBlock.ReplayLength <= 0)
		{
			Console.Clear();
			Cmd.WriteLine("Waiting for replay to be loaded...");

			await Task.Delay(TimeSpan.FromSeconds(0.5));
			return;
		}

		_isRecording = false;

		Console.Clear();
		Cmd.WriteLine("Validating...");
		Cmd.WriteLine();

		string? errorMessage = ValidateRunLocally();
		if (errorMessage == null)
		{
			GetUploadSuccess? uploadSuccess = await _uploadService.UploadRun();
			if (uploadSuccess != null)
			{
				_uploadSuccess = uploadSuccess;
				_finalRecordedMainBlock = _scannerService.MainBlock;
				RenderSuccessfulSubmit();
			}
			else
			{
				await Task.Delay(TimeSpan.FromSeconds(0.5));
			}
		}
		else
		{
			Cmd.WriteLine("Validation failed", ColorUtils.Error);
			Cmd.WriteLine(errorMessage);
			_logger.LogWarning("Validation failed - {errorMessage}", errorMessage);

			await Task.Delay(TimeSpan.FromSeconds(0.5));
		}

		Console.SetCursorPosition(0, 0);
		Cmd.WriteLine("Ready to restart", string.Empty);
		Cmd.WriteLine();
	}

	private void RenderSuccessfulSubmit()
	{
		if (_uploadSuccess == null)
			return;

		Console.SetCursorPosition(0, 2);

		Cmd.WriteLine("Upload successful", ColorUtils.Success);
		Cmd.WriteLine(_uploadSuccess.Message);
		Cmd.WriteLine();
		Cmd.WriteLine("Use the arrow keys to navigate. Press [Enter] to load selected replay into Devil Daggers.");
		Cmd.WriteLine();

		_uploadSuccess.WriteLeaderboard(_scannerService.MainBlock.PlayerId, _selectedIndex);

		Cmd.WriteLine();

		if (_uploadSuccess.IsHighscore)
			_uploadSuccess.WriteHighscoreStats(_finalRecordedMainBlock);
		else
			_uploadSuccess.WriteStats(_finalRecordedMainBlock);

		Cmd.WriteLine();
	}

	private string? ValidateRunLocally()
	{
		const float minimalTime = 1f;

		if (_scannerService.MainBlock.PlayerId <= 0)
		{
			_logger.LogWarning("Invalid player ID: {playerId}", _scannerService.MainBlock.PlayerId);
			return "Invalid player ID.";
		}

		if (_scannerService.MainBlock.Time < minimalTime)
			return $"Timer is under {minimalTime:0.0000}. Unable to validate.";

		if (_scannerService.MainBlock.Status == (int)GameStatus.LocalReplay)
			return "Local replays are not uploaded.";

		return null;
	}

	private async Task HandleInput()
	{
		if (!Console.KeyAvailable || _uploadSuccess == null)
			return;

		List<int> customEntryIds = _uploadSuccess.Entries.ConvertAll(e => e.Id);
		switch (Console.ReadKey(true).Key)
		{
			case ConsoleKey.Enter:
				byte[]? replay = await _networkService.GetReplay(customEntryIds[Math.Clamp(_selectedIndex, 0, customEntryIds.Count - 1)]);
				if (replay != null)
					_scannerService.WriteReplayToMemory(replay);
				break;
			case ConsoleKey.UpArrow:
				_selectedIndex = Math.Max(0, _selectedIndex - 1);
				RenderSuccessfulSubmit();
				break;
			case ConsoleKey.DownArrow:
				_selectedIndex = Math.Min(customEntryIds.Count - 1, _selectedIndex + 1);
				RenderSuccessfulSubmit();
				break;
		}
	}
}
