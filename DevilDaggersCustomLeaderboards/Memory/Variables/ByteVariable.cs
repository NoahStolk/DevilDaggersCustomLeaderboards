namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public class ByteVariable : AbstractVariable<byte>
	{
		public ByteVariable(long localBaseAddress)
			: base(localBaseAddress, sizeof(byte))
		{
		}

		public override byte ValuePrevious => BytesPrevious[0];
		public override byte Value => Bytes[0];
	}
}
