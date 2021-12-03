using DevilDaggersCustomLeaderboards.Native;
using System;
using System.Diagnostics;

namespace DevilDaggersCustomLeaderboards.Utils
{
	public static class OperatingSystemUtils
	{
		public static void ReadMemory(Process process, long address, byte[] bytes, int size)
		{
			NativeMethods.ReadProcessMemory(process.Handle, new(address), bytes, (uint)size, out _);
		}

		public static long? GetMemoryBlockAddress(Process process, long ddstatsMarkerOffset)
		{
			if (process.MainModule == null)
				return null;

			byte[] pointerBytes = new byte[sizeof(long)];
			ReadMemory(process, process.MainModule.BaseAddress.ToInt64() + ddstatsMarkerOffset, pointerBytes, sizeof(long));
			return BitConverter.ToInt64(pointerBytes);
		}

		public static Process? GetDevilDaggersProcess()
		{
			foreach (Process process in Process.GetProcessesByName("dd"))
			{
				if (process.MainWindowTitle == "Devil Daggers")
					return process;
			}

			return null;
		}
	}
}
