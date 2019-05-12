using System;

namespace DevilDaggersCustomLeaderboards
{
	public class GameVariable<T> where T : struct
	{
		public IntPtr ParentOffset { get; set; }
		public int[] Offsets { get; set; }
		public string Name { get; set; }

		public GameVariable(IntPtr parentOffset, int[] offsets, string name)
		{
			ParentOffset = parentOffset;
			Offsets = offsets;
			Name = name;
		}
	}
}