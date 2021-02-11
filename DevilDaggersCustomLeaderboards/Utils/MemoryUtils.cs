using DevilDaggersCustomLeaderboards.Memory;
using DevilDaggersCustomLeaderboards.Native;
using System;

namespace DevilDaggersCustomLeaderboards.Utils
{
	public static class MemoryUtils
	{
		// TODO: Remove.
		public static byte[] Read(IntPtr memoryAddress, uint size)
		{
			byte[] buffer = new byte[size];
			int errorCode = NativeMethods.ReadProcessMemory(Scanner.Instance.ProcessAddress, memoryAddress, buffer, size, out _);
			if (errorCode == 0)
				throw new($"{nameof(NativeMethods.ReadProcessMemory)} failed.");
			return buffer;
		}

		public static void Read(IntPtr memoryAddress, byte[] bytes, uint length)
		{
			int errorCode = NativeMethods.ReadProcessMemory(Scanner.Instance.ProcessAddress, memoryAddress, bytes, length, out _);
			if (errorCode == 0)
				throw new($"{nameof(NativeMethods.ReadProcessMemory)} failed.");
		}
	}
}
