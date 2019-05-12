using System;

namespace DevilDaggersCustomLeaderboards
{
	public class GameVariable
	{
		public IntPtr ParentOffset { get; set; }
		public int[] Offsets { get; set; }
		public string Name { get; set; }
		public Type Type { get; set; }

		public GameVariable(IntPtr parentOffset, int[] offsets, string name, Type type)
		{
			ParentOffset = parentOffset;
			Offsets = offsets;
			Name = name;
			Type = type;
		}
	}
}