using DevilDaggersCustomLeaderboards.Exceptions;
using DevilDaggersCustomLeaderboards.Native;
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
					// TODO
				case Os.Linux:
					break;
				default:
					throw new OperatingSystemNotSupportedException();
			}
		}
	}
}
