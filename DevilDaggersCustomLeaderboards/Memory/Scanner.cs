//#define POINTER_READ
using DevilDaggersCore;
using DevilDaggersCore.Processes;
using DevilDaggersCore.Spawnsets;
using DevilDaggersCustomLeaderboards.Memory.Variables;
using System;
using System.Diagnostics;
using System.IO;

namespace DevilDaggersCustomLeaderboards.Memory
{
	public sealed class Scanner
	{
		private const int magicStatic = 0x001F30C0;
		private const int magicDynamic = 0x001F8084;

		public Process Process { get; private set; }
		public Memory Memory { get; private set; } = new Memory();

		public string SpawnsetHash { get; private set; } = string.Empty;

		public IntVariable PlayerId { get; private set; } = new IntVariable(magicStatic, 0x5C);
		public StringVariable Username { get; private set; } = new StringVariable(magicStatic, 0x60, 32);
		public FloatVariable Time { get; private set; } = new FloatVariable(magicStatic, 0x1A0);
		public IntVariable Gems { get; private set; } = new IntVariable(magicStatic, 0x1C0);
		public IntVariable Kills { get; private set; } = new IntVariable(magicStatic, 0x1BC);
		public IntVariable DeathType { get; private set; } = new IntVariable(magicStatic, 0x1C4);
		public IntVariable ShotsFired { get; private set; } = new IntVariable(magicStatic, 0x1B4);
		public IntVariable ShotsHit { get; private set; } = new IntVariable(magicStatic, 0x1B8);
		public IntVariable EnemiesAlive { get; private set; } = new IntVariable(magicStatic, 0x1FC);
		public BoolVariable IsAlive { get; private set; } = new BoolVariable(magicStatic, 0x1A4);
		public BoolVariable IsReplay { get; private set; } = new BoolVariable(magicStatic, 0x35D);

		public float LevelUpTime2 { get; private set; }
		public float LevelUpTime3 { get; private set; }
		public float LevelUpTime4 { get; private set; }

		public int LevelGems { get; private set; }
		public int Homing { get; private set; }

		private static readonly Lazy<Scanner> lazy = new Lazy<Scanner>(() => new Scanner());
		public static Scanner Instance => lazy.Value;

		private Scanner()
		{
		}

		public void FindWindow()
		{
			Process = ProcessUtils.GetDevilDaggersProcess();
		}

		private string CalculateCurrentSurvivalHash()
		{
			try
			{
				using (FileStream fs = new FileStream(Path.Combine(Path.GetDirectoryName(Process.MainModule.FileName), "dd", "survival"), FileMode.Open, FileAccess.Read))
				{
					if (Spawnset.TryParse(fs, out Spawnset spawnset))
						return spawnset.GetHashString();

					Logging.Log.Error("Failed to calculate spawnset hash because the survival file could not be parsed.");
				}

				return string.Empty;
			}
			catch (Exception ex)
			{
				Logging.Log.Error("Failed to calculate spawnset hash.", ex);

				return string.Empty;
			}
		}

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
			Time.PreScan();
			Kills.PreScan();
			Gems.PreScan();
			ShotsFired.PreScan();
			ShotsHit.PreScan();

			if (IsAlive)
				EnemiesAlive.PreScan();

			if (!IsAlive)
				DeathType.PreScan();
		}

		public void Scan()
		{
			try
			{
				// Always scan these values.
				PlayerId.Scan();
				Username.Scan();

				// Always calculate the spawnset in menu or lobby.
				// Otherwise you can first normally load a spawnset to set the hash, exit and load an empty spawnset in the menu/lobby, then during playing the empty spawnset change it back to the same original spawnset and upload a cheated score.
				if (Time == 0 && Time.ValuePrevious == 0)
					SpawnsetHash = CalculateCurrentSurvivalHash();

				// Stop scanning if it is a replay.
				IsReplay.Scan();
				if (IsReplay)
					return;

				IsAlive.Scan();
				Time.Scan();
				Kills.Scan();
				Gems.Scan();
				ShotsFired.Scan();
				ShotsHit.Scan();

				if (IsAlive)
				{
					// Enemy count might increase on death, so only scan while player is alive.
					EnemiesAlive.Scan();

					// TODO: Clean up.
#if POINTER_READ
					byte[] bytes = Memory.PointerRead(Process.MainModule.BaseAddress, 4, new[] { magicDynamic, 0x218 }, out _);
#else
					byte[] bytes = Memory.Read(Process.MainModule.BaseAddress + magicDynamic, 4, out _);
					int ptr = AddressUtils.ToDec(AddressUtils.MakeAddress(bytes));
					bytes = Memory.Read(new IntPtr(ptr), 4, out _);
					ptr = AddressUtils.ToDec(AddressUtils.MakeAddress(bytes));
					bytes = Memory.Read(new IntPtr(ptr) + 0x218, 4, out _);
#endif
					LevelGems = BitConverter.ToInt32(bytes, 0);

#if POINTER_READ
					bytes = Memory.PointerRead(Process.MainModule.BaseAddress, 4, new[] { magicDynamic, 0x224 }, out _);
#else
					bytes = Memory.Read(new IntPtr(ptr) + 0x224, 4, out _);
#endif
					Homing = BitConverter.ToInt32(bytes, 0);

					if (LevelUpTime2 == 0 && LevelGems >= 10 && LevelGems < 70)
						LevelUpTime2 = Time;
					if (LevelUpTime3 == 0 && LevelGems == 70)
						LevelUpTime3 = Time;
					if (LevelUpTime4 == 0 && LevelGems == 71)
						LevelUpTime4 = Time;
				}
				else
				{
					// Only scan death type when dead.
					DeathType.Scan();
				}

				if (string.IsNullOrEmpty(SpawnsetHash))
					SpawnsetHash = CalculateCurrentSurvivalHash();
			}
			catch (Exception ex)
			{
				Logging.Log.Error("Scan failed", ex);
			}
		}
	}
}