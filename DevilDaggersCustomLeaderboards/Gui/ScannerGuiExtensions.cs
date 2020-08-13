using DevilDaggersCore.CustomLeaderboards;
using DevilDaggersCore.Game;
using DevilDaggersCustomLeaderboards.Memory;
using System;
using System.Globalization;
using Cmd = DevilDaggersCustomLeaderboards.Gui.ConsoleUtils;

namespace DevilDaggersCustomLeaderboards.Gui
{
	public static class ScannerGuiExtensions
	{
		public static void WriteRecording(this Scanner scanner)
		{
			Cmd.WriteLine($"Scanning process '{scanner.Process.ProcessName}' ({scanner.Process.MainWindowTitle})");
			Cmd.WriteLine("Recording...");
			Cmd.WriteLine();

			Cmd.WriteLine("Player ID", scanner.PlayerId);
			Cmd.WriteLine("Username", scanner.Username);
			Cmd.WriteLine();

			Cmd.WriteLine("Time", scanner.TimeFloat.Value.ToString("0.0000", CultureInfo.InvariantCulture));
			Cmd.WriteLine("Gems", scanner.Gems);
			Cmd.WriteLine("Kills", scanner.Kills);
			Cmd.WriteLine("Shots Hit", scanner.ShotsHit);
			Cmd.WriteLine("Shots Fired", scanner.ShotsFired);
			Cmd.WriteLine("Accuracy", $"{(scanner.ShotsFired == 0 ? 0 : scanner.ShotsHit / (float)scanner.ShotsFired * 100):0.00}%");
			Cmd.WriteLine("Enemies Alive", scanner.EnemiesAlive);
#if DEBUG
			Cmd.WriteLine("Death Type", GameInfo.GetDeathByType(scanner.DeathType).Name, Cmd.GetDeathColor(scanner.DeathType));
			Cmd.WriteLine("Alive", scanner.IsAlive);
			Cmd.WriteLine("Replay", scanner.IsReplay);
#endif
			Cmd.WriteLine();

			if (scanner.LevelGems == 0 && scanner.Gems != 0 && scanner.IsAlive && !scanner.IsReplay && scanner.Time != 0)
			{
				// TODO: Log addresses.
				Cmd.WriteLine("WARNING: Level up times and homing count are not being detected.\nRestart Devil Daggers to fix this issue.", ConsoleColor.Red);
			}

			Cmd.WriteLine("Hand", GetHand(scanner.LevelGems));
			static int GetHand(int levelGems)
			{
				if (levelGems < 10)
					return 1;
				if (levelGems < 70)
					return 2;
				if (levelGems == 70)
					return 3;
				return 4;
			}

			Cmd.WriteLine("Homing", scanner.Homing);
			Cmd.WriteLine();

			Cmd.WriteLine("Level 2", (scanner.LevelUpTime2 / 10000f).ToString("0.0000", CultureInfo.InvariantCulture));
			Cmd.WriteLine("Level 3", (scanner.LevelUpTime3 / 10000f).ToString("0.0000", CultureInfo.InvariantCulture));
			Cmd.WriteLine("Level 4", (scanner.LevelUpTime4 / 10000f).ToString("0.0000", CultureInfo.InvariantCulture));
			Cmd.WriteLine();
		}

		public static void WriteStats(this Scanner scanner, CustomLeaderboardBase leaderboard, CustomLeaderboardCategoryBase category, CustomEntryBase entry)
		{
			double accuracy = scanner.ShotsFired == 0 ? 0 : scanner.ShotsHit / (double)scanner.ShotsFired;
			double accuracyOld = entry.ShotsFired == 0 ? 0 : entry.ShotsHit / (double)entry.ShotsFired;

			Cmd.Write($"{GameInfo.GetDeathFromDeathType(scanner.DeathType).Name}", Cmd.GetDeathColor(scanner.DeathType));
			Cmd.WriteLine();
			Cmd.WriteLine();

			int timeDiff = scanner.Time - entry.Time;
			Cmd.Write($"{$"Time",-Cmd.TextWidthLeft}");
			Cmd.Write($"{scanner.Time / 10000f,Cmd.TextWidthRight:0.0000}", Cmd.GetDaggerColor(scanner.Time, leaderboard, category));
			Cmd.WriteLine($" ({(timeDiff < 0 ? string.Empty : "+")}{timeDiff / 10000f:0.0000})", ConsoleColor.Red);

			WriteIntField("Kills", scanner.Kills, scanner.Kills - entry.Kills);
			WriteIntField("Gems", scanner.Gems, scanner.Gems - entry.Gems);
			WriteIntField("Shots hit", scanner.ShotsHit, scanner.ShotsHit - entry.ShotsHit);
			WriteIntField("Shots fired", scanner.ShotsFired, scanner.ShotsFired - entry.ShotsFired);
			WritePercentageField("Accuracy", accuracy, accuracy - accuracyOld);
			WriteIntField("Enemies alive", scanner.EnemiesAlive, scanner.EnemiesAlive - entry.EnemiesAlive);
			WriteIntField("Homing", scanner.Homing, scanner.Homing - entry.Homing);
			WriteTimeField("Level 2", scanner.LevelUpTime2, scanner.LevelUpTime2 - entry.LevelUpTime2);
			WriteTimeField("Level 3", scanner.LevelUpTime3, scanner.LevelUpTime3 - entry.LevelUpTime3);
			WriteTimeField("Level 4", scanner.LevelUpTime4, scanner.LevelUpTime4 - entry.LevelUpTime4);

			static void WriteIntField(string fieldName, int value, int valueDiff)
			{
				Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{value,Cmd.TextWidthRight}");
				Cmd.WriteLine($" ({valueDiff:+0;-#})", Cmd.GetImprovementColor(valueDiff));
			}

			static void WritePercentageField(string fieldName, double value, double valueDiff)
			{
				Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{value,Cmd.TextWidthRight:0.00%}");
				Cmd.WriteLine($" ({(valueDiff < 0 ? string.Empty : "+")}{valueDiff:0.00%})", Cmd.GetImprovementColor(valueDiff));
			}

			static void WriteTimeField(string fieldName, int value, int valueDiff)
			{
				Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{(value == 0 ? "N/A" : $"{value / 10000f:0.0000}"),Cmd.TextWidthRight:0.0000}");
				if (value == 0)
					Cmd.WriteLine();
				else
					Cmd.WriteLine($" ({(valueDiff < 0 ? string.Empty : "+")}{valueDiff / 10000f:0.0000})", Cmd.GetImprovementColor(-valueDiff));
			}
		}
	}
}