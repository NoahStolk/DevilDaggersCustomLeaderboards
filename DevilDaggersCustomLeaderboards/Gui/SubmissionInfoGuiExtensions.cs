using DevilDaggersCore.Game;
using DevilDaggersCustomLeaderboards.Clients;
using System;
using System.Globalization;
using Cmd = DevilDaggersCustomLeaderboards.Gui.ConsoleUtils;

namespace DevilDaggersCustomLeaderboards.Gui
{
	public static class SubmissionInfoGuiExtensions
	{
		public static bool IsHighscore(this UploadSuccess us) => us.Rank != 0;

		public static void WriteLeaderboard(this UploadSuccess us, int currentPlayerId)
		{
			for (int i = 0; i < us.TotalPlayers; i++)
			{
				int spaceCountCurrent = (i + 1).ToString(CultureInfo.InvariantCulture).Length;
				int spaceCountTotal = us.TotalPlayers.ToString(CultureInfo.InvariantCulture).Length;

				CustomEntry entry = us.Entries[i];
				ConsoleColor color = Cmd.GetDaggerColor(entry.Time, us.Leaderboard, us.Category);

				if (entry.PlayerId == currentPlayerId)
					Console.BackgroundColor = ConsoleColor.DarkGray;
				Cmd.Write($"{new string(' ', spaceCountTotal - spaceCountCurrent)}{i + 1}. ");
				Cmd.Write($"{entry.Username.Substring(0, Math.Min(entry.Username.Length, Cmd.TextWidthLeft))}", color);
				Cmd.Write($"{entry.Time / 10000f,Cmd.TextWidthRight:0.0000}\n", color);
				Console.BackgroundColor = ConsoleColor.Black;
			}
		}

		public static void WriteHighscoreStats(this UploadSuccess us)
		{
			int deathType = us.Entries[us.Rank - 1].DeathType;

			double accuracy = us.DaggersFired == 0 ? 0 : us.DaggersHit / (double)us.DaggersFired;

			int shotsHitOld = us.DaggersHit - us.DaggersHitDiff;
			int shotsFiredOld = us.DaggersFired - us.DaggersFiredDiff;
			double accuracyOld = shotsFiredOld == 0 ? 0 : shotsHitOld / (double)shotsFiredOld;
			double accuracyDiff = accuracy - accuracyOld;

			Cmd.Write($"{GameInfo.GetDeathByType(deathType)?.Name ?? "Invalid death type"}", Cmd.GetDeathColor(deathType));
			Cmd.WriteLine();
			Cmd.WriteLine();

			Cmd.Write($"{$"Rank",-Cmd.TextWidthLeft}{$"{us.Rank} / {us.TotalPlayers}",Cmd.TextWidthRight}");
			if (!us.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({us.RankDiff:+0;-#})", Cmd.GetImprovementColor(us.RankDiff));
			Cmd.WriteLine();

			float time = us.Time / 10000f;
			float timeDiff = us.TimeDiff / 10000f;
			Cmd.Write($"{$"Time",-Cmd.TextWidthLeft}");
			Cmd.Write($"{time,Cmd.TextWidthRight:0.0000}", Cmd.GetDaggerColor(us.Time, us.Leaderboard, us.Category));
			if (!us.IsNewUserOnThisLeaderboard)
				Cmd.Write($" ({(timeDiff < 0 ? string.Empty : "+")}{timeDiff:0.0000})", ConsoleColor.Green);
			Cmd.WriteLine();

			WriteIntField(us.IsNewUserOnThisLeaderboard, "Kills", us.Kills, us.KillsDiff);
			WriteIntField(us.IsNewUserOnThisLeaderboard, "Gems", us.Gems, us.GemsDiff);
			WriteIntField(us.IsNewUserOnThisLeaderboard, "Daggers hit", us.DaggersHit, us.DaggersHitDiff);
			WriteIntField(us.IsNewUserOnThisLeaderboard, "Daggers fired", us.DaggersFired, us.DaggersFiredDiff);
			WritePercentageField(us.IsNewUserOnThisLeaderboard, "Accuracy", accuracy, accuracyDiff);
			WriteIntField(us.IsNewUserOnThisLeaderboard, "Enemies alive", us.EnemiesAlive, us.EnemiesAliveDiff);
			WriteIntField(us.IsNewUserOnThisLeaderboard, "Homing", us.Homing, us.HomingDiff);
			WriteTimeField(us.IsNewUserOnThisLeaderboard, "Level 2", us.LevelUpTime2, us.LevelUpTime2Diff);
			WriteTimeField(us.IsNewUserOnThisLeaderboard, "Level 3", us.LevelUpTime3, us.LevelUpTime3Diff);
			WriteTimeField(us.IsNewUserOnThisLeaderboard, "Level 4", us.LevelUpTime4, us.LevelUpTime4Diff);

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