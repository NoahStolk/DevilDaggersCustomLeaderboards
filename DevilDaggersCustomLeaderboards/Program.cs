﻿using System;
using System.Diagnostics;
using System.Threading;

namespace DevilDaggersCustomLeaderboards
{
	public static class Program
	{
		private const string ProcessNameToFind = "dd";

		private static Process Process;
		private static readonly Memory Memory = new Memory();

		private static readonly GameVariable<float> time = new GameVariable<float>((IntPtr)0x001F8084, new int[] { 0x1A0 /*test*/ }, "Time");
		private static readonly GameVariable<int> homing = new GameVariable<int>((IntPtr)0x001F8084, new int[] { 0x220B4B68 /*this address is different every time, taken from CE table for now*/}, "Homing");
		private static readonly GameVariable<int> gems = new GameVariable<int>((IntPtr)0x001F8084, new int[] { 0x233C834C /*this address is different every time, taken from CE table for now*/}, "Gems");
		private static readonly GameVariable<int> daggersFired = new GameVariable<int>((IntPtr)0x001F8084, new int[] { 0x012FF7CC /*this address is different every time, taken from CE table for now*/}, "Daggers Fired");

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

		private static void OutputResult<T>(GameVariable<T> gameVariable) where T : struct
		{
			byte[] bytes = Memory.PointerRead(gameVariable, out _);
			if (typeof(T) == typeof(float))
				Console.WriteLine($"{gameVariable.Name.PadRight(30)}{BitConverter.ToSingle(bytes, 0)}");
			else if (typeof(T) == typeof(int))
				Console.WriteLine($"{gameVariable.Name.PadRight(30)}{Address.ToDec(Address.MakeAddress(bytes))}");
		}
	}
}