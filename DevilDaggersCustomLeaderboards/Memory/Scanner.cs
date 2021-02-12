using DevilDaggersCore.Utils;
using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Memory.Variables;
using DevilDaggersCustomLeaderboards.Native;
using DevilDaggersCustomLeaderboards.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DevilDaggersCustomLeaderboards.Memory
{
	public sealed class Scanner
	{
		private static readonly Lazy<Scanner> _lazy = new(() => new());

		private Scanner()
		{
			Magic = GetMagic() + 12 + sizeof(int);

			PlayerId = new(Magic);
			Username = new(Magic + sizeof(int), 32);
			Time = new(Magic + sizeof(int) + 32);
			GemsCollected = new(Magic + sizeof(int) + 32 + sizeof(float));
			Kills = new(Magic + sizeof(int) + 32 + sizeof(float) + sizeof(int));
			DaggersFired = new(Magic + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int));
			DaggersHit = new(Magic + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int));
			EnemiesAlive = new(Magic + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int));
			LevelGems = new(Magic + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int));
			HomingDaggers = new(Magic + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int));
			LeviathansAlive = new(Magic + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int));
			OrbsAlive = new(Magic + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int));
			GemsDespawned = new(Magic + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int));
			GemsEaten = new(Magic + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int));
			IsPlayerAlive = new(Magic + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int));
			IsReplay = new(Magic + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(bool));
			DeathType = new(Magic + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(bool) + sizeof(bool));
			IsInGame = new(Magic + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(bool) + sizeof(bool) + sizeof(byte));
			SurvivalHash = new(Magic + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(bool) + sizeof(bool) + sizeof(byte) + sizeof(bool));
		}

		public static Scanner Instance => _lazy.Value;

		public Process? Process { get; private set; } = ProcessUtils.GetDevilDaggersProcess();

		public long Magic { get; }

		public IntPtr ProcessAddress { get; private set; } = IntPtr.Zero;

		public IntVariable PlayerId { get; }
		public StringVariable Username { get; }
		public FloatVariable Time { get; }
		public IntVariable GemsCollected { get; }
		public IntVariable Kills { get; }
		public IntVariable DaggersFired { get; }
		public IntVariable DaggersHit { get; }
		public IntVariable EnemiesAlive { get; }
		public IntVariable LevelGems { get; }
		public IntVariable HomingDaggers { get; }
		public IntVariable LeviathansAlive { get; }
		public IntVariable OrbsAlive { get; }
		public IntVariable GemsDespawned { get; }
		public IntVariable GemsEaten { get; }

		public BoolVariable IsPlayerAlive { get; }
		public BoolVariable IsReplay { get; }
		public ByteVariable DeathType { get; }
		public BoolVariable IsInGame { get; }
		public ULongVariable SurvivalHash { get; }

		public int TimeInt => (int)(Time * 10000);
		public int LevelUpTime2 { get; private set; }
		public int LevelUpTime3 { get; private set; }
		public int LevelUpTime4 { get; private set; }

		public List<GameState> GameStates { get; } = new();

		public void FindWindow()
		{
			Process = ProcessUtils.GetDevilDaggersProcess();
		}

		public void RestartScan()
		{
			HomingDaggers.HardReset();
			LevelUpTime2 = 0;
			LevelUpTime3 = 0;
			LevelUpTime4 = 0;

			GameStates.Clear();
		}

		public void Open()
		{
			if (Process == null)
				return;

			const ProcessAccessType access = ProcessAccessType.PROCESS_VM_READ | ProcessAccessType.PROCESS_VM_WRITE | ProcessAccessType.PROCESS_VM_OPERATION;
			ProcessAddress = NativeMethods.OpenProcess((uint)access, 1, (uint)Process.Id);
		}

		public long GetMagic()
		{
			return 0xE40BBCF350;

			long memoryLeft;
			uint length;
			int amount = 0;
			long magic = 0;

			try
			{
				byte[] marker = new byte[] { 0x5F, 0x5F, 0x64, 0x64, 0x73, 0x74, 0x61, 0x74, 0x73, 0x5F, 0x5F };

				const int readSize = 8192;
				byte[] read = new byte[readSize];
				List<byte> successBytes = new();
				ProcessModule module = Process!.MainModule!;

				for (long i = 0; i < module.ModuleMemorySize; i += readSize)
				{
					memoryLeft = module.ModuleMemorySize - i;

					length = readSize;
					if (length > memoryLeft)
						length = (uint)memoryLeft;

					MemoryUtils.Read(new IntPtr(module.BaseAddress.ToInt64() + i), read, length);

					for (int j = 0; j < readSize; j++)
					{
						if (read[j] == marker[successBytes.Count])
						{
							successBytes.Add(read[j]);

							if (successBytes.Count == marker.Length)
							{
								amount++;
								successBytes.Clear();
								magic = i + j + 2; // one for null terminator and one for next index to read from
							}
						}
						else
						{
							successBytes.Clear();
						}
					}
				}

				return magic;
			}
			catch (Exception ex)
			{
				return 0;
			}
		}

		public void Scan()
		{
			PlayerId.Scan();
			Username.Scan();
			Time.Scan();
			GemsCollected.Scan();
			Kills.Scan();
			DaggersFired.Scan();
			DaggersHit.Scan();

			IsPlayerAlive.Scan();
			IsReplay.Scan();
			IsInGame.Scan();
			SurvivalHash.Scan();

			if (IsPlayerAlive)
			{
				EnemiesAlive.Scan();
				LevelGems.Scan();
				LeviathansAlive.Scan();
				OrbsAlive.Scan();
				GemsDespawned.Scan();
				GemsEaten.Scan();

				if (LevelGems != 0)
				{
					HomingDaggers.Scan();

					if (LevelUpTime2 == 0 && LevelGems >= 10 && LevelGems < 70)
						LevelUpTime2 = TimeInt;
					if (LevelUpTime3 == 0 && LevelGems == 70)
						LevelUpTime3 = TimeInt;
					if (LevelUpTime4 == 0 && LevelGems == 71)
						LevelUpTime4 = TimeInt;
				}

				if (Time >= GameStates.Count && Time > 0)
				{
					GameStates.Add(new()
					{
						DaggersFired = DaggersFired,
						DaggersHit = DaggersHit,
						EnemiesAlive = EnemiesAlive,
						GemsCollected = GemsCollected,
						GemsDespawned = GemsDespawned,
						GemsEaten = GemsEaten,
						HomingDaggers = HomingDaggers,
						Kills = Kills,
					});
				}
			}
			else
			{
				DeathType.Scan();
			}
		}
	}
}
