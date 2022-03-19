using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Enums;
using DevilDaggersCustomLeaderboards.Extensions;
using DevilDaggersCustomLeaderboards.Memory.Variables;
using System;
using Cmd = DevilDaggersCustomLeaderboards.Utils.ConsoleUtils;

namespace DevilDaggersCustomLeaderboards.Utils;

public static class GuiUtils
{
	public static void WriteRecording()
	{
		Cmd.WriteLine($"Scanning process '{ScannerService.Process?.ProcessName ?? "No process"}' ({ScannerService.Process?.MainWindowTitle ?? "No title"})...");
		Cmd.WriteLine();

		Cmd.WriteLine("Player ID", ScannerService.PlayerId);
		Cmd.WriteLine("Player Name", ScannerService.PlayerName);
		Cmd.WriteLine();

#if DEBUG
		WriteDebug();
#endif

		if (ScannerService.IsInGame)
		{
			Cmd.WriteLine("Player", ScannerService.IsPlayerAlive ? "Alive" : DeathUtils.GetName(ScannerService.DeathType), ScannerService.IsPlayerAlive ? CustomColor.Gray : ColorUtils.GetDeathColor(ScannerService.DeathType));
			Cmd.WriteLine();
			Cmd.WriteLine("Time", ScannerService.Time.Value.ToString("0.0000"));
			Cmd.WriteLine();
			Cmd.WriteLine("Hand", $"Level {GetHand(ScannerService.LevelGems)}");
			Cmd.WriteLine("Level 2", ScannerService.LevelUpTime2.Value.ToString("0.0000"));
			Cmd.WriteLine("Level 3", ScannerService.LevelUpTime3.Value.ToString("0.0000"));
			Cmd.WriteLine("Level 4", ScannerService.LevelUpTime4.Value.ToString("0.0000"));
			Cmd.WriteLine();
			WriteVariable("Gems Collected", ScannerService.GemsCollected, CustomColor.Red);
			WriteVariable("Gems Despawned", ScannerService.GemsDespawned, CustomColor.Red);
			WriteVariable("Gems Eaten", ScannerService.GemsEaten, CustomColor.Green);
			WriteVariable("Gems Total", ScannerService.GemsTotal, CustomColor.Red);
			Cmd.WriteLine("Gems In Arena", Math.Max(0, ScannerService.GemsTotal - ScannerService.GemsCollected - ScannerService.GemsDespawned - ScannerService.GemsEaten));
			Cmd.WriteLine();
			WriteVariable("Homing Stored", ScannerService.HomingDaggers, CustomColor.Magenta);
			WriteVariable("Homing Eaten", ScannerService.HomingDaggersEaten, CustomColor.Ghostpede);
			Cmd.WriteLine();
			WriteEnemyHeaders("Enemies", "Alive", "Killed");
			WriteEnemyVariables("Total", ScannerService.EnemiesAlive, ScannerService.EnemiesKilled, ColorUtils.Entangled);
			WriteEnemyVariables("Skull I", ScannerService.Skull1sAlive, ScannerService.Skull1sKilled, ColorUtils.Swarmed);
			WriteEnemyVariables("Skull II", ScannerService.Skull2sAlive, ScannerService.Skull2sKilled, ColorUtils.Impaled);
			WriteEnemyVariables("Skull III", ScannerService.Skull3sAlive, ScannerService.Skull3sKilled, ColorUtils.Gored);
			WriteEnemyVariables("Skull IV", ScannerService.Skull4sAlive, ScannerService.Skull4sKilled, ColorUtils.Opened);
			WriteEnemyVariables("Squid I", ScannerService.Squid1sAlive, ScannerService.Squid1sKilled, ColorUtils.Purged);
			WriteEnemyVariables("Squid II", ScannerService.Squid2sAlive, ScannerService.Squid2sKilled, ColorUtils.Desecrated);
			WriteEnemyVariables("Squid III", ScannerService.Squid3sAlive, ScannerService.Squid3sKilled, ColorUtils.Sacrificed);
			WriteEnemyVariables("Spiderling", ScannerService.SpiderlingsAlive, ScannerService.SpiderlingsKilled, ColorUtils.Infested);
			WriteEnemyVariables("Spider I", ScannerService.Spider1sAlive, ScannerService.Spider1sKilled, ColorUtils.Intoxicated);
			WriteEnemyVariables("Spider II", ScannerService.Spider2sAlive, ScannerService.Spider2sKilled, ColorUtils.Envenomated);
			WriteEnemyVariables("Spider Egg", ScannerService.SpiderEggsAlive, ScannerService.SpiderEggsKilled, ColorUtils.Intoxicated);
			WriteEnemyVariables("Centipede", ScannerService.CentipedesAlive, ScannerService.CentipedesKilled, ColorUtils.Eviscerated);
			WriteEnemyVariables("Gigapede", ScannerService.GigapedesAlive, ScannerService.GigapedesKilled, ColorUtils.Annihilated);
			WriteEnemyVariables("Ghostpede", ScannerService.GhostpedesAlive, ScannerService.GhostpedesKilled, ColorUtils.Haunted);
			WriteEnemyVariables("Thorn", ScannerService.ThornsAlive, ScannerService.ThornsKilled, ColorUtils.Entangled);
			WriteEnemyVariables("Leviathan", ScannerService.LeviathansAlive, ScannerService.LeviathansKilled, ColorUtils.Incarnated);
			WriteEnemyVariables("Orb", ScannerService.OrbsAlive, ScannerService.OrbsKilled, ColorUtils.Discarnated);
			Cmd.WriteLine();
			WriteVariable("Daggers Hit", ScannerService.DaggersHit, CustomColor.Green);
			WriteVariable("Daggers Fired", ScannerService.DaggersFired, CustomColor.Yellow);
			Cmd.WriteLine("Accuracy", $"{(ScannerService.DaggersFired == 0 ? 0 : ScannerService.DaggersHit / (float)ScannerService.DaggersFired * 100):0.00}%");
			Cmd.WriteLine();
			Cmd.WriteLine("Leviathan Down", ScannerService.LeviathanDownTime.Value.ToString("0.0000"));
			Cmd.WriteLine("Orb Down", ScannerService.OrbDownTime.Value.ToString("0.0000"));
			Cmd.WriteLine();
		}
		else
		{
			Cmd.WriteLine("Not in game", string.Empty);
			for (int i = 0; i < 47; i++)
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
			Console.Write($"{variableHeaderLeft,Cmd.TextWidthRight - Cmd.LeftMargin}");
			Console.Write($"{variableHeaderRight,Cmd.TextWidthRight - Cmd.RightMargin}");
			Console.WriteLine($"{new string(' ', Cmd.TextWidthFull)}");
		}

		static void WriteEnemyVariables<T>(object textLeft, AbstractVariable<T> variableLeft, AbstractVariable<T> variableRight, CustomColor enemyColor, CustomColor foregroundColor = ColorUtils.ForegroundDefault, CustomColor backgroundColor = ColorUtils.BackgroundDefault)
		{
			Console.ForegroundColor = (ConsoleColor)enemyColor;
			Console.BackgroundColor = (ConsoleColor)backgroundColor;
			Console.Write($"{textLeft,-Cmd.TextWidthLeft}");

			Console.ForegroundColor = (ConsoleColor)(variableLeft.IsChanged ? enemyColor : foregroundColor);
			Console.Write($"{variableLeft,Cmd.TextWidthRight - Cmd.LeftMargin}");
			Console.ForegroundColor = (ConsoleColor)(variableRight.IsChanged ? enemyColor : foregroundColor);
			Console.Write($"{variableRight,Cmd.TextWidthRight - Cmd.RightMargin}");

			Console.BackgroundColor = (ConsoleColor)backgroundColor;
			Console.WriteLine($"{new string(' ', Cmd.TextWidthFull)}");
		}
	}

#if DEBUG
	private static void WriteDebug()
	{
		Cmd.WriteLine("Is Player Alive", Scanner.IsPlayerAlive);
		Cmd.WriteLine("Is Replay", Scanner.IsReplay);
		Cmd.WriteLine("Is In-Game", Scanner.IsInGame);
		Cmd.WriteLine("Status", (Status)Scanner.Status.Value);
		Cmd.WriteLine("SurvivalHash", HashUtils.ByteArrayToHexString(Scanner.SurvivalHashMd5));
		Cmd.WriteLine();
		Cmd.WriteLine("Replay Player ID", Scanner.ReplayPlayerId);
		Cmd.WriteLine("Replay Player Name", Scanner.ReplayPlayerName);
		Cmd.WriteLine();
		Cmd.WriteLine("Homing Max", Scanner.HomingMax);
		Cmd.WriteLine("Homing Max Time", Scanner.HomingMaxTime.Value.ToString("0.0000"));
		Cmd.WriteLine("Enemies Alive Max", Scanner.EnemiesAliveMax);
		Cmd.WriteLine("Enemies Alive Max Time", Scanner.EnemiesAliveMaxTime.Value.ToString("0.0000"));
		Cmd.WriteLine("Max Time", Scanner.MaxTime.Value.ToString("0.0000"));
		Cmd.WriteLine();
		Cmd.WriteLine("Stats Base", Scanner.StatsBase);
		Cmd.WriteLine("Stats Count", Scanner.StatsCount);
		Cmd.WriteLine("Stats Loaded", Scanner.StatsLoaded);
		Cmd.WriteLine();
		Cmd.WriteLine("Prohibited Mods", Scanner.ProhibitedMods);
		Cmd.WriteLine();
		Cmd.WriteLine("Replay Base", Scanner.ReplayBase);
		Cmd.WriteLine("Replay Length", Scanner.ReplayLength);
		Cmd.WriteLine();
		Cmd.WriteLine("Game Mode", Scanner.GameMode);
		Cmd.WriteLine("TA/R Finished", Scanner.TimeAttackOrRaceFinished);
		Cmd.WriteLine();
	}
#endif

	public static void WriteStats(GetCustomLeaderboardDdcl leaderboard, GetCustomEntryDdcl? entry)
	{
		if (entry == null)
		{
			// Should never happen.
			Cmd.WriteLine("Current player not found on the leaderboard.");
			return;
		}

		double accuracy = ScannerService.DaggersFired == 0 ? 0 : ScannerService.DaggersHit / (double)ScannerService.DaggersFired;
		double accuracyOld = entry.DaggersFired == 0 ? 0 : entry.DaggersHit / (double)entry.DaggersFired;

		Cmd.Write(DeathUtils.GetName(ScannerService.DeathType), ColorUtils.GetDeathColor(ScannerService.DeathType));
		Cmd.WriteLine();
		Cmd.WriteLine();

		int timeDiff = ScannerService.Time.ConvertToTimeInt() - entry.Time;
		Cmd.Write($"{"Time",-Cmd.TextWidthLeft}");
		Cmd.Write($"{ScannerService.Time.Value,Cmd.TextWidthRight:0.0000}", ColorUtils.GetDaggerColor(ScannerService.Time.ConvertToTimeInt(), leaderboard));
		Cmd.WriteLine($" ({(timeDiff < 0 ? string.Empty : "+")}{timeDiff / 10000f:0.0000})", ColorUtils.Worse);

#if DEBUG
		WriteDebug();
#endif

		WriteIntField("Gems Collected", ScannerService.GemsCollected, ScannerService.GemsCollected - entry.GemsCollected);
		WriteIntField("Gems Despawned", ScannerService.GemsDespawned, ScannerService.GemsDespawned - entry.GemsDespawned, true);
		WriteIntField("Gems Eaten", ScannerService.GemsEaten, ScannerService.GemsEaten - entry.GemsEaten, true);
		WriteIntField("Gems Total", ScannerService.GemsTotal, ScannerService.GemsTotal - entry.GemsTotal);
		WriteIntField("Enemies Killed", ScannerService.EnemiesKilled, ScannerService.EnemiesKilled - entry.EnemiesKilled);
		WriteIntField("Enemies Alive", ScannerService.EnemiesAlive, ScannerService.EnemiesAlive - entry.EnemiesAlive);
		WriteIntField("Daggers Fired", ScannerService.DaggersFired, ScannerService.DaggersFired - entry.DaggersFired);
		WriteIntField("Daggers Hit", ScannerService.DaggersHit, ScannerService.DaggersHit - entry.DaggersHit);
		WritePercentageField("Accuracy", accuracy, accuracy - accuracyOld);
		WriteIntField("Homing Stored", ScannerService.HomingDaggers, ScannerService.HomingDaggers - entry.HomingDaggers);
		WriteIntField("Homing Eaten", ScannerService.HomingDaggersEaten, ScannerService.HomingDaggersEaten - entry.HomingDaggersEaten);
		WriteTimeField("Level 2", ScannerService.LevelUpTime2.ConvertToTimeInt(), ScannerService.LevelUpTime2.ConvertToTimeInt() - entry.LevelUpTime2);
		WriteTimeField("Level 3", ScannerService.LevelUpTime3.ConvertToTimeInt(), ScannerService.LevelUpTime3.ConvertToTimeInt() - entry.LevelUpTime3);
		WriteTimeField("Level 4", ScannerService.LevelUpTime4.ConvertToTimeInt(), ScannerService.LevelUpTime4.ConvertToTimeInt() - entry.LevelUpTime4);

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

	public static bool IsHighscore(this GetUploadSuccess us)
		=> us.Rank != 0;

	public static void WriteLeaderboard(this GetUploadSuccess us, int currentPlayerId)
	{
		for (int i = 0; i < us.TotalPlayers; i++)
		{
			int spaceCountCurrent = (i + 1).ToString().Length;
			int spaceCountTotal = us.TotalPlayers.ToString().Length;

			GetCustomEntryDdcl entry = us.Entries[i];
			CustomColor daggerColor = ColorUtils.GetDaggerColor(entry.Time, us.Leaderboard);

			bool isCurrentPlayer = entry.PlayerId == currentPlayerId;
			CustomColor foregroundColor = isCurrentPlayer ? ColorUtils.GetDaggerHighlightColor(daggerColor) : daggerColor;
			CustomColor backgroundColor = isCurrentPlayer ? daggerColor : ColorUtils.BackgroundDefault;

			Cmd.Write($"{new string(' ', spaceCountTotal - spaceCountCurrent)}{i + 1}. ", foregroundColor, backgroundColor);
			Cmd.Write($"{entry.PlayerName[..Math.Min(entry.PlayerName.Length, Cmd.TextWidthLeft)]}", foregroundColor, backgroundColor);
			Cmd.Write($"{entry.Time / 10000f,Cmd.TextWidthRight:0.0000}\n", foregroundColor, backgroundColor);
		}

		Console.BackgroundColor = (ConsoleColor)ColorUtils.BackgroundDefault;
	}

	public static void WriteHighscoreStats(this GetUploadSuccess us)
	{
		int deathType = us.Entries[us.Rank - 1].DeathType;

		double accuracy = us.DaggersFired == 0 ? 0 : us.DaggersHit / (double)us.DaggersFired;

		int shotsHitOld = us.DaggersHit - us.DaggersHitDiff;
		int shotsFiredOld = us.DaggersFired - us.DaggersFiredDiff;
		double accuracyOld = shotsFiredOld == 0 ? 0 : shotsHitOld / (double)shotsFiredOld;
		double accuracyDiff = accuracy - accuracyOld;

		Cmd.Write(DeathUtils.GetName(ScannerService.DeathType), ColorUtils.GetDeathColor(deathType));
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
		WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Homing Stored", us.HomingDaggers, us.HomingDaggersDiff);
		WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Homing Eaten", us.HomingDaggersEaten, us.HomingDaggersEatenDiff);

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
