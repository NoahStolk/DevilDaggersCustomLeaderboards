using DevilDaggersCore.CustomLeaderboards;
using NetBase.Extensions;
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
				ConsoleColor color = Cmd.GetDaggerColor(entry.Time, si.Leaderboard);

				if (entry.PlayerId == currentPlayerId)
					Console.BackgroundColor = ConsoleColor.DarkGray;
				Cmd.Write($"{new string(' ', spaceCountTotal - spaceCountCurrent)}{i + 1}. ");
				Cmd.Write($"{entry.Username.SubstringSafe(0, Cmd.TextWidthLeft)}", color);
				Cmd.Write($"{entry.Time,Cmd.TextWidthRight:0.0000}\n", color);
				Console.BackgroundColor = ConsoleColor.Black;
			}
		}

		internal static void WriteHighscoreStats(this SubmissionInfo si)
		{
			double accuracy = si.ShotsFired == 0 ? 0 : si.ShotsHit / (double)si.ShotsFired;

			int shotsHitOld = si.ShotsHit - si.ShotsHitDiff;
			int shotsFiredOld = si.ShotsFired - si.ShotsFiredDiff;
			double accuracyOld = shotsFiredOld == 0 ? 0 : shotsHitOld / (double)shotsFiredOld;
			double accuracyDiff = accuracy - accuracyOld;

			Cmd.Write($"{$"Rank",-Cmd.TextWidthLeft}{$"{si.Rank} / {si.TotalPlayers}",Cmd.TextWidthRight}");
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({si.RankDiff:+0;-#})", Cmd.GetImprovementColor(si.RankDiff));
			Cmd.WriteLine();

			Cmd.Write($"{$"Time",-Cmd.TextWidthLeft}");
			Cmd.Write($"{si.Time,Cmd.TextWidthRight:0.0000}", Cmd.GetDaggerColor(si.Time, si.Leaderboard));
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" (+{si.TimeDiff:0.0000})", ConsoleColor.Green);
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

			Cmd.Write($"{$"Level 2",-Cmd.TextWidthLeft}{si.LevelUpTime2,Cmd.TextWidthRight:0.0000}");
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({(si.LevelUpTime2Diff < 0 ? "" : "+")}{si.LevelUpTime2Diff:0.0000})", Cmd.GetImprovementColor(-si.LevelUpTime2Diff));
			Cmd.WriteLine();

			Cmd.Write($"{$"Level 3",-Cmd.TextWidthLeft}{si.LevelUpTime3,Cmd.TextWidthRight:0.0000}");
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({(si.LevelUpTime3Diff < 0 ? "" : "+")}{si.LevelUpTime3Diff:0.0000})", Cmd.GetImprovementColor(-si.LevelUpTime3Diff));
			Cmd.WriteLine();

			Cmd.Write($"{$"Level 4",-Cmd.TextWidthLeft}{si.LevelUpTime4,Cmd.TextWidthRight:0.0000}");
			if (!si.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({(si.LevelUpTime4Diff < 0 ? "" : "+")}{si.LevelUpTime4Diff:0.0000})", Cmd.GetImprovementColor(-si.LevelUpTime4Diff));
			Cmd.WriteLine();
		}
	}
}