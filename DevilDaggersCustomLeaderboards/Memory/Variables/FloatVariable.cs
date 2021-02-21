using System;
using System.Linq;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public class FloatVariable : AbstractVariable<float>
	{
		public FloatVariable(long localBaseAddress)
			: base(localBaseAddress, sizeof(float))
		{
		}

		public override float ValuePrevious => BitConverter.ToSingle(BytesPrevious.ToArray(), 0);
		public override float Value => BitConverter.ToSingle(Bytes.ToArray(), 0);
	}
}
