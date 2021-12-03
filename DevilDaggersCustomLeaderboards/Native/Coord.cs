using System.Runtime.InteropServices;

namespace DevilDaggersCustomLeaderboards.Native;

[StructLayout(LayoutKind.Sequential)]
internal struct Coord
{
	internal short _x;
	internal short _y;
}
