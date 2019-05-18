﻿using DDCL.Variables;
using System;
using System.Diagnostics;

namespace DDCL.MemoryHandling
{
	public sealed class Scanner
	{
		private const string ProcessNameToFind = "dd";
		private const string ProcessMainWindowTitle = "Devil Daggers";
		private const int Magic = 0x001F30C0;

		public Process Process { get; set; }
		public Memory Memory { get; private set; } = new Memory();

		public string SpawnsetHash { get; private set; }

		public IntVariable PlayerID { get; private set; } = new IntVariable(Magic, 0x5C);
		public StringVariable PlayerName { get; private set; } = new StringVariable(Magic, 0x60, 32);
		public FloatVariable Time { get; private set; } = new FloatVariable(Magic, 0x1A0);
		public IntVariable Gems { get; private set; } = new IntVariable(Magic, 0x1C0);
		public IntVariable Kills { get; private set; } = new IntVariable(Magic, 0x1BC);
		public IntVariable DeathType { get; private set; } = new IntVariable(Magic, 0x1C4);
		public IntVariable ShotsFired { get; private set; } = new IntVariable(Magic, 0x1B4);
		public IntVariable ShotsHit { get; private set; } = new IntVariable(Magic, 0x1B8);
		public IntVariable EnemiesAlive { get; private set; } = new IntVariable(Magic, 0x1FC);
		public BoolVariable IsAlive { get; private set; } = new BoolVariable(Magic, 0x1A4);
		public BoolVariable IsReplay { get; private set; } = new BoolVariable(Magic, 0x35D);

		private static readonly Lazy<Scanner> lazy = new Lazy<Scanner>(() => new Scanner());
		public static Scanner Instance => lazy.Value;

		private Scanner()
		{
		}

		public void FindWindow()
		{
			Process = null;
			foreach (Process proc in Process.GetProcessesByName(ProcessNameToFind))
			{
				if (proc.MainWindowTitle == ProcessMainWindowTitle)
				{
					Process = proc;
					return;
				}
			}
		}

		public void PreScan()
		{
			// Always scan these values
			PlayerID.PreScan();
			PlayerName.PreScan();

			// Stop scanning if it is a replay
			IsReplay.PreScan();
			if (IsReplay.Value)
				return;

			IsAlive.PreScan();
			Time.PreScan();
			Gems.PreScan();
			Kills.PreScan();
			ShotsFired.PreScan();
			ShotsHit.PreScan();

			// Enemy count might increase on death
			if (IsAlive.Value)
				EnemiesAlive.PreScan();

			// Only scan death type when dead
			if (!IsAlive.Value)
				DeathType.PreScan();
		}

		public void Scan()
		{
			// Always scan these values
			PlayerID.Scan();
			PlayerName.Scan();

			// Stop scanning if it is a replay
			IsReplay.Scan();
			if (IsReplay.Value)
				return;

			IsAlive.Scan();
			Time.Scan();
			Gems.Scan();
			Kills.Scan();
			ShotsFired.Scan();
			ShotsHit.Scan();

			// Enemy count might increase on death
			if (IsAlive.Value)
				EnemiesAlive.Scan();

			// Only scan death type when dead
			if (!IsAlive.Value)
				DeathType.Scan();

			if (Time.Value < 1)
				SpawnsetHash = Utils.CalculateSpawnsetHash();
		}

		public void Reset()
		{
			SpawnsetHash = string.Empty;
		}
	}
}