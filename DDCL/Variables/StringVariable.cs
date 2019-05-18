using System.Text;

namespace DDCL.Variables
{
	public class StringVariable : AbstractVariable<string>
	{
		public override string Value
		{
			get
			{
				string val = Encoding.UTF8.GetString(Bytes);
				return val.Substring(0, val.IndexOf('\0'));
			}
		}

		public StringVariable(int localBaseAddress, int offset, uint size)
			: base(localBaseAddress, offset, size)
		{
		}
	}
}