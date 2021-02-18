using System;
using System.Linq;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public class ShortVariable : AbstractVariable<short>
	{
		public ShortVariable(long localBaseAddress)
			: base(localBaseAddress, sizeof(short))
		{
		}

		public override short ValuePrevious => BitConverter.ToInt16(BytesPrevious.ToArray(), 0);
		public override short Value => BitConverter.ToInt16(Bytes.ToArray(), 0);
	}
}
