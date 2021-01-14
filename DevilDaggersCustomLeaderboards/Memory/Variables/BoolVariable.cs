using System;
using System.Linq;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public class BoolVariable : AbstractVariable<bool>
	{
		public BoolVariable(int localBaseAddress, params int[] offsets)
			: base(localBaseAddress, 4/*Not 1 byte!*/, offsets)
		{
		}

		public override bool ValuePrevious => BitConverter.ToBoolean(BytesPrevious.ToArray(), 0);
		public override bool Value => BitConverter.ToBoolean(Bytes.ToArray(), 0);
	}
}
