using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Enums;
using DevilDaggersCustomLeaderboards.Memory;
using System;
using System.Diagnostics;

namespace DevilDaggersCustomLeaderboards.Utils;

public static class GuiUtils
{
	public const int PageSize = 20;

	public static void WriteRecording(Process process, MainBlock mainBlock, MainBlock mainBlockPrevious)
	{
		Cmd.WriteLine($"Scanning process '{process.ProcessName ?? "Nameless"}' ({process.MainWindowTitle ?? "No title"})...");
		Cmd.WriteLine();

		Cmd.WriteLine("Player ID", mainBlock.PlayerId);
		Cmd.WriteLine("Player Name", mainBlock.PlayerName);
		Cmd.WriteLine();

#if DEBUG
		WriteDebug(mainBlock);
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
			WriteVariable("Homing Stored", mainBlock.HomingStored, mainBlockPrevious.HomingStored, CustomColor.Magenta);
			WriteVariable("Homing Eaten", mainBlock.HomingEaten, mainBlockPrevious.HomingEaten, CustomColor.Ghostpede);
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
	private static void WriteDebug(MainBlock mainBlock)
	{
		Cmd.WriteLine("Is Player Alive", mainBlock.IsPlayerAlive);
		Cmd.WriteLine("Is Replay", mainBlock.IsReplay);
		Cmd.WriteLine("Is In-Game", mainBlock.IsInGame);
		Cmd.WriteLine("Status", (GameStatus)mainBlock.Status);
		Cmd.WriteLine("SurvivalHash", HashUtils.ByteArrayToHexString(mainBlock.SurvivalHashMd5));
		Cmd.WriteLine();
		Cmd.WriteLine("Replay Player ID", mainBlock.ReplayPlayerId);
		Cmd.WriteLine("Replay Player Name", mainBlock.ReplayPlayerName);
		Cmd.WriteLine();
		Cmd.WriteLine("Homing Max", mainBlock.HomingMax);
		Cmd.WriteLine("Homing Max Time", mainBlock.HomingMaxTime.ToString("0.0000"));
		Cmd.WriteLine("Enemies Alive Max", mainBlock.EnemiesAliveMax);
		Cmd.WriteLine("Enemies Alive Max Time", mainBlock.EnemiesAliveMaxTime.ToString("0.0000"));
		Cmd.WriteLine("Max Time", mainBlock.MaxTime.ToString("0.0000"));
		Cmd.WriteLine();
		Cmd.WriteLine("Stats Base", mainBlock.StatsBase);
		Cmd.WriteLine("Stats Count", mainBlock.StatsCount);
		Cmd.WriteLine("Stats Loaded", mainBlock.StatsLoaded);
		Cmd.WriteLine();
		Cmd.WriteLine("Prohibited Mods", mainBlock.ProhibitedMods);
		Cmd.WriteLine();
		Cmd.WriteLine("Replay Base", mainBlock.ReplayBase);
		Cmd.WriteLine("Replay Length", mainBlock.ReplayLength);
		Cmd.WriteLine();
		Cmd.WriteLine("Game Mode", mainBlock.GameMode);
		Cmd.WriteLine("TA/R Finished", mainBlock.TimeAttackOrRaceFinished);
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

		double timeDiff = mainBlock.Time - entry.TimeInSeconds;
		Cmd.Write($"{"Time",-Cmd.TextWidthLeft}");
		Cmd.Write($"{mainBlock.Time,Cmd.TextWidthRight:0.0000}", ColorUtils.GetDaggerColor(mainBlock.Time, us.Leaderboard));
		Cmd.WriteLine($" ({(timeDiff < 0 ? string.Empty : "+")}{timeDiff:0.0000})", ColorUtils.Worse);

#if DEBUG
		WriteDebug(mainBlock);
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
		WriteIntField("Homing Stored", mainBlock.HomingStored, mainBlock.HomingStored - entry.HomingStored);
		WriteIntField("Homing Eaten", mainBlock.HomingEaten, mainBlock.HomingEaten - entry.HomingEaten);
		WriteTimeField("Level 2", mainBlock.LevelUpTime2, mainBlock.LevelUpTime2 - entry.LevelUpTime2InSeconds);
		WriteTimeField("Level 3", mainBlock.LevelUpTime3, mainBlock.LevelUpTime3 - entry.LevelUpTime3InSeconds);
		WriteTimeField("Level 4", mainBlock.LevelUpTime4, mainBlock.LevelUpTime4 - entry.LevelUpTime4InSeconds);

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

		static void WriteTimeField(string fieldName, double value, double valueDiff)
		{
			Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{(value == 0 ? "N/A" : $"{value:0.0000}"),Cmd.TextWidthRight:0.0000}");
			if (value == 0 || valueDiff == value)
				Cmd.WriteLine();
			else
				Cmd.WriteLine($" ({(valueDiff < 0 ? string.Empty : "+")}{valueDiff:0.0000})", ColorUtils.GetImprovementColor(-valueDiff));
		}
	}

	public static void WriteLeaderboard(this GetUploadSuccess us, int currentPlayerId, int selectedIndex, int pageIndex)
	{
		int pageCount = (int)Math.Ceiling(us.TotalPlayers / (float)PageSize);

		Cmd.WriteLine("Upload successful", ColorUtils.Success);
		Cmd.WriteLine(us.Message);
		Cmd.WriteLine();
		Cmd.WriteLine("Use the arrow keys to navigate. Press [Enter] to load selected replay into Devil Daggers.");
		Cmd.WriteLine();
		Cmd.WriteLine($"PAGE {pageIndex + 1} / {pageCount}");
		Cmd.WriteLine();

		pageIndex = Math.Min(pageIndex, pageCount - 1);

		int start = pageIndex * PageSize;
		for (int i = start; i < start + PageSize; i++)
		{
			if (i >= us.Entries.Count)
			{
				Cmd.WriteLine();
				continue;
			}

			GetCustomEntryDdcl entry = us.Entries[i];
			CustomColor daggerColor = ColorUtils.GetDaggerColor(entry.TimeInSeconds, us.Leaderboard);

			bool isCurrentPlayer = entry.PlayerId == currentPlayerId;
			CustomColor foregroundColor = isCurrentPlayer ? ColorUtils.GetDaggerHighlightColor(daggerColor) : daggerColor;
			CustomColor backgroundColor = isCurrentPlayer ? daggerColor : ColorUtils.BackgroundDefault;

			int spaceCountCurrent = (i + 1).ToString().Length;
			int spaceCountTotal = us.TotalPlayers.ToString().Length;
			Cmd.Write($"{new string(' ', spaceCountTotal - spaceCountCurrent)}{i + 1}. ", foregroundColor, backgroundColor);
			Cmd.Write($"{entry.PlayerName[..Math.Min(entry.PlayerName.Length, Cmd.TextWidthLeft)]}", foregroundColor, backgroundColor);
			Cmd.Write($"{entry.TimeInSeconds,Cmd.TextWidthRight:0.0000}", foregroundColor, backgroundColor);

			const int replayUiSize = 25;
			if (selectedIndex == i)
				Cmd.WriteLine(entry.HasReplay ? " < Watch replay".PadRight(replayUiSize) : " < Replay not available".PadRight(replayUiSize), entry.HasReplay ? CustomColor.Green : CustomColor.Yellow, CustomColor.Black);
			else
				Cmd.WriteLine(new string(' ', replayUiSize));
		}

		Console.BackgroundColor = (ConsoleColor)ColorUtils.BackgroundDefault;
	}

	public static void WriteHighscoreStats(this GetUploadSuccess us, MainBlock mainBlock)
	{
		int deathType = us.Entries[us.RankState.Value - 1].DeathType;

		double accuracy = us.DaggersFiredState.Value == 0 ? 0 : us.DaggersHitState.Value / (double)us.DaggersFiredState.Value;

		int shotsHitOld = us.DaggersHitState.Value - us.DaggersHitState.ValueDifference;
		int shotsFiredOld = us.DaggersFiredState.Value - us.DaggersFiredState.ValueDifference;
		double accuracyOld = shotsFiredOld == 0 ? 0 : shotsHitOld / (double)shotsFiredOld;
		double accuracyDiff = accuracy - accuracyOld;

		Cmd.Write(DeathUtils.GetName(mainBlock.DeathType), ColorUtils.GetDeathColor(deathType));
		Cmd.WriteLine();
		Cmd.WriteLine();

		Cmd.Write($"{"Rank",-Cmd.TextWidthLeft}{$"{us.RankState.Value} / {us.TotalPlayers}",Cmd.TextWidthRight}");
		if (!us.IsNewPlayerOnThisLeaderboard)
			Cmd.Write($" ({us.RankState.ValueDifference:+0;-#})", ColorUtils.GetImprovementColor(us.RankState.ValueDifference));
		Cmd.WriteLine();

		double time = us.TimeState.Value;
		double timeDiff = us.TimeState.ValueDifference;
		Cmd.Write($"{"Time",-Cmd.TextWidthLeft}");
		Cmd.Write($"{time,Cmd.TextWidthRight:0.0000}", ColorUtils.GetDaggerColor(us.TimeState.Value, us.Leaderboard));
		if (!us.IsNewPlayerOnThisLeaderboard)
			Cmd.Write($" ({(timeDiff < 0 ? string.Empty : "+")}{timeDiff:0.0000})", ColorUtils.Better);
		Cmd.WriteLine();

		WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Gems Collected", us.GemsCollectedState.Value, us.GemsCollectedState.ValueDifference);
		WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Gems Despawned", us.GemsDespawnedState.Value, us.GemsDespawnedState.ValueDifference, true);
		WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Gems Eaten", us.GemsEatenState.Value, us.GemsEatenState.ValueDifference, true);
		WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Gems Total", us.GemsTotalState.Value, us.GemsTotalState.ValueDifference);
		WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Enemies Killed", us.EnemiesKilledState.Value, us.EnemiesKilledState.ValueDifference);
		WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Enemies Alive", us.EnemiesAliveState.Value, us.EnemiesAliveState.ValueDifference);
		WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Daggers Fired", us.DaggersFiredState.Value, us.DaggersFiredState.ValueDifference);
		WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Daggers Hit", us.DaggersHitState.Value, us.DaggersHitState.ValueDifference);
		WritePercentageField(!us.IsNewPlayerOnThisLeaderboard, "Accuracy", accuracy, accuracyDiff);
		WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Homing Stored", us.HomingStoredState.Value, us.HomingStoredState.ValueDifference);
		WriteIntField(!us.IsNewPlayerOnThisLeaderboard, "Homing Eaten", us.HomingEatenState.Value, us.HomingEatenState.ValueDifference);

		WriteTimeField(!us.IsNewPlayerOnThisLeaderboard && us.LevelUpTime2State.Value != us.LevelUpTime2State.ValueDifference, "Level 2", us.LevelUpTime2State.Value, us.LevelUpTime2State.ValueDifference);
		WriteTimeField(!us.IsNewPlayerOnThisLeaderboard && us.LevelUpTime3State.Value != us.LevelUpTime3State.ValueDifference, "Level 3", us.LevelUpTime3State.Value, us.LevelUpTime3State.ValueDifference);
		WriteTimeField(!us.IsNewPlayerOnThisLeaderboard && us.LevelUpTime4State.Value != us.LevelUpTime4State.ValueDifference, "Level 4", us.LevelUpTime4State.Value, us.LevelUpTime4State.ValueDifference);

		static void WriteTimeField(bool writeDifference, string fieldName, double value, double valueDiff)
		{
			Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{value,Cmd.TextWidthRight:0.0000}");
			if (writeDifference)
				Cmd.Write($" ({(valueDiff < 0 ? string.Empty : "+")}{valueDiff:0.0000})", ColorUtils.GetImprovementColor(-valueDiff));
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
