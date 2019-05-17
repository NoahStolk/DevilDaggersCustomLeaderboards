using System;

namespace DDCL.Variables
{
	public class IntVariable : AbstractVariable<int>
	{
		public override int Value => BitConverter.ToInt32(GetBytes(), 0);

		public IntVariable(int localBaseAddress, int offset)
			: base(localBaseAddress, offset, sizeof(int))
		{
		}
	}
}