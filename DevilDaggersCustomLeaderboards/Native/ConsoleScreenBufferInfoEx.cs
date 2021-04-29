using System.Runtime.InteropServices;

namespace DevilDaggersCustomLeaderboards.Native
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct ConsoleScreenBufferInfoEx
	{
		internal int _cbSize;
		internal Coord _dwSize;
		internal Coord _dwCursorPosition;
		internal ushort _wAttributes;
		internal SmallRect _srWindow;
		internal Coord _dwMaximumWindowSize;
		internal ushort _wPopupAttributes;
		internal bool _bFullscreenSupported;

		internal ColorReference _black;
		internal ColorReference _darkBlue;
		internal ColorReference _darkGreen;
		internal ColorReference _darkCyan;
		internal ColorReference _darkRed;
		internal ColorReference _darkMagenta;
		internal ColorReference _darkYellow;
		internal ColorReference _gray;
		internal ColorReference _darkGray;
		internal ColorReference _blue;
		internal ColorReference _green;
		internal ColorReference _cyan;
		internal ColorReference _red;
		internal ColorReference _magenta;
		internal ColorReference _yellow;
		internal ColorReference _white;
	}
}
