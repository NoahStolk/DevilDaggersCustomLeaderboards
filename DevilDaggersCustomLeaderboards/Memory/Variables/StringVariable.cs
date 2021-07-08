using System;
using System.Linq;
using System.Text;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public class StringVariable : AbstractVariable<string>
	{
		public StringVariable(long localBaseAddress, uint stringLength)
			: base(localBaseAddress, stringLength)
		{
		}

		public override string ValuePrevious => GetUtf8StringFromBytes(BytesPrevious.ToArray());
		public override string Value => GetUtf8StringFromBytes(Bytes.ToArray());

		private static string GetUtf8StringFromBytes(byte[] bytes)
			=> Encoding.UTF8.GetString(bytes[0..Array.IndexOf(bytes, (byte)0)]);
	}
}
