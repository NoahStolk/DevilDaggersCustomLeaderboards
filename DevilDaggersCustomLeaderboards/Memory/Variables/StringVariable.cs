using System;
using System.Linq;
using System.Text;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public class StringVariable : AbstractVariable<string>
	{
		public StringVariable(long localBaseAddress, uint maxSize)
			: base(localBaseAddress, maxSize)
		{
		}

		public override string ValuePrevious => GetStringFromBytes(BytesPrevious.ToArray());
		public override string Value => GetStringFromBytes(Bytes.ToArray());

		private static string GetStringFromBytes(byte[] bytes)
		{
			int length = 0;
			for (int i = 0; i < bytes.Length; i++)
			{
				if (bytes[i] == 0x00)
				{
					length = i;
					break;
				}
			}

			byte[] newBytes = new byte[length];
			Buffer.BlockCopy(bytes, 0, newBytes, 0, length);

			return Encoding.UTF8.GetString(newBytes);
		}
	}
}
