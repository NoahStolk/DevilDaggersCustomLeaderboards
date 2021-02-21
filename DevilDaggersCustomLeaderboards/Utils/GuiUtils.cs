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
			Cmd.WriteLine($"Scanning process '{Scanner.Process?.ProcessName ?? "No process"}' ({Scanner.Process?.MainWindowTitle ?? "No title"})...");
			Cmd.WriteLine();

			Cmd.WriteLine("Player ID", Scanner.PlayerId);
			Cmd.WriteLine("Player Name", Scanner.PlayerName);
			Cmd.WriteLine();

#if DEBUG
			Cmd.WriteLine("Is Player Alive", Scanner.IsPlayerAlive);
			Cmd.WriteLine("Is Replay", Scanner.IsReplay);
			Cmd.WriteLine("Is In-Game", Scanner.IsInGame);
			Cmd.WriteLine("SurvivalHash", HashUtils.ByteArrayToHexString(Scanner.SurvivalHashMd5));
			Cmd.WriteLine();
#endif

			if (Scanner.IsInGame)
			{
				Cmd.WriteLine("Player", Scanner.IsPlayerAlive ? "Alive" : (GameInfo.GetDeathByType(Scanner.DeathType)?.Name ?? "Invalid death type"), Scanner.IsPlayerAlive ? CustomColor.Gray : ColorUtils.GetDeathColor(Scanner.DeathType));
				Cmd.WriteLine();
				Cmd.WriteLine("Time", Scanner.Time.Value.ToString("0.0000", CultureInfo.InvariantCulture));
				Cmd.WriteLine();
				Cmd.WriteLine("Hand", $"Level {GetHand(Scanner.LevelGems)}");
				Cmd.WriteLine("Level 2", Scanner.LevelUpTime2.Value.ToString("0.0000", CultureInfo.InvariantCulture));
				Cmd.WriteLine("Level 3", Scanner.LevelUpTime3.Value.ToString("0.0000", CultureInfo.InvariantCulture));
				Cmd.WriteLine("Level 4", Scanner.LevelUpTime4.Value.ToString("0.0000", CultureInfo.InvariantCulture));
				WriteVariable("Homing Daggers", Scanner.HomingDaggers, CustomColor.Magenta);
				Cmd.WriteLine();
				WriteVariable("Gems Collected", Scanner.GemsCollected, CustomColor.Red);
				WriteVariable("Gems Despawned", Scanner.GemsDespawned, CustomColor.Red);
				WriteVariable("Gems Eaten", Scanner.GemsEaten, CustomColor.Green);
				WriteVariable("Gems Total", Scanner.GemsTotal, CustomColor.Red);
				Cmd.WriteLine("Gems In Arena", Math.Max(0, Scanner.GemsTotal - Scanner.GemsCollected - Scanner.GemsDespawned - Scanner.GemsEaten));
				Cmd.WriteLine();
				WriteEnemyHeaders("Enemies", "Alive", "Killed");
				WriteEnemyVariables("Total", Scanner.EnemiesAlive, Scanner.EnemiesKilled, ColorUtils.Entangled);
				WriteEnemyVariables("Skull I", Scanner.Skull1sAlive, Scanner.Skull1sKilled, ColorUtils.Swarmed);
				WriteEnemyVariables("Skull II", Scanner.Skull2sAlive, Scanner.Skull2sKilled, ColorUtils.Impaled);
				WriteEnemyVariables("Skull III", Scanner.Skull3sAlive, Scanner.Skull3sKilled, ColorUtils.Gored);
				WriteEnemyVariables("Skull IV", Scanner.Skull4sAlive, Scanner.Skull4sKilled, ColorUtils.Opened);
				WriteEnemyVariables("Squid I", Scanner.Squid1sAlive, Scanner.Squid1sKilled, ColorUtils.Purged);
				WriteEnemyVariables("Squid II", Scanner.Squid2sAlive, Scanner.Squid2sKilled, ColorUtils.Desecrated);
				WriteEnemyVariables("Squid III", Scanner.Squid3sAlive, Scanner.Squid3sKilled, ColorUtils.Sacrificed);
				WriteEnemyVariables("Spiderling", Scanner.SpiderlingsAlive, Scanner.SpiderlingsKilled, ColorUtils.Infested);
				WriteEnemyVariables("Spider I", Scanner.Spider1sAlive, Scanner.Spider1sKilled, ColorUtils.Intoxicated);
				WriteEnemyVariables("Spider II", Scanner.Spider2sAlive, Scanner.Spider2sKilled, ColorUtils.Envenomated);
				WriteEnemyVariables("Spider Egg", Scanner.SpiderEggsAlive, Scanner.SpiderEggsKilled, ColorUtils.Intoxicated);
				WriteEnemyVariables("Centipede", Scanner.CentipedesAlive, Scanner.CentipedesKilled, ColorUtils.Eviscerated);
				WriteEnemyVariables("Gigapede", Scanner.GigapedesAlive, Scanner.GigapedesKilled, ColorUtils.Annihilated);
				WriteEnemyVariables("Ghostpede", Scanner.GhostpedesAlive, Scanner.GhostpedesKilled, CustomColor.White);
				WriteEnemyVariables("Thorn", Scanner.ThornsAlive, Scanner.ThornsKilled, ColorUtils.Entangled);
				WriteEnemyVariables("Leviathan", Scanner.LeviathansAlive, Scanner.LeviathansKilled, ColorUtils.Incarnated);
				WriteEnemyVariables("Orb", Scanner.OrbsAlive, Scanner.OrbsKilled, ColorUtils.Discarnated);
				Cmd.WriteLine();
				WriteVariable("Daggers Hit", Scanner.DaggersHit, CustomColor.Green);
				WriteVariable("Daggers Fired", Scanner.DaggersFired, CustomColor.Yellow);
				Cmd.WriteLine("Accuracy", $"{(Scanner.DaggersFired == 0 ? 0 : Scanner.DaggersHit / (float)Scanner.DaggersFired * 100):0.00}%");
				Cmd.WriteLine();
				Cmd.WriteLine("Leviathan Down", Scanner.LeviathanDownTime.Value.ToString("0.0000", CultureInfo.InvariantCulture));
				Cmd.WriteLine("Orb Down", Scanner.OrbDownTime.Value.ToString("0.0000", CultureInfo.InvariantCulture));
				Cmd.WriteLine();
			}
			else
			{
				Cmd.WriteLine("Not in game");
				for (int i = 0; i < 45; i++)
					Cmd.WriteLine();
			}

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

			static void WriteVariable<T>(object textLeft, AbstractVariable<T> variable, CustomColor foregroundColorModify, CustomColor foregroundColor = ColorUtils.ForegroundDefault, CustomColor backgroundColor = ColorUtils.BackgroundDefault)
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

			static void WriteEnemyHeaders(object textLeft, string variableHeaderLeft, string variableHeaderRight, CustomColor foregroundColor = ColorUtils.ForegroundDefault, CustomColor backgroundColor = ColorUtils.BackgroundDefault)
			{
				Console.ForegroundColor = (ConsoleColor)foregroundColor;
				Console.BackgroundColor = (ConsoleColor)backgroundColor;

				Console.Write($"{textLeft,-Cmd.TextWidthLeft}");
				Console.Write($"{variableHeaderLeft,Cmd.TextWidthRight - 15}");
				Console.Write($"{variableHeaderRight,Cmd.TextWidthRight - 10}");
				Console.WriteLine($"{new string(' ', Cmd.TextWidthFull)}");
			}

			static void WriteEnemyVariables<T>(object textLeft, AbstractVariable<T> variableLeft, AbstractVariable<T> variableRight, CustomColor enemyColor, CustomColor foregroundColor = ColorUtils.ForegroundDefault, CustomColor backgroundColor = ColorUtils.BackgroundDefault)
			{
				Console.ForegroundColor = (ConsoleColor)enemyColor;
				Console.BackgroundColor = (ConsoleColor)backgroundColor;
				Console.Write($"{textLeft,-Cmd.TextWidthLeft}");

				Console.ForegroundColor = (ConsoleColor)(variableLeft.IsChanged ? enemyColor : foregroundColor);
				Console.Write($"{variableLeft,Cmd.TextWidthRight - 15}");
				Console.ForegroundColor = (ConsoleColor)(variableRight.IsChanged ? enemyColor : foregroundColor);
				Console.Write($"{variableRight,Cmd.TextWidthRight - 10}");

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

			int timeDiff = Scanner.Time.ConvertToTimeInt() - entry.Time;
			Cmd.Write($"{"Time",-Cmd.TextWidthLeft}");
			Cmd.Write($"{Scanner.Time.Value,Cmd.TextWidthRight:0.0000}", ColorUtils.GetDaggerColor(Scanner.Time.ConvertToTimeInt(), leaderboard));
			Cmd.WriteLine($" ({(timeDiff < 0 ? string.Empty : "+")}{timeDiff / 10000f:0.0000})", ColorUtils.Worse);

			WriteIntField("Gems Collected", Scanner.GemsCollected, Scanner.GemsCollected - entry.GemsCollected);
			WriteIntField("Gems Despawned", Scanner.GemsDespawned, Scanner.GemsDespawned - entry.GemsDespawned, true);
			WriteIntField("Gems Eaten", Scanner.GemsEaten, Scanner.GemsEaten - entry.GemsEaten, true);
			WriteIntField("Gems Total", Scanner.GemsTotal, Scanner.GemsTotal - entry.GemsTotal);
			WriteIntField("Enemies Killed", Scanner.EnemiesKilled, Scanner.EnemiesKilled - entry.EnemiesKilled);
			WriteIntField("Enemies Alive", Scanner.EnemiesAlive, Scanner.EnemiesAlive - entry.EnemiesAlive);
			WriteIntField("Daggers Fired", Scanner.DaggersFired, Scanner.DaggersFired - entry.DaggersFired);
			WriteIntField("Daggers Hit", Scanner.DaggersHit, Scanner.DaggersHit - entry.DaggersHit);
			WritePercentageField("Accuracy", accuracy, accuracy - accuracyOld);
			WriteIntField("Homing Daggers", Scanner.HomingDaggers, Scanner.HomingDaggers - entry.HomingDaggers);
			WriteTimeField("Level 2", Scanner.LevelUpTime2.ConvertToTimeInt(), Scanner.LevelUpTime2.ConvertToTimeInt() - entry.LevelUpTime2);
			WriteTimeField("Level 3", Scanner.LevelUpTime3.ConvertToTimeInt(), Scanner.LevelUpTime3.ConvertToTimeInt() - entry.LevelUpTime3);
			WriteTimeField("Level 4", Scanner.LevelUpTime4.ConvertToTimeInt(), Scanner.LevelUpTime4.ConvertToTimeInt() - entry.LevelUpTime4);

			static void WriteIntField(string fieldName, int value, int valueDiff, bool negate = false)
			{
				Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{value,Cmd.TextWidthRight}");
				Cmd.WriteLine($" ({valueDiff:+0;-#})", ColorUtils.GetImprovementColor(valueDiff * (negate ? -1 : 1)));
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
				Cmd.Write($"{entry.PlayerName.Substring(0, Math.Min(entry.PlayerName.Length, Cmd.TextWidthLeft))}", foregroundColor, backgroundColor);
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
			if (!us.IsNewPlayerOnThisLeaderboard)
				Cmd.Write($" ({us.RankDiff:+0;-#})", ColorUtils.GetImprovementColor(us.RankDiff));
			Cmd.WriteLine();

			float time = us.Time / 10000f;
			float timeDiff = us.TimeDiff / 10000f;
			Cmd.Write($"{"Time",-Cmd.TextWidthLeft}");
			Cmd.Write($"{time,Cmd.TextWidthRight:0.0000}", ColorUtils.GetDaggerColor(us.Time, us.Leaderboard));
			if (!us.IsNewPlayerOnThisLeaderboard)
				Cmd.Write($" ({(timeDiff < 0 ? string.Empty : "+")}{timeDiff:0.0000})", ColorUtils.Better);
			Cmd.WriteLine();

			WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Gems Collected", us.GemsCollected, us.GemsCollectedDiff);
			WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Gems Despawned", us.GemsDespawned, us.GemsDespawnedDiff, true);
			WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Gems Eaten", us.GemsEaten, us.GemsEatenDiff, true);
			WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Gems Total", us.GemsTotal, us.GemsTotalDiff);
			WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Enemies Killed", us.EnemiesKilled, us.EnemiesKilledDiff);
			WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Enemies Alive", us.EnemiesAlive, us.EnemiesAliveDiff);
			WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Daggers Fired", us.DaggersFired, us.DaggersFiredDiff);
			WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Daggers Hit", us.DaggersHit, us.DaggersHitDiff);
			WritePercentageField(!us.IsNewPlayerOnThisLeaderboard, "Accuracy", accuracy, accuracyDiff);
			WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Homing Daggers", us.HomingDaggers, us.HomingDaggersDiff);

			WriteTimeField(!us.IsNewPlayerOnThisLeaderboard && us.LevelUpTime2 != us.LevelUpTime2Diff, "Level 2", us.LevelUpTime2, us.LevelUpTime2Diff);
			WriteTimeField(!us.IsNewPlayerOnThisLeaderboard && us.LevelUpTime3 != us.LevelUpTime3Diff, "Level 3", us.LevelUpTime3, us.LevelUpTime3Diff);
			WriteTimeField(!us.IsNewPlayerOnThisLeaderboard && us.LevelUpTime4 != us.LevelUpTime4Diff, "Level 4", us.LevelUpTime4, us.LevelUpTime4Diff);

			static void WriteTimeField(bool writeDifference, string fieldName, int value, int valueDiff)
			{
				Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{value / 10000f,Cmd.TextWidthRight:0.0000}");
				if (writeDifference)
					Cmd.Write($" ({(valueDiff < 0 ? string.Empty : "+")}{valueDiff / 10000f:0.0000})", ColorUtils.GetImprovementColor(-valueDiff));
				Cmd.WriteLine();
			}

			static void WritePercentageField(bool writeDifference, string fieldName, double value, double valueDiff)
			{
				Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{value,Cmd.TextWidthRight:0.00%}");
				if (writeDifference)
					Cmd.Write($" ({(valueDiff < 0 ? string.Empty : "+")}{valueDiff:0.00%})", ColorUtils.GetImprovementColor(valueDiff));
				Cmd.WriteLine();
			}

			static void WriteIntField(bool writeDifference, string fieldName, int value, int valueDiff, bool negate = false)
			{
				Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{value,Cmd.TextWidthRight}");
				if (writeDifference)
					Cmd.Write($" ({valueDiff:+0;-#})", ColorUtils.GetImprovementColor(valueDiff * (negate ? -1 : 1)));
				Cmd.WriteLine();
			}
		}
	}
}
