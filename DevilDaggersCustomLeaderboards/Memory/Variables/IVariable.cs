namespace DevilDaggersCustomLeaderboards.Memory.Variables;

public interface IVariable
{
	int Offset { get; set; }
	int Size { get; set; }
	bool IsChanged { get; set; }
}
