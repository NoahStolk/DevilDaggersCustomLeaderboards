using DDCL.MemoryHandling;
using DDCL.Network;
using log4net;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace DDCL
{
	/// <summary>
	/// Handles the main program and GUI-related tasks.
	/// Special Write methods are used to output to the console, as clearing the console after every update makes everything flicker which is ugly.
	/// So instead of clearing the console using Console.Clear(), we just reset the cursor to the top-left, and then overwrite everything from the previous update using the special Write methods.
	/// </summary>
	public static class Program
	{
		private static readonly int TextWidth = 70;
		private static readonly int TextWidthLeft = 20;
		private static readonly int TextWidthRight = 20;

		private const int MF_BYCOMMAND = 0x00000000;
		private const int SC_MINIMIZE = 0xF020;
		private const int SC_MAXIMIZE = 0xF030;
		private const int SC_SIZE = 0xF000;

		[DllImport("user32.dll")]
		public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

		[DllImport("user32.dll")]
		private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("kernel32.dll", ExactSpelling = true)]
		private static extern IntPtr GetConsoleWindow();

		private static readonly Scanner scanner = Scanner.Instance;
		private static bool recording = true;

		public static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static void Main()
		{
			Console.CursorVisible = false;
			Console.WindowHeight = 40;

			DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
			DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
			DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);

			Console.Title = $"Devil Daggers Custom Leaderboards - {Utils.GetVersion()}";

			Thread.CurrentThread.CurrentCulture = Constants.Culture;
			Thread.CurrentThread.CurrentUICulture = Constants.Culture;

			for (; ; )
			{
				scanner.FindWindow();

				if (scanner.Process == null)
				{
					Write($"Devil Daggers not found. Retrying in a second.");
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

					Write("Player ID", scanner.PlayerID.Value.ToString());
					Write("Player name", scanner.PlayerName.Value);
					Write();

					Write("Time", scanner.Time.Value.ToString("0.0000"));
					Write("Gems", scanner.Gems.Value.ToString());
					Write("Kills", scanner.Kills.Value.ToString());
					Write("Shots Hit", scanner.ShotsHit.Value.ToString());
					Write("Shots Fired", scanner.ShotsFired.Value.ToString());
					Write("Accuracy", $"{(scanner.ShotsFired.Value == 0 ? 0 : scanner.ShotsHit.Value / (float)scanner.ShotsFired.Value * 100).ToString("0.00")}%");
					Write("Enemies Alive", scanner.EnemiesAlive.Value.ToString());
					Write("Death Type", Utils.GetDeathName(scanner.DeathType.Value));
					Write("Alive", scanner.IsAlive.Value.ToString());
					Write("Replay", scanner.IsReplay.Value.ToString());
					Write();

					Write("Hand", scanner.Hand.ToString());
					Write("Homing", scanner.Homing.ToString());
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
						JsonResult jsonResult;
						do
						{
							Console.Clear();
							Write("Uploading... Please wait for the upload to finish before restarting.");
							Write();
							jsonResult = NetworkHandler.Instance.Upload();
							// Thread is being blocked by the upload

							if (jsonResult.success)
							{
								Write("Upload successful", ConsoleColor.Green);
								Write(jsonResult.message);
								Write();

								Write("Player name", scanner.PlayerName.Value);
								Write("Time", scanner.Time.Value.ToString("0.0000"));
								Write("Gems", scanner.Gems.Value.ToString());
								Write("Kills", scanner.Kills.Value.ToString());
								Write("Accuracy", $"{(scanner.ShotsFired.Value == 0 ? 0 : scanner.ShotsHit.Value / (float)scanner.ShotsFired.Value * 100).ToString("0.00")}% ({scanner.ShotsHit.Value} / {scanner.ShotsFired.Value})");
								Write("Death Type", Utils.GetDeathName(scanner.DeathType.Value));
								Write("Enemies Alive", scanner.EnemiesAlive.Value.ToString());
								Write("Homing", scanner.Homing.ToString());
								Write("Level 2", scanner.LevelUpTimes[0].ToString("0.0000"));
								Write("Level 3", scanner.LevelUpTimes[1].ToString("0.0000"));
								Write("Level 4", scanner.LevelUpTimes[2].ToString("0.0000"));
							}
							else
							{
								Write("Upload failed", ConsoleColor.Red);
								Write(jsonResult.message);
								string attempts = jsonResult.tryCount > 1 ? $"(attempt {++tries} / {jsonResult.tryCount})" : "";
								Write($"Retrying {attempts}");
								logger.Warn($"Upload failed - {jsonResult.message}");

								Thread.Sleep(500);
							}
						}
						while (!jsonResult.success && tries < jsonResult.tryCount);

						Console.SetCursorPosition(0, 0);
						Write("Ready to restart");
						Write();
					}
				}
				else if (scanner.IsAlive.Value && !scanner.IsAlive.ValuePrevious || scanner.Time.Value > scanner.Time.ValuePrevious)
				{
					scanner.Reset();
					Console.Clear();
					recording = true;
				}
			}
		}

		private static void Write()
		{
			Console.WriteLine(new string(' ', TextWidth));
		}

		private static void Write(string text, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(text.PadRight(TextWidth));
			Console.ForegroundColor = ConsoleColor.White;
		}

		private static void Write(string textLeft, string textRight, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine($"{textLeft.PadRight(TextWidthLeft)}{textRight.PadRight(TextWidthRight)}{new string(' ', TextWidth - TextWidthLeft - TextWidthRight)}");
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}