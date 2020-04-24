﻿using DevilDaggersCore.CustomLeaderboards;
using System;

namespace DevilDaggersCustomLeaderboards.Gui
{
	/// <summary>
	/// Special Write methods are used to output to the console, as clearing the console after every update makes everything flicker which is ugly.
	/// So instead of clearing the console using Console.Clear(), we just reset the cursor to the top-left, and then overwrite everything from the previous update using the special Write methods.
	/// </summary>
	internal static class ConsoleUtils
	{
		internal const int TextWidthFull = 50;
		internal const int TextWidthLeft = 20;
		internal const int TextWidthRight = 25;

		internal static void Write(object text, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.Write($"{text,-TextWidthLeft}");
			Console.ForegroundColor = ConsoleColor.White;
		}

		internal static void WriteLine()
		{
			Console.WriteLine(new string(' ', TextWidthFull));
		}

		internal static void WriteLine(object text, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine($"{text,-TextWidthLeft}");
			Console.ForegroundColor = ConsoleColor.White;
		}

		internal static void WriteLine(object textLeft, object textRight, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine($"{textLeft,-TextWidthLeft}{textRight,TextWidthRight}{new string(' ', TextWidthFull)}");
			Console.ForegroundColor = ConsoleColor.White;
		}

		internal static ConsoleColor GetDaggerColor(float seconds, CustomLeaderboardBase leaderboard)
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

		internal static ConsoleColor GetImprovementColor<T>(T n)
			where T : IComparable<T>
		{
			int comparison = n.CompareTo(default);
			return comparison == 0 ? ConsoleColor.White : comparison == 1 ? ConsoleColor.Green : ConsoleColor.Red;
		}
	}
}