using System.Text;

namespace DDCL.Variables
{
	public class StringVariable : AbstractVariable<string>
	{
		public override string ValuePrevious => GetStringFromBytes(BytesPrevious);
		public override string Value => GetStringFromBytes(Bytes);

		public StringVariable(int localBaseAddress, int offset, uint size)
			: base(localBaseAddress, offset, size)
		{
		}

		private string GetStringFromBytes(byte[] bytes)
		{
			string val = Encoding.UTF8.GetString(bytes);
			return val.Substring(0, val.IndexOf('\0'));
		}
	}
}