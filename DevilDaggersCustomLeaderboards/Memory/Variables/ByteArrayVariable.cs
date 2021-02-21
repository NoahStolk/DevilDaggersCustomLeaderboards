namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public class ByteArrayVariable : AbstractVariable<byte[]>
	{
		public ByteArrayVariable(long localBaseAddress, uint arrayLength)
			: base(localBaseAddress, sizeof(byte) * arrayLength)
		{
		}

		public override byte[] ValuePrevious => BytesPrevious;
		public override byte[] Value => Bytes;
	}
}
