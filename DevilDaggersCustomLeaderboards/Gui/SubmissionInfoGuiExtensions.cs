using DevilDaggersCore.CustomLeaderboards;
using DevilDaggersCore.Game;
using System;
using Cmd = DevilDaggersCustomLeaderboards.Gui.ConsoleUtils;

namespace DevilDaggersCustomLeaderboards.Gui
{
	internal static class SubmissionInfoGuiExtensions
	{
		internal static bool IsHighscore(this SubmissionInfo si) => si.Rank != 0;

		internal static void WriteLeaderboard(this SubmissionInfo si, int currentPlayerId)
		{
			for (int i = 0; i < si.TotalPlayers; i++)
			{
				int spaceCountCurrent = (i + 1).ToString().Length;
				int spaceCountTotal = si.TotalPlayers.ToString().Length;

				CustomEntryBase entry = si.Entries[i];
				ConsoleColor color = Cmd.GetDaggerColor(entry.Time, si.Leaderboard, si.Category);

				if (entry.PlayerId == currentPlayerId)
					Console.BackgroundColor = ConsoleColor.DarkGray;
				Cmd.Write($"{new string(' ', spaceCountTotal - spaceCountCurrent)}{i + 1}. ");
				Cmd.Write($"{entry.Username.Substring(0, Math.Min(entry.Username.Length, Cmd.TextWidthLeft))}", color);
				Cmd.Write($"{entry.Time / 10000f,Cmd.TextWidthRight:0.0000}\n", color);
				Console.BackgroundColor = ConsoleColor.Black;
			}
		}

		internal static void WriteHighscoreStats(this SubmissionInfo si)
		{
			int deathType = si.Entries[si.Rank - 1].DeathType;

			double accuracy = si.ShotsFired == 0 ? 0 : si.ShotsHit / (double)si.ShotsFired;

			int shotsHitOld = si.ShotsHit - si.ShotsHitDiff;
			int shotsFiredOld = si.ShotsFired - si.ShotsFiredDiff;
			double accuracyOld = shotsFiredOld == 0 ? 0 : shotsHitOld / (double)shotsFiredOld;
			double accuracyDiff = accuracy - accuracyOld;

			Cmd.Write($"{GameInfo.GetDeathFromDeathType(deathType).Name}", Cmd.GetDeathColor(deathType));
			Cmd.WriteLine();
			Cmd.WriteLine();

			Cmd.Write($"{$"Rank",-Cmd.TextWidthLeft}{$"{si.Rank} / {si.TotalPlayers}",Cmd.TextWidthRight}");
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({si.RankDiff:+0;-#})", Cmd.GetImprovementColor(si.RankDiff));
			Cmd.WriteLine();

			float time = si.Time / 10000f;
			float timeDiff = si.TimeDiff / 10000f;
			Cmd.Write($"{$"Time",-Cmd.TextWidthLeft}");
			Cmd.Write($"{time,Cmd.TextWidthRight:0.0000}", Cmd.GetDaggerColor(si.Time, si.Leaderboard, si.Category));
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({(timeDiff < 0 ? "" : "+")}{timeDiff:0.0000})", ConsoleColor.Green);
			Cmd.WriteLine();

			Cmd.Write($"{$"Kills",-Cmd.TextWidthLeft}{si.Kills,Cmd.TextWidthRight}");
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({si.KillsDiff:+0;-#})", Cmd.GetImprovementColor(si.KillsDiff));
			Cmd.WriteLine();

			Cmd.Write($"{$"Gems",-Cmd.TextWidthLeft}{si.Gems,Cmd.TextWidthRight}");
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({si.GemsDiff:+0;-#})", Cmd.GetImprovementColor(si.GemsDiff));
			Cmd.WriteLine();

			Cmd.Write($"{$"Shots Hit",-Cmd.TextWidthLeft}{si.ShotsHit,Cmd.TextWidthRight}");
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({si.ShotsHitDiff:+0;-#})", Cmd.GetImprovementColor(si.ShotsHitDiff));
			Cmd.WriteLine();

			Cmd.Write($"{$"Shots Fired",-Cmd.TextWidthLeft}{si.ShotsFired,Cmd.TextWidthRight}");
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({si.ShotsFiredDiff:+0;-#})", Cmd.GetImprovementColor(si.ShotsFiredDiff));
			Cmd.WriteLine();

			Cmd.Write($"{$"Accuracy",-Cmd.TextWidthLeft}{accuracy,Cmd.TextWidthRight:0.00%}");
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({(accuracyDiff < 0 ? "" : "+")}{accuracyDiff:0.00%})", Cmd.GetImprovementColor(accuracyDiff));
			Cmd.WriteLine();

			Cmd.Write($"{$"Enemies Alive",-Cmd.TextWidthLeft}{si.EnemiesAlive,Cmd.TextWidthRight}");
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({si.EnemiesAliveDiff:+0;-#})", Cmd.GetImprovementColor(si.EnemiesAliveDiff));
			Cmd.WriteLine();

			Cmd.Write($"{$"Homing",-Cmd.TextWidthLeft}{si.Homing,Cmd.TextWidthRight}");
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({si.HomingDiff:+0;-#})", Cmd.GetImprovementColor(si.HomingDiff));
			Cmd.WriteLine();

			float level2 = si.LevelUpTime2 / 10000f;
			float level2Diff = si.LevelUpTime2Diff / 10000f;
			Cmd.Write($"{$"Level 2",-Cmd.TextWidthLeft}{level2,Cmd.TextWidthRight:0.0000}");
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({(level2Diff < 0 ? "" : "+")}{level2Diff:0.0000})", Cmd.GetImprovementColor(-level2Diff));
			Cmd.WriteLine();

			float level3 = si.LevelUpTime3 / 10000f;
			float level3Diff = si.LevelUpTime3Diff / 10000f;
			Cmd.Write($"{$"Level 3",-Cmd.TextWidthLeft}{level3,Cmd.TextWidthRight:0.0000}");
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({(level3Diff < 0 ? "" : "+")}{level3Diff:0.0000})", Cmd.GetImprovementColor(-level3Diff));
			Cmd.WriteLine();

			float level4 = si.LevelUpTime4 / 10000f;
			float level4Diff = si.LevelUpTime4Diff / 10000f;
			Cmd.Write($"{$"Level 4",-Cmd.TextWidthLeft}{level4,Cmd.TextWidthRight:0.0000}");
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({(level4Diff < 0 ? "" : "+")}{level4Diff:0.0000})", Cmd.GetImprovementColor(-level4Diff));
			Cmd.WriteLine();
		}
	}
}