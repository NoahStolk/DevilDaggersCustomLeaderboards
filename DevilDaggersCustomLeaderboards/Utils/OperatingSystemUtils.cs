using DevilDaggersCustomLeaderboards.Exceptions;
using DevilDaggersCustomLeaderboards.Native;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Os = DevilDaggersCustomLeaderboards.Clients.OperatingSystem;

namespace DevilDaggersCustomLeaderboards.Utils
{
	public static class OperatingSystemUtils
	{
		static OperatingSystemUtils()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				OperatingSystem = Os.Windows;
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				OperatingSystem = Os.Linux;
			else
				OperatingSystem = Os.None;
		}

		public static Os OperatingSystem { get; }

		public static string GetProcessName() => OperatingSystem switch
		{
			Os.Windows => "dd",
			Os.Linux => "devildaggers",
			_ => throw new OperatingSystemNotSupportedException(),
		};

		public static string GetProcessWindowTitle() => OperatingSystem switch
		{
			Os.Windows => "Devil Daggers",
			Os.Linux => string.Empty,
			_ => throw new OperatingSystemNotSupportedException(),
		};

		public static void ReadMemory(Process process, long address, byte[] bytes, int size)
		{
			switch (OperatingSystem)
			{
				case Os.Windows:
					NativeMethods.ReadProcessMemory(process.Handle, new(address), bytes, (uint)size, out _);
					break;
				case Os.Linux:
					// TODO
					break;
				default:
					throw new OperatingSystemNotSupportedException();
			}
		}

		public static long? GetMemoryBlockAddress(Process process, long ddstatsMarkerOffset)
		{
			if (process.MainModule == null)
				return null;

			byte[] pointerBytes = new byte[sizeof(long)];
			if (OperatingSystem == Os.Windows)
			{
				ReadMemory(process, process.MainModule.BaseAddress.ToInt64() + ddstatsMarkerOffset, pointerBytes, sizeof(long));
				return BitConverter.ToInt64(pointerBytes);
			}
			else if (OperatingSystem == Os.Linux)
			{
				// TODO
				return null;
			}
			else
			{
				throw new OperatingSystemNotSupportedException();
			}
		}
	}
}
