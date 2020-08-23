﻿using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Enumerators;
using DevilDaggersCustomLeaderboards.Native;
using System;
using System.Runtime.InteropServices;

namespace DevilDaggersCustomLeaderboards.Utils
{
	public static class ColorUtils
	{
		/*
		 * 0 = Black (backgroundDefault)
		 * 1 = White (defaultText, neutral, fallen)
		 * 2 = Red (error, leviathan, devil, worse)
		 * 3 = Green (success, better)
		 * 4 = Yellow (warning, golden)
		 * 5 =
		 * 6 =
		 * 7 =
		 * 8 =
		 * 9 =
		 * 10 =
		 * 11 =
		 * 12 =
		 * 13 =
		 * 14 =
		 * 15 =
		 */

		public const CustomColor ForegroundDefault = CustomColor.Gray;
		public const CustomColor BackgroundDefault = CustomColor.Black;

		public const CustomColor Error = CustomColor.Red;
		public const CustomColor Warning = CustomColor.Yellow;
		public const CustomColor Success = CustomColor.Green;

		public const CustomColor Better = CustomColor.Green;
		public const CustomColor Neutral = CustomColor.White;
		public const CustomColor Worse = CustomColor.Red;

		public const CustomColor Fallen = CustomColor.White;
		public const CustomColor Swarmed = CustomColor.DarkYellow;
		public const CustomColor Impaled = CustomColor.DarkYellow;
		public const CustomColor Gored = CustomColor.DarkYellow;
		public const CustomColor Infested = CustomColor.Green;
		public const CustomColor Opened = CustomColor.DarkYellow;
		public const CustomColor Purged = CustomColor.DarkYellow;
		public const CustomColor Desecrated = CustomColor.DarkYellow;
		public const CustomColor Sacrificed = CustomColor.DarkYellow;
		public const CustomColor Eviscerated = CustomColor.Gray;
		public const CustomColor Annihilated = CustomColor.DarkGreen;
		public const CustomColor Intoxicated = CustomColor.Green;
		public const CustomColor Envenomated = CustomColor.Green;
		public const CustomColor Incarnated = CustomColor.Red;
		public const CustomColor Discarnated = CustomColor.Magenta;
		public const CustomColor Barbed = CustomColor.DarkMagenta;

		public const CustomColor Homing = CustomColor.Magenta;
		public const CustomColor Devil = CustomColor.Red;
		public const CustomColor Golden = CustomColor.Yellow;
		public const CustomColor Silver = CustomColor.Gray;
		public const CustomColor Bronze = CustomColor.DarkRed;
		public const CustomColor Default = CustomColor.DarkGray;

		public static CustomColor GetDaggerHighlightColor(CustomColor daggerColor) => daggerColor switch
		{
			Homing => BackgroundDefault,
			Devil => ForegroundDefault,
			Golden => BackgroundDefault,
			Silver => BackgroundDefault,
			Bronze => ForegroundDefault,
			Default => ForegroundDefault,
			_ => BackgroundDefault,
		};

		public static int ModifyConsoleColor(byte colorIndex, byte r, byte g, byte b)
		{
			ConsoleScreenBufferInfoEx csbe = default;
			csbe._cbSize = Marshal.SizeOf(csbe);
			IntPtr hConsoleOutput = NativeMethods.GetStdHandle(StdHandle.OutputHandle);
			if (hConsoleOutput == new IntPtr(-1))
				return Marshal.GetLastWin32Error();

			bool brc = NativeMethods.GetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
			if (!brc)
				return Marshal.GetLastWin32Error();

			switch (colorIndex)
			{
				case 0: csbe._black = new ColorReference(r, g, b); break;
				case 1: csbe._darkBlue = new ColorReference(r, g, b); break;
				case 2: csbe._darkGreen = new ColorReference(r, g, b); break;
				case 3: csbe._darkCyan = new ColorReference(r, g, b); break;
				case 4: csbe._darkRed = new ColorReference(r, g, b); break;
				case 5: csbe._darkMagenta = new ColorReference(r, g, b); break;
				case 6: csbe._darkYellow = new ColorReference(r, g, b); break;
				case 7: csbe._gray = new ColorReference(r, g, b); break;
				case 8: csbe._darkGray = new ColorReference(r, g, b); break;
				case 9: csbe._blue = new ColorReference(r, g, b); break;
				case 10: csbe._green = new ColorReference(r, g, b); break;
				case 11: csbe._cyan = new ColorReference(r, g, b); break;
				case 12: csbe._red = new ColorReference(r, g, b); break;
				case 13: csbe._magenta = new ColorReference(r, g, b); break;
				case 14: csbe._yellow = new ColorReference(r, g, b); break;
				case 15: csbe._white = new ColorReference(r, g, b); break;
			}

			++csbe._srWindow._bottom;
			++csbe._srWindow._right;
			brc = NativeMethods.SetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
			if (!brc)
				return Marshal.GetLastWin32Error();

			return 0;
		}

		public static CustomColor GetDeathColor(int deathType) => deathType switch
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

		public static CustomColor GetDaggerColor(int time, CustomLeaderboard leaderboard, CustomLeaderboardCategory category)
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

		public static CustomColor GetImprovementColor<T>(T n)
			where T : IComparable<T>
		{
			int comparison = n.CompareTo(default);
			return comparison == 0 ? Neutral : comparison == 1 ? Better : Worse;
		}
	}
}