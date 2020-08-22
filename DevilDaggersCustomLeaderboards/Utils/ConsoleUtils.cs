using DevilDaggersCustomLeaderboards.Clients;
using System;

namespace DevilDaggersCustomLeaderboards.Utils
{
	/// <summary>
	/// Special methods are used to output to the console, as clearing the console after every update makes everything flicker which is ugly.
	/// So instead of clearing the console using <see cref="Console.Clear"/>, we just reset the cursor to the top-left, and then overwrite everything from the previous update using these methods.
	/// </summary>
	public static class ConsoleUtils
	{
		public const int TextWidthFull = 50;
		public const int TextWidthLeft = 20;
		public const int TextWidthRight = 25;

		public static void Write(object text, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.Write($"{text,-TextWidthLeft}");
			Console.ForegroundColor = ConsoleColor.White;
		}

		public static void WriteLine()
			=> Console.WriteLine(new string(' ', TextWidthFull));

		public static void WriteLine(object text, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine($"{text,-TextWidthLeft}");
			Console.ForegroundColor = ConsoleColor.White;
		}

		public static void WriteLine(object textLeft, object textRight, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine($"{textLeft,-TextWidthLeft}{textRight,TextWidthRight}{new string(' ', TextWidthFull)}");
			Console.ForegroundColor = ConsoleColor.White;
		}

		public static ConsoleColor GetDaggerColor(int time, CustomLeaderboard leaderboard, CustomLeaderboardCategory category)
		{
			if (leaderboard.Homing != 0 && Compare(time, leaderboard.Homing))
				return ConsoleColor.Magenta;
			if (Compare(time, leaderboard.Devil))
				return ConsoleColor.Red;
			if (Compare(time, leaderboard.Golden))
				return ConsoleColor.Yellow;
			if (Compare(time, leaderboard.Silver))
				return ConsoleColor.Gray;
			if (Compare(time, leaderboard.Bronze))
				return ConsoleColor.DarkRed;
			return ConsoleColor.DarkGray;

			bool Compare(int time, int daggerTime)
			{
				if (category.Ascending)
					return time < daggerTime;
				return time > daggerTime;
			}
		}

		public static ConsoleColor GetDeathColor(int deathType)
		{
			return deathType switch
			{
				1 => ConsoleColor.DarkYellow,
				2 => ConsoleColor.DarkYellow,
				3 => ConsoleColor.DarkYellow,
				4 => ConsoleColor.Green,
				5 => ConsoleColor.DarkYellow,
				6 => ConsoleColor.DarkYellow,
				7 => ConsoleColor.DarkYellow,
				8 => ConsoleColor.DarkYellow,
				9 => ConsoleColor.Gray,
				10 => ConsoleColor.DarkGreen,
				11 => ConsoleColor.Green,
				12 => ConsoleColor.Green,
				13 => ConsoleColor.Red,
				14 => ConsoleColor.Magenta,
				15 => ConsoleColor.DarkMagenta,
				_ => ConsoleColor.White,
			};
		}

		public static ConsoleColor GetImprovementColor<T>(T n)
			where T : IComparable<T>
		{
			int comparison = n.CompareTo(default);
			return comparison == 0 ? ConsoleColor.White : comparison == 1 ? ConsoleColor.Green : ConsoleColor.Red;
		}
	}
}