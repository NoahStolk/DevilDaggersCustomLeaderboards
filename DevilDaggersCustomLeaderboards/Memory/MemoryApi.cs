using System;
using System.Runtime.InteropServices;

namespace DevilDaggersCustomLeaderboards.Memory
{
	internal static class MemoryApi
	{
		[DllImport("kernel32.dll")]
		internal static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

		[DllImport("kernel32.dll")]
		internal static extern int CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll")]
		internal static extern int ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, uint size, out IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll")]
		internal static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, uint size, out IntPtr lpNumberOfBytesWritten);
	}
}