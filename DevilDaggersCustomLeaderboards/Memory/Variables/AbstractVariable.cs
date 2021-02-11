using DevilDaggersCustomLeaderboards.Utils;
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public abstract class AbstractVariable<TVariable>
	{
		protected AbstractVariable(long localBaseAddress, uint size)
		{
			LocalBaseAddress = localBaseAddress;
			Size = size;

			BytesPrevious = new byte[Size].ToImmutableArray();
			Bytes = new byte[Size].ToImmutableArray();
		}

		protected ImmutableArray<byte> BytesPrevious { get; private set; }
		protected ImmutableArray<byte> Bytes { get; private set; }
		public abstract TVariable ValuePrevious { get; }
		public abstract TVariable Value { get; }

		public long LocalBaseAddress { get; set; }
		public uint Size { get; set; }

		public static implicit operator TVariable(AbstractVariable<TVariable> variable)
			=> variable.Value;

		public void PreScan()
			=> BytesPrevious = Bytes;

		public void HardReset()
		{
			BytesPrevious = new byte[Size].ToImmutableArray();
			Bytes = new byte[Size].ToImmutableArray();
		}

		/// <summary>
		/// <para>
		/// Gets the bytes for this <see cref="AbstractVariable{T}"/>.
		/// </para>
		/// <para>
		/// <see cref="ProcessModule.BaseAddress"/> is where the process has its memory start point.
		/// <see cref="LocalBaseAddress"/> bytes ahead of the process base address brings us to 4 bytes (for a 32-bit application), which contain the value we want.
		/// </para>
		/// </summary>
		public void Scan()
		{
			try
			{
				if (Scanner.Instance.Process?.MainModule == null)
					return;

				Bytes = MemoryUtils.Read(new IntPtr(/*Scanner.Instance.Process.MainModule.BaseAddress.ToInt64() + */LocalBaseAddress), Size).ToImmutableArray();
			}
			catch (Exception ex)
			{
				Program.Log.Error($"Error while scanning {typeof(TVariable)} variable.", ex);
			}
		}

		public override string? ToString()
			=> Value?.ToString();

		public TVariable ToTVariable()
			=> Value;
	}
}
