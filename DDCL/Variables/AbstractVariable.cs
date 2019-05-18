using DDCL.MemoryHandling;
using System;

namespace DDCL.Variables
{
	public abstract class AbstractVariable<T>
	{
		private const uint PointerSize = 4; // For 32-bit applications

		public byte[] Bytes { get; private set; }
		public abstract T Value { get; }

		public int LocalBaseAddress { get; set; }
		public int Offset { get; set; }
		public uint Size { get; set; }

		public AbstractVariable(int localBaseAddress, int offset, uint size)
		{
			LocalBaseAddress = localBaseAddress;
			Offset = offset;
			Size = size;
		}

		/// <summary>
		/// Gets the bytes for this <see cref="AbstractGameVariable"/>.
		/// 
		/// Process.MainModule.BaseAddress is where the process has its memory start point.
		/// <see cref="LocalBaseAddress"/> bytes ahead of the process base address brings us to 4 bytes (32-bit application), which contain a memory address.
		/// 
		/// Use that memory address and add the <see cref="Offset"/> to it to get to the bytes that contain the actual value.
		/// Note that in the second read the process's base address is not needed.
		/// </summary>
		public void Scan()
		{
			try
			{
				Memory mem = Scanner.Instance.Memory;

				byte[] bytes = mem.Read(mem.ReadProcess.MainModule.BaseAddress + LocalBaseAddress, PointerSize, out _);
				int ptr = AddressUtils.ToDec(AddressUtils.MakeAddress(bytes));

				Bytes = mem.Read(new IntPtr(ptr) + Offset, Size, out _);
			}
			catch
			{
				Bytes = new byte[4] { 0, 0, 0, 0 };
			}
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}