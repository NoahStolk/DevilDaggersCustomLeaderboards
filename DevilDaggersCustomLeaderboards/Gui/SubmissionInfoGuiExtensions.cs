using DevilDaggersCore.CustomLeaderboards;
using NetBase.Extensions;
using System;
using Cmd = DevilDaggersCustomLeaderboards.Gui.ConsoleUtils;

namespace DevilDaggersCustomLeaderboards.Gui
{
	internal static class SubmissionInfoGuiExtensions
	{
		internal static void WriteLeaderboard(this SubmissionInfo si, int currentPlayerId)
		{
			for (int i = 1; i <= si.TotalPlayers; i++)
			{
				int spaceCountCurrent = i.ToString().Length;
				int spaceCountTotal = si.TotalPlayers.ToString().Length;

				CustomEntryBase entry = si.Entries[i - 1];
				ConsoleColor color = GetDaggerColor(entry.Time, si.Leaderboard);

				if (entry.PlayerId == currentPlayerId)
					Console.BackgroundColor = ConsoleColor.DarkGray;
				Cmd.Write($"{new string(' ', spaceCountTotal - spaceCountCurrent)}{i}. ");
				Cmd.Write($"{entry.Username.SubstringSafe(0, Cmd.TextWidthLeft)}", color);
				Cmd.Write($"{entry.Time,Cmd.TextWidthRight:0.0000}\n", color);
				Console.BackgroundColor = ConsoleColor.Black;
			}

			ConsoleColor GetDaggerColor(float seconds, CustomLeaderboardBase leaderboard)
			{
				if (leaderboard.Homing != 0 && seconds > leaderboard.Homing)
					return ConsoleColor.Magenta;
				if (seconds > leaderboard.Devil)
					return ConsoleColor.Red;
				if (seconds > leaderboard.Golden)
					return ConsoleColor.Yellow;
				if (seconds > leaderboard.Silver)
					return ConsoleColor.Gray;
				if (seconds > leaderboard.Bronze)
					return ConsoleColor.DarkRed;
				return ConsoleColor.DarkGray;
			}
		}

		internal static void WriteStats(this SubmissionInfo si)
		{
			double accuracy = si.ShotsFired == 0 ? 0 : si.ShotsHit / (double)si.ShotsFired;

			int shotsHitOld = si.ShotsHit - si.ShotsHitDiff;
			int shotsFiredOld = si.ShotsFired - si.ShotsFiredDiff;
			double accuracyOld = shotsFiredOld == 0 ? 0 : shotsHitOld / (double)shotsFiredOld;
			double accuracyDiff = accuracy - accuracyOld;

			if (si.Rank != 0)
			{
				Cmd.Write($"{$"Rank",-Cmd.TextWidthLeft}{$"{si.Rank} / {si.TotalPlayers}",Cmd.TextWidthRight}");
				if (!si.IsNewUserOnThisLeaderboard)
					Cmd.Write($" ({si.RankDiff:+0;-#})", GetImprovementColor(si.RankDiff));
				Cmd.WriteLine();

				Cmd.Write($"{$"Time",-Cmd.TextWidthLeft}{si.Time,Cmd.TextWidthRight:0.0000}");
				if (!si.IsNewUserOnThisLeaderboard)
					Cmd.Write($" (+{si.TimeDiff:0.0000})", ConsoleColor.Green);
				Cmd.WriteLine();

				Cmd.Write($"{$"Kills",-Cmd.TextWidthLeft}{si.Kills,Cmd.TextWidthRight}");
				if (!si.IsNewUserOnThisLeaderboard)
					Cmd.Write($" ({si.KillsDiff:+0;-#})", GetImprovementColor(si.KillsDiff));
				Cmd.WriteLine();

				Cmd.Write($"{$"Gems",-Cmd.TextWidthLeft}{si.Gems,Cmd.TextWidthRight}");
				if (!si.IsNewUserOnThisLeaderboard)
					Cmd.Write($" ({si.GemsDiff:+0;-#})", GetImprovementColor(si.GemsDiff));
				Cmd.WriteLine();

				Cmd.Write($"{$"Shots Hit",-Cmd.TextWidthLeft}{si.ShotsHit,Cmd.TextWidthRight}");
				if (!si.IsNewUserOnThisLeaderboard)
					Cmd.Write($" ({si.ShotsHitDiff:+0;-#})", GetImprovementColor(si.ShotsHitDiff));
				Cmd.WriteLine();

				Cmd.Write($"{$"Shots Fired",-Cmd.TextWidthLeft}{si.ShotsFired,Cmd.TextWidthRight}");
				if (!si.IsNewUserOnThisLeaderboard)
					Cmd.Write($" ({si.ShotsFiredDiff:+0;-#})", GetImprovementColor(si.ShotsFiredDiff));
				Cmd.WriteLine();

				Cmd.Write($"{$"Accuracy",-Cmd.TextWidthLeft}{accuracy,Cmd.TextWidthRight:0.00%}");
				if (!si.IsNewUserOnThisLeaderboard)
					Cmd.Write($" ({(accuracyDiff < 0 ? "" : "+")}{accuracyDiff:0.00%})", GetImprovementColor(accuracyDiff));
				Cmd.WriteLine();

				Cmd.Write($"{$"Enemies Alive",-Cmd.TextWidthLeft}{si.EnemiesAlive,Cmd.TextWidthRight}");
				if (!si.IsNewUserOnThisLeaderboard)
					Cmd.Write($" ({si.EnemiesAliveDiff:+0;-#})", GetImprovementColor(si.EnemiesAliveDiff));
				Cmd.WriteLine();

				Cmd.Write($"{$"Homing",-Cmd.TextWidthLeft}{si.Homing,Cmd.TextWidthRight}");
				if (!si.IsNewUserOnThisLeaderboard)
					Cmd.Write($" ({si.HomingDiff:+0;-#})", GetImprovementColor(si.HomingDiff));
				Cmd.WriteLine();

				Cmd.Write($"{$"Level 2",-Cmd.TextWidthLeft}{si.LevelUpTime2,Cmd.TextWidthRight:0.0000}");
				if (!si.IsNewUserOnThisLeaderboard)
					Cmd.Write($" ({(si.LevelUpTime2Diff < 0 ? "" : "+")}{si.LevelUpTime2Diff:0.0000})", GetImprovementColor(-si.LevelUpTime2Diff));
				Cmd.WriteLine();

				Cmd.Write($"{$"Level 3",-Cmd.TextWidthLeft}{si.LevelUpTime3,Cmd.TextWidthRight:0.0000}");
				if (!si.IsNewUserOnThisLeaderboard)
					Cmd.Write($" ({(si.LevelUpTime3Diff < 0 ? "" : "+")}{si.LevelUpTime3Diff:0.0000})", GetImprovementColor(-si.LevelUpTime3Diff));
				Cmd.WriteLine();

				Cmd.Write($"{$"Level 4",-Cmd.TextWidthLeft}{si.LevelUpTime4,Cmd.TextWidthRight:0.0000}");
				if (!si.IsNewUserOnThisLeaderboard)
					Cmd.Write($" ({(si.LevelUpTime4Diff < 0 ? "" : "+")}{si.LevelUpTime4Diff:0.0000})", GetImprovementColor(-si.LevelUpTime4Diff));
				Cmd.WriteLine();

				ConsoleColor GetImprovementColor<T>(T n)
					where T : IComparable<T>
				{
					int comparison = n.CompareTo(default(T));
					return comparison == 0 ? ConsoleColor.White : comparison == 1 ? ConsoleColor.Green : ConsoleColor.Red;
				}
			}
		}
	}
}