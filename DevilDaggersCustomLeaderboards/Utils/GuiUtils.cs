using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Enums;
using DevilDaggersCustomLeaderboards.Extensions;
using DevilDaggersCustomLeaderboards.Memory;
using System;
using System.Diagnostics;

namespace DevilDaggersCustomLeaderboards.Utils;

public static class GuiUtils
{
	public static void WriteRecording(Process process, MainBlock mainBlock, MainBlock mainBlockPrevious)
	{
		Cmd.WriteLine($"Scanning process '{process.ProcessName ?? "Nameless"}' ({process.MainWindowTitle ?? "No title"})...");
		Cmd.WriteLine();

		Cmd.WriteLine("Player ID", mainBlock.PlayerId);
		Cmd.WriteLine("Player Name", mainBlock.PlayerName);
		Cmd.WriteLine();

#if DEBUG
		WriteDebug();
#endif

		if (mainBlock.IsInGame)
		{
			Cmd.WriteLine("Player", mainBlock.IsPlayerAlive ? "Alive" : DeathUtils.GetName(mainBlock.DeathType), mainBlock.IsPlayerAlive ? CustomColor.Gray : ColorUtils.GetDeathColor(mainBlock.DeathType));
			Cmd.WriteLine();
			Cmd.WriteLine("Time", mainBlock.Time.ToString("0.0000"));
			Cmd.WriteLine();
			Cmd.WriteLine("Hand", $"Level {GetHand(mainBlock.LevelGems)}");
			Cmd.WriteLine("Level 2", mainBlock.LevelUpTime2.ToString("0.0000"));
			Cmd.WriteLine("Level 3", mainBlock.LevelUpTime3.ToString("0.0000"));
			Cmd.WriteLine("Level 4", mainBlock.LevelUpTime4.ToString("0.0000"));
			Cmd.WriteLine();
			WriteVariable("Gems Collected", mainBlock.GemsCollected, mainBlockPrevious.GemsCollected, CustomColor.Red);
			WriteVariable("Gems Despawned", mainBlock.GemsDespawned, mainBlockPrevious.GemsDespawned, CustomColor.Red);
			WriteVariable("Gems Eaten", mainBlock.GemsEaten, mainBlockPrevious.GemsEaten, CustomColor.Green);
			WriteVariable("Gems Total", mainBlock.GemsTotal, mainBlockPrevious.GemsTotal, CustomColor.Red);
			Cmd.WriteLine("Gems In Arena", Math.Max(0, mainBlock.GemsTotal - mainBlock.GemsCollected - mainBlock.GemsDespawned - mainBlock.GemsEaten));
			Cmd.WriteLine();
			WriteVariable("Homing Stored", mainBlock.HomingDaggers, mainBlockPrevious.HomingDaggers, CustomColor.Magenta);
			WriteVariable("Homing Eaten", mainBlock.HomingDaggersEaten, mainBlockPrevious.HomingDaggersEaten, CustomColor.Ghostpede);
			Cmd.WriteLine();
			WriteEnemyHeaders("Enemies", "Alive", "Killed");
			WriteEnemyVariables("Total", mainBlock.EnemiesAlive, mainBlock.EnemiesKilled, mainBlockPrevious.EnemiesAlive, mainBlockPrevious.EnemiesKilled, ColorUtils.Entangled);
			WriteEnemyVariables("Skull I", mainBlock.Skull1sAlive, mainBlock.Skull1sKilled, mainBlockPrevious.Skull1sAlive, mainBlockPrevious.Skull1sKilled, ColorUtils.Swarmed);
			WriteEnemyVariables("Skull II", mainBlock.Skull2sAlive, mainBlock.Skull2sKilled, mainBlockPrevious.Skull2sAlive, mainBlockPrevious.Skull2sKilled, ColorUtils.Impaled);
			WriteEnemyVariables("Skull III", mainBlock.Skull3sAlive, mainBlock.Skull3sKilled, mainBlockPrevious.Skull3sAlive, mainBlockPrevious.Skull3sKilled, ColorUtils.Gored);
			WriteEnemyVariables("Skull IV", mainBlock.Skull4sAlive, mainBlock.Skull4sKilled, mainBlockPrevious.Skull4sAlive, mainBlockPrevious.Skull4sKilled, ColorUtils.Opened);
			WriteEnemyVariables("Squid I", mainBlock.Squid1sAlive, mainBlock.Squid1sKilled, mainBlockPrevious.Squid1sAlive, mainBlockPrevious.Squid1sKilled, ColorUtils.Purged);
			WriteEnemyVariables("Squid II", mainBlock.Squid2sAlive, mainBlock.Squid2sKilled, mainBlockPrevious.Squid2sAlive, mainBlockPrevious.Squid2sKilled, ColorUtils.Desecrated);
			WriteEnemyVariables("Squid III", mainBlock.Squid3sAlive, mainBlock.Squid3sKilled, mainBlockPrevious.Squid3sAlive, mainBlockPrevious.Squid3sKilled, ColorUtils.Sacrificed);
			WriteEnemyVariables("Spiderling", mainBlock.SpiderlingsAlive, mainBlock.SpiderlingsKilled, mainBlockPrevious.SpiderlingsAlive, mainBlockPrevious.SpiderlingsKilled, ColorUtils.Infested);
			WriteEnemyVariables("Spider I", mainBlock.Spider1sAlive, mainBlock.Spider1sKilled, mainBlockPrevious.Spider1sAlive, mainBlockPrevious.Spider1sKilled, ColorUtils.Intoxicated);
			WriteEnemyVariables("Spider II", mainBlock.Spider2sAlive, mainBlock.Spider2sKilled, mainBlockPrevious.Spider2sAlive, mainBlockPrevious.Spider2sKilled, ColorUtils.Envenomated);
			WriteEnemyVariables("Spider Egg", mainBlock.SpiderEggsAlive, mainBlock.SpiderEggsKilled, mainBlockPrevious.SpiderEggsAlive, mainBlockPrevious.SpiderEggsKilled, ColorUtils.Intoxicated);
			WriteEnemyVariables("Centipede", mainBlock.CentipedesAlive, mainBlock.CentipedesKilled, mainBlockPrevious.CentipedesAlive, mainBlockPrevious.CentipedesKilled, ColorUtils.Eviscerated);
			WriteEnemyVariables("Gigapede", mainBlock.GigapedesAlive, mainBlock.GigapedesKilled, mainBlockPrevious.GigapedesAlive, mainBlockPrevious.GigapedesKilled, ColorUtils.Annihilated);
			WriteEnemyVariables("Ghostpede", mainBlock.GhostpedesAlive, mainBlock.GhostpedesKilled, mainBlockPrevious.GhostpedesAlive, mainBlockPrevious.GhostpedesKilled, ColorUtils.Haunted);
			WriteEnemyVariables("Thorn", mainBlock.ThornsAlive, mainBlock.ThornsKilled, mainBlockPrevious.ThornsAlive, mainBlockPrevious.ThornsKilled, ColorUtils.Entangled);
			WriteEnemyVariables("Leviathan", mainBlock.LeviathansAlive, mainBlock.LeviathansKilled, mainBlockPrevious.LeviathansAlive, mainBlockPrevious.LeviathansKilled, ColorUtils.Incarnated);
			WriteEnemyVariables("Orb", mainBlock.OrbsAlive, mainBlock.OrbsKilled, mainBlockPrevious.OrbsAlive, mainBlockPrevious.OrbsKilled, ColorUtils.Discarnated);
			Cmd.WriteLine();
			WriteVariable("Daggers Hit", mainBlock.DaggersHit, mainBlockPrevious.DaggersHit, CustomColor.Green);
			WriteVariable("Daggers Fired", mainBlock.DaggersFired, mainBlockPrevious.DaggersFired, CustomColor.Yellow);
			Cmd.WriteLine("Accuracy", $"{(mainBlock.DaggersFired == 0 ? 0 : mainBlock.DaggersHit / (float)mainBlock.DaggersFired * 100):0.00}%");
			Cmd.WriteLine();
			Cmd.WriteLine("Leviathan Down", mainBlock.LeviathanDownTime.ToString("0.0000"));
			Cmd.WriteLine("Orb Down", mainBlock.OrbDownTime.ToString("0.0000"));
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

		static void WriteVariable<T>(object textLeft, T value, T valuePrevious, CustomColor foregroundColorModify, CustomColor foregroundColor = ColorUtils.ForegroundDefault, CustomColor backgroundColor = ColorUtils.BackgroundDefault)
			where T : struct, IComparable<T>
		{
			Console.ForegroundColor = (ConsoleColor)foregroundColor;

			Console.BackgroundColor = (ConsoleColor)backgroundColor;
			Console.Write($"{textLeft,-Cmd.TextWidthLeft}");

			if (value.CompareTo(valuePrevious) != 0)
				Console.ForegroundColor = (ConsoleColor)foregroundColorModify;
			Console.Write($"{value,Cmd.TextWidthRight}");

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

		static void WriteEnemyVariables<T>(object textLeft, T valueLeft, T valueRight, T valueLeftPrevious, T valueRightPrevious, CustomColor enemyColor, CustomColor foregroundColor = ColorUtils.ForegroundDefault, CustomColor backgroundColor = ColorUtils.BackgroundDefault)
			where T : struct, IComparable<T>
		{
			Console.ForegroundColor = (ConsoleColor)enemyColor;
			Console.BackgroundColor = (ConsoleColor)backgroundColor;
			Console.Write($"{textLeft,-Cmd.TextWidthLeft}");

			Console.ForegroundColor = (ConsoleColor)(valueLeft.CompareTo(valueLeftPrevious) != 0 ? enemyColor : foregroundColor);
			Console.Write($"{valueLeft,Cmd.TextWidthRight - Cmd.LeftMargin}");
			Console.ForegroundColor = (ConsoleColor)(valueRight.CompareTo(valueRightPrevious) != 0 ? enemyColor : foregroundColor);
			Console.Write($"{valueRight,Cmd.TextWidthRight - Cmd.RightMargin}");

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

	public static void WriteStats(this GetUploadSuccess us, MainBlock mainBlock)
	{
		GetCustomEntryDdcl? entry = us.Entries.Find(e => e.PlayerId == mainBlock.PlayerId);
		if (entry == null)
		{
			// Should never happen.
			Cmd.WriteLine("Current player not found on the leaderboard.");
			return;
		}

		double accuracy = mainBlock.DaggersFired == 0 ? 0 : mainBlock.DaggersHit / (double)mainBlock.DaggersFired;
		double accuracyOld = entry.DaggersFired == 0 ? 0 : entry.DaggersHit / (double)entry.DaggersFired;

		Cmd.Write(DeathUtils.GetName(mainBlock.DeathType), ColorUtils.GetDeathColor(mainBlock.DeathType));
		Cmd.WriteLine();
		Cmd.WriteLine();

		int timeDiff = mainBlock.Time.ConvertToTimeInt() - entry.Time;
		Cmd.Write($"{"Time",-Cmd.TextWidthLeft}");
		Cmd.Write($"{mainBlock.Time,Cmd.TextWidthRight:0.0000}", ColorUtils.GetDaggerColor(mainBlock.Time.ConvertToTimeInt(), us.Leaderboard));
		Cmd.WriteLine($" ({(timeDiff < 0 ? string.Empty : "+")}{timeDiff / 10000f:0.0000})", ColorUtils.Worse);

#if DEBUG
		WriteDebug();
#endif

		WriteIntField("Gems Collected", mainBlock.GemsCollected, mainBlock.GemsCollected - entry.GemsCollected);
		WriteIntField("Gems Despawned", mainBlock.GemsDespawned, mainBlock.GemsDespawned - entry.GemsDespawned, true);
		WriteIntField("Gems Eaten", mainBlock.GemsEaten, mainBlock.GemsEaten - entry.GemsEaten, true);
		WriteIntField("Gems Total", mainBlock.GemsTotal, mainBlock.GemsTotal - entry.GemsTotal);
		WriteIntField("Enemies Killed", mainBlock.EnemiesKilled, mainBlock.EnemiesKilled - entry.EnemiesKilled);
		WriteIntField("Enemies Alive", mainBlock.EnemiesAlive, mainBlock.EnemiesAlive - entry.EnemiesAlive);
		WriteIntField("Daggers Fired", mainBlock.DaggersFired, mainBlock.DaggersFired - entry.DaggersFired);
		WriteIntField("Daggers Hit", mainBlock.DaggersHit, mainBlock.DaggersHit - entry.DaggersHit);
		WritePercentageField("Accuracy", accuracy, accuracy - accuracyOld);
		WriteIntField("Homing Stored", mainBlock.HomingDaggers, mainBlock.HomingDaggers - entry.HomingDaggers);
		WriteIntField("Homing Eaten", mainBlock.HomingDaggersEaten, mainBlock.HomingDaggersEaten - entry.HomingDaggersEaten);
		WriteTimeField("Level 2", mainBlock.LevelUpTime2.ConvertToTimeInt(), mainBlock.LevelUpTime2.ConvertToTimeInt() - entry.LevelUpTime2);
		WriteTimeField("Level 3", mainBlock.LevelUpTime3.ConvertToTimeInt(), mainBlock.LevelUpTime3.ConvertToTimeInt() - entry.LevelUpTime3);
		WriteTimeField("Level 4", mainBlock.LevelUpTime4.ConvertToTimeInt(), mainBlock.LevelUpTime4.ConvertToTimeInt() - entry.LevelUpTime4);

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

	public static void WriteHighscoreStats(this GetUploadSuccess us, MainBlock mainBlock)
	{
		int deathType = us.Entries[us.Rank - 1].DeathType;

		double accuracy = us.DaggersFired == 0 ? 0 : us.DaggersHit / (double)us.DaggersFired;

		int shotsHitOld = us.DaggersHit - us.DaggersHitDiff;
		int shotsFiredOld = us.DaggersFired - us.DaggersFiredDiff;
		double accuracyOld = shotsFiredOld == 0 ? 0 : shotsHitOld / (double)shotsFiredOld;
		double accuracyDiff = accuracy - accuracyOld;

		Cmd.Write(DeathUtils.GetName(mainBlock.DeathType), ColorUtils.GetDeathColor(deathType));
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
