using System;

namespace DevilDaggersCustomLeaderboards.Variables
{
	public class BoolVariable : AbstractVariable<bool>
	{
		public override bool Value => BitConverter.ToBoolean(GetBytes());

		public BoolVariable(int localBaseAddress, int offset)
			: base(localBaseAddress, offset)
		{
		}
	}
}