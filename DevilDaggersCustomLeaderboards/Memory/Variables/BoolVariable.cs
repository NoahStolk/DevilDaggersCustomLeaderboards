﻿using System;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public class BoolVariable : AbstractVariable<bool>
	{
		public override bool ValuePrevious => BitConverter.ToBoolean(BytesPrevious, 0);
		public override bool Value => BitConverter.ToBoolean(Bytes, 0);

		public BoolVariable(int localBaseAddress, int offset)
			: base(localBaseAddress, offset, 4) // Not 1 byte!
		{
		}
	}
}