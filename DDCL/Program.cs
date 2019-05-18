using DDCL.MemoryHandling;
using DDCL.Network;
using System;
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
		/// <summary>
		/// I don't think you can die before 2.5 seconds in Devil Daggers unless there is no arena. This should fix the broken submissions that occasionally get sent for some reason.
		/// </summary>
		private const float MinimalTime = 2.5f;

		private static readonly Scanner scanner = Scanner.Instance;
		private static bool recording = true;

		public static void Main()
		{
			Console.CursorVisible = false;
			Console.WindowHeight = 40;
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
					Write("Death Type", scanner.DeathType.Value.ToString());
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
					if (!scanner.IsAlive.Value && scanner.IsAlive.ValuePrevious && scanner.Time.Value > MinimalTime && scanner.PlayerID.Value > 0 && !scanner.IsReplay.Value)
					{
						recording = false;

						JsonResult jsonResult;
						do
						{
							Console.Clear();
							Write("Uploading...");
							jsonResult = NetworkHandler.Instance.Upload();
							// Thread is being blocked

							Console.Clear();
							if (jsonResult.success)
							{
								Write("Upload successful", ConsoleColor.Green);
								Write(jsonResult.message);
								Write();

								Write("Player name", scanner.PlayerName.Value);
								Write("Time", scanner.Time.Value.ToString("0.0000"));
								Write("Gems", scanner.Gems.Value.ToString());
								Write("Kills", scanner.Kills.Value.ToString());
								Write("Accuracy", $"{(scanner.ShotsFired.Value == 0 ? 0 : scanner.ShotsHit.Value / (float)scanner.ShotsFired.Value * 100).ToString("0.00")}%");
								Write("Death Type", scanner.DeathType.Value.ToString());
								Write("Enemies Alive", scanner.EnemiesAlive.Value.ToString());
								Write("Homing", scanner.Homing.ToString());
								Write("Level 2", scanner.LevelUpTimes[0].ToString());
								Write("Level 3", scanner.LevelUpTimes[1].ToString());
								Write("Level 4", scanner.LevelUpTimes[2].ToString());
							}
							else
							{
								Write("Upload failed", ConsoleColor.Red);
								Write(jsonResult.message);
								Thread.Sleep(500);
							}
						}
						while (!jsonResult.success);
					}
				}
				// TODO: Check for increasing time instead
				else if (scanner.IsAlive.Value && !scanner.IsAlive.ValuePrevious)
				{
					scanner.Reset();
					Console.Clear();
					recording = true;
				}
			}
		}

		private static void Write()
		{
			Console.WriteLine(new string(' ', 40));
		}

		private static void Write(string text, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(text.PadRight(40));
			Console.ForegroundColor = ConsoleColor.White;
		}

		private static void Write(string textLeft, string textRight, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine($"{textLeft.PadRight(20)}{textRight.PadRight(20)}");
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}