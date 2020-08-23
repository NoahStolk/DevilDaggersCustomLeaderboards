using System.Runtime.InteropServices;

namespace DevilDaggersCustomLeaderboards.Native
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct SmallRect
	{
		internal short _left;
		internal short _top;
		internal short _right;
		internal short _bottom;
	}
}