using DevilDaggersCore.Game;
using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Enumerators;
using DevilDaggersCustomLeaderboards.Memory;
using DevilDaggersCustomLeaderboards.Memory.Variables;
using DevilDaggersCustomLeaderboards.Utils;
using System;
using System.Globalization;
using Cmd = DevilDaggersCustomLeaderboards.Utils.ConsoleUtils;

namespace DevilDaggersCustomLeaderboards.Extensions
{
	public static class GuiExtensions
	{
		public static void WriteRecording(this Scanner scanner)
		{
			Cmd.WriteLine($"Scanning process '{scanner.Process?.ProcessName ?? "No process"}' ({scanner.Process?.MainWindowTitle ?? "No title"})");
			Cmd.WriteLine("Recording...");
			Cmd.WriteLine();
			Cmd.WriteLine("Player ID", scanner.PlayerId);
			Cmd.WriteLine("Username", scanner.Username);
			Cmd.WriteLine("Time", scanner.Time.Value.ToString("0.0000", CultureInfo.InvariantCulture));
			WriteVariable("Gems Collected", scanner.GemsCollected, CustomColor.Red);
			WriteVariable("Kills", scanner.Kills, CustomColor.Thorn);
			WriteVariable("Daggers Hit", scanner.DaggersHit, CustomColor.Green);
			WriteVariable("Daggers Fired", scanner.DaggersFired, CustomColor.Yellow);
			Cmd.WriteLine("Accuracy", $"{(scanner.DaggersFired == 0 ? 0 : scanner.DaggersHit / (float)scanner.DaggersFired * 100):0.00}%");
			Cmd.WriteLine("Enemies Alive", scanner.EnemiesAlive);
			Cmd.WriteLine("Hand", GetHand(scanner.LevelGems));
			WriteVariable("Homing Daggers", scanner.HomingDaggers, CustomColor.Magenta);
			Cmd.WriteLine("Leviathans Alive", scanner.LeviathansAlive);
			Cmd.WriteLine("Orbs Alive", scanner.OrbsAlive);
			WriteVariable("Gems Despawned", scanner.GemsDespawned, CustomColor.Red);
			WriteVariable("Gems Eaten", scanner.GemsEaten, CustomColor.Green);
			Cmd.WriteLine();
			Cmd.WriteLine("Is Player Alive", scanner.IsPlayerAlive);
			Cmd.WriteLine("Is Replay", scanner.IsReplay);
			Cmd.WriteLine("Death Type", GameInfo.GetDeathByType(scanner.DeathType)?.Name ?? "Invalid death type", ColorUtils.GetDeathColor(scanner.DeathType));
			Cmd.WriteLine("Is In-Game", scanner.IsInGame);
			Cmd.WriteLine("SurvivalHash", scanner.SurvivalHash);
			Cmd.WriteLine();
			Cmd.WriteLine("Level 2", (scanner.LevelUpTime2 / 10000f).ToString("0.0000", CultureInfo.InvariantCulture));
			Cmd.WriteLine("Level 3", (scanner.LevelUpTime3 / 10000f).ToString("0.0000", CultureInfo.InvariantCulture));
			Cmd.WriteLine("Level 4", (scanner.LevelUpTime4 / 10000f).ToString("0.0000", CultureInfo.InvariantCulture));
			Cmd.WriteLine();

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

			static void WriteVariable<T>(object textLeft, AbstractVariable<T> variable, CustomColor foregroundColorModify = ColorUtils.BackgroundDefault, CustomColor foregroundColor = ColorUtils.ForegroundDefault, CustomColor backgroundColor = ColorUtils.BackgroundDefault)
			{
				Console.ForegroundColor = (ConsoleColor)foregroundColor;

				Console.BackgroundColor = (ConsoleColor)backgroundColor;
				Console.Write($"{textLeft,-Cmd.TextWidthLeft}");

				if (variable.IsChanged)
					Console.ForegroundColor = (ConsoleColor)foregroundColorModify;
				Console.Write($"{variable,Cmd.TextWidthRight}");

				Console.BackgroundColor = (ConsoleColor)backgroundColor;
				Console.WriteLine($"{new string(' ', Cmd.TextWidthFull)}");
			}
		}

		public static void WriteStats(this Scanner scanner, CustomLeaderboard leaderboard, CustomEntry? entry)
		{
			if (entry == null)
			{
				Cmd.WriteLine("Current player not found on the leaderboard.");
				return;
			}

			double accuracy = scanner.DaggersFired == 0 ? 0 : scanner.DaggersHit / (double)scanner.DaggersFired;
			double accuracyOld = entry.DaggersFired == 0 ? 0 : entry.DaggersHit / (double)entry.DaggersFired;

			Cmd.Write($"{GameInfo.GetDeathByType(scanner.DeathType)?.Name ?? "Invalid death type"}", ColorUtils.GetDeathColor(scanner.DeathType));
			Cmd.WriteLine();
			Cmd.WriteLine();

			int timeDiff = scanner.TimeInt - entry.Time;
			Cmd.Write($"{"Time",-Cmd.TextWidthLeft}");
			Cmd.Write($"{scanner.Time,Cmd.TextWidthRight:0.0000}", ColorUtils.GetDaggerColor(scanner.TimeInt, leaderboard));
			Cmd.WriteLine($" ({(timeDiff < 0 ? string.Empty : "+")}{timeDiff / 10000f:0.0000})", ColorUtils.Worse);

			WriteIntField("Gems Collected", scanner.GemsCollected, scanner.GemsCollected - entry.GemsCollected);
			WriteIntField("Gems Despawned", scanner.GemsDespawned, scanner.GemsDespawned - entry.GemsDespawned);
			WriteIntField("Gems Eaten", scanner.GemsEaten, scanner.GemsEaten - entry.GemsEaten);
			WriteIntField("Kills", scanner.Kills, scanner.Kills - entry.Kills);
			WriteIntField("Daggers Hit", scanner.DaggersHit, scanner.DaggersHit - entry.DaggersHit);
			WriteIntField("Daggers Fired", scanner.DaggersFired, scanner.DaggersFired - entry.DaggersFired);
			WritePercentageField("Accuracy", accuracy, accuracy - accuracyOld);
			WriteIntField("Enemies Alive", scanner.EnemiesAlive, scanner.EnemiesAlive - entry.EnemiesAlive);
			WriteIntField("Homing Daggers", scanner.HomingDaggers, scanner.HomingDaggers - entry.HomingDaggers);
			WriteTimeField("Level 2", scanner.LevelUpTime2, scanner.LevelUpTime2 - entry.LevelUpTime2);
			WriteTimeField("Level 3", scanner.LevelUpTime3, scanner.LevelUpTime3 - entry.LevelUpTime3);
			WriteTimeField("Level 4", scanner.LevelUpTime4, scanner.LevelUpTime4 - entry.LevelUpTime4);

			static void WriteIntField(string fieldName, int value, int valueDiff)
			{
				Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{value,Cmd.TextWidthRight}");
				Cmd.WriteLine($" ({valueDiff:+0;-#})", ColorUtils.GetImprovementColor(valueDiff));
			}

			static void WritePercentageField(string fieldName, double value, double valueDiff)
			{
				Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{value,Cmd.TextWidthRight:0.00%}");
				Cmd.WriteLine($" ({(valueDiff < 0 ? string.Empty : "+")}{valueDiff:0.00%})", ColorUtils.GetImprovementColor(valueDiff));
			}

			static void WriteTimeField(string fieldName, int value, int valueDiff)
			{
				Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{(value == 0 ? "N/A" : $"{value / 10000f:0.0000}"),Cmd.TextWidthRight:0.0000}");
				if (value == 0 || valueDiff == value)
					Cmd.WriteLine();
				else
					Cmd.WriteLine($" ({(valueDiff < 0 ? string.Empty : "+")}{valueDiff / 10000f:0.0000})", ColorUtils.GetImprovementColor(-valueDiff));
			}
		}

		public static bool IsHighscore(this UploadSuccess us)
			=> us.Rank != 0;

		public static void WriteLeaderboard(this UploadSuccess us, int currentPlayerId)
		{
			for (int i = 0; i < us.TotalPlayers; i++)
			{
				int spaceCountCurrent = (i + 1).ToString(CultureInfo.InvariantCulture).Length;
				int spaceCountTotal = us.TotalPlayers.ToString(CultureInfo.InvariantCulture).Length;

				CustomEntry entry = us.Entries[i];
				CustomColor daggerColor = ColorUtils.GetDaggerColor(entry.Time, us.Leaderboard);

				bool isCurrentPlayer = entry.PlayerId == currentPlayerId;
				CustomColor foregroundColor = isCurrentPlayer ? ColorUtils.GetDaggerHighlightColor(daggerColor) : daggerColor;
				CustomColor backgroundColor = isCurrentPlayer ? daggerColor : ColorUtils.BackgroundDefault;

				Cmd.Write($"{new string(' ', spaceCountTotal - spaceCountCurrent)}{i + 1}. ", foregroundColor, backgroundColor);
				Cmd.Write($"{entry.Username.Substring(0, Math.Min(entry.Username.Length, Cmd.TextWidthLeft))}", foregroundColor, backgroundColor);
				Cmd.Write($"{entry.Time / 10000f,Cmd.TextWidthRight:0.0000}\n", foregroundColor, backgroundColor);
			}

			Console.BackgroundColor = (ConsoleColor)ColorUtils.BackgroundDefault;
		}

		public static void WriteHighscoreStats(this UploadSuccess us)
		{
			int deathType = us.Entries[us.Rank - 1].DeathType;

			double accuracy = us.DaggersFired == 0 ? 0 : us.DaggersHit / (double)us.DaggersFired;

			int shotsHitOld = us.DaggersHit - us.DaggersHitDiff;
			int shotsFiredOld = us.DaggersFired - us.DaggersFiredDiff;
			double accuracyOld = shotsFiredOld == 0 ? 0 : shotsHitOld / (double)shotsFiredOld;
			double accuracyDiff = accuracy - accuracyOld;

			Cmd.Write($"{GameInfo.GetDeathByType(deathType)?.Name ?? "Invalid death type"}", ColorUtils.GetDeathColor(deathType));
			Cmd.WriteLine();
			Cmd.WriteLine();

			Cmd.Write($"{"Rank",-Cmd.TextWidthLeft}{$"{us.Rank} / {us.TotalPlayers}",Cmd.TextWidthRight}");
			if (!us.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({us.RankDiff:+0;-#})", ColorUtils.GetImprovementColor(us.RankDiff));
			Cmd.WriteLine();

			float time = us.Time / 10000f;
			float timeDiff = us.TimeDiff / 10000f;
			Cmd.Write($"{"Time",-Cmd.TextWidthLeft}");
			Cmd.Write($"{time,Cmd.TextWidthRight:0.0000}", ColorUtils.GetDaggerColor(us.Time, us.Leaderboard));
			if (!us.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({(timeDiff < 0 ? string.Empty : "+")}{timeDiff:0.0000})", ColorUtils.Better);
			Cmd.WriteLine();

			WriteIntField(us.IsNewUserOnThisLeaderboard, "Gems Collected", us.GemsCollected, us.GemsCollectedDiff);
			WriteIntField(us.IsNewUserOnThisLeaderboard, "Gems Despawned", us.GemsDespawned, us.GemsDespawnedDiff);
			WriteIntField(us.IsNewUserOnThisLeaderboard, "Gems Eaten", us.GemsEaten, us.GemsEatenDiff);
			WriteIntField(us.IsNewUserOnThisLeaderboard, "Kills", us.Kills, us.KillsDiff);
			WriteIntField(us.IsNewUserOnThisLeaderboard, "Daggers Hit", us.DaggersHit, us.DaggersHitDiff);
			WriteIntField(us.IsNewUserOnThisLeaderboard, "Daggers Fired", us.DaggersFired, us.DaggersFiredDiff);
			WritePercentageField(us.IsNewUserOnThisLeaderboard, "Accuracy", accuracy, accuracyDiff);
			WriteIntField(us.IsNewUserOnThisLeaderboard, "Enemies Alive", us.EnemiesAlive, us.EnemiesAliveDiff);
			WriteIntField(us.IsNewUserOnThisLeaderboard, "Homing Daggers", us.HomingDaggers, us.HomingDaggersDiff);

			WriteTimeField(us.IsNewUserOnThisLeaderboard || us.LevelUpTime2 == us.LevelUpTime2Diff, "Level 2", us.LevelUpTime2, us.LevelUpTime2Diff);
			WriteTimeField(us.IsNewUserOnThisLeaderboard || us.LevelUpTime3 == us.LevelUpTime3Diff, "Level 3", us.LevelUpTime3, us.LevelUpTime3Diff);
			WriteTimeField(us.IsNewUserOnThisLeaderboard || us.LevelUpTime4 == us.LevelUpTime4Diff, "Level 4", us.LevelUpTime4, us.LevelUpTime4Diff);

			static void WriteTimeField(bool writeDifference, string fieldName, int value, int valueDiff)
			{
				Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{value / 10000f,Cmd.TextWidthRight:0.0000}");
				if (writeDifference)
					Cmd.Write($" ({(valueDiff < 0 ? string.Empty : "+")}{valueDiff / 10000f:0.0000})", ColorUtils.GetImprovementColor(-valueDiff));
				Cmd.WriteLine();
			}

			static void WritePercentageField(bool isNewUser, string fieldName, double value, double valueDiff)
			{
				Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{value,Cmd.TextWidthRight:0.00%}");
				if (!isNewUser)
					Cmd.Write($" ({(valueDiff < 0 ? string.Empty : "+")}{valueDiff:0.00%})", ColorUtils.GetImprovementColor(valueDiff));
				Cmd.WriteLine();
			}

			static void WriteIntField(bool isNewUser, string fieldName, int value, int valueDiff)
			{
				Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{value,Cmd.TextWidthRight}");
				if (!isNewUser)
					Cmd.Write($" ({valueDiff:+0;-#})", ColorUtils.GetImprovementColor(valueDiff));
				Cmd.WriteLine();
			}
		}
	}
}
