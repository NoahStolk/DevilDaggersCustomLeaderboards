using DevilDaggersCore.Game;
using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Enumerators;
using DevilDaggersCustomLeaderboards.Memory;
using DevilDaggersCustomLeaderboards.Memory.Variables;
using System;
using System.Globalization;
using Cmd = DevilDaggersCustomLeaderboards.Utils.ConsoleUtils;

namespace DevilDaggersCustomLeaderboards.Utils
{
	public static class GuiUtils
	{
		public static void WriteRecording()
		{
			Cmd.WriteLine($"Scanning process '{Scanner.Process?.ProcessName ?? "No process"}' ({Scanner.Process?.MainWindowTitle ?? "No title"})");
			Cmd.WriteLine("Recording...");
			Cmd.WriteLine();
			Cmd.WriteLine("Player ID", Scanner.PlayerId);
			Cmd.WriteLine("Username", Scanner.Username);
			Cmd.WriteLine("Time", Scanner.Time.Value.ToString("0.0000", CultureInfo.InvariantCulture));
			WriteVariable("Gems Collected", Scanner.GemsCollected, CustomColor.Red);
			WriteVariable("Kills", Scanner.Kills, CustomColor.Thorn);
			WriteVariable("Daggers Hit", Scanner.DaggersHit, CustomColor.Green);
			WriteVariable("Daggers Fired", Scanner.DaggersFired, CustomColor.Yellow);
			Cmd.WriteLine("Accuracy", $"{(Scanner.DaggersFired == 0 ? 0 : Scanner.DaggersHit / (float)Scanner.DaggersFired * 100):0.00}%");
			Cmd.WriteLine("Enemies Alive", Scanner.EnemiesAlive);
			Cmd.WriteLine("Hand", GetHand(Scanner.LevelGems));
			WriteVariable("Homing Daggers", Scanner.HomingDaggers, CustomColor.Magenta);
			Cmd.WriteLine("Leviathans Alive", Scanner.LeviathansAlive);
			Cmd.WriteLine("Orbs Alive", Scanner.OrbsAlive);
			WriteVariable("Gems Despawned", Scanner.GemsDespawned, CustomColor.Red);
			WriteVariable("Gems Eaten", Scanner.GemsEaten, CustomColor.Green);
			Cmd.WriteLine();
			Cmd.WriteLine("Is Player Alive", Scanner.IsPlayerAlive);
			Cmd.WriteLine("Is Replay", Scanner.IsReplay);
			Cmd.WriteLine("Death Type", GameInfo.GetDeathByType(Scanner.DeathType)?.Name ?? "Invalid death type", ColorUtils.GetDeathColor(Scanner.DeathType));
			Cmd.WriteLine("Is In-Game", Scanner.IsInGame);
			Cmd.WriteLine("SurvivalHash", Scanner.SurvivalHash);
			Cmd.WriteLine();
			Cmd.WriteLine("Level 2", (Scanner.LevelUpTime2 / 10000f).ToString("0.0000", CultureInfo.InvariantCulture));
			Cmd.WriteLine("Level 3", (Scanner.LevelUpTime3 / 10000f).ToString("0.0000", CultureInfo.InvariantCulture));
			Cmd.WriteLine("Level 4", (Scanner.LevelUpTime4 / 10000f).ToString("0.0000", CultureInfo.InvariantCulture));
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

		public static void WriteStats(CustomLeaderboard leaderboard, CustomEntry? entry)
		{
			if (entry == null)
			{
				Cmd.WriteLine("Current player not found on the leaderboard.");
				return;
			}

			double accuracy = Scanner.DaggersFired == 0 ? 0 : Scanner.DaggersHit / (double)Scanner.DaggersFired;
			double accuracyOld = entry.DaggersFired == 0 ? 0 : entry.DaggersHit / (double)entry.DaggersFired;

			Cmd.Write($"{GameInfo.GetDeathByType(Scanner.DeathType)?.Name ?? "Invalid death type"}", ColorUtils.GetDeathColor(Scanner.DeathType));
			Cmd.WriteLine();
			Cmd.WriteLine();

			int timeDiff = Scanner.TimeInt - entry.Time;
			Cmd.Write($"{"Time",-Cmd.TextWidthLeft}");
			Cmd.Write($"{Scanner.Time,Cmd.TextWidthRight:0.0000}", ColorUtils.GetDaggerColor(Scanner.TimeInt, leaderboard));
			Cmd.WriteLine($" ({(timeDiff < 0 ? string.Empty : "+")}{timeDiff / 10000f:0.0000})", ColorUtils.Worse);

			WriteIntField("Gems Collected", Scanner.GemsCollected, Scanner.GemsCollected - entry.GemsCollected);
			WriteIntField("Gems Despawned", Scanner.GemsDespawned, Scanner.GemsDespawned - entry.GemsDespawned);
			WriteIntField("Gems Eaten", Scanner.GemsEaten, Scanner.GemsEaten - entry.GemsEaten);
			WriteIntField("Kills", Scanner.Kills, Scanner.Kills - entry.Kills);
			WriteIntField("Daggers Hit", Scanner.DaggersHit, Scanner.DaggersHit - entry.DaggersHit);
			WriteIntField("Daggers Fired", Scanner.DaggersFired, Scanner.DaggersFired - entry.DaggersFired);
			WritePercentageField("Accuracy", accuracy, accuracy - accuracyOld);
			WriteIntField("Enemies Alive", Scanner.EnemiesAlive, Scanner.EnemiesAlive - entry.EnemiesAlive);
			WriteIntField("Homing Daggers", Scanner.HomingDaggers, Scanner.HomingDaggers - entry.HomingDaggers);
			WriteTimeField("Level 2", Scanner.LevelUpTime2, Scanner.LevelUpTime2 - entry.LevelUpTime2);
			WriteTimeField("Level 3", Scanner.LevelUpTime3, Scanner.LevelUpTime3 - entry.LevelUpTime3);
			WriteTimeField("Level 4", Scanner.LevelUpTime4, Scanner.LevelUpTime4 - entry.LevelUpTime4);

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
