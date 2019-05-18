using System;

namespace DDCL.Variables
{
	public class BoolVariable : AbstractVariable<bool>
	{
		public override bool Value => BitConverter.ToBoolean(Bytes, 0);

		public BoolVariable(int localBaseAddress, int offset)
			: base(localBaseAddress, offset, 4) // Not 1 byte!
		{
		}
	}
}