using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Network;
using DevilDaggersCustomLeaderboards.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Threading;
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
			try
			{
				await ExecuteMainLoop();
			}
			catch (Win32Exception)
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
#if DEBUG
			Console.SetCursorPosition(0, 0);
			GuiUtils.WriteRecording(_scannerService.Process, _scannerService.MainBlock, _scannerService.MainBlockPrevious);
#endif
			if (_scannerService.MainBlock.Time == _scannerService.MainBlockPrevious.Time)
				return;

			Console.Clear();
			_isRecording = true;
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
				Cmd.WriteLine("Upload successful", ColorUtils.Success);
				Cmd.WriteLine(uploadSuccess.Message);
				Cmd.WriteLine();
				uploadSuccess.WriteLeaderboard(_scannerService.MainBlock.PlayerId);

				Cmd.WriteLine();

				if (uploadSuccess.IsHighscore)
					uploadSuccess.WriteHighscoreStats(_scannerService.MainBlock);
				else
					uploadSuccess.WriteStats(_scannerService.MainBlock);

				Cmd.WriteLine();
			}
			else
			{
				Thread.Sleep(500);
			}
		}
		else
		{
			Cmd.WriteLine("Validation failed", ColorUtils.Error);
			Cmd.WriteLine(errorMessage);
			_logger.LogWarning("Validation failed - {errorMessage}", errorMessage);

			Thread.Sleep(500);
		}

		Console.SetCursorPosition(0, 0);
		Cmd.WriteLine("Ready to restart", string.Empty);
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

		return null;
	}
}
