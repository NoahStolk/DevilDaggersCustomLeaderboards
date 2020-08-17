using DevilDaggersCore.CustomLeaderboards;
using DevilDaggersCore.Game;
using System;
using System.Globalization;
using Cmd = DevilDaggersCustomLeaderboards.Gui.ConsoleUtils;

namespace DevilDaggersCustomLeaderboards.Gui
{
	public static class SubmissionInfoGuiExtensions
	{
		public static bool IsHighscore(this UploadSuccess si) => si.Rank != 0;

		public static void WriteLeaderboard(this UploadSuccess si, int currentPlayerId, string currentUsername)
		{
			for (int i = 0; i < si.TotalPlayers; i++)
			{
				int spaceCountCurrent = (i + 1).ToString(CultureInfo.InvariantCulture).Length;
				int spaceCountTotal = si.TotalPlayers.ToString(CultureInfo.InvariantCulture).Length;

				CustomEntryBase entry = si.Entries[i];
				ConsoleColor color = Cmd.GetDaggerColor(entry.Time, si.Leaderboard, si.Category);

				if (entry.PlayerId == currentPlayerId)
					Console.BackgroundColor = ConsoleColor.DarkGray;
				Cmd.Write($"{new string(' ', spaceCountTotal - spaceCountCurrent)}{i + 1}. ");
				Cmd.Write($"{currentUsername.Substring(0, Math.Min(currentUsername.Length, Cmd.TextWidthLeft))}", color);
				Cmd.Write($"{entry.Time / 10000f,Cmd.TextWidthRight:0.0000}\n", color);
				Console.BackgroundColor = ConsoleColor.Black;
			}
		}

		public static void WriteHighscoreStats(this UploadSuccess si)
		{
			int deathType = si.Entries[si.Rank - 1].DeathType;

			double accuracy = si.ShotsFired == 0 ? 0 : si.ShotsHit / (double)si.ShotsFired;

			int shotsHitOld = si.ShotsHit - si.ShotsHitDiff;
			int shotsFiredOld = si.ShotsFired - si.ShotsFiredDiff;
			double accuracyOld = shotsFiredOld == 0 ? 0 : shotsHitOld / (double)shotsFiredOld;
			double accuracyDiff = accuracy - accuracyOld;

			Cmd.Write($"{GameInfo.GetDeathByType(deathType).Name}", Cmd.GetDeathColor(deathType));
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
				Cmd.Write($" ({(timeDiff < 0 ? string.Empty : "+")}{timeDiff:0.0000})", ConsoleColor.Green);
			Cmd.WriteLine();

			WriteIntField(si.IsNewUserOnThisLeaderboard, "Kills", si.Kills, si.KillsDiff);
			WriteIntField(si.IsNewUserOnThisLeaderboard, "Gems", si.Gems, si.GemsDiff);
			WriteIntField(si.IsNewUserOnThisLeaderboard, "Shots hit", si.ShotsHit, si.ShotsHitDiff);
			WriteIntField(si.IsNewUserOnThisLeaderboard, "Shots fired", si.ShotsFired, si.ShotsFiredDiff);
			WritePercentageField(si.IsNewUserOnThisLeaderboard, "Accuracy", accuracy, accuracyDiff);
			WriteIntField(si.IsNewUserOnThisLeaderboard, "Enemies alive", si.EnemiesAlive, si.EnemiesAliveDiff);
			WriteIntField(si.IsNewUserOnThisLeaderboard, "Homing", si.Homing, si.HomingDiff);
			WriteTimeField(si.IsNewUserOnThisLeaderboard, "Level 2", si.LevelUpTime2, si.LevelUpTime2Diff);
			WriteTimeField(si.IsNewUserOnThisLeaderboard, "Level 3", si.LevelUpTime3, si.LevelUpTime3Diff);
			WriteTimeField(si.IsNewUserOnThisLeaderboard, "Level 4", si.LevelUpTime4, si.LevelUpTime4Diff);

			static void WriteTimeField(bool isNewUser, string fieldName, int value, int valueDiff)
			{
				Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{value / 10000f,Cmd.TextWidthRight:0.0000}");
				if (!isNewUser)
					Cmd.Write($" ({(valueDiff < 0 ? string.Empty : "+")}{valueDiff / 10000f:0.0000})", Cmd.GetImprovementColor(-valueDiff));
				Cmd.WriteLine();
			}

			static void WritePercentageField(bool isNewUser, string fieldName, double value, double valueDiff)
			{
				Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{value,Cmd.TextWidthRight:0.00%}");
				if (!isNewUser)
					Cmd.Write($" ({(valueDiff < 0 ? string.Empty : "+")}{valueDiff:0.00%})", Cmd.GetImprovementColor(valueDiff));
				Cmd.WriteLine();
			}

			static void WriteIntField(bool isNewUser, string fieldName, int value, int valueDiff)
			{
				Cmd.Write($"{fieldName,-Cmd.TextWidthLeft}{value,Cmd.TextWidthRight}");
				if (!isNewUser)
					Cmd.Write($" ({valueDiff:+0;-#})", Cmd.GetImprovementColor(valueDiff));
				Cmd.WriteLine();
			}
		}
	}
}