using System;
using System.Linq;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public class BoolVariable : AbstractVariable<bool>
	{
		public BoolVariable(int offset)
			: base(offset, sizeof(bool))
		{
		}

		public override bool ValuePrevious => BitConverter.ToBoolean(BytesPrevious.ToArray(), 0);
		public override bool Value => BitConverter.ToBoolean(Bytes.ToArray(), 0);
	}
}
