using System;
using System.Linq;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public class ULongVariable : AbstractVariable<ulong>
	{
		public ULongVariable(long localBaseAddress)
			: base(localBaseAddress, sizeof(ulong))
		{
		}

		public override ulong ValuePrevious => BitConverter.ToUInt64(BytesPrevious.ToArray(), 0);
		public override ulong Value => BitConverter.ToUInt64(Bytes.ToArray(), 0);
	}
}
