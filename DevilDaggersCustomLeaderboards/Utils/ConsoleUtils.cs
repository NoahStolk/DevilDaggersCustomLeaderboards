﻿using DevilDaggersCustomLeaderboards.Enumerators;
using System;

namespace DevilDaggersCustomLeaderboards.Utils
{
	/// <summary>
	/// Special methods are used to output to the console, as clearing the console after every update makes everything flicker which is ugly.
	/// So instead of clearing the console using <see cref="Console.Clear"/>, we just reset the cursor to the top-left, and then overwrite everything from the previous update using these methods.
	/// </summary>
	public static class ConsoleUtils
	{
#if WINDOWS
		public const int TextWidthFull = 50;
		public const int TextWidthLeft = 20;
		public const int TextWidthRight = 25;
		public const int LeftMargin = 15;
		public const int RightMargin = 10;
#elif LINUX
		public const int TextWidthFull = 30;
		public const int TextWidthLeft = 15;
		public const int TextWidthRight = 20;
		public const int LeftMargin = 10;
		public const int RightMargin = 10;
#endif

		public static void Write(object text, CustomColor foregroundColor = ColorUtils.ForegroundDefault, CustomColor backgroundColor = ColorUtils.BackgroundDefault)
		{
			Console.ForegroundColor = (ConsoleColor)foregroundColor;
			Console.BackgroundColor = (ConsoleColor)backgroundColor;
			Console.Write($"{text,-TextWidthLeft}");
		}

		public static void WriteLine()
			=> Console.WriteLine(new string(' ', TextWidthFull));

		public static void WriteLine(object text, CustomColor foregroundColor = ColorUtils.ForegroundDefault, CustomColor backgroundColor = ColorUtils.BackgroundDefault)
		{
			Console.ForegroundColor = (ConsoleColor)foregroundColor;
			Console.BackgroundColor = (ConsoleColor)backgroundColor;
			Console.WriteLine($"{text,-TextWidthLeft}");
		}

		public static void WriteLine(object textLeft, object textRight, CustomColor foregroundColor = ColorUtils.ForegroundDefault, CustomColor backgroundColor = ColorUtils.BackgroundDefault)
		{
			Console.ForegroundColor = (ConsoleColor)foregroundColor;
			Console.BackgroundColor = (ConsoleColor)backgroundColor;
			Console.WriteLine($"{textLeft,-TextWidthLeft}{textRight,TextWidthRight}{new string(' ', TextWidthFull)}");
		}
	}
}
