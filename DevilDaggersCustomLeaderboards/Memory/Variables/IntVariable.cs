using System;
using System.Linq;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public class IntVariable : AbstractVariable<int>
	{
		public IntVariable(long localBaseAddress)
			: base(localBaseAddress, sizeof(int))
		{
		}

		public override int ValuePrevious => BitConverter.ToInt32(BytesPrevious.ToArray(), 0);
		public override int Value => BitConverter.ToInt32(Bytes.ToArray(), 0);
	}
}
