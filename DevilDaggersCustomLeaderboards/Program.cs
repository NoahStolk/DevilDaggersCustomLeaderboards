using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Memory;
using DevilDaggersCustomLeaderboards.Native;
using DevilDaggersCustomLeaderboards.Network;
using DevilDaggersCustomLeaderboards.Utils;
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Cmd = DevilDaggersCustomLeaderboards.Utils.ConsoleUtils;

namespace DevilDaggersCustomLeaderboards
{
	public static class Program
	{
		private const int _mainLoopSleepMilliseconds = 50;

		/// <summary>
		/// Only accepts submissions of at least one second.
		/// </summary>
		private const float _minimalTime = 1f;

#pragma warning disable IDE1006, SA1310 // Field names should not contain underscore
		private const int MF_BYCOMMAND = 0x00000000;
		private const int SC_MINIMIZE = 0xF020;
		private const int SC_MAXIMIZE = 0xF030;
		private const int SC_SIZE = 0xF000;
#pragma warning restore IDE1006, SA1310 // Field names should not contain underscore

		private static bool _isRecording = true;

		public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? throw new("Could not retrieve logger declaring type."));

		private static long _marker;

		public static string ApplicationName => "DevilDaggersCustomLeaderboards";
		public static string ApplicationDisplayName => "Devil Daggers Custom Leaderboards";

		public static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
		public static Version LocalVersion { get; } = Version.Parse(FileVersionInfo.GetVersionInfo(Assembly.Location).FileVersion ?? throw new("Could not get file version from current assembly."));

		public static async Task Main()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			ILoggerRepository? logRepository = LogManager.GetRepository(Assembly);
			XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

			InitializeConsole();

			Cmd.WriteLine("Checking for updates...");
			Tool tool = await NetworkHandler.Instance.ApiClient.Tools_GetToolAsync(ApplicationName);
			Console.Clear();

			if (tool != null)
			{
				if (LocalVersion < Version.Parse(tool.VersionNumberRequired))
				{
					Cmd.WriteLine($"You are using an unsupported and outdated version of {ApplicationDisplayName} ({LocalVersion}).\n\nYou must use version {tool.VersionNumberRequired} or higher.\n\nPlease update the program.\n\n(Press any key to continue.)", ColorUtils.Error);
					Console.ReadKey();
				}
				else if (LocalVersion < Version.Parse(tool.VersionNumber))
				{
					Cmd.WriteLine($"{ApplicationDisplayName} version {tool.VersionNumber} is available.\n\n(Press any key to continue.)", ColorUtils.Warning);
					Console.ReadKey();
				}
			}
			else
			{
				Cmd.WriteLine($"Failed to check for updates (host: {NetworkHandler.BaseUrl}).\n\n(Press any key to continue.)", ColorUtils.Error);
				Console.ReadKey();
			}

			Cmd.WriteLine("Retrieving marker...");
			Marker marker = await NetworkHandler.Instance.ApiClient.ProcessMemory_GetMarkerAsync(OperatingSystemUtils.OperatingSystem);
			_marker = marker.Value;
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

		private static void InitializeConsole()
		{
			Console.CursorVisible = false;

			if (OperatingSystemUtils.IsWindows)
			{
				try
				{
#pragma warning disable CA1416 // Validate platform compatibility
#if DEBUG
					Console.WindowHeight = 80;
#else
					Console.WindowHeight = 60;
#endif
					Console.WindowWidth = 170;
#pragma warning restore CA1416 // Validate platform compatibility
				}
				catch
				{
					// Do nothing if resizing the console failed. It usually means a very large custom font caused the window to be too large which throws an exception.
				}

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
				ColorUtils.ModifyConsoleColor(9, 0xC8, 0xA2, 0xC8);
				ColorUtils.ModifyConsoleColor(11, 0x80, 0x06, 0x00);
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
			Scanner.FindWindow();

			if (Scanner.Process == null)
			{
				Scanner.IsInitialized = false;
				Cmd.WriteLine("Devil Daggers not found. Make sure the game is running. Retrying in a second...");
				Thread.Sleep(1000);
				Console.Clear();
				return;
			}

			Scanner.Initialize(_marker);
			if (!Scanner.IsInitialized)
			{
				Cmd.WriteLine("Could not find memory block starting address. Retrying in a second...");
				Thread.Sleep(1000);
				Console.Clear();
				return;
			}

			Scanner.Scan();

			if (!_isRecording)
			{
#if DEBUG
				Console.SetCursorPosition(0, 0);
				GuiUtils.WriteRecording();
#endif
				if (Scanner.Time == Scanner.Time.ValuePrevious)
					return;

				Console.Clear();
				_isRecording = true;
			}

			GuiUtils.WriteRecording();

			Thread.Sleep(_mainLoopSleepMilliseconds);
			Cmd.Clear();

			if (!Scanner.IsPlayerAlive && Scanner.IsPlayerAlive.ValuePrevious)
			{
				Console.Clear();
				Cmd.WriteLine("Waiting for stats to be loaded...");

				while (!Scanner.StatsLoaded)
				{
					Scanner.StatsLoaded.Scan();
					Thread.Sleep(500);
				}

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
						uploadSuccess.WriteLeaderboard(Scanner.PlayerId);

						Cmd.WriteLine();

						if (uploadSuccess.IsHighscore())
							uploadSuccess.WriteHighscoreStats();
						else
							GuiUtils.WriteStats(uploadSuccess.Leaderboard, uploadSuccess.Entries.Find(e => e.PlayerId == Scanner.PlayerId));

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

				Cmd.Clear();
				Cmd.WriteLine("Ready to restart");
				Cmd.WriteLine();
			}
		}

		private static async Task<UploadSuccess?> UploadRun()
		{
			try
			{
				string toEncrypt = string.Join(
					";",
					Scanner.PlayerId,
					Scanner.Time.ConvertToTimeInt(),
					Scanner.GemsCollected,
					Scanner.GemsDespawned,
					Scanner.GemsEaten,
					Scanner.GemsTotal,
					Scanner.EnemiesKilled,
					Scanner.DeathType,
					Scanner.DaggersHit,
					Scanner.DaggersFired,
					Scanner.EnemiesAlive,
					Scanner.HomingDaggers,
					Scanner.HomingDaggersEaten,
					Scanner.IsReplay ? 1 : 0,
					HashUtils.ByteArrayToHexString(Scanner.SurvivalHashMd5),
					string.Join(",", new[] { Scanner.LevelUpTime2.ConvertToTimeInt(), Scanner.LevelUpTime3.ConvertToTimeInt(), Scanner.LevelUpTime4.ConvertToTimeInt() }));
				string validation = Secrets.EncryptionWrapper.EncryptAndEncode(toEncrypt);

				UploadRequest uploadRequest = new()
				{
					DaggersFired = Scanner.DaggersFired,
					DaggersHit = Scanner.DaggersHit,
					ClientVersion = LocalVersion.ToString(),
					DeathType = Scanner.DeathType,
					EnemiesAlive = Scanner.EnemiesAlive,
					GemsCollected = Scanner.GemsCollected,
					GemsDespawned = Scanner.GemsDespawned,
					GemsEaten = Scanner.GemsEaten,
					GemsTotal = Scanner.GemsTotal,
					HomingDaggers = Scanner.HomingDaggers,
					HomingDaggersEaten = Scanner.HomingDaggersEaten,
					EnemiesKilled = Scanner.EnemiesKilled,
					LevelUpTime2 = Scanner.LevelUpTime2.ConvertToTimeInt(),
					LevelUpTime3 = Scanner.LevelUpTime3.ConvertToTimeInt(),
					LevelUpTime4 = Scanner.LevelUpTime4.ConvertToTimeInt(),
					PlayerId = Scanner.PlayerId,
					SurvivalHashMd5 = Scanner.SurvivalHashMd5,
					Time = Scanner.Time.ConvertToTimeInt(),
					PlayerName = Scanner.PlayerName,
					IsReplay = Scanner.IsReplay,
					Validation = HttpUtility.HtmlEncode(validation),
					GameStates = Scanner.GetGameStates(),
#if DEBUG
					BuildMode = BuildMode.Debug,
#else
					BuildMode = BuildMode.Release,
#endif
					OperatingSystem = OperatingSystemUtils.OperatingSystem,
					ProhibitedMods = Scanner.ProhibitedMods,
				};

				return await NetworkHandler.Instance.ApiClient.CustomEntries_SubmitScoreAsync(uploadRequest);
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
			if (Scanner.PlayerId <= 0)
			{
				Log.Warn($"Invalid player ID: {Scanner.PlayerId}");
				return "Invalid player ID.";
			}

			if (Scanner.Time < _minimalTime)
				return $"Timer is under {_minimalTime:0.0000}. Unable to validate.";

			return null;
		}
	}
}
