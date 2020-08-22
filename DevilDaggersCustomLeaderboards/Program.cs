using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Extensions;
using DevilDaggersCustomLeaderboards.Memory;
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
		public static Version LocalVersion { get; private set; } = Version.Parse(FileVersionInfo.GetVersionInfo(Assembly.Location).FileVersion);

		public static async Task Main()
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

			await NetworkHandler.Instance.GetOnlineTool();

			Console.Clear();
			if (NetworkHandler.Instance.Tool != null)
			{
				if (LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumberRequired))
				{
					Cmd.WriteLine($"You are using an unsupported and outdated version of {ApplicationDisplayName}. Please update the program.\n(Press any key to continue.)", ColorUtils.Error);
					Console.ReadKey();
				}
				else if (LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumber))
				{
					Cmd.WriteLine($"An update for {ApplicationDisplayName} is available.\n(Press any key to continue.)", ColorUtils.Warning);
					Console.ReadKey();
				}
			}
			else
			{
				Cmd.WriteLine("Failed to check for updates.\n(Press any key to continue.)", ColorUtils.Error);
				Console.ReadKey();
			}

			Console.Clear();
			while (true)
				await ExecuteMainLoop();
		}

		private static async Task ExecuteMainLoop()
		{
			scanner.FindWindow();

			if (scanner.Process == null)
			{
				Cmd.WriteLine($"Devil Daggers not found. Make sure the game is running. Retrying in a second...");
				Thread.Sleep(1000);
				Console.Clear();
				return;
			}

			scanner.Open();

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
					UploadSuccess? uploadSuccess = await UploadRun();

					if (uploadSuccess != null)
					{
						Cmd.WriteLine("Upload successful", ColorUtils.Success);
						Cmd.WriteLine(uploadSuccess.Message);
						Cmd.WriteLine();
						uploadSuccess.WriteLeaderboard(scanner.PlayerId);

						Cmd.WriteLine();

						if (uploadSuccess.IsHighscore())
							uploadSuccess.WriteHighscoreStats();
						else
							scanner.WriteStats(uploadSuccess.Leaderboard, uploadSuccess.Category, uploadSuccess.Entries.FirstOrDefault(e => e.PlayerId == scanner.PlayerId));

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
					Cmd.WriteLine(message);
					Log.Warn($"Validation failed - {message}");

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
				string toEncrypt = string.Join(";", scanner.PlayerId, scanner.Time, scanner.Gems, scanner.Kills, scanner.DeathType, scanner.DaggersHit, scanner.DaggersFired, scanner.EnemiesAlive, scanner.Homing, string.Join(",", new[] { scanner.LevelUpTime2, scanner.LevelUpTime3, scanner.LevelUpTime4 }));
				string validation = Secrets.EncryptionWrapper.EncryptAndEncode(toEncrypt);

				UploadRequest uploadRequest = new UploadRequest
				{
					DaggersFired = scanner.DaggersFired,
					DaggersHit = scanner.DaggersHit,
					DdclClientVersion = LocalVersion.ToString(),
					DeathType = scanner.DeathType,
					EnemiesAlive = scanner.EnemiesAlive,
					Gems = scanner.Gems,
					Homing = scanner.Homing,
					Kills = scanner.Kills,
					LevelUpTime2 = scanner.LevelUpTime2,
					LevelUpTime3 = scanner.LevelUpTime3,
					LevelUpTime4 = scanner.LevelUpTime4,
					PlayerId = scanner.PlayerId,
					SpawnsetHash = scanner.SpawnsetHash,
					Time = scanner.Time,
					Username = scanner.Username,
					Validation = HttpUtility.HtmlEncode(validation),
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