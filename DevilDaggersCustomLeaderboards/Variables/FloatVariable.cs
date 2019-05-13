using System;

namespace DevilDaggersCustomLeaderboards.Variables
{
	public class FloatVariable : AbstractVariable<float>
	{
		public override float Value => BitConverter.ToSingle(GetBytes());

		public FloatVariable(int localBaseAddress, int offset)
			: base(localBaseAddress, offset)
		{
		}
	}
}