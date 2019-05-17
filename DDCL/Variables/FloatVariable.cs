using System;

namespace DDCL.Variables
{
	public class FloatVariable : AbstractVariable<float>
	{
		public override float Value => BitConverter.ToSingle(GetBytes(), 0);

		public FloatVariable(int localBaseAddress, int offset)
			: base(localBaseAddress, offset)
		{
		}
	}
}