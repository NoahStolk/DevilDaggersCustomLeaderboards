using DevilDaggersCustomLeaderboards.Clients;
using System;

namespace DevilDaggersCustomLeaderboards.Utils
{
	public static class ColorUtils
	{
		public const ConsoleColor BackgroundDefault = ConsoleColor.Black;
		public const ConsoleColor BackgroundHighlight = ConsoleColor.DarkBlue;

		public const ConsoleColor Error = ConsoleColor.Red;
		public const ConsoleColor Success = ConsoleColor.Green;

		public const ConsoleColor VersionUnsupported = ConsoleColor.Red;
		public const ConsoleColor VersionOutdated = ConsoleColor.Yellow;

		public const ConsoleColor Better = ConsoleColor.Green;
		public const ConsoleColor Neutral = ConsoleColor.White;
		public const ConsoleColor Worse = ConsoleColor.Red;

		public const ConsoleColor Fallen = ConsoleColor.White;
		public const ConsoleColor Swarmed = ConsoleColor.DarkYellow;
		public const ConsoleColor Impaled = ConsoleColor.DarkYellow;
		public const ConsoleColor Gored = ConsoleColor.DarkYellow;
		public const ConsoleColor Infested = ConsoleColor.Green;
		public const ConsoleColor Opened = ConsoleColor.DarkYellow;
		public const ConsoleColor Purged = ConsoleColor.DarkYellow;
		public const ConsoleColor Desecrated = ConsoleColor.DarkYellow;
		public const ConsoleColor Sacrificed = ConsoleColor.DarkYellow;
		public const ConsoleColor Eviscerated = ConsoleColor.Gray;
		public const ConsoleColor Annihilated = ConsoleColor.DarkGreen;
		public const ConsoleColor Intoxicated = ConsoleColor.Green;
		public const ConsoleColor Envenomated = ConsoleColor.Green;
		public const ConsoleColor Incarnated = ConsoleColor.Red;
		public const ConsoleColor Discarnated = ConsoleColor.Magenta;
		public const ConsoleColor Barbed = ConsoleColor.DarkMagenta;

		public const ConsoleColor Homing = ConsoleColor.Magenta;
		public const ConsoleColor Devil = ConsoleColor.Red;
		public const ConsoleColor Golden = ConsoleColor.Yellow;
		public const ConsoleColor Silver = ConsoleColor.Gray;
		public const ConsoleColor Bronze = ConsoleColor.DarkRed;
		public const ConsoleColor Default = ConsoleColor.DarkGray;

		public static ConsoleColor GetDeathColor(int deathType) => deathType switch
		{
			1 => Swarmed,
			2 => Impaled,
			3 => Gored,
			4 => Infested,
			5 => Opened,
			6 => Purged,
			7 => Desecrated,
			8 => Sacrificed,
			9 => Eviscerated,
			10 => Annihilated,
			11 => Intoxicated,
			12 => Envenomated,
			13 => Incarnated,
			14 => Discarnated,
			15 => Barbed,
			_ => Fallen,
		};

		public static ConsoleColor GetDaggerColor(int time, CustomLeaderboard leaderboard, CustomLeaderboardCategory category)
		{
			if (leaderboard.Homing != 0 && Compare(time, leaderboard.Homing))
				return Homing;
			if (Compare(time, leaderboard.Devil))
				return Devil;
			if (Compare(time, leaderboard.Golden))
				return Golden;
			if (Compare(time, leaderboard.Silver))
				return Silver;
			if (Compare(time, leaderboard.Bronze))
				return Bronze;
			return Default;

			bool Compare(int time, int daggerTime)
			{
				if (category.Ascending)
					return time < daggerTime;
				return time > daggerTime;
			}
		}

		public static ConsoleColor GetImprovementColor<T>(T n)
			where T : IComparable<T>
		{
			int comparison = n.CompareTo(default);
			return comparison == 0 ? Neutral : comparison == 1 ? Better : Worse;
		}
	}
}