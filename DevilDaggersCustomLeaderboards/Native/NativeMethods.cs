using DevilDaggersCustomLeaderboards.Exceptions;
using DevilDaggersCustomLeaderboards.Utils;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DevilDaggersCustomLeaderboards.Native
{
	internal static class NativeMethods
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref ConsoleScreenBufferInfoEx csbe);

		[DllImport("kernel32.dll", ExactSpelling = true)]
		internal static extern IntPtr GetConsoleWindow();

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr GetStdHandle(StdHandle index);

		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

		[DllImport("kernel32.dll")]
		private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, uint size, out uint lpNumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool SetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref ConsoleScreenBufferInfoEx csbe);

		[DllImport("user32.dll")]
		internal static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

		[DllImport("user32.dll")]
		internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("libc")]
		private static extern int read(int handle, byte[] buf, int n);

		public static void ReadMemory(nint processAddress, long address, byte[] bytes, int size)
		{
			Action call = OperatingSystemUtils.OperatingSystem switch
			{
				Clients.OperatingSystem.Windows => () => ReadProcessMemory(processAddress, new(address), bytes, (uint)size, out _),
				Clients.OperatingSystem.Linux => () => read((int)(processAddress + address), bytes, size),
				_ => throw new OperatingSystemNotSupportedException(),
			};
			call();
		}

		public static nint OpenProcess(Process process) => OperatingSystemUtils.OperatingSystem switch
		{
			Clients.OperatingSystem.Windows => OpenProcess((uint)ProcessAccessType.PROCESS_VM_READ, 1, (uint)process.Id),
			Clients.OperatingSystem.Linux => process.Handle,
			_ => throw new OperatingSystemNotSupportedException(),
		};
	}
}
