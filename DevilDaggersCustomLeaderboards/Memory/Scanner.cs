using DevilDaggersCore.Utils;
using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Memory.Variables;
using DevilDaggersCustomLeaderboards.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DevilDaggersCustomLeaderboards.Memory
{
	public static class Scanner
	{
		public static bool IsInitialized { get; private set; }

		public static Process? Process { get; private set; }

		public static IntPtr ProcessAddress { get; private set; } = IntPtr.Zero;

		public static IntVariable PlayerId { get; private set; } = new(0);
		public static StringVariable Username { get; private set; } = new(0, 32);
		public static FloatVariable Time { get; private set; } = new(0);
		public static IntVariable GemsCollected { get; private set; } = new(0);
		public static IntVariable Kills { get; private set; } = new(0);
		public static IntVariable DaggersFired { get; private set; } = new(0);
		public static IntVariable DaggersHit { get; private set; } = new(0);
		public static IntVariable EnemiesAlive { get; private set; } = new(0);
		public static IntVariable LevelGems { get; private set; } = new(0);
		public static IntVariable HomingDaggers { get; private set; } = new(0);
		public static IntVariable LeviathansAlive { get; private set; } = new(0);
		public static IntVariable OrbsAlive { get; private set; } = new(0);
		public static IntVariable GemsDespawned { get; private set; } = new(0);
		public static IntVariable GemsEaten { get; private set; } = new(0);

		public static BoolVariable IsPlayerAlive { get; private set; } = new(0);
		public static BoolVariable IsReplay { get; private set; } = new(0);
		public static ByteVariable DeathType { get; private set; } = new(0);
		public static BoolVariable IsInGame { get; private set; } = new(0);
		public static ULongVariable SurvivalHash { get; private set; } = new(0);

		public static int TimeInt => (int)(Time * 10000);
		public static int LevelUpTime2 { get; private set; }
		public static int LevelUpTime3 { get; private set; }
		public static int LevelUpTime4 { get; private set; }

		public static List<GameState> GameStates { get; } = new();

		public static void FindWindow()
		{
			Process = ProcessUtils.GetDevilDaggersProcess();
		}

		public static void Open()
		{
			if (Process == null)
				return;

			const ProcessAccessType access = ProcessAccessType.PROCESS_VM_READ;
			ProcessAddress = NativeMethods.OpenProcess((uint)access, 1, (uint)Process.Id);
		}

		public static void Initialize()
		{
			long startAddress = GetMarker() + 12 + sizeof(int);

			PlayerId = new(startAddress);
			Username = new(startAddress + sizeof(int), 32);
			Time = new(startAddress + sizeof(int) + 32);
			GemsCollected = new(startAddress + sizeof(int) + 32 + sizeof(float));
			Kills = new(startAddress + sizeof(int) + 32 + sizeof(float) + sizeof(int));
			DaggersFired = new(startAddress + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int));
			DaggersHit = new(startAddress + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int));
			EnemiesAlive = new(startAddress + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int));
			LevelGems = new(startAddress + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int));
			HomingDaggers = new(startAddress + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int));
			LeviathansAlive = new(startAddress + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int));
			OrbsAlive = new(startAddress + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int));
			GemsDespawned = new(startAddress + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int));
			GemsEaten = new(startAddress + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int));
			IsPlayerAlive = new(startAddress + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int));
			IsReplay = new(startAddress + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(bool));
			DeathType = new(startAddress + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(bool) + sizeof(bool));
			IsInGame = new(startAddress + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(bool) + sizeof(bool) + sizeof(byte));
			SurvivalHash = new(startAddress + sizeof(int) + 32 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(bool) + sizeof(bool) + sizeof(byte) + sizeof(bool));

			IsInitialized = true;
		}

		private static long GetMarker()
		{
			// return 0x5849EFF030;
			// return 0x22D68FAAEB8;

			long start = Process.MainModule.BaseAddress.ToInt64();

			byte[] marker = new byte[] { /*0x5F, 0x5F, 0x64, 0x64, 0x73, 0x74, 0x61, 0x74, 0x73, 0x5F, 0x5F, 0x00*/0x78, 0x76, 0x6c, 0x76 };

			const int readSize = 4096;
			byte[] read = new byte[readSize];
			List<byte> successBytes = new();

			for (long i = 0; ; i += readSize)
			{
				NativeMethods.ReadProcessMemory(ProcessAddress, new IntPtr(start + i), read, readSize, out _);

				for (int j = 0; j < readSize; j++)
				{
					if (read[j] == marker[successBytes.Count])
					{
						successBytes.Add(read[j]);

						if (successBytes.Count == marker.Length)
							return start + i + j + 2; // one for null terminator and one for next index to read from
					}
					else
					{
						successBytes.Clear();
					}
				}
			}

			return 0;
		}

		public static void Scan()
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

		public static void RestartScan()
		{
			HomingDaggers.HardReset();
			LevelUpTime2 = 0;
			LevelUpTime3 = 0;
			LevelUpTime4 = 0;

			GameStates.Clear();
		}
	}
}
