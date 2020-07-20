using System;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public class FloatVariable : AbstractVariable<float>
	{
		public FloatVariable(int localBaseAddress, int offset)
			: base(localBaseAddress, offset, sizeof(float))
		{
		}

		public override float ValuePrevious => BitConverter.ToSingle(BytesPrevious, 0);
		public override float Value => BitConverter.ToSingle(Bytes, 0);
	}
}