using DDCL.Variables;
using System;
using System.Diagnostics;

namespace DDCL.MemoryHandling
{
	public sealed class Scanner
	{
		public const float MaxHashTime = 1.5f;

		private const string ProcessNameToFind = "dd";
		private const string ProcessMainWindowTitle = "Devil Daggers";
		private const int Magic = 0x001F30C0;

		public Process Process { get; set; }
		public Memory Memory { get; private set; } = new Memory();

		public string SpawnsetHash { get; private set; } = string.Empty;

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

		public int HandPrevious { get; private set; } = 1;
		public int Hand { get; private set; } = 1;

		public float[] LevelUpTimes { get; private set; } = new float[3] { 0, 0, 0 };

		public int HomingPrevious { get; private set; }
		public int Homing { get; private set; }

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

		public void RestartScan()
		{
			SpawnsetHash = string.Empty;

			LevelUpTimes = new float[3] { 0, 0, 0 };
		}

		/// <summary>
		/// Used to set previous values for every <see cref="AbstractVariable{T}"/>. Must use the same order and logic as the <see cref="Scan"/> method.
		/// </summary>
		public void PreScan()
		{
			PlayerID.PreScan();
			PlayerName.PreScan();

			IsReplay.PreScan();
			if (IsReplay.Value)
				return;

			IsAlive.PreScan();
			Time.PreScan();
			Kills.PreScan();
			Gems.PreScan();
			ShotsFired.PreScan();
			ShotsHit.PreScan();

			if (IsAlive.Value)
			{
				EnemiesAlive.PreScan();

				HandPrevious = Hand;
				HomingPrevious = Homing;
			}

			if (!IsAlive.Value)
				DeathType.PreScan();
		}

		public void Scan()
		{
			try
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
				Kills.Scan();
				Gems.Scan();
				ShotsFired.Scan();
				ShotsHit.Scan();

				if (IsAlive.Value)
				{
					// Enemy count might increase on death
					EnemiesAlive.Scan();

					// TODO: Clean up
					byte[] bytes = Memory.Read(Process.MainModule.BaseAddress + 0x001F8084, 4, out _);
					int ptr = AddressUtils.ToDec(AddressUtils.MakeAddress(bytes));
					bytes = Memory.Read(new IntPtr(ptr), 4, out _);
					ptr = AddressUtils.ToDec(AddressUtils.MakeAddress(bytes));
					bytes = Memory.Read(new IntPtr(ptr) + 0x218, 4, out _);
					int levelGems = BitConverter.ToInt32(bytes, 0);

					bytes = Memory.Read(new IntPtr(ptr) + 0x224, 4, out _);
					Homing = BitConverter.ToInt32(bytes, 0);

					Hand = GetHand(levelGems);
					if (Hand > HandPrevious)
						LevelUpTimes[HandPrevious - 1] = Time.Value;
				}

				// Only scan death type when dead
				if (!IsAlive.Value)
					DeathType.Scan();

				if (Time.Value < MaxHashTime)
					SpawnsetHash = Utils.CalculateSpawnsetHash();
			}
			catch (Exception ex)
			{
				Program.logger.Error("Scan failed", ex);
			}
		}

		public void PrepareUpload()
		{
			// HACK
			if (Homing == 0 && HomingPrevious > 30)
				Homing = HomingPrevious;
		}

		private int GetHand(int levelGems)
		{
			if (levelGems < 10)
				return 1;
			if (levelGems < 70)
				return 2;
			if (levelGems == 70)
				return 3;
			return 4;
		}
	}
}