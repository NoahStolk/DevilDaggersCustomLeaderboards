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
	}
}