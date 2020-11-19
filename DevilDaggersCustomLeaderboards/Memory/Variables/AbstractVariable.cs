using DevilDaggersCustomLeaderboards.Native;
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace DevilDaggersCustomLeaderboards.Memory.Variables
{
	public abstract class AbstractVariable<TVariable>
	{
		private readonly int[] _offsets;

		protected AbstractVariable(int localBaseAddress, uint size, params int[] offsets)
		{
			LocalBaseAddress = localBaseAddress;
			Size = size;
			_offsets = offsets;

			BytesPrevious = new byte[Size].ToImmutableArray();
			Bytes = new byte[Size].ToImmutableArray();
		}

		protected ImmutableArray<byte> BytesPrevious { get; private set; }
		protected ImmutableArray<byte> Bytes { get; private set; }
		public abstract TVariable ValuePrevious { get; }
		public abstract TVariable Value { get; }

		public int LocalBaseAddress { get; set; }
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
		/// <see cref="LocalBaseAddress"/> bytes ahead of the process base address brings us to 4 bytes (for a 32-bit application), which contain a memory address.
		/// </para>
		/// <para>
		/// Use that memory address and add the next offset from <see cref="_offsets"/> to it to get to the bytes that contain the actual value.
		/// Note that in the second read the process's base address is not needed.
		/// </para>
		/// </summary>
		public void Scan()
		{
			try
			{
				if (Scanner.Instance.Process?.MainModule == null)
					return;

				IntPtr ptr = ReadPointer(Scanner.Instance.Process.MainModule.BaseAddress + LocalBaseAddress);
				for (int i = 0; i < _offsets.Length - 1; i++)
					ptr = ReadPointer(ptr + _offsets[i]);
				Bytes = Read(ptr + _offsets[^1], Size).ToImmutableArray();
			}
			catch (Exception ex)
			{
				Program.Log.Error($"Error while scanning {typeof(TVariable)} variable.", ex);
			}
		}

		private static IntPtr ReadPointer(IntPtr memoryAddress)
			=> new IntPtr(BitConverter.ToInt32(Read(memoryAddress, sizeof(int))));

		private static byte[] Read(IntPtr memoryAddress, uint size)
		{
			byte[] buffer = new byte[size];
			if (NativeMethods.ReadProcessMemory(Scanner.Instance.ProcessAddress, memoryAddress, buffer, size, out _) == 0)
				throw new($"{nameof(NativeMethods.ReadProcessMemory)} failed.");
			return buffer;
		}

		public override string? ToString()
			=> Value?.ToString();

		public TVariable ToTVariable()
			=> Value;
	}
}