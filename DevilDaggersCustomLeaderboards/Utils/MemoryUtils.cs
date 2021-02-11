using DevilDaggersCustomLeaderboards.Memory;
using DevilDaggersCustomLeaderboards.Native;
using System;

namespace DevilDaggersCustomLeaderboards.Utils
{
	public static class MemoryUtils
	{
		public static IntPtr ReadPointer(IntPtr memoryAddress)
			=> new IntPtr(BitConverter.ToInt32(Read(memoryAddress, sizeof(int))));

		public static byte[] Read(IntPtr memoryAddress, uint size)
		{
			byte[] buffer = new byte[size];
			int errorCode = NativeMethods.ReadProcessMemory(Scanner.Instance.ProcessAddress, memoryAddress, buffer, size, out _);
			if (errorCode == 0)
				throw new($"{nameof(NativeMethods.ReadProcessMemory)} failed.");
			return buffer;
		}
	}
}
