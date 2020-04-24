﻿using System;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public class IntVariable : AbstractVariable<int>
	{
		public override int ValuePrevious => BitConverter.ToInt32(BytesPrevious, 0);
		public override int Value => BitConverter.ToInt32(Bytes, 0);

		public IntVariable(int localBaseAddress, int offset)
			: base(localBaseAddress, offset, sizeof(int))
		{
		}
	}
}