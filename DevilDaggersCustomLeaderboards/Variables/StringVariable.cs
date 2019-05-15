using System.Text;

namespace DevilDaggersCustomLeaderboards.Variables
{
	public class StringVariable : AbstractVariable<string>
	{
		public override string Value => Encoding.UTF8.GetString(GetBytes());

		public StringVariable(int localBaseAddress, int offset)
			: base(localBaseAddress, offset)
		{
		}
	}
}