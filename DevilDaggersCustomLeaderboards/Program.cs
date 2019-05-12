using System;
using System.Diagnostics;
using System.Threading;

namespace DevilDaggersCustomLeaderboards
{
	public static class Program
	{
		private const string ProcessNameToFind = "dd";

		private static Process Process;
		private static readonly Memory Memory = new Memory();

		private static readonly GameVariable time = new GameVariable((IntPtr)0x001F8084, new int[] { 0x1A0 /*test*/ }, "Time", typeof(float));
		private static readonly GameVariable homing = new GameVariable((IntPtr)0x001F8084, new int[] { 0x220B4B68 /*this address is different every time, taken from CE table for now*/}, "Homing", typeof(int));
		private static readonly GameVariable gems = new GameVariable((IntPtr)0x001F8084, new int[] { 0x233C834C /*this address is different every time, taken from CE table for now*/}, "Gems", typeof(int));
		private static readonly GameVariable daggersFired = new GameVariable((IntPtr)0x001F8084, new int[] { 0x012FF7CC /*this address is different every time, taken from CE table for now*/}, "Daggers Fired", typeof(int));

		public static void Main()
		{
			FindWindow();

			for (; ; )
			{
				Thread.Sleep(50);
				Console.Clear();

				if (Process == null)
				{
					Console.WriteLine($"Process '{ProcessNameToFind}' not found");
					FindWindow();
					continue;
				}

				Console.WriteLine($"Scanning process '{Process.ProcessName}'");

				Scan();
			}
		}

		private static void FindWindow()
		{
			foreach (Process proc in Process.GetProcesses())
			{
				if (proc.ProcessName.Contains(ProcessNameToFind))
				{
					Process = proc;
					return;
				}
				else
				{
					Process = null;
				}
			}
		}

		private static void Scan()
		{
			Memory.ReadProcess = Process;
			Memory.Open();

			OutputResult(time);
			OutputResult(homing);
			OutputResult(gems);
			OutputResult(daggersFired);
		}

		private static void OutputResult(GameVariable gameVariable)
		{
			byte[] bytes = Memory.PointerRead(gameVariable, out _);
			if (gameVariable.Type == typeof(float))
				Console.WriteLine($"{gameVariable.Name.PadRight(30)}{BitConverter.ToSingle(bytes, 0)}");
			else if (gameVariable.Type == typeof(int))
				Console.WriteLine($"{gameVariable.Name.PadRight(30)}{Address.ToDec(Address.MakeAddress(bytes))}");
		}
	}
}