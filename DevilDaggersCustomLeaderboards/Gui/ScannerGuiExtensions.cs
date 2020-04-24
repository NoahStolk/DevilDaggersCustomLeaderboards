using DevilDaggersCore.CustomLeaderboards;
using DevilDaggersCore.Game;
using DevilDaggersCustomLeaderboards.Memory;
using System;
using Cmd = DevilDaggersCustomLeaderboards.Gui.ConsoleUtils;

namespace DevilDaggersCustomLeaderboards.Gui
{
	internal static class ScannerGuiExtensions
	{
		internal static void WriteRecording(this Scanner scanner)
		{
			Cmd.WriteLine($"Scanning process '{scanner.Process.ProcessName}' ({scanner.Process.MainWindowTitle})");
			Cmd.WriteLine("Recording...");
			Cmd.WriteLine();

			Cmd.WriteLine("Player ID", scanner.PlayerId);
			Cmd.WriteLine("Username", scanner.Username);
			Cmd.WriteLine();

			Cmd.WriteLine("Time", scanner.Time.Value.ToString("0.0000"));
			Cmd.WriteLine("Gems", scanner.Gems);
			Cmd.WriteLine("Kills", scanner.Kills);
			Cmd.WriteLine("Shots Hit", scanner.ShotsHit);
			Cmd.WriteLine("Shots Fired", scanner.ShotsFired);
			Cmd.WriteLine("Accuracy", $"{(scanner.ShotsFired == 0 ? 0 : scanner.ShotsHit / (float)scanner.ShotsFired * 100):0.00}%");
			Cmd.WriteLine("Enemies Alive", scanner.EnemiesAlive);
			Cmd.WriteLine("Death Type", GameInfo.GetDeathFromDeathType(scanner.DeathType).Name);
			Cmd.WriteLine("Alive", scanner.IsAlive);
			Cmd.WriteLine("Replay", scanner.IsReplay);
			Cmd.WriteLine();

			if (scanner.LevelGems == 0 && scanner.Gems != 0 && scanner.IsAlive && !scanner.IsReplay)
			{
				Cmd.WriteLine("WARNING: Level up times and homing count are not being detected.\nRestart Devil Daggers to fix this issue.", ConsoleColor.Red);
				// TODO: Log addresses.
			}

			Cmd.WriteLine("Hand", GetHand(scanner.LevelGems));
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

			Cmd.WriteLine("Homing", scanner.Homing);
			Cmd.WriteLine();

			Cmd.WriteLine("Level 2", scanner.LevelUpTime2.ToString("0.0000"));
			Cmd.WriteLine("Level 3", scanner.LevelUpTime3.ToString("0.0000"));
			Cmd.WriteLine("Level 4", scanner.LevelUpTime4.ToString("0.0000"));
			Cmd.WriteLine();
		}

		internal static void WriteStats(this Scanner scanner, CustomLeaderboardBase leaderboard, CustomEntryBase entry)
		{
			double accuracy = scanner.ShotsFired == 0 ? 0 : scanner.ShotsHit / (double)scanner.ShotsFired;
			double accuracyOld = entry.ShotsFired == 0 ? 0 : entry.ShotsHit / (double)entry.ShotsFired;

			float timeDiff = scanner.Time - entry.Time;
			float killsDiff = scanner.Kills - entry.Kills;
			float gemsDiff = scanner.Gems - entry.Gems;
			float shotsHitDiff = scanner.ShotsHit - entry.ShotsHit;
			float shotsFiredDiff = scanner.ShotsFired - entry.ShotsFired;
			double accuracyDiff = accuracy - accuracyOld;
			float enemiesAliveDiff = scanner.EnemiesAlive - entry.EnemiesAlive;
			float homingDiff = scanner.Homing - entry.Homing;
			float levelUpTime2Diff = scanner.LevelUpTime2 - entry.LevelUpTime2;
			float levelUpTime3Diff = scanner.LevelUpTime3 - entry.LevelUpTime3;
			float levelUpTime4Diff = scanner.LevelUpTime4 - entry.LevelUpTime4;

			Cmd.Write($"{$"Time",-Cmd.TextWidthLeft}");
			Cmd.Write($"{(float)scanner.Time,Cmd.TextWidthRight:0.0000}", Cmd.GetDaggerColor(scanner.Time, leaderboard));
			Cmd.WriteLine($" ({timeDiff:0.0000})", ConsoleColor.Red);

			Cmd.Write($"{$"Kills",-Cmd.TextWidthLeft}{scanner.Kills,Cmd.TextWidthRight}");
			Cmd.WriteLine($" ({killsDiff:+0;-#})", Cmd.GetImprovementColor(killsDiff));

			Cmd.Write($"{$"Gems",-Cmd.TextWidthLeft}{scanner.Gems,Cmd.TextWidthRight}");
			Cmd.WriteLine($" ({gemsDiff:+0;-#})", Cmd.GetImprovementColor(gemsDiff));

			Cmd.Write($"{$"Shots Hit",-Cmd.TextWidthLeft}{scanner.ShotsHit,Cmd.TextWidthRight}");
			Cmd.WriteLine($" ({shotsHitDiff:+0;-#})", Cmd.GetImprovementColor(shotsHitDiff));

			Cmd.Write($"{$"Shots Fired",-Cmd.TextWidthLeft}{scanner.ShotsFired,Cmd.TextWidthRight}");
			Cmd.WriteLine($" ({shotsFiredDiff:+0;-#})", Cmd.GetImprovementColor(shotsFiredDiff));

			Cmd.Write($"{$"Accuracy",-Cmd.TextWidthLeft}{accuracy,Cmd.TextWidthRight:0.00%}");
			Cmd.WriteLine($" ({(accuracyDiff < 0 ? "" : "+")}{accuracyDiff:0.00%})", Cmd.GetImprovementColor(accuracyDiff));

			Cmd.Write($"{$"Enemies Alive",-Cmd.TextWidthLeft}{scanner.EnemiesAlive,Cmd.TextWidthRight}");
			Cmd.WriteLine($" ({enemiesAliveDiff:+0;-#})", Cmd.GetImprovementColor(enemiesAliveDiff));

			Cmd.Write($"{$"Homing",-Cmd.TextWidthLeft}{scanner.Homing,Cmd.TextWidthRight}");
			Cmd.WriteLine($" ({homingDiff:+0;-#})", Cmd.GetImprovementColor(homingDiff));

			Cmd.Write($"{$"Level 2",-Cmd.TextWidthLeft}{(scanner.LevelUpTime2 == 0 ? "N/A" : $"{scanner.LevelUpTime2:0.0000}"),Cmd.TextWidthRight:0.0000}");
			if (scanner.LevelUpTime2 == 0)
				Cmd.WriteLine();
			else
				Cmd.WriteLine($" ({(levelUpTime2Diff < 0 ? "" : "+")}{levelUpTime2Diff:0.0000})", Cmd.GetImprovementColor(-levelUpTime2Diff));

			Cmd.Write($"{$"Level 3",-Cmd.TextWidthLeft}{(scanner.LevelUpTime3 == 0 ? "N/A" : $"{scanner.LevelUpTime3:0.0000}"),Cmd.TextWidthRight:0.0000}");
			if (scanner.LevelUpTime3 == 0)
				Cmd.WriteLine();
			else
				Cmd.WriteLine($" ({(levelUpTime3Diff < 0 ? "" : "+")}{levelUpTime3Diff:0.0000})", Cmd.GetImprovementColor(-levelUpTime3Diff));

			Cmd.Write($"{$"Level 4",-Cmd.TextWidthLeft}{(scanner.LevelUpTime4 == 0 ? "N/A" : $"{scanner.LevelUpTime4:0.0000}"),Cmd.TextWidthRight:0.0000}");
			if (scanner.LevelUpTime4 == 0)
				Cmd.WriteLine();
			else
				Cmd.WriteLine($" ({(levelUpTime4Diff < 0 ? "" : "+")}{levelUpTime4Diff:0.0000})", Cmd.GetImprovementColor(-levelUpTime4Diff));
		}
	}
}