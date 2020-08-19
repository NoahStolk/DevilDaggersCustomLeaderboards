using System;
using System.Diagnostics;

namespace DevilDaggersCustomLeaderboards.Memory
{
	public class ProcessMemory
	{
		private IntPtr hProcess = IntPtr.Zero;

		public ProcessMemory(Process? readProcess)
		{
			ReadProcess = readProcess;
		}

		public Process? ReadProcess { get; set; }

		public void Open()
		{
			if (ReadProcess == null)
				return;

			ProcessAccessType access = ProcessAccessType.PROCESS_VM_READ | ProcessAccessType.PROCESS_VM_WRITE | ProcessAccessType.PROCESS_VM_OPERATION;
			hProcess = NativeMethods.OpenProcess((uint)access, 1, (uint)ReadProcess.Id);
		}

		public byte[] Read(IntPtr memoryAddress, uint bytesToRead, out int bytesRead)
		{
			byte[] buffer = new byte[bytesToRead];
			if (NativeMethods.ReadProcessMemory(hProcess, memoryAddress, buffer, bytesToRead, out IntPtr ptrBytesRead) == 0)
				throw new Exception($"{nameof(NativeMethods.ReadProcessMemory)} failed.");
			bytesRead = ptrBytesRead.ToInt32();
			return buffer;
		}
	}
}