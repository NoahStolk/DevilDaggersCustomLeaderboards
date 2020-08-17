using DevilDaggersCore.CustomLeaderboards;
using DevilDaggersCore.Tools;
using DevilDaggersCore.Utils;
using DevilDaggersCustomLeaderboards.Gui;
using DevilDaggersCustomLeaderboards.Memory;
using log4net;
using log4net.Config;
using log4net.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web;
using Cmd = DevilDaggersCustomLeaderboards.Gui.ConsoleUtils;

namespace DevilDaggersCustomLeaderboards
{
	public static class Program
	{
		private const float minimalTime = 1f;

#pragma warning disable IDE1006
#pragma warning disable SA1310 // Field names should not contain underscore
		private const int MF_BYCOMMAND = 0x00000000;
		private const int SC_MINIMIZE = 0xF020;
		private const int SC_MAXIMIZE = 0xF030;
		private const int SC_SIZE = 0xF000;
#pragma warning restore IDE1006
#pragma warning restore SA1310 // Field names should not contain underscore

		private static readonly CultureInfo culture = CultureInfo.InvariantCulture;

		private static readonly Scanner scanner = Scanner.Instance;

		private static bool recording = true;

		public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? throw new Exception("Could not retrieve logger declaring type."));

		public static string ApplicationName => "DevilDaggersCustomLeaderboards";
		public static string ApplicationDisplayName => "Devil Daggers Custom Leaderboards";

		public static Assembly Assembly { get; private set; } = Assembly.GetExecutingAssembly();
		public static Version LocalVersion { get; private set; } = VersionHandler.GetLocalVersion(Assembly);

		public static void Main()
		{
			ILoggerRepository? logRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
			XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

			Console.CursorVisible = false;
			try
			{
				Console.WindowHeight = 40;
				Console.WindowWidth = 170;
			}
			catch
			{
			}

#pragma warning disable CA1806 // Do not ignore method results
			NativeMethods.DeleteMenu(NativeMethods.GetSystemMenu(NativeMethods.GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
			NativeMethods.DeleteMenu(NativeMethods.GetSystemMenu(NativeMethods.GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
			NativeMethods.DeleteMenu(NativeMethods.GetSystemMenu(NativeMethods.GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);
#pragma warning restore CA1806 // Do not ignore method results

			Console.Title = $"{ApplicationDisplayName} {LocalVersion}";

			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;

			Cmd.WriteLine("Checking for updates...");

			VersionHandler.Instance.GetOnlineVersion(ApplicationName, LocalVersion);
			VersionResult versionResult = VersionHandler.Instance.VersionResult;
			Console.Clear();
			if (versionResult.IsUpToDate.HasValue)
			{
				if (LocalVersion < versionResult.Tool.VersionNumberRequired)
				{
					Cmd.WriteLine($"You are using an unsupported and outdated version of {ApplicationDisplayName}. Please update the program.\n(Press any key to continue.)", ConsoleColor.Red);
					Console.ReadKey();
				}
				else if (LocalVersion < versionResult.Tool.VersionNumber)
				{
					Cmd.WriteLine($"An update for {ApplicationDisplayName} is available.\n(Press any key to continue.)", ConsoleColor.Yellow);
					Console.ReadKey();
				}
			}
			else
			{
				Cmd.WriteLine("Failed to check for updates.\n(Press any key to continue.)", ConsoleColor.Red);
				Console.ReadKey();
			}

			Console.Clear();
			while (true)
				ExecuteMainLoop();
		}

		private static void ExecuteMainLoop()
		{
			scanner.FindWindow();

			if (scanner.Process == null)
			{
				Cmd.WriteLine($"Devil Daggers not found. Make sure the game is running. Retrying in a second...");
				Thread.Sleep(1000);
				Console.Clear();
				return;
			}

			scanner.ProcessMemory.ReadProcess = scanner.Process;
			scanner.ProcessMemory.Open();

			scanner.PreScan();
			scanner.Scan();

			if (!recording)
			{
				if (scanner.TimeFloat == scanner.TimeFloat.ValuePrevious)
					return;

				Console.Clear();
				recording = true;
				scanner.RestartScan();
			}

			scanner.WriteRecording();

			Thread.Sleep(50);
			Console.SetCursorPosition(0, 0);

			if (!scanner.IsAlive && scanner.IsAlive.ValuePrevious)
			{
				recording = false;

				Console.Clear();
				Cmd.WriteLine("Validating...");
				Cmd.WriteLine();

				(bool isValid, string message) = ValidateRunLocally();

				if (isValid)
				{
					Console.Clear();
					Cmd.WriteLine("Uploading...");
					Cmd.WriteLine();

					// Thread is being blocked by the upload.
					UploadSuccess? uploadSuccess = Upload();

					if (uploadSuccess != null)
					{
						Cmd.WriteLine("Upload successful", ConsoleColor.Green);
						Cmd.WriteLine(uploadSuccess.Message);
						Cmd.WriteLine();
						uploadSuccess.WriteLeaderboard(scanner.PlayerId, scanner.Username);

						Cmd.WriteLine();

						if (uploadSuccess.IsHighscore())
							uploadSuccess.WriteHighscoreStats();
						else
							scanner.WriteStats(uploadSuccess.Leaderboard, uploadSuccess.Category, uploadSuccess.Entries.FirstOrDefault(e => e.PlayerId == scanner.PlayerId));

						Cmd.WriteLine();
					}
					else
					{
						Cmd.WriteLine("Upload failed", ConsoleColor.Red);

						Thread.Sleep(500);
					}
				}
				else
				{
					Cmd.WriteLine("Validation failed", ConsoleColor.Red);
					Cmd.WriteLine(message);
					Log.Warn($"Validation failed - {message}");

					Thread.Sleep(500);
				}

				Console.SetCursorPosition(0, 0);
				Cmd.WriteLine("Ready to restart");
				Cmd.WriteLine();
			}
		}

		private static UploadSuccess? Upload()
		{
			try
			{
				string toEncrypt = string.Join(
					";",
					scanner.PlayerId,
					scanner.Username,
					scanner.Time,
					scanner.Gems,
					scanner.Kills,
					scanner.DeathType,
					scanner.ShotsHit,
					scanner.ShotsFired,
					scanner.EnemiesAlive,
					scanner.Homing,
					string.Join(",", new[] { scanner.LevelUpTime2, scanner.LevelUpTime3, scanner.LevelUpTime4 }));
				string validation = Secrets.EncryptionWrapper.EncryptAndEncode(toEncrypt);

				List<string> queryValues = new List<string>
				{
					$"spawnsetHash={scanner.SpawnsetHash}",
					$"playerId={scanner.PlayerId}",
					$"username={scanner.Username}",
					$"time={scanner.Time}",
					$"gems={scanner.Gems}",
					$"kills={scanner.Kills}",
					$"deathType={scanner.DeathType}",
					$"shotsHit={scanner.ShotsHit}",
					$"shotsFired={scanner.ShotsFired}",
					$"enemiesAlive={scanner.EnemiesAlive}",
					$"homing={scanner.Homing}",
					$"levelUpTime2={scanner.LevelUpTime2}",
					$"levelUpTime3={scanner.LevelUpTime3}",
					$"levelUpTime4={scanner.LevelUpTime4}",
					$"ddclClientVersion={LocalVersion}",
					$"v={HttpUtility.HtmlEncode(validation)}",
				};

				using WebClient wc = new WebClient();
				return JsonConvert.DeserializeObject<UploadSuccess>(wc.DownloadString(UrlUtils.UploadCustomEntry(queryValues)));
			}
			catch (Exception ex)
			{
				Log.Error("Error trying to submit score", ex);
				return null;
			}
		}

		private static (bool isValid, string message) ValidateRunLocally()
		{
			if (scanner.PlayerId <= 0)
			{
				Log.Warn($"Invalid player ID: {scanner.PlayerId}");
				return (false, "Invalid player ID.");
			}

			if (scanner.IsReplay)
				return (false, "Run is replay. Unable to validate.");

			// This should fix the broken submissions that occasionally get sent for some reason.
			if (scanner.Time < minimalTime)
				return (false, $"Timer is under {minimalTime:0.0000}. Unable to validate.");

			if (string.IsNullOrEmpty(scanner.SpawnsetHash))
			{
				Log.Warn("Spawnset hash has not been calculated.");
				return (false, "Spawnset hash has not been calculated.");
			}

			// This is to prevent people from initially starting an easy spawnset to get e.g. 800 seconds, then change the survival file during the run to a different (harder) spawnset to trick the application into uploading it to the wrong leaderboard.
			if (HashUtils.CalculateCurrentSurvivalHash() != scanner.SpawnsetHash)
				return (false, "Cheats suspected. Spawnset hash has been changed since the run was started.");

			return (true, string.Empty);
		}
	}
}