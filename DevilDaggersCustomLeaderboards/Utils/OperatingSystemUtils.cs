using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Exceptions;
using DevilDaggersCustomLeaderboards.Native;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DevilDaggersCustomLeaderboards.Utils
{
	public static class OperatingSystemUtils
	{
		static OperatingSystemUtils()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				OperatingSystem = OperatingSystem.Windows;
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				OperatingSystem = OperatingSystem.Linux;
			else
				OperatingSystem = OperatingSystem.None;
		}

		public static OperatingSystem OperatingSystem { get; }

		public static string GetProcessName() => OperatingSystem switch
		{
			OperatingSystem.Windows => "dd",
			OperatingSystem.Linux => "devildaggers",
			_ => throw new OperatingSystemNotSupportedException(),
		};

		public static string GetProcessWindowTitle() => OperatingSystem switch
		{
			OperatingSystem.Windows => "Devil Daggers",
			OperatingSystem.Linux => string.Empty,
			_ => throw new OperatingSystemNotSupportedException(),
		};

		public static void ReadMemory(Process process, long address, byte[] bytes, int size)
		{
			switch (OperatingSystem)
			{
				case OperatingSystem.Windows:
					NativeMethods.ReadProcessMemory(process.Handle, new(address), bytes, (uint)size, out _);
					break;
				case OperatingSystem.Linux:
					// TODO
					break;
				default:
					throw new OperatingSystemNotSupportedException();
			}
		}
	}
}
