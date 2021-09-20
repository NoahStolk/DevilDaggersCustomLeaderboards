using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Enumerators;
using DevilDaggersCustomLeaderboards.Native;
using System;
using System.Runtime.InteropServices;

namespace DevilDaggersCustomLeaderboards.Utils
{
	public static class ColorUtils
	{
		public const CustomColor ForegroundDefault = CustomColor.Gray;
		public const CustomColor BackgroundDefault = CustomColor.Black;

		public const CustomColor Error = CustomColor.Red;
		public const CustomColor Warning = CustomColor.Yellow;
		public const CustomColor Success = CustomColor.Green;

		public const CustomColor Better = CustomColor.Green;
		public const CustomColor Neutral = CustomColor.Gray;
		public const CustomColor Worse = CustomColor.Red;

		public const CustomColor Fallen = CustomColor.Gray;
		public const CustomColor Swarmed = CustomColor.Skull;
		public const CustomColor Impaled = CustomColor.Skull;
		public const CustomColor Gored = CustomColor.Skull;
		public const CustomColor Infested = CustomColor.Yellow;
		public const CustomColor Opened = CustomColor.Skull;
		public const CustomColor Purged = CustomColor.Squid;
		public const CustomColor Desecrated = CustomColor.Squid;
		public const CustomColor Sacrificed = CustomColor.Squid;
		public const CustomColor Eviscerated = CustomColor.Gray;
		public const CustomColor Annihilated = CustomColor.Gigapede;
		public const CustomColor Intoxicated = CustomColor.Green;
		public const CustomColor Envenomated = CustomColor.Green;
		public const CustomColor Incarnated = CustomColor.Red;
		public const CustomColor Discarnated = CustomColor.Magenta;
		public const CustomColor Entangled = CustomColor.Thorn;
		public const CustomColor Haunted = CustomColor.Ghostpede;

		public const CustomColor Leviathan = CustomColor.LeviathanDagger;
		public const CustomColor Devil = CustomColor.Red;
		public const CustomColor Golden = CustomColor.Yellow;
		public const CustomColor Silver = CustomColor.Gray;
		public const CustomColor Bronze = CustomColor.Bronze;
		public const CustomColor Default = CustomColor.DarkGray;

		public static CustomColor GetDaggerHighlightColor(CustomColor daggerColor) => daggerColor switch
		{
			Devil or Bronze or Default => ForegroundDefault,
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

			ColorReference colorReference = new(r, g, b);
			switch (colorIndex)
			{
				case 0: csbe._black = colorReference; break;
				case 1: csbe._darkBlue = colorReference; break;
				case 2: csbe._darkGreen = colorReference; break;
				case 3: csbe._darkCyan = colorReference; break;
				case 4: csbe._darkRed = colorReference; break;
				case 5: csbe._darkMagenta = colorReference; break;
				case 6: csbe._darkYellow = colorReference; break;
				case 7: csbe._gray = colorReference; break;
				case 8: csbe._darkGray = colorReference; break;
				case 9: csbe._blue = colorReference; break;
				case 10: csbe._green = colorReference; break;
				case 11: csbe._cyan = colorReference; break;
				case 12: csbe._red = colorReference; break;
				case 13: csbe._magenta = colorReference; break;
				case 14: csbe._yellow = colorReference; break;
				case 15: csbe._white = colorReference; break;
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
			15 => Entangled,
			16 => Haunted,
			_ => Fallen,
		};

		public static CustomColor GetDaggerColor(int time, GetCustomLeaderboard leaderboard)
		{
			if (Compare(time, leaderboard.TimeLeviathan))
				return Leviathan;
			if (Compare(time, leaderboard.TimeDevil))
				return Devil;
			if (Compare(time, leaderboard.TimeGolden))
				return Golden;
			if (Compare(time, leaderboard.TimeSilver))
				return Silver;
			if (Compare(time, leaderboard.TimeBronze))
				return Bronze;
			return Default;

			bool Compare(int time, int daggerTime)
			{
				if (leaderboard.IsAscending)
					return time <= daggerTime;
				return time >= daggerTime;
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
