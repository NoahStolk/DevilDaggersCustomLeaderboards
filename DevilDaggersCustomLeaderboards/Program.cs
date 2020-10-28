using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Extensions;
using DevilDaggersCustomLeaderboards.Memory;
using DevilDaggersCustomLeaderboards.Native;
using DevilDaggersCustomLeaderboards.Network;
using DevilDaggersCustomLeaderboards.Utils;
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Cmd = DevilDaggersCustomLeaderboards.Utils.ConsoleUtils;

namespace DevilDaggersCustomLeaderboards
{
	public static class Program
	{
		private const float _minimalTime = 1f;

#pragma warning disable IDE1006
#pragma warning disable SA1310 // Field names should not contain underscore
		private const int MF_BYCOMMAND = 0x00000000;
		private const int SC_MINIMIZE = 0xF020;
		private const int SC_MAXIMIZE = 0xF030;
		private const int SC_SIZE = 0xF000;
#pragma warning restore IDE1006
#pragma warning restore SA1310 // Field names should not contain underscore

		private static readonly Scanner _scanner = Scanner.Instance;

		private static bool _isRecording = true;

		public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? throw new Exception("Could not retrieve logger declaring type."));

		public static string ApplicationName => "DevilDaggersCustomLeaderboards";
		public static string ApplicationDisplayName => "Devil Daggers Custom Leaderboards";

		public static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
		public static Version LocalVersion { get; } = Version.Parse(FileVersionInfo.GetVersionInfo(Assembly.Location).FileVersion);

		public static async Task Main()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			ILoggerRepository? logRepository = LogManager.GetRepository(Assembly);
			XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

			InitializeConsole();

			Cmd.WriteLine("Checking for updates...");

			await NetworkHandler.Instance.GetOnlineTool();

			Console.Clear();
			if (NetworkHandler.Instance.Tool != null)
			{
				if (LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumberRequired))
				{
					Cmd.WriteLine($"You are using an unsupported and outdated version of {ApplicationDisplayName} ({LocalVersion}).\n\nYou must use version {NetworkHandler.Instance.Tool.VersionNumberRequired} or higher.\n\nPlease update the program.\n\n(Press any key to continue.)", ColorUtils.Error);
					Console.ReadKey();
				}
				else if (LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumber))
				{
					Cmd.WriteLine($"{ApplicationDisplayName} version {NetworkHandler.Instance.Tool.VersionNumber} is available.\n\n(Press any key to continue.)", ColorUtils.Warning);
					Console.ReadKey();
				}
			}
			else
			{
				Cmd.WriteLine("Failed to check for updates.\n\n(Press any key to continue.)", ColorUtils.Error);
				Console.ReadKey();
			}

			Console.Clear();
			while (true)
				await ExecuteMainLoop();
		}

		private static void InitializeConsole()
		{
			Console.CursorVisible = false;
			try
			{
				Console.WindowHeight = 40;
				Console.WindowWidth = 170;
			}
			catch
			{
				// Do nothing if resizing the console failed. It usually means a very large custom font caused the window to be too large which throws an exception.
			}

			if (OperatingSystemUtils.IsWindows())
			{
#pragma warning disable CA1806 // Do not ignore method results
				NativeMethods.DeleteMenu(NativeMethods.GetSystemMenu(NativeMethods.GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
				NativeMethods.DeleteMenu(NativeMethods.GetSystemMenu(NativeMethods.GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
				NativeMethods.DeleteMenu(NativeMethods.GetSystemMenu(NativeMethods.GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);
#pragma warning restore CA1806 // Do not ignore method results

				ColorUtils.ModifyConsoleColor(2, 0x47, 0x8B, 0x41);
				ColorUtils.ModifyConsoleColor(3, 0xCD, 0x7F, 0x32);
				ColorUtils.ModifyConsoleColor(4, 0x77, 0x1D, 0x00);
				ColorUtils.ModifyConsoleColor(5, 0xAF, 0x6B, 0x00);
				ColorUtils.ModifyConsoleColor(6, 0x97, 0x6E, 0x2E);
				ColorUtils.ModifyConsoleColor(7, 0xDD, 0xDD, 0xDD);
				ColorUtils.ModifyConsoleColor(14, 0xFF, 0xDF, 0x00);
			}

#if DEBUG
			Console.Title = $"{ApplicationDisplayName} {LocalVersion} DEBUG";
#else
			Console.Title = $"{ApplicationDisplayName} {LocalVersion}";
#endif
		}

		private static async Task ExecuteMainLoop()
		{
			_scanner.FindWindow();

			if (_scanner.Process == null)
			{
				Cmd.WriteLine($"Devil Daggers not found. Make sure the game is running. Retrying in a second...");
				Thread.Sleep(1000);
				Console.Clear();
				return;
			}

			_scanner.Open();

			_scanner.PreScan();
			_scanner.Scan();

			if (!_isRecording)
			{
				if (_scanner.TimeFloat == _scanner.TimeFloat.ValuePrevious)
					return;

				Console.Clear();
				_isRecording = true;
				_scanner.RestartScan();
			}

			if (_scanner.IsInLobby())
			{
				Console.Clear();
				Cmd.WriteLine("Currently in lobby...");
			}
			else if (_scanner.IsInMenu())
			{
				Console.Clear();
				Cmd.WriteLine("Currently in menu...");
			}
			else
			{
				_scanner.WriteRecording();
			}

			Thread.Sleep(50);
			Console.SetCursorPosition(0, 0);

			if (!_scanner.IsAlive && _scanner.IsAlive.ValuePrevious)
			{
				_isRecording = false;

				Console.Clear();
				Cmd.WriteLine("Validating...");
				Cmd.WriteLine();

				string? errorMessage = ValidateRunLocally();
				if (errorMessage == null)
				{
					Console.Clear();
					Cmd.WriteLine("Uploading...");
					Cmd.WriteLine();

					// Thread is being blocked by the upload.
					UploadSuccess? uploadSuccess = await UploadRun();

					if (uploadSuccess != null)
					{
						Cmd.WriteLine("Upload successful", ColorUtils.Success);
						Cmd.WriteLine(uploadSuccess.Message);
						Cmd.WriteLine();
						uploadSuccess.WriteLeaderboard(_scanner.PlayerId);

						Cmd.WriteLine();

						if (uploadSuccess.IsHighscore())
							uploadSuccess.WriteHighscoreStats();
						else
							_scanner.WriteStats(uploadSuccess.Leaderboard, uploadSuccess.Entries.FirstOrDefault(e => e.PlayerId == _scanner.PlayerId));

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
					Log.Warn($"Validation failed - {errorMessage}");

					Thread.Sleep(500);
				}

				Console.SetCursorPosition(0, 0);
				Cmd.WriteLine("Ready to restart");
				Cmd.WriteLine();
			}
		}

		private static async Task<UploadSuccess?> UploadRun()
		{
			try
			{
				string toEncrypt = string.Join(";", _scanner.PlayerId, _scanner.Time, _scanner.Gems, _scanner.Kills, _scanner.DeathType, _scanner.DaggersHit, _scanner.DaggersFired, _scanner.EnemiesAlive, _scanner.Homing, string.Join(",", new[] { _scanner.LevelUpTime2, _scanner.LevelUpTime3, _scanner.LevelUpTime4 }));
				string validation = Secrets.EncryptionWrapper.EncryptAndEncode(toEncrypt);

				UploadRequest uploadRequest = new UploadRequest
				{
					DaggersFired = _scanner.DaggersFired,
					DaggersHit = _scanner.DaggersHit,
					ClientVersion = LocalVersion.ToString(),
					DeathType = _scanner.DeathType,
					EnemiesAlive = _scanner.EnemiesAlive,
					Gems = _scanner.Gems,
					Homing = _scanner.Homing,
					Kills = _scanner.Kills,
					LevelUpTime2 = _scanner.LevelUpTime2,
					LevelUpTime3 = _scanner.LevelUpTime3,
					LevelUpTime4 = _scanner.LevelUpTime4,
					PlayerId = _scanner.PlayerId,
					SpawnsetHash = _scanner.SpawnsetHash,
					Time = _scanner.Time,
					Username = _scanner.Username,
					Validation = HttpUtility.HtmlEncode(validation),
					GameStates = _scanner.GameStates,
#if DEBUG
					BuildMode = BuildMode.Debug,
#else
					BuildMode = BuildMode.Release,
#endif
					OperatingSystem = Clients.OperatingSystem.Windows,
				};

				return await NetworkHandler.Instance.ApiClient.CustomLeaderboards_UploadScoreAsync(uploadRequest);
			}
			catch (DevilDaggersInfoApiException<ProblemDetails> ex)
			{
				Cmd.WriteLine("Upload failed", ex.Result?.Title ?? "Empty response", ColorUtils.Error);
				return null;
			}
			catch (Exception ex)
			{
				Cmd.WriteLine("Upload failed", ex.Message, ColorUtils.Error);
				Log.Error("Error trying to submit score", ex);
				return null;
			}
		}

		private static string? ValidateRunLocally()
		{
			if (_scanner.PlayerId <= 0)
			{
				Log.Warn($"Invalid player ID: {_scanner.PlayerId}");
				return "Invalid player ID.";
			}

			if (_scanner.IsReplay)
				return "Run is replay. Unable to validate.";

			// This should fix the broken submissions that occasionally get sent for some reason.
			if (_scanner.Time < _minimalTime)
				return $"Timer is under {_minimalTime:0.0000}. Unable to validate.";

			if (string.IsNullOrEmpty(_scanner.SpawnsetHash))
			{
				Log.Warn("Spawnset hash has not been calculated.");
				return "Spawnset hash has not been calculated.";
			}

			// This is to prevent people from initially starting an easy spawnset to get e.g. 800 seconds, then change the survival file during the run to a different (harder) spawnset to trick the application into uploading it to the wrong leaderboard.
			if (HashUtils.CalculateCurrentSurvivalHash() != _scanner.SpawnsetHash)
				return "Cheats suspected. Spawnset hash has been changed since the run was started.";

			return null;
		}
	}
}