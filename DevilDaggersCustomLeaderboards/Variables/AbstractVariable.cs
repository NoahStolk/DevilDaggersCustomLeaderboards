using DevilDaggersCustomLeaderboards.MemoryHandling;
using System;

namespace DevilDaggersCustomLeaderboards.Variables
{
	public abstract class AbstractVariable<T>
	{
		private const uint PointerSize = 4; // For 32-bit applications

		public abstract T Value { get; }

		public int LocalBaseAddress { get; set; }
		public int Offset { get; set; }

		public AbstractVariable(int localBaseAddress, int offset)
		{
			LocalBaseAddress = localBaseAddress;
			Offset = offset;
		}

		/// <summary>
		/// Gets the bytes for this <see cref="AbstractGameVariable"/>.
		/// 
		/// Explanation:
		/// 
		/// Process.MainModule.BaseAddress is where the process has its memory start point.
		/// <see cref="LocalBaseAddress"/> bytes ahead of the process base address brings us to 4 bytes (32-bit application), which contain a memory address.
		/// 
		/// Use that memory address and add the <see cref="Offset"/> to it to get to the bytes that contain the actual value.
		/// Note that in the second read the process's base address is not needed.
		/// </summary>
		/// <returns>The bytes read from the process's memory.</returns>
		protected byte[] GetBytes()
		{
			try
			{
				Memory mem = Scanner.Instance.Memory;

				byte[] bytes = mem.Read(mem.ReadProcess.MainModule.BaseAddress + LocalBaseAddress, PointerSize, out _);
				int ptr = AddressUtils.ToDec(AddressUtils.MakeAddress(bytes));

				return mem.Read(new IntPtr(ptr) + Offset, PointerSize, out _);
			}
			catch
			{
				return new byte[4] { 0, 0, 0, 0 };
			}
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}