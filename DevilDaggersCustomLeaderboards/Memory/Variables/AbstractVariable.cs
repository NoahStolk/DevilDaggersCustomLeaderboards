using DevilDaggersCustomLeaderboards.Utils;
using System;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public abstract class AbstractVariable<TVariable>
	{
		protected AbstractVariable(long address, uint size)
		{
			Address = address;
			Size = size;
			BytesPrevious = new byte[Size];
			Bytes = new byte[Size];
		}

		protected byte[] BytesPrevious { get; private set; }
		protected byte[] Bytes { get; private set; }
		public abstract TVariable ValuePrevious { get; }
		public abstract TVariable Value { get; }

		public bool IsChanged { get; private set; }

		public long Address { get; set; }
		public uint Size { get; set; }

		public static implicit operator TVariable(AbstractVariable<TVariable> variable)
			=> variable.Value;

		public void HardReset()
		{
			BytesPrevious = new byte[Size];
			Bytes = new byte[Size];
		}

		public void Scan()
		{
			Buffer.BlockCopy(Bytes, 0, BytesPrevious, 0, (int)Size);

			try
			{
				if (Scanner.Instance.Process?.MainModule == null)
					return;

				MemoryUtils.Read(new IntPtr(Address), Bytes, Size);
				IsChanged = !AreBytesEqual();
			}
			catch (Exception ex)
			{
				Program.Log.Error($"Error while scanning {typeof(TVariable)} variable.", ex);

#if DEBUG
				throw;
#endif
			}
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
}
