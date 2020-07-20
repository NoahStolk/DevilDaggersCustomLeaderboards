using DevilDaggersCore;
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public abstract class AbstractVariable<TVariable>
	{
		private const uint pointerSize = 4; // For 32-bit applications

		public AbstractVariable(int localBaseAddress, int offset, uint size)
		{
			LocalBaseAddress = localBaseAddress;
			Offset = offset;
			Size = size;
		}

		protected ImmutableArray<byte> BytesPrevious { get; private set; }
		protected ImmutableArray<byte> Bytes { get; private set; }
		public abstract TVariable ValuePrevious { get; }
		public abstract TVariable Value { get; }

		public int LocalBaseAddress { get; set; }
		public int Offset { get; set; }
		public uint Size { get; set; }

		public static implicit operator TVariable(AbstractVariable<TVariable> variable)
			=> variable.Value;

		public void PreScan()
			=> BytesPrevious = Bytes;

		/// <summary>
		/// Gets the bytes for this <see cref="AbstractVariable{T}"/>.
		///
		/// <see cref="ProcessModule.BaseAddress"/> is where the process has its memory start point.
		/// <see cref="LocalBaseAddress"/> bytes ahead of the process base address brings us to 4 bytes (for a 32-bit application), which contain a memory address.
		///
		/// Use that memory address and add the <see cref="Offset"/> to it to get to the bytes that contain the actual value.
		/// Note that in the second read the process's base address is not needed.
		/// </summary>
		public void Scan()
		{
			try
			{
				ProcessMemory memory = Scanner.Instance.Memory;

				byte[] bytes = memory.Read(memory.ReadProcess.MainModule.BaseAddress + LocalBaseAddress, pointerSize, out _);
				int ptr = AddressUtils.ToDec(AddressUtils.MakeAddress(bytes));

				Bytes = memory.Read(new IntPtr(ptr) + Offset, Size, out _).ToImmutableArray();
			}
			catch (Exception ex)
			{
				Logging.Log.Error($"Error while scanning {typeof(TVariable)} variable.", ex);
			}
		}

		public override string? ToString()
			=> Value?.ToString();

		public TVariable ToTVariable()
			=> Value;
	}
}