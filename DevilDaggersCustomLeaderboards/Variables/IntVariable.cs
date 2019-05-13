using System;

namespace DevilDaggersCustomLeaderboards.Variables
{
	public class IntVariable : AbstractVariable<int>
	{
		public override int Value => BitConverter.ToInt32(GetBytes());

		public IntVariable(int localBaseAddress, int offset)
			: base(localBaseAddress, offset)
		{
		}
	}
}