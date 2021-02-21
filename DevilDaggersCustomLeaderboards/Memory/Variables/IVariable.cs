namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public interface IVariable
	{
		long Address { get; set; }
		uint Size { get; set; }

		bool IsChanged { get; set; }
	}
}
