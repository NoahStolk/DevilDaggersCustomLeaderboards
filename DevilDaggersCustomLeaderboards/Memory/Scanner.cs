using DevilDaggersCore.Utils;
using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Memory.Variables;
using DevilDaggersCustomLeaderboards.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DevilDaggersCustomLeaderboards.Memory
{
	public static class Scanner
	{
		private const int _bufferSize = 301;
		private const int _statesBufferSize = 112;
		private const string _markerValue = "__ddstats__\0";

		private static long _memoryBlockAddress;

		public static bool IsInitialized { get; set; }

		public static Process? Process { get; private set; }

		public static byte[] Buffer { get; } = new byte[_bufferSize];

		#region Variables

		public static StringVariable Marker { get; private set; } = new(0, 12);
		public static IntVariable FormatVersion { get; private set; } = new(0);

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

		#endregion Variables

		public static void FindWindow()
		{
			Process = ProcessUtils.GetDevilDaggersProcess(OperatingSystemUtils.GetProcessName(), OperatingSystemUtils.GetProcessWindowTitle());
		}

		public static void Initialize(long ddstatsMarkerOffset)
		{
			if (IsInitialized || Process?.MainModule == null)
				return;

			long? memoryBlockAddress = OperatingSystemUtils.GetMemoryBlockAddress(Process, ddstatsMarkerOffset);
			if (!memoryBlockAddress.HasValue)
				return;

			_memoryBlockAddress = memoryBlockAddress.Value;

			int offset = 0;
			Marker = InitiateStringVariable(ref offset, _markerValue.Length);
			FormatVersion = InitiateVariable(addr => new IntVariable(addr), ref offset);

			PlayerId = InitiateVariable(addr => new IntVariable(addr), ref offset);
			PlayerName = InitiateStringVariable(ref offset, 32);
			Time = InitiateVariable(addr => new FloatVariable(addr), ref offset);
			GemsCollected = InitiateVariable(addr => new IntVariable(addr), ref offset);
			EnemiesKilled = InitiateVariable(addr => new IntVariable(addr), ref offset);
			DaggersFired = InitiateVariable(addr => new IntVariable(addr), ref offset);
			DaggersHit = InitiateVariable(addr => new IntVariable(addr), ref offset);
			EnemiesAlive = InitiateVariable(addr => new IntVariable(addr), ref offset);
			LevelGems = InitiateVariable(addr => new IntVariable(addr), ref offset);
			HomingDaggers = InitiateVariable(addr => new IntVariable(addr), ref offset);
			GemsDespawned = InitiateVariable(addr => new IntVariable(addr), ref offset);
			GemsEaten = InitiateVariable(addr => new IntVariable(addr), ref offset);
			GemsTotal = InitiateVariable(addr => new IntVariable(addr), ref offset);
			HomingDaggersEaten = InitiateVariable(addr => new IntVariable(addr), ref offset);

			Skull1sAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			Skull2sAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			Skull3sAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			SpiderlingsAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			Skull4sAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			Squid1sAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			Squid2sAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			Squid3sAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			CentipedesAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			GigapedesAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			Spider1sAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			Spider2sAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			LeviathansAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			OrbsAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			ThornsAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			GhostpedesAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			SpiderEggsAlive = InitiateVariable(addr => new ShortVariable(addr), ref offset);

			Skull1sKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			Skull2sKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			Skull3sKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			SpiderlingsKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			Skull4sKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			Squid1sKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			Squid2sKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			Squid3sKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			CentipedesKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			GigapedesKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			Spider1sKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			Spider2sKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			LeviathansKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			OrbsKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			ThornsKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			GhostpedesKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);
			SpiderEggsKilled = InitiateVariable(addr => new ShortVariable(addr), ref offset);

			IsPlayerAlive = InitiateVariable(addr => new BoolVariable(addr), ref offset);
			IsReplay = InitiateVariable(addr => new BoolVariable(addr), ref offset);
			DeathType = InitiateVariable(addr => new ByteVariable(addr), ref offset);
			IsInGame = InitiateVariable(addr => new BoolVariable(addr), ref offset);

			ReplayPlayerId = InitiateVariable(addr => new IntVariable(addr), ref offset);
			ReplayPlayerName = InitiateStringVariable(ref offset, 32);

			SurvivalHashMd5 = InitiateByteArrayVariable(ref offset, 16);

			LevelUpTime2 = InitiateVariable(addr => new FloatVariable(addr), ref offset);
			LevelUpTime3 = InitiateVariable(addr => new FloatVariable(addr), ref offset);
			LevelUpTime4 = InitiateVariable(addr => new FloatVariable(addr), ref offset);

			LeviathanDownTime = InitiateVariable(addr => new FloatVariable(addr), ref offset);
			OrbDownTime = InitiateVariable(addr => new FloatVariable(addr), ref offset);

			Status = InitiateVariable(addr => new IntVariable(addr), ref offset);

			HomingMax = InitiateVariable(addr => new IntVariable(addr), ref offset);
			HomingMaxTime = InitiateVariable(addr => new FloatVariable(addr), ref offset);
			EnemiesAliveMax = InitiateVariable(addr => new IntVariable(addr), ref offset);
			EnemiesAliveMaxTime = InitiateVariable(addr => new FloatVariable(addr), ref offset);
			MaxTime = InitiateVariable(addr => new FloatVariable(addr), ref offset);

			// Padding
			offset += 4;

			StatsBase = InitiateVariable(addr => new LongVariable(addr), ref offset);
			StatsCount = InitiateVariable(addr => new IntVariable(addr), ref offset);
			StatsLoaded = InitiateVariable(addr => new BoolVariable(addr), ref offset);

			// 3 byte padding + int starting_hand_level + int starting_homing_count + float starting_time
			offset += 3 + sizeof(int) + sizeof(int) + sizeof(float);

			ProhibitedMods = InitiateVariable(addr => new BoolVariable(addr), ref offset);

			IsInitialized = true;

			static TVariable InitiateVariable<TVariable>(Func<int, TVariable> constructor, ref int offset)
				where TVariable : IVariable
			{
				TVariable variable = constructor(offset);
				offset += variable.Size;
				return variable;
			}

			static StringVariable InitiateStringVariable(ref int offset, int stringLength)
			{
				StringVariable variable = new(offset, stringLength);
				offset += stringLength;
				return variable;
			}

			static ByteArrayVariable InitiateByteArrayVariable(ref int offset, int arrayLength)
			{
				ByteArrayVariable variable = new(offset, arrayLength);
				offset += arrayLength;
				return variable;
			}
		}

		public static void Scan()
		{
			if (Process == null)
				return;

			OperatingSystemUtils.ReadMemory(Process, _memoryBlockAddress, Buffer, _bufferSize);

			// TODO: Emit warning when this value does not hold MarkerValue.
			Marker.Scan();
			FormatVersion.Scan();

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

				LevelUpTime2.Scan();
				LevelUpTime3.Scan();
				LevelUpTime4.Scan();

				HomingDaggers.Scan();
				HomingDaggersEaten.Scan();
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
			if (Process == null)
				return new();

			byte[] buffer = new byte[_statesBufferSize * StatsCount];
			OperatingSystemUtils.ReadMemory(Process, StatsBase.Value, buffer, buffer.Length);

			List<GameState> gameStates = new();

			using MemoryStream ms = new(buffer);
			using BinaryReader br = new(ms);
			for (int i = 0; i < StatsCount; i++)
			{
				GameState gameState = new();

				gameState.GemsCollected = br.ReadInt32();
				gameState.EnemiesKilled = br.ReadInt32();
				gameState.DaggersFired = br.ReadInt32();
				gameState.DaggersHit = br.ReadInt32();
				gameState.EnemiesAlive = br.ReadInt32();
				_ = br.ReadInt32(); // Skip level gems.
				gameState.HomingDaggers = br.ReadInt32();
				gameState.GemsDespawned = br.ReadInt32();
				gameState.GemsEaten = br.ReadInt32();
				gameState.GemsTotal = br.ReadInt32();
				gameState.HomingDaggersEaten = br.ReadInt32();

				gameState.Skull1sAlive = br.ReadInt16();
				gameState.Skull2sAlive = br.ReadInt16();
				gameState.Skull3sAlive = br.ReadInt16();
				gameState.SpiderlingsAlive = br.ReadInt16();
				gameState.Skull4sAlive = br.ReadInt16();
				gameState.Squid1sAlive = br.ReadInt16();
				gameState.Squid2sAlive = br.ReadInt16();
				gameState.Squid3sAlive = br.ReadInt16();
				gameState.CentipedesAlive = br.ReadInt16();
				gameState.GigapedesAlive = br.ReadInt16();
				gameState.Spider1sAlive = br.ReadInt16();
				gameState.Spider2sAlive = br.ReadInt16();
				gameState.LeviathansAlive = br.ReadInt16();
				gameState.OrbsAlive = br.ReadInt16();
				gameState.ThornsAlive = br.ReadInt16();
				gameState.GhostpedesAlive = br.ReadInt16();
				gameState.SpiderEggsAlive = br.ReadInt16();

				gameState.Skull1sKilled = br.ReadInt16();
				gameState.Skull2sKilled = br.ReadInt16();
				gameState.Skull3sKilled = br.ReadInt16();
				gameState.SpiderlingsKilled = br.ReadInt16();
				gameState.Skull4sKilled = br.ReadInt16();
				gameState.Squid1sKilled = br.ReadInt16();
				gameState.Squid2sKilled = br.ReadInt16();
				gameState.Squid3sKilled = br.ReadInt16();
				gameState.CentipedesKilled = br.ReadInt16();
				gameState.GigapedesKilled = br.ReadInt16();
				gameState.Spider1sKilled = br.ReadInt16();
				gameState.Spider2sKilled = br.ReadInt16();
				gameState.LeviathansKilled = br.ReadInt16();
				gameState.OrbsKilled = br.ReadInt16();
				gameState.ThornsKilled = br.ReadInt16();
				gameState.GhostpedesKilled = br.ReadInt16();
				gameState.SpiderEggsKilled = br.ReadInt16();

				gameStates.Add(gameState);
			}

			return gameStates;
		}
	}
}
