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

		private const int textWidthFull = 50;
		private const int textWidthLeft = 15;
		private const int textWidthRight = 15;

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

			WriteLine("Checking for updates...");

			VersionHandler.Instance.GetOnlineVersion(ApplicationName, LocalVersion);
			VersionResult versionResult = VersionHandler.Instance.VersionResult;
			Console.Clear();
			if (versionResult.IsUpToDate.HasValue)
			{
				if (LocalVersion < versionResult.Tool.VersionNumberRequired)
				{
					WriteLine($"You are using an unsupported and outdated version of {ApplicationDisplayName}. Please update the program.\n(Press any key to continue.)", ConsoleColor.Red);
					Console.ReadKey();
				}
				else if (LocalVersion < versionResult.Tool.VersionNumber)
				{
					WriteLine($"An update for {ApplicationDisplayName} is available.\n(Press any key to continue.)", ConsoleColor.Yellow);
					Console.ReadKey();
				}
			}
			else
			{
				WriteLine("Failed to check for updates.\n(Press any key to continue.)", ConsoleColor.Red);
				Console.ReadKey();
			}

			Console.Clear();
			for (; ; )
			{
				scanner.FindWindow();

				if (scanner.Process == null)
				{
					WriteLine($"Devil Daggers not found. Make sure the game is running. Retrying in a second...");
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
					WriteLine($"Scanning process '{scanner.Process.ProcessName}' ({scanner.Process.MainWindowTitle})");
					WriteLine("Recording...");
					WriteLine();

					WriteLine("Player ID", scanner.PlayerId);
					WriteLine("Username", scanner.Username);
					WriteLine();

					WriteLine("Time", scanner.Time.Value.ToString("0.0000"));
					WriteLine("Gems", scanner.Gems);
					WriteLine("Kills", scanner.Kills);
					WriteLine("Shots Hit", scanner.ShotsHit);
					WriteLine("Shots Fired", scanner.ShotsFired);
					WriteLine("Accuracy", $"{(scanner.ShotsFired == 0 ? 0 : scanner.ShotsHit / (float)scanner.ShotsFired * 100):0.00}%");
					WriteLine("Enemies Alive", scanner.EnemiesAlive);
					WriteLine("Death Type", GameInfo.GetDeathFromDeathType(scanner.DeathType).Name);
					WriteLine("Alive", scanner.IsAlive);
					WriteLine("Replay", scanner.IsReplay);
					WriteLine();

					WriteLine("Hand", GetHand(scanner.LevelGems));
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

					WriteLine("Homing", scanner.Homing);
					WriteLine();

					WriteLine("Level 2", scanner.LevelUpTime2.ToString("0.0000"));
					WriteLine("Level 3", scanner.LevelUpTime3.ToString("0.0000"));
					WriteLine("Level 4", scanner.LevelUpTime4.ToString("0.0000"));
					WriteLine();

					Thread.Sleep(50);
					Console.SetCursorPosition(0, 0);

					// If player just died
					if (!scanner.IsAlive && scanner.IsAlive.ValuePrevious)
					{
						recording = false;

						int tries = 0;
						UploadResult uploadResult;
						do
						{
							Console.Clear();
							WriteLine("Uploading...");
							WriteLine();
							uploadResult = NetworkHandler.Instance.Upload();
							// Thread is being blocked by the upload.

							if (uploadResult.Success)
							{
								WriteLine("Upload successful", ConsoleColor.Green);
								WriteLine(uploadResult.Message);
								if (uploadResult.SubmissionInfo != null)
									WriteSubmissionInfo(uploadResult.SubmissionInfo);
								WriteLine();

								WriteLine("Username", scanner.Username);
								WriteLine("Time", scanner.Time.Value.ToString("0.0000"));
								WriteLine("Kills", scanner.Kills);
								WriteLine("Gems", scanner.Gems);
								WriteLine("Shots Hit", scanner.ShotsHit);
								WriteLine("Shots Fired", scanner.ShotsFired);
								WriteLine("Accuracy", $"{(scanner.ShotsFired == 0 ? 0 : scanner.ShotsHit / (float)scanner.ShotsFired):0.00%}");
								WriteLine("Death Type", GameInfo.GetDeathFromDeathType(scanner.DeathType).Name);
								WriteLine("Enemies Alive", scanner.EnemiesAlive);
								WriteLine("Homing", scanner.Homing);
								WriteLine("Level 2", scanner.LevelUpTime2 == 0 ? "N/A" : scanner.LevelUpTime2.ToString("0.0000"));
								WriteLine("Level 3", scanner.LevelUpTime3 == 0 ? "N/A" : scanner.LevelUpTime3.ToString("0.0000"));
								WriteLine("Level 4", scanner.LevelUpTime4 == 0 ? "N/A" : scanner.LevelUpTime4.ToString("0.0000"));
							}
							else
							{
								WriteLine("Upload failed", ConsoleColor.Red);
								WriteLine(uploadResult.Message);
								tries++;
								if (uploadResult.TryCount > 1)
									WriteLine($"Retrying (attempt {tries} / {uploadResult.TryCount})");
								Log.Warn($"Upload failed - {uploadResult.Message}");

								Thread.Sleep(500);
							}
						}
						while (!uploadResult.Success && tries < uploadResult.TryCount);

						Console.SetCursorPosition(0, 0);
						WriteLine("Ready to restart");
						WriteLine();
					}
				}
				else if (scanner.Time < scanner.Time.ValuePrevious)
				{
					Console.Clear();
					recording = true;
					scanner.RestartScan();
				}
			}
		}

		private static void WriteSubmissionInfo(SubmissionInfo si)
		{
			double accuracy = si.ShotsFired == 0 ? 0 : si.ShotsHit / (double)si.ShotsFired;

			int shotsHitOld = si.ShotsHit - si.ShotsHitDiff;
			int shotsFiredOld = si.ShotsFired - si.ShotsFiredDiff;
			double accuracyOld = shotsFiredOld == 0 ? 0 : shotsHitOld / (double)shotsFiredOld;
			double accuracyDiff = accuracy - accuracyOld;

			Write($"{$"Rank",-textWidthLeft}{$"{si.Rank} / {si.TotalPlayers}",textWidthRight}");
			Write($" ({si.RankDiff:+0;-#})\n", GetColor(si.RankDiff));

			Write($"{$"Time",-textWidthLeft}{si.Time,textWidthRight:0.0000}");
			Write($" (+{si.TimeDiff:0.0000})\n", ConsoleColor.Green);

			Write($"{$"Kills",-textWidthLeft}{si.Kills,textWidthRight}");
			Write($" ({si.KillsDiff:+0;-#})\n", GetColor(si.KillsDiff));

			Write($"{$"Gems",-textWidthLeft}{si.Gems,textWidthRight}");
			Write($" ({si.GemsDiff:+0;-#})\n", GetColor(si.GemsDiff));

			Write($"{$"Shots Hit",-textWidthLeft}{si.ShotsHit,textWidthRight}");
			Write($" ({si.ShotsHitDiff:+0;-#})\n", GetColor(si.ShotsHitDiff));

			Write($"{$"Shots Fired",-textWidthLeft}{si.ShotsFired,textWidthRight}");
			Write($" ({si.ShotsFiredDiff:+0;-#})\n", GetColor(si.ShotsFiredDiff));

			Write($"{$"Accuracy",-textWidthLeft}{accuracy,textWidthRight:0.00%}");
			Write($" ({(accuracyDiff < 0 ? "" : "+")}{accuracyDiff:0.00%})\n", GetColor(accuracyDiff));

			Write($"{$"Enemies Alive",-textWidthLeft}{si.EnemiesAlive,textWidthRight}");
			Write($" ({si.EnemiesAliveDiff:+0;-#})\n", GetColor(si.EnemiesAliveDiff));

			Write($"{$"Homing",-textWidthLeft}{si.Homing,textWidthRight}");
			Write($" ({si.HomingDiff:+0;-#})\n", GetColor(si.HomingDiff));

			Write($"{$"Level 2",-textWidthLeft}{si.LevelUpTime2,textWidthRight:0.0000}");
			Write($" ({(si.LevelUpTime2Diff < 0 ? "" : "+")}{si.LevelUpTime2Diff:0.0000})\n", GetColor(-si.LevelUpTime2Diff));

			Write($"{$"Level 3",-textWidthLeft}{si.LevelUpTime3,textWidthRight:0.0000}");
			Write($" ({(si.LevelUpTime3Diff < 0 ? "" : "+")}{si.LevelUpTime3Diff:0.0000})\n", GetColor(-si.LevelUpTime3Diff));

			Write($"{$"Level 4",-textWidthLeft}{si.LevelUpTime4,textWidthRight:0.0000}");
			Write($" ({(si.LevelUpTime4Diff < 0 ? "" : "+")}{si.LevelUpTime4Diff:0.0000})\n", GetColor(-si.LevelUpTime4Diff));

			ConsoleColor GetColor<T>(T n)
				where T : IComparable<T>
			{
				int comparison = n.CompareTo(default(T));
				return comparison == 0 ? ConsoleColor.White : comparison == 1 ? ConsoleColor.Green : ConsoleColor.Red;
			}
		}

		private static void Write(object text, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.Write(text.ToString());
			Console.ForegroundColor = ConsoleColor.White;
		}

		private static void WriteLine()
		{
			Console.WriteLine(new string(' ', textWidthFull));
		}

		private static void WriteLine(object text, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine($"{text,-textWidthLeft}");
			Console.ForegroundColor = ConsoleColor.White;
		}

		private static void WriteLine(object textLeft, object textRight, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine($"{textLeft,-textWidthLeft}{textRight,textWidthRight}{new string(' ', textWidthFull)}");
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}