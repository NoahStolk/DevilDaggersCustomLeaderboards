using System;

namespace DevilDaggersCustomLeaderboards.Memory.Variables;

public abstract class AbstractVariable<TVariable> : IVariable
{
	protected AbstractVariable(int offset, int size)
	{
		Offset = offset;
		Size = size;
		BytesPrevious = new byte[Size];
		Bytes = new byte[Size];
	}

	protected byte[] BytesPrevious { get; }
	protected byte[] Bytes { get; }
	public abstract TVariable ValuePrevious { get; }
	public abstract TVariable Value { get; }

	public int Offset { get; set; }
	public int Size { get; set; }
	public bool IsChanged { get; set; }

	public static implicit operator TVariable(AbstractVariable<TVariable> variable)
		=> variable.Value;

	public void Scan()
	{
		Buffer.BlockCopy(Bytes, 0, BytesPrevious, 0, Size);
		Buffer.BlockCopy(Scanner.Buffer, Offset, Bytes, 0, Size);
		IsChanged = !AreBytesEqual();
	}

	private bool AreBytesEqual()
	{
		for (int i = 0; i < Bytes.Length; i++)
		{
			if (Bytes[i] != BytesPrevious[i])
				return false;
		}

		return true;
	}

	public override string? ToString()
		=> Value?.ToString();

	public TVariable ToTVariable()
		=> Value;
}
