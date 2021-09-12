using DevilDaggersCore.Utils;
using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Exceptions;
using DevilDaggersCustomLeaderboards.Memory.Variables;
using DevilDaggersCustomLeaderboards.Native;
using DevilDaggersCustomLeaderboards.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DevilDaggersCustomLeaderboards.Memory
{
	public static class Scanner
	{
		public static bool IsInitialized { get; set; }

		public static Process? Process { get; private set; }

		public static IntVariable PlayerId { get; private set; } = new(0);
		public static StringVariable PlayerName { get; private set; } = new(0, 32);
		public static FloatVariable Time { get; private set; } = new(0);
		public static IntVariable GemsCollected { get; private set; } = new(0);
		public static IntVariable EnemiesKilled { get; private set; } = new(0);
		public static IntVariable DaggersFired { get; private set; } = new(0);
		public static IntVariable DaggersHit { get; private set; } = new(0);
		public static IntVariable EnemiesAlive { get; private set; } = new(0);
		public static IntVariable LevelGems { get; private set; } = new(0);
		public static IntVariable HomingDaggers { get; private set; } = new(0);
		public static IntVariable GemsDespawned { get; private set; } = new(0);
		public static IntVariable GemsEaten { get; private set; } = new(0);
		public static IntVariable GemsTotal { get; private set; } = new(0);
		public static IntVariable HomingDaggersEaten { get; private set; } = new(0);

		public static ShortVariable Skull1sAlive { get; private set; } = new(0);
		public static ShortVariable Skull2sAlive { get; private set; } = new(0);
		public static ShortVariable Skull3sAlive { get; private set; } = new(0);
		public static ShortVariable SpiderlingsAlive { get; private set; } = new(0);
		public static ShortVariable Skull4sAlive { get; private set; } = new(0);
		public static ShortVariable Squid1sAlive { get; private set; } = new(0);
		public static ShortVariable Squid2sAlive { get; private set; } = new(0);
		public static ShortVariable Squid3sAlive { get; private set; } = new(0);
		public static ShortVariable CentipedesAlive { get; private set; } = new(0);
		public static ShortVariable GigapedesAlive { get; private set; } = new(0);
		public static ShortVariable Spider1sAlive { get; private set; } = new(0);
		public static ShortVariable Spider2sAlive { get; private set; } = new(0);
		public static ShortVariable LeviathansAlive { get; private set; } = new(0);
		public static ShortVariable OrbsAlive { get; private set; } = new(0);
		public static ShortVariable ThornsAlive { get; private set; } = new(0);
		public static ShortVariable GhostpedesAlive { get; private set; } = new(0);
		public static ShortVariable SpiderEggsAlive { get; private set; } = new(0);

		public static ShortVariable Skull1sKilled { get; private set; } = new(0);
		public static ShortVariable Skull2sKilled { get; private set; } = new(0);
		public static ShortVariable Skull3sKilled { get; private set; } = new(0);
		public static ShortVariable SpiderlingsKilled { get; private set; } = new(0);
		public static ShortVariable Skull4sKilled { get; private set; } = new(0);
		public static ShortVariable Squid1sKilled { get; private set; } = new(0);
		public static ShortVariable Squid2sKilled { get; private set; } = new(0);
		public static ShortVariable Squid3sKilled { get; private set; } = new(0);
		public static ShortVariable CentipedesKilled { get; private set; } = new(0);
		public static ShortVariable GigapedesKilled { get; private set; } = new(0);
		public static ShortVariable Spider1sKilled { get; private set; } = new(0);
		public static ShortVariable Spider2sKilled { get; private set; } = new(0);
		public static ShortVariable LeviathansKilled { get; private set; } = new(0);
		public static ShortVariable OrbsKilled { get; private set; } = new(0);
		public static ShortVariable ThornsKilled { get; private set; } = new(0);
		public static ShortVariable GhostpedesKilled { get; private set; } = new(0);
		public static ShortVariable SpiderEggsKilled { get; private set; } = new(0);

		public static BoolVariable IsPlayerAlive { get; private set; } = new(0);
		public static BoolVariable IsReplay { get; private set; } = new(0);
		public static ByteVariable DeathType { get; private set; } = new(0);
		public static BoolVariable IsInGame { get; private set; } = new(0);

		public static IntVariable ReplayPlayerId { get; private set; } = new(0);
		public static StringVariable ReplayPlayerName { get; private set; } = new(0, 32);

		public static ByteArrayVariable SurvivalHashMd5 { get; private set; } = new(0, 16);

		public static FloatVariable LevelUpTime2 { get; private set; } = new(0);
		public static FloatVariable LevelUpTime3 { get; private set; } = new(0);
		public static FloatVariable LevelUpTime4 { get; private set; } = new(0);

		public static FloatVariable LeviathanDownTime { get; private set; } = new(0);
		public static FloatVariable OrbDownTime { get; private set; } = new(0);

		public static IntVariable Status { get; private set; } = new(0);

		public static IntVariable HomingMax { get; private set; } = new(0);
		public static FloatVariable HomingMaxTime { get; private set; } = new(0);
		public static IntVariable EnemiesAliveMax { get; private set; } = new(0);
		public static FloatVariable EnemiesAliveMaxTime { get; private set; } = new(0);
		public static FloatVariable MaxTime { get; private set; } = new(0);

		public static LongVariable StatsBase { get; private set; } = new(0);
		public static IntVariable StatsCount { get; private set; } = new(0);
		public static BoolVariable StatsLoaded { get; private set; } = new(0);

		public static BoolVariable ProhibitedMods { get; private set; } = new(0);

		public static void FindWindow()
		{
			Process = ProcessUtils.GetDevilDaggersProcess(OperatingSystemUtils.GetProcessName(), OperatingSystemUtils.GetProcessWindowTitle());
		}

		public static void Initialize(long ddstatsMarkerOffset)
		{
			if (IsInitialized || Process?.MainModule == null)
				return;

			byte[] pointerBytes = new byte[sizeof(long)];

			ReadMemory(Process.Handle, Process.MainModule.BaseAddress.ToInt64() + ddstatsMarkerOffset, pointerBytes, sizeof(long));
			if (OperatingSystemUtils.OperatingSystem == Clients.OperatingSystem.Linux)
				ReadMemory(Process.Handle, BitConverter.ToInt64(pointerBytes) + 0x1F10, pointerBytes, sizeof(long));

			int headerSize = "__ddstats__\0".Length;
			long address = BitConverter.ToInt64(pointerBytes) + headerSize + sizeof(int);

			PlayerId = InitiateVariable(addr => new IntVariable(addr), ref address);
			PlayerName = InitiateStringVariable(ref address, 32);
			Time = InitiateVariable(addr => new FloatVariable(addr), ref address);
			GemsCollected = InitiateVariable(addr => new IntVariable(addr), ref address);
			EnemiesKilled = InitiateVariable(addr => new IntVariable(addr), ref address);
			DaggersFired = InitiateVariable(addr => new IntVariable(addr), ref address);
			DaggersHit = InitiateVariable(addr => new IntVariable(addr), ref address);
			EnemiesAlive = InitiateVariable(addr => new IntVariable(addr), ref address);
			LevelGems = InitiateVariable(addr => new IntVariable(addr), ref address);
			HomingDaggers = InitiateVariable(addr => new IntVariable(addr), ref address);
			GemsDespawned = InitiateVariable(addr => new IntVariable(addr), ref address);
			GemsEaten = InitiateVariable(addr => new IntVariable(addr), ref address);
			GemsTotal = InitiateVariable(addr => new IntVariable(addr), ref address);
			HomingDaggersEaten = InitiateVariable(addr => new IntVariable(addr), ref address);

			Skull1sAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);
			Skull2sAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);
			Skull3sAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);
			SpiderlingsAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);
			Skull4sAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);
			Squid1sAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);
			Squid2sAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);
			Squid3sAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);
			CentipedesAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);
			GigapedesAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);
			Spider1sAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);
			Spider2sAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);
			LeviathansAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);
			OrbsAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);
			ThornsAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);
			GhostpedesAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);
			SpiderEggsAlive = InitiateVariable(addr => new ShortVariable(addr), ref address);

			Skull1sKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);
			Skull2sKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);
			Skull3sKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);
			SpiderlingsKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);
			Skull4sKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);
			Squid1sKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);
			Squid2sKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);
			Squid3sKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);
			CentipedesKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);
			GigapedesKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);
			Spider1sKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);
			Spider2sKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);
			LeviathansKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);
			OrbsKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);
			ThornsKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);
			GhostpedesKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);
			SpiderEggsKilled = InitiateVariable(addr => new ShortVariable(addr), ref address);

			IsPlayerAlive = InitiateVariable(addr => new BoolVariable(addr), ref address);
			IsReplay = InitiateVariable(addr => new BoolVariable(addr), ref address);
			DeathType = InitiateVariable(addr => new ByteVariable(addr), ref address);
			IsInGame = InitiateVariable(addr => new BoolVariable(addr), ref address);

			ReplayPlayerId = InitiateVariable(addr => new IntVariable(addr), ref address);
			ReplayPlayerName = InitiateStringVariable(ref address, 32);

			SurvivalHashMd5 = InitiateByteArrayVariable(ref address, 16);

			LevelUpTime2 = InitiateVariable(addr => new FloatVariable(addr), ref address);
			LevelUpTime3 = InitiateVariable(addr => new FloatVariable(addr), ref address);
			LevelUpTime4 = InitiateVariable(addr => new FloatVariable(addr), ref address);

			LeviathanDownTime = InitiateVariable(addr => new FloatVariable(addr), ref address);
			OrbDownTime = InitiateVariable(addr => new FloatVariable(addr), ref address);

			Status = InitiateVariable(addr => new IntVariable(addr), ref address);

			HomingMax = InitiateVariable(addr => new IntVariable(addr), ref address);
			HomingMaxTime = InitiateVariable(addr => new FloatVariable(addr), ref address);
			EnemiesAliveMax = InitiateVariable(addr => new IntVariable(addr), ref address);
			EnemiesAliveMaxTime = InitiateVariable(addr => new FloatVariable(addr), ref address);
			MaxTime = InitiateVariable(addr => new FloatVariable(addr), ref address);

			address += 4;

			StatsBase = InitiateVariable(addr => new LongVariable(addr), ref address);
			StatsCount = InitiateVariable(addr => new IntVariable(addr), ref address);
			StatsLoaded = InitiateVariable(addr => new BoolVariable(addr), ref address);

			// 3 byte padding + int starting_hand_level + int starting_homing_count + float starting_time
			address += 3 + sizeof(int) + sizeof(int) + sizeof(float);

			ProhibitedMods = InitiateVariable(addr => new BoolVariable(addr), ref address);

			IsInitialized = true;

			static TVariable InitiateVariable<TVariable>(Func<long, TVariable> constructor, ref long address)
				where TVariable : IVariable
			{
				TVariable variable = constructor(address);
				address += variable.Size;
				return variable;
			}

			static StringVariable InitiateStringVariable(ref long address, uint stringLength)
			{
				StringVariable variable = new(address, stringLength);
				address += stringLength;
				return variable;
			}

			static ByteArrayVariable InitiateByteArrayVariable(ref long address, uint arrayLength)
			{
				ByteArrayVariable variable = new(address, arrayLength);
				address += arrayLength;
				return variable;
			}
		}

		public static void Scan()
		{
			PlayerId.Scan();
			PlayerName.Scan();
			Time.Scan();
			GemsCollected.Scan();
			EnemiesKilled.Scan();
			DaggersFired.Scan();
			DaggersHit.Scan();

			IsPlayerAlive.Scan();
			IsReplay.Scan();
			IsInGame.Scan();
			SurvivalHashMd5.Scan();
			Status.Scan();

			HomingMax.Scan();
			HomingMaxTime.Scan();
			EnemiesAliveMax.Scan();
			EnemiesAliveMaxTime.Scan();
			MaxTime.Scan();

			StatsBase.Scan();
			StatsCount.Scan();
			StatsLoaded.Scan();

			ProhibitedMods.Scan();

			if (IsPlayerAlive)
			{
				EnemiesAlive.Scan();
				LevelGems.Scan();

				Skull1sAlive.Scan();
				Skull2sAlive.Scan();
				Skull3sAlive.Scan();
				SpiderlingsAlive.Scan();
				Skull4sAlive.Scan();
				Squid1sAlive.Scan();
				Squid2sAlive.Scan();
				Squid3sAlive.Scan();
				CentipedesAlive.Scan();
				GigapedesAlive.Scan();
				Spider1sAlive.Scan();
				Spider2sAlive.Scan();
				LeviathansAlive.Scan();
				OrbsAlive.Scan();
				ThornsAlive.Scan();
				GhostpedesAlive.Scan();
				SpiderEggsAlive.Scan();

				Skull1sKilled.Scan();
				Skull2sKilled.Scan();
				Skull3sKilled.Scan();
				SpiderlingsKilled.Scan();
				Skull4sKilled.Scan();
				Squid1sKilled.Scan();
				Squid2sKilled.Scan();
				Squid3sKilled.Scan();
				CentipedesKilled.Scan();
				GigapedesKilled.Scan();
				Spider1sKilled.Scan();
				Spider2sKilled.Scan();
				LeviathansKilled.Scan();
				OrbsKilled.Scan();
				ThornsKilled.Scan();
				GhostpedesKilled.Scan();
				SpiderEggsKilled.Scan();

				GemsDespawned.Scan();
				GemsEaten.Scan();
				GemsTotal.Scan();

				LeviathanDownTime.Scan();
				OrbDownTime.Scan();

				if (LevelGems > 0)
				{
					LevelUpTime2.Scan();
					LevelUpTime3.Scan();
					LevelUpTime4.Scan();

					if (LevelGems == 70 || LevelGems == 71)
					{
						HomingDaggers.Scan();
						HomingDaggersEaten.Scan();
					}
				}
			}
			else
			{
				DeathType.Scan();
			}

			if (IsReplay)
			{
				ReplayPlayerId.Scan();
				ReplayPlayerName.Scan();
			}
		}

		public static List<GameState> GetGameStates()
		{
			byte[] intBytes = new byte[4];
			byte[] shortBytes = new byte[2];
			int offset = 0;
			List<GameState> gameStates = new();
			for (int i = 0; i < StatsCount; i++)
			{
				GameState gameState = new();

				gameState.GemsCollected = ReadInt(ref offset);
				gameState.EnemiesKilled = ReadInt(ref offset);
				gameState.DaggersFired = ReadInt(ref offset);
				gameState.DaggersHit = ReadInt(ref offset);
				gameState.EnemiesAlive = ReadInt(ref offset);
				offset += sizeof(int);
				gameState.HomingDaggers = ReadInt(ref offset);
				gameState.GemsDespawned = ReadInt(ref offset);
				gameState.GemsEaten = ReadInt(ref offset);
				gameState.GemsTotal = ReadInt(ref offset);
				gameState.HomingDaggersEaten = ReadInt(ref offset);

				gameState.Skull1sAlive = ReadShort(ref offset);
				gameState.Skull2sAlive = ReadShort(ref offset);
				gameState.Skull3sAlive = ReadShort(ref offset);
				gameState.SpiderlingsAlive = ReadShort(ref offset);
				gameState.Skull4sAlive = ReadShort(ref offset);
				gameState.Squid1sAlive = ReadShort(ref offset);
				gameState.Squid2sAlive = ReadShort(ref offset);
				gameState.Squid3sAlive = ReadShort(ref offset);
				gameState.CentipedesAlive = ReadShort(ref offset);
				gameState.GigapedesAlive = ReadShort(ref offset);
				gameState.Spider1sAlive = ReadShort(ref offset);
				gameState.Spider2sAlive = ReadShort(ref offset);
				gameState.LeviathansAlive = ReadShort(ref offset);
				gameState.OrbsAlive = ReadShort(ref offset);
				gameState.ThornsAlive = ReadShort(ref offset);
				gameState.GhostpedesAlive = ReadShort(ref offset);
				gameState.SpiderEggsAlive = ReadShort(ref offset);

				gameState.Skull1sKilled = ReadShort(ref offset);
				gameState.Skull2sKilled = ReadShort(ref offset);
				gameState.Skull3sKilled = ReadShort(ref offset);
				gameState.SpiderlingsKilled = ReadShort(ref offset);
				gameState.Skull4sKilled = ReadShort(ref offset);
				gameState.Squid1sKilled = ReadShort(ref offset);
				gameState.Squid2sKilled = ReadShort(ref offset);
				gameState.Squid3sKilled = ReadShort(ref offset);
				gameState.CentipedesKilled = ReadShort(ref offset);
				gameState.GigapedesKilled = ReadShort(ref offset);
				gameState.Spider1sKilled = ReadShort(ref offset);
				gameState.Spider2sKilled = ReadShort(ref offset);
				gameState.LeviathansKilled = ReadShort(ref offset);
				gameState.OrbsKilled = ReadShort(ref offset);
				gameState.ThornsKilled = ReadShort(ref offset);
				gameState.GhostpedesKilled = ReadShort(ref offset);
				gameState.SpiderEggsKilled = ReadShort(ref offset);

				gameStates.Add(gameState);
			}

			return gameStates;

			int ReadInt(ref int offset)
			{
				if (Process == null)
					return 0;

				ReadMemory(Process.Handle, StatsBase.Value + offset, intBytes, sizeof(int));
				offset += sizeof(int);
				return BitConverter.ToInt32(intBytes);
			}

			short ReadShort(ref int offset)
			{
				if (Process == null)
					return 0;

				ReadMemory(Process.Handle, StatsBase.Value + offset, shortBytes, sizeof(short));
				offset += sizeof(short);
				return BitConverter.ToInt16(shortBytes);
			}
		}

		public static void ReadMemory(nint processAddress, long address, byte[] bytes, int size)
		{
			if (Process == null)
				return;

			switch (OperatingSystemUtils.OperatingSystem)
			{
				case Clients.OperatingSystem.Windows:
					NativeMethods.ReadProcessMemory(processAddress, new(address), bytes, (uint)size, out _);
					break;
				case Clients.OperatingSystem.Linux:
					// TODO
					break;
				default:
					throw new OperatingSystemNotSupportedException();
			}
		}
	}
}
