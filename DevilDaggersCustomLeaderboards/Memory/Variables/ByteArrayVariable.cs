namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public class ByteArrayVariable : AbstractVariable<byte[]>
	{
		public ByteArrayVariable(int offset, int arrayLength)
			: base(offset, sizeof(byte) * arrayLength)
		{
		}

		public override byte[] ValuePrevious => BytesPrevious;
		public override byte[] Value => Bytes;
	}
}
