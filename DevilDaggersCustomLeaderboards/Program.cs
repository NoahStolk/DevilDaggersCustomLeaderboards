using DevilDaggersCore.CustomLeaderboards;
using DevilDaggersCore.Game;
using DevilDaggersCore.MemoryHandling;
using DevilDaggersCore.Tools;
using log4net;
using log4net.Config;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace DevilDaggersCustomLeaderboards
{
	/// <summary>
	/// Handles the main program and GUI-related tasks.
	/// Special Write methods are used to output to the console, as clearing the console after every update makes everything flicker which is ugly.
	/// So instead of clearing the console using Console.Clear(), we just reset the cursor to the top-left, and then overwrite everything from the previous update using the special Write methods.
	/// </summary>
	public static class Program
	{
		public static string ApplicationName => "DevilDaggersCustomLeaderboards";
		public static string ApplicationDisplayName => "Devil Daggers Custom Leaderboards";

		private const int textWidth = 70;
		private const int textWidthLeft = 20;
		private const int textWidthRight = 20;

		private static readonly CultureInfo culture = CultureInfo.InvariantCulture;

#pragma warning disable IDE1006
		private const int MF_BYCOMMAND = 0x00000000;
		private const int SC_MINIMIZE = 0xF020;
		private const int SC_MAXIMIZE = 0xF030;
		private const int SC_SIZE = 0xF000;
#pragma warning restore IDE1006

		[DllImport("user32.dll")]
		public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

		[DllImport("user32.dll")]
		private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("kernel32.dll", ExactSpelling = true)]
		private static extern IntPtr GetConsoleWindow();

		private static readonly Scanner scanner = Scanner.Instance;
		private static bool recording = true;

		public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static Assembly Assembly { get; private set; }
		public static Version LocalVersion { get; private set; }

		public static void Main()
		{
			XmlConfigurator.Configure();

			Console.CursorVisible = false;
			Console.WindowHeight = 40;

			DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
			DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
			DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);

			Assembly = Assembly.GetExecutingAssembly();
			LocalVersion = VersionHandler.GetLocalVersion(Assembly);

			Console.Title = $"{ApplicationDisplayName} {LocalVersion}";

			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;

			Write("Checking for updates...");

			VersionHandler.Instance.GetOnlineVersion(ApplicationName, LocalVersion);
			VersionResult versionResult = VersionHandler.Instance.VersionResult;
			Console.Clear();
			if (versionResult.IsUpToDate.HasValue)
			{
				if (LocalVersion < versionResult.Tool.VersionNumberRequired)
				{
					Write($"You are using an unsupported and outdated version of {ApplicationDisplayName}. Please update the program.\n(Press any key to continue.)", ConsoleColor.Red);
					Console.ReadKey();
				}
				else if (LocalVersion < versionResult.Tool.VersionNumber)
				{
					Write($"An update for {ApplicationDisplayName} is available.\n(Press any key to continue.)", ConsoleColor.Yellow);
					Console.ReadKey();
				}
			}
			else
			{
				Write("Failed to check for updates.\n(Press any key to continue.)", ConsoleColor.Red);
				Console.ReadKey();
			}

			Console.Clear();
			for (; ; )
			{
				scanner.FindWindow();

				if (scanner.Process == null)
				{
					Write($"Devil Daggers not found. Make sure the game is running. Retrying in a second...");
					Thread.Sleep(1000);
					Console.Clear();
					continue;
				}

				scanner.Memory.ReadProcess = scanner.Process;
				scanner.Memory.Open();

				scanner.PreScan();
				scanner.Scan();

				if (recording)
				{
					Write($"Scanning process '{scanner.Process.ProcessName}' ({scanner.Process.MainWindowTitle})");
					Write("Recording...");
					Write();

					Write("Spawnset Hash", scanner.SpawnsetHash);

					Write("Player ID", scanner.PlayerId.Value.ToString());
					Write("Username", scanner.Username.Value);
					Write();

					Write("Time", scanner.Time.Value.ToString("0.0000"));
					Write("Gems", scanner.Gems.Value.ToString());
					Write("Kills", scanner.Kills.Value.ToString());
					Write("Shots Hit", scanner.ShotsHit.Value.ToString());
					Write("Shots Fired", scanner.ShotsFired.Value.ToString());
					Write("Accuracy", $"{(scanner.ShotsFired.Value == 0 ? 0 : scanner.ShotsHit.Value / (float)scanner.ShotsFired.Value * 100):0.00}%");
					Write("Enemies Alive", scanner.EnemiesAlive.Value.ToString());
					Write("Death Type", GameInfo.GetDeathFromDeathType(scanner.DeathType.Value).Name);
					Write("Alive", scanner.IsAlive.Value.ToString());
					Write("Replay", scanner.IsReplay.Value.ToString());
					Write();

					Write("Hand", GetHand(scanner.LevelGems).ToString());
					int GetHand(int levelGems)
					{
						if (levelGems < 10)
							return 1;
						if (levelGems < 70)
							return 2;
						if (levelGems == 70)
							return 3;
						return 4;
					}

					Write("Homing", $"{scanner.Homing} - ({string.Join(", ", scanner.HomingLog)})");
					Write();

					Write("Level 2", scanner.LevelUpTimes[0].ToString("0.0000"));
					Write("Level 3", scanner.LevelUpTimes[1].ToString("0.0000"));
					Write("Level 4", scanner.LevelUpTimes[2].ToString("0.0000"));
					Write();

					Thread.Sleep(50);
					Console.SetCursorPosition(0, 0);

					// If player just died
					if (!scanner.IsAlive.Value && scanner.IsAlive.ValuePrevious)
					{
						scanner.PrepareUpload();
						recording = false;

						int tries = 0;
						UploadResult jsonResult;
						do
						{
							Console.Clear();
							Write("Uploading...");
							Write();
							jsonResult = NetworkHandler.Instance.Upload();
							// Thread is being blocked by the upload.

							if (jsonResult.Success)
							{
								Write("Upload successful", ConsoleColor.Green);
								Write(jsonResult.Message);
								Write();

								Write("Username", scanner.Username.Value);
								Write("Time", scanner.Time.Value.ToString("0.0000"));
								Write("Kills", scanner.Kills.Value.ToString());
								Write("Gems", scanner.Gems.Value.ToString());
								Write("Accuracy", $"{(scanner.ShotsFired.Value == 0 ? 0 : scanner.ShotsHit.Value / (float)scanner.ShotsFired.Value):0.00%} ({scanner.ShotsHit.Value} / {scanner.ShotsFired.Value})");
								Write("Death Type", GameInfo.GetDeathFromDeathType(scanner.DeathType.Value).Name);
								Write("Enemies Alive", scanner.EnemiesAlive.Value.ToString());
								Write("Homing", scanner.Homing.ToString());
								Write("Level 2", scanner.LevelUpTimes[0] == 0 ? "N/A" : scanner.LevelUpTimes[0].ToString("0.0000"));
								Write("Level 3", scanner.LevelUpTimes[1] == 0 ? "N/A" : scanner.LevelUpTimes[1].ToString("0.0000"));
								Write("Level 4", scanner.LevelUpTimes[2] == 0 ? "N/A" : scanner.LevelUpTimes[2].ToString("0.0000"));
							}
							else
							{
								Write("Upload failed", ConsoleColor.Red);
								Write(jsonResult.Message);
								tries++;
								if (jsonResult.TryCount > 1)
									Write($"Retrying (attempt {tries} / {jsonResult.TryCount})");
								Log.Warn($"Upload failed - {jsonResult.Message}");

								Thread.Sleep(500);
							}
						}
						while (!jsonResult.Success && tries < jsonResult.TryCount);

						Console.SetCursorPosition(0, 0);
						Write("Ready to restart");
						Write();
					}
				}
				else if (scanner.Time.Value < scanner.Time.ValuePrevious)
				{
					Console.Clear();
					recording = true;
					scanner.RestartScan();
				}
			}
		}

		private static void Write()
		{
			Console.WriteLine(new string(' ', textWidth));
		}

		private static void Write(string text, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(text.PadRight(textWidth));
			Console.ForegroundColor = ConsoleColor.White;
		}

		private static void Write(string textLeft, string textRight, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine($"{textLeft,textWidthLeft}{textRight,textWidthRight}{new string(' ', textWidth - textWidthLeft - textWidthRight)}");
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}