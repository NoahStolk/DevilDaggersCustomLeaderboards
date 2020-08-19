using DevilDaggersCore.Utils;
using DevilDaggersCustomLeaderboards.Memory.Variables;
using System;
using System.Diagnostics;

namespace DevilDaggersCustomLeaderboards.Memory
{
	public sealed class Scanner
	{
		private const int magicStatic = 0x001F30C0;
		private const int magicDynamic = 0x001F8084;

		private static readonly Lazy<Scanner> lazy = new Lazy<Scanner>(() => new Scanner());

		private Scanner()
		{
			ProcessMemory = new ProcessMemory(Process);
		}

		public static Scanner Instance => lazy.Value;

		public Process? Process { get; private set; } = ProcessUtils.GetDevilDaggersProcess();
		public ProcessMemory ProcessMemory { get; private set; }

		public string SpawnsetHash { get; private set; } = string.Empty;

		public IntVariable PlayerId { get; private set; } = new IntVariable(magicStatic, 0x5C);
		public StringVariable Username { get; private set; } = new StringVariable(magicStatic, 0x60, 32);
		public FloatVariable TimeFloat { get; private set; } = new FloatVariable(magicStatic, 0x1A0);
		public IntVariable Gems { get; private set; } = new IntVariable(magicStatic, 0x1C0);
		public IntVariable Kills { get; private set; } = new IntVariable(magicStatic, 0x1BC);
		public IntVariable DeathType { get; private set; } = new IntVariable(magicStatic, 0x1C4);
		public IntVariable DaggersFired { get; private set; } = new IntVariable(magicStatic, 0x1B4);
		public IntVariable DaggersHit { get; private set; } = new IntVariable(magicStatic, 0x1B8);
		public IntVariable EnemiesAlive { get; private set; } = new IntVariable(magicStatic, 0x1FC);
		public BoolVariable IsAlive { get; private set; } = new BoolVariable(magicStatic, 0x1A4);
		public BoolVariable IsReplay { get; private set; } = new BoolVariable(magicStatic, 0x35D);

		public int Time => (int)(TimeFloat * 10000);
		public int LevelUpTime2 { get; private set; }
		public int LevelUpTime3 { get; private set; }
		public int LevelUpTime4 { get; private set; }

		public int LevelGems { get; private set; }
		public int Homing { get; private set; }

		public void FindWindow()
			=> Process = ProcessUtils.GetDevilDaggersProcess();

		public void RestartScan()
		{
			SpawnsetHash = string.Empty;

			LevelUpTime2 = 0;
			LevelUpTime3 = 0;
			LevelUpTime4 = 0;
		}

		/// <summary>
		/// Used to set previous values for every <see cref="AbstractVariable{T}"/>. Must use the same order and logic as the <see cref="Scan"/> method.
		/// </summary>
		public void PreScan()
		{
			PlayerId.PreScan();
			Username.PreScan();

			IsReplay.PreScan();
			if (IsReplay)
				return;

			IsAlive.PreScan();
			TimeFloat.PreScan();
			Kills.PreScan();
			Gems.PreScan();
			DaggersFired.PreScan();
			DaggersHit.PreScan();

			if (IsAlive)
				EnemiesAlive.PreScan();

			if (!IsAlive)
				DeathType.PreScan();
		}

		public void Scan()
		{
			if (Process == null)
				return;

			try
			{
				// Always scan these values.
				PlayerId.Scan();
				Username.Scan();

				// Always calculate the spawnset in menu or lobby.
				// Otherwise you can first normally load a spawnset to set the hash, exit and load an empty spawnset in the menu/lobby, then during playing the empty spawnset change it back to the same original spawnset and upload a cheated score.
				if (TimeFloat == 0 && TimeFloat.ValuePrevious == 0)
					SpawnsetHash = HashUtils.CalculateCurrentSurvivalHash();

				// Stop scanning if it is a replay.
				IsReplay.Scan();
				if (IsReplay)
					return;

				IsAlive.Scan();
				TimeFloat.Scan();
				Kills.Scan();
				Gems.Scan();
				DaggersFired.Scan();
				DaggersHit.Scan();

				if (IsAlive)
				{
					// Enemy count might increase on death, so only scan while player is alive.
					EnemiesAlive.Scan();

					byte[] pointerBytes = ProcessMemory.Read(Process.MainModule.BaseAddress + magicDynamic, sizeof(int), out _);
					IntPtr ptr = new IntPtr(BitConverter.ToInt32(pointerBytes));
					pointerBytes = ProcessMemory.Read(ptr, 4, out _);
					ptr = new IntPtr(BitConverter.ToInt32(pointerBytes));

					pointerBytes = ProcessMemory.Read(ptr + 0x218, 4, out _);
					LevelGems = BitConverter.ToInt32(pointerBytes, 0);

					if (LevelGems != 0)
					{
						pointerBytes = ProcessMemory.Read(ptr + 0x224, 4, out _);
						Homing = BitConverter.ToInt32(pointerBytes, 0);

						if (LevelUpTime2 == 0 && LevelGems >= 10 && LevelGems < 70)
							LevelUpTime2 = Time;
						if (LevelUpTime3 == 0 && LevelGems == 70)
							LevelUpTime3 = Time;
						if (LevelUpTime4 == 0 && LevelGems == 71)
							LevelUpTime4 = Time;
					}
				}
				else
				{
					// Only scan death type when dead.
					DeathType.Scan();
				}

				if (string.IsNullOrEmpty(SpawnsetHash))
					SpawnsetHash = HashUtils.CalculateCurrentSurvivalHash();
			}
			catch (Exception ex)
			{
				Program.Log.Error("Scan failed", ex);
			}
		}
	}
}