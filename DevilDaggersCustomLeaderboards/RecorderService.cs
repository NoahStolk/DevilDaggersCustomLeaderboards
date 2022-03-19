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
	private readonly MemoryService _memoryService;
	private readonly UploadService _uploadService;
	private readonly ILogger<RecorderService> _logger;

	private bool _isRecording = true;
	private long _marker;
	private int _selectedIndex;
	private int _pageIndex;
	private GetUploadSuccess? _uploadSuccess;
	private MainBlock _finalRecordedMainBlock;

	public RecorderService(NetworkService networkService, MemoryService memoryService, UploadService uploadService, ILogger<RecorderService> logger)
	{
		_networkService = networkService;
		_memoryService = memoryService;
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
		_memoryService.FindWindow();
		if (_memoryService.Process == null)
		{
			_memoryService.IsInitialized = false;
			Cmd.WriteLine("Devil Daggers not found. Make sure the game is running. Retrying in a second...");
			await Task.Delay(TimeSpan.FromSeconds(1));
			Console.Clear();
			return;
		}

		_memoryService.Initialize(_marker);
		if (!_memoryService.IsInitialized)
		{
			Cmd.WriteLine("Could not find memory block starting address. Retrying in a second...");
			await Task.Delay(TimeSpan.FromSeconds(1));
			Console.Clear();
			return;
		}

		_memoryService.Scan();

		if (!_isRecording)
		{
			if (_memoryService.MainBlock.Time == _memoryService.MainBlockPrevious.Time || _memoryService.MainBlock.Status == (int)GameStatus.LocalReplay)
				return;

			Console.Clear();
			_isRecording = true;
			_uploadSuccess = null;
		}

		GuiUtils.WriteRecording(_memoryService.Process, _memoryService.MainBlock, _memoryService.MainBlockPrevious);

		await Task.Delay(TimeSpan.FromMilliseconds(_mainLoopSleepMilliseconds));
		Console.SetCursorPosition(0, 0);

		bool justDied = !_memoryService.MainBlock.IsPlayerAlive && _memoryService.MainBlockPrevious.IsPlayerAlive;
		bool uploadRun = justDied && (_memoryService.MainBlock.GameMode == 0 || _memoryService.MainBlock.TimeAttackOrRaceFinished);
		if (!uploadRun)
			return;

		if (!_memoryService.MainBlock.StatsLoaded)
		{
			Console.Clear();
			Cmd.WriteLine("Waiting for stats to be loaded...");

			await Task.Delay(TimeSpan.FromSeconds(0.5));
			return;
		}

		if (_memoryService.MainBlock.ReplayLength <= 0)
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
				_finalRecordedMainBlock = _memoryService.MainBlock;
				_selectedIndex = 0;
				_pageIndex = 0;
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

		_uploadSuccess.WriteLeaderboard(_memoryService.MainBlock.PlayerId, _selectedIndex, _pageIndex);

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

		if (_memoryService.MainBlock.PlayerId <= 0)
		{
			_logger.LogWarning("Invalid player ID: {playerId}", _memoryService.MainBlock.PlayerId);
			return "Invalid player ID.";
		}

		if (_memoryService.MainBlock.Time < minimalTime)
			return $"Timer is under {minimalTime:0.0000}. Unable to validate.";

		if (_memoryService.MainBlock.Status == (int)GameStatus.LocalReplay)
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
					_memoryService.WriteReplayToMemory(replay);
				break;
			case ConsoleKey.UpArrow:
				ChangeSelection(_selectedIndex - 1);
				break;
			case ConsoleKey.DownArrow:
				ChangeSelection(_selectedIndex + 1);
				break;
			case ConsoleKey.LeftArrow:
				ChangeSelection(_selectedIndex - GuiUtils.PageSize);
				break;
			case ConsoleKey.RightArrow:
				ChangeSelection(_selectedIndex + GuiUtils.PageSize);
				RenderSuccessfulSubmit();
				break;
		}

		void ChangeSelection(int newIndex)
		{
			_selectedIndex = Math.Clamp(newIndex, 0, customEntryIds.Count - 1);
			_pageIndex = _selectedIndex / GuiUtils.PageSize;
			RenderSuccessfulSubmit();
		}
	}
}
