using DevilDaggersCustomLeaderboards.Memory;
using DevilDaggersCustomLeaderboards.Native;
using System;

namespace DevilDaggersCustomLeaderboards.Utils
{
	public static class MemoryUtils
	{
		public static void Read(IntPtr memoryAddress, byte[] bytes, uint length)
		{
			int errorCode = NativeMethods.ReadProcessMemory(Scanner.Instance.ProcessAddress, memoryAddress, bytes, length, out _);
			if (errorCode == 0)
				throw new($"{nameof(NativeMethods.ReadProcessMemory)} failed.");
		}
	}
}
