using DevilDaggersCustomLeaderboards.Exceptions;
using DevilDaggersCustomLeaderboards.Memory.Linux;
using DevilDaggersCustomLeaderboards.Native;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Os = DevilDaggersCustomLeaderboards.Clients.OperatingSystem;

namespace DevilDaggersCustomLeaderboards.Utils
{
	public static class OperatingSystemUtils
	{
		private static LinuxHeapAccessor? _linuxHeapAccessor;

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
					if (_linuxHeapAccessor != null)
					{
						_linuxHeapAccessor.Stream.Seek(address, SeekOrigin.Begin);
						_linuxHeapAccessor.Stream.Read(bytes, 0, size);
					}
					else
					{
						Console.WriteLine("Linux: Cannot read memory block when heap is not accessible.");
					}

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
				try
				{
					return BruteForceFindMarkerOnLinux(process);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Linux: Error while trying to find memory block:\n{ex.Message}");
					return null;
				}
			}
			else
			{
				throw new OperatingSystemNotSupportedException();
			}
		}

		private static long BruteForceFindMarkerOnLinux(Process process)
		{
			string[] mapLines = File.ReadAllLines($"/proc/{process.Id}/maps");
			string heapLine = Array.Find(mapLines, s => s.Contains("heap")) ?? throw new("No heap line in map file.");
			string[] heapInfo = heapLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

			string[] addressInfo = heapInfo[0].Split('-');
			if (!heapInfo[1].Contains('r'))
				throw new("No read permissions for heap.");

			_linuxHeapAccessor = new($"/proc/{process.Id}/mem");
			long heapStart = long.Parse(addressInfo[0], NumberStyles.HexNumber);
			long heapEnd = long.Parse(addressInfo[1], NumberStyles.HexNumber);
			long total = heapEnd - heapStart;
			_linuxHeapAccessor.Stream.Seek(heapStart, SeekOrigin.Begin);

			const int chunkSize = 1024 * 1024 * 8;
			const int markerSize = 12;
			byte[] markerConst = new byte[markerSize] { 0x5F, 0x5F, 0x64, 0x64, 0x73, 0x74, 0x61, 0x74, 0x73, 0x5F, 0x5F, 0x00 };
			byte[] chunk = new byte[chunkSize];

			for (long bytesRead = 0; bytesRead < total; bytesRead += chunkSize)
			{
				_linuxHeapAccessor.Stream.Read(chunk, 0, chunk.Length);

				int highestConsecutive = 0;
				int consecutive = 0;
				for (int i = 0; i < chunkSize; i++)
				{
					if (consecutive >= markerSize)
						return heapStart + bytesRead + i - markerSize;

					if (chunk[i] == markerConst[consecutive])
						consecutive++;
					else
						consecutive = 0;

					if (highestConsecutive < consecutive)
						highestConsecutive = consecutive;
				}
			}

			return 0;
		}

		public static Process? GetDevilDaggersProcess()
		{
			if (OperatingSystem == Os.Linux)
			{
				foreach (Process process in Process.GetProcesses())
				{
					if (process.ProcessName.StartsWith("devildaggers"))
						return process;
				}

				return null;
			}
			else if (OperatingSystem == Os.Windows)
			{
				foreach (Process process in Process.GetProcessesByName("dd"))
				{
					if (process.MainWindowTitle == "Devil Daggers")
						return process;
				}

				return null;
			}
			else
			{
				throw new OperatingSystemNotSupportedException();
			}
		}
	}
}
