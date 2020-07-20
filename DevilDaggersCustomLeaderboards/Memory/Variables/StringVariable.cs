using System;
using System.Linq;
using System.Text;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public class StringVariable : AbstractVariable<string>
	{
		public StringVariable(int localBaseAddress, int offset, uint maxSize)
			: base(localBaseAddress, offset, maxSize)
		{
		}

		public override string ValuePrevious => GetStringFromBytes(BytesPrevious.ToArray());
		public override string Value => GetStringFromBytes(Bytes.ToArray());

		private static string GetStringFromBytes(byte[] bytes)
		{
			string str = Encoding.UTF8.GetString(bytes);
			return str.Substring(0, str.IndexOf('\0', StringComparison.InvariantCulture));
		}
	}
}