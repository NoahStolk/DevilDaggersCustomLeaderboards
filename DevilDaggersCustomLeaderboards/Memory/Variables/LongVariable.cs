using System;
using System.Linq;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public class LongVariable : AbstractVariable<long>
	{
		public LongVariable(long localBaseAddress)
			: base(localBaseAddress, sizeof(long))
		{
		}

		public override long ValuePrevious => BitConverter.ToInt64(BytesPrevious.ToArray(), 0);
		public override long Value => BitConverter.ToInt64(Bytes.ToArray(), 0);
	}
}
