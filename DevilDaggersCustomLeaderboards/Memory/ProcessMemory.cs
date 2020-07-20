using System;
using System.Diagnostics;

namespace DevilDaggersCustomLeaderboards.Memory
{
	public class ProcessMemory
	{
		private IntPtr hProcess = IntPtr.Zero;

		public ProcessMemory(Process readProcess)
		{
			ReadProcess = readProcess;
		}

		public Process ReadProcess { get; set; }

		public void Open()
		{
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

		public byte[] PointerRead(IntPtr memoryAddress, uint bytesToRead, int[] offset, out int bytesRead)
		{
			int iPointerCount = offset.Length - 1;
			IntPtr ptrBytesRead;
			bytesRead = 0;
			byte[] buffer = new byte[4]; // DWORD to hold an Address
			int tempAddress = 0;

			if (iPointerCount == 0)
			{
				if (NativeMethods.ReadProcessMemory(hProcess, memoryAddress, buffer, 4, out _) == 0)
					throw new Exception($"{nameof(NativeMethods.ReadProcessMemory)} failed.");
				tempAddress = AddressUtils.ToDec(AddressUtils.MakeAddress(buffer)) + offset[0]; // Final Address

				buffer = new byte[bytesToRead];
				if (NativeMethods.ReadProcessMemory(hProcess, (IntPtr)tempAddress, buffer, bytesToRead, out ptrBytesRead) == 0)
					throw new Exception($"{nameof(NativeMethods.ReadProcessMemory)} failed.");

				bytesRead = ptrBytesRead.ToInt32();
				return buffer;
			}

			for (int i = 0; i <= iPointerCount; i++)
			{
				if (i == iPointerCount)
				{
					if (NativeMethods.ReadProcessMemory(hProcess, (IntPtr)tempAddress, buffer, 4, out _) == 0)
						throw new Exception($"{nameof(NativeMethods.ReadProcessMemory)} failed.");
					tempAddress = AddressUtils.ToDec(AddressUtils.MakeAddress(buffer)) + offset[i]; // Final Address

					buffer = new byte[bytesToRead];
					if (NativeMethods.ReadProcessMemory(hProcess, (IntPtr)tempAddress, buffer, bytesToRead, out ptrBytesRead) == 0)
						throw new Exception($"{nameof(NativeMethods.ReadProcessMemory)} failed.");

					bytesRead = ptrBytesRead.ToInt32();
					return buffer;
				}
				else if (i == 0)
				{
					if (NativeMethods.ReadProcessMemory(hProcess, memoryAddress, buffer, 4, out _) == 0)
						throw new Exception($"{nameof(NativeMethods.ReadProcessMemory)} failed.");
					tempAddress = AddressUtils.ToDec(AddressUtils.MakeAddress(buffer)) + offset[1];
				}
				else
				{
					if (NativeMethods.ReadProcessMemory(hProcess, (IntPtr)tempAddress, buffer, 4, out _) == 0)
						throw new Exception($"{nameof(NativeMethods.ReadProcessMemory)} failed.");
					tempAddress = AddressUtils.ToDec(AddressUtils.MakeAddress(buffer)) + offset[i];
				}
			}

			return buffer;
		}

#if WRITE
		public void Write(IntPtr memoryAddress, byte[] bytesToWrite, out int bytesWritten)
		{
			NativeMethods.WriteProcessMemory(hProcess, memoryAddress, bytesToWrite, (uint)bytesToWrite.Length, out IntPtr ptrBytesWritten);
			bytesWritten = ptrBytesWritten.ToInt32();
		}

		public string PointerWrite(IntPtr memoryAddress, byte[] bytesToWrite, int[] offset, out int bytesWritten)
		{
			int iPointerCount = offset.Length - 1;
			IntPtr ptrBytesWritten;
			bytesWritten = 0;
			byte[] buffer = new byte[4]; // DWORD to hold an Address
			int tempAddress = 0;

			if (iPointerCount == 0)
			{
				NativeMethods.ReadProcessMemory(hProcess, memoryAddress, buffer, 4, out _);
				tempAddress = AddressUtils.ToDec(AddressUtils.MakeAddress(buffer)) + offset[0]; // Final Address
				NativeMethods.WriteProcessMemory(hProcess, (IntPtr)tempAddress, bytesToWrite, (uint)bytesToWrite.Length, out ptrBytesWritten);

				bytesWritten = ptrBytesWritten.ToInt32();
				return AddressUtils.ToHex(tempAddress);
			}

			for (int i = 0; i <= iPointerCount; i++)
			{
				if (i == iPointerCount)
				{
					NativeMethods.ReadProcessMemory(hProcess, (IntPtr)tempAddress, buffer, 4, out _);
					tempAddress = AddressUtils.ToDec(AddressUtils.MakeAddress(buffer)) + offset[i]; // Final Address
					NativeMethods.WriteProcessMemory(hProcess, (IntPtr)tempAddress, bytesToWrite, (uint)bytesToWrite.Length, out ptrBytesWritten);

					bytesWritten = ptrBytesWritten.ToInt32();
					return AddressUtils.ToHex(tempAddress);
				}
				else if (i == 0)
				{
					NativeMethods.ReadProcessMemory(hProcess, memoryAddress, buffer, 4, out _);
					tempAddress = AddressUtils.ToDec(AddressUtils.MakeAddress(buffer)) + offset[i];
				}
				else
				{
					NativeMethods.ReadProcessMemory(hProcess, (IntPtr)tempAddress, buffer, 4, out _);
					tempAddress = AddressUtils.ToDec(AddressUtils.MakeAddress(buffer)) + offset[i];
				}
			}

			return AddressUtils.ToHex(tempAddress);
		}
#endif
	}
}