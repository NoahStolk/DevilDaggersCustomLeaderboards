using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Memory.Variables;
using DevilDaggersCustomLeaderboards.Utils;
using System;
using System.Diagnostics;
using System.IO;

namespace DevilDaggersCustomLeaderboards;

public class ScannerService
{
	private const int _bufferSize = 319;
	private const int _statesBufferSize = 112;
	private const string _markerValue = "__ddstats__\0";

	private long _memoryBlockAddress;

	public bool IsInitialized { get; set; }

	public Process? Process { get; private set; }

	public byte[] Buffer { get; } = new byte[_bufferSize];

	#region Variables

	public StringVariable Marker { get; private set; } = new(0, 12);
	public IntVariable FormatVersion { get; private set; } = new(0);

	public IntVariable PlayerId { get; private set; } = new(0);
	public StringVariable PlayerName { get; private set; } = new(0, 32);
	public FloatVariable Time { get; private set; } = new(0);
	public IntVariable GemsCollected { get; private set; } = new(0);
	public IntVariable EnemiesKilled { get; private set; } = new(0);
	public IntVariable DaggersFired { get; private set; } = new(0);
	public IntVariable DaggersHit { get; private set; } = new(0);
	public IntVariable EnemiesAlive { get; private set; } = new(0);
	public IntVariable LevelGems { get; private set; } = new(0);
	public IntVariable HomingDaggers { get; private set; } = new(0);
	public IntVariable GemsDespawned { get; private set; } = new(0);
	public IntVariable GemsEaten { get; private set; } = new(0);
	public IntVariable GemsTotal { get; private set; } = new(0);
	public IntVariable HomingDaggersEaten { get; private set; } = new(0);

	public ShortVariable Skull1sAlive { get; private set; } = new(0);
	public ShortVariable Skull2sAlive { get; private set; } = new(0);
	public ShortVariable Skull3sAlive { get; private set; } = new(0);
	public ShortVariable SpiderlingsAlive { get; private set; } = new(0);
	public ShortVariable Skull4sAlive { get; private set; } = new(0);
	public ShortVariable Squid1sAlive { get; private set; } = new(0);
	public ShortVariable Squid2sAlive { get; private set; } = new(0);
	public ShortVariable Squid3sAlive { get; private set; } = new(0);
	public ShortVariable CentipedesAlive { get; private set; } = new(0);
	public ShortVariable GigapedesAlive { get; private set; } = new(0);
	public ShortVariable Spider1sAlive { get; private set; } = new(0);
	public ShortVariable Spider2sAlive { get; private set; } = new(0);
	public ShortVariable LeviathansAlive { get; private set; } = new(0);
	public ShortVariable OrbsAlive { get; private set; } = new(0);
	public ShortVariable ThornsAlive { get; private set; } = new(0);
	public ShortVariable GhostpedesAlive { get; private set; } = new(0);
	public ShortVariable SpiderEggsAlive { get; private set; } = new(0);

	public ShortVariable Skull1sKilled { get; private set; } = new(0);
	public ShortVariable Skull2sKilled { get; private set; } = new(0);
	public ShortVariable Skull3sKilled { get; private set; } = new(0);
	public ShortVariable SpiderlingsKilled { get; private set; } = new(0);
	public ShortVariable Skull4sKilled { get; private set; } = new(0);
	public ShortVariable Squid1sKilled { get; private set; } = new(0);
	public ShortVariable Squid2sKilled { get; private set; } = new(0);
	public ShortVariable Squid3sKilled { get; private set; } = new(0);
	public ShortVariable CentipedesKilled { get; private set; } = new(0);
	public ShortVariable GigapedesKilled { get; private set; } = new(0);
	public ShortVariable Spider1sKilled { get; private set; } = new(0);
	public ShortVariable Spider2sKilled { get; private set; } = new(0);
	public ShortVariable LeviathansKilled { get; private set; } = new(0);
	public ShortVariable OrbsKilled { get; private set; } = new(0);
	public ShortVariable ThornsKilled { get; private set; } = new(0);
	public ShortVariable GhostpedesKilled { get; private set; } = new(0);
	public ShortVariable SpiderEggsKilled { get; private set; } = new(0);

	public BoolVariable IsPlayerAlive { get; private set; } = new(0);
	public BoolVariable IsReplay { get; private set; } = new(0);
	public ByteVariable DeathType { get; private set; } = new(0);
	public BoolVariable IsInGame { get; private set; } = new(0);

	public IntVariable ReplayPlayerId { get; private set; } = new(0);
	public StringVariable ReplayPlayerName { get; private set; } = new(0, 32);

	public ByteArrayVariable SurvivalHashMd5 { get; private set; } = new(0, 16);

	public FloatVariable LevelUpTime2 { get; private set; } = new(0);
	public FloatVariable LevelUpTime3 { get; private set; } = new(0);
	public FloatVariable LevelUpTime4 { get; private set; } = new(0);

	public FloatVariable LeviathanDownTime { get; private set; } = new(0);
	public FloatVariable OrbDownTime { get; private set; } = new(0);

	public IntVariable Status { get; private set; } = new(0);

	public IntVariable HomingMax { get; private set; } = new(0);
	public FloatVariable HomingMaxTime { get; private set; } = new(0);
	public IntVariable EnemiesAliveMax { get; private set; } = new(0);
	public FloatVariable EnemiesAliveMaxTime { get; private set; } = new(0);
	public FloatVariable MaxTime { get; private set; } = new(0);

	public LongVariable StatsBase { get; private set; } = new(0);
	public IntVariable StatsCount { get; private set; } = new(0);
	public BoolVariable StatsLoaded { get; private set; } = new(0);

	public BoolVariable ProhibitedMods { get; private set; } = new(0);

	public LongVariable ReplayBase { get; private set; } = new(0);
	public IntVariable ReplayLength { get; private set; } = new(0);

	public BoolVariable PlayReplayFromMemory { get; private set; } = new(0);
	public ByteVariable GameMode { get; private set; } = new(0);
	public BoolVariable TimeAttackOrRaceFinished { get; private set; } = new(0);

	#endregion Variables

	public void FindWindow()
	{
		Process = OperatingSystemUtils.GetDevilDaggersProcess();
	}

	public void Initialize(long ddstatsMarkerOffset)
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

		// Padding
		offset += 3;

		ReplayBase = InitiateVariable(addr => new LongVariable(addr), ref offset);
		ReplayLength = InitiateVariable(addr => new IntVariable(addr), ref offset);

		PlayReplayFromMemory = InitiateVariable(addr => new BoolVariable(addr), ref offset);
		GameMode = InitiateVariable(addr => new ByteVariable(addr), ref offset);
		TimeAttackOrRaceFinished = InitiateVariable(addr => new BoolVariable(addr), ref offset);

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

	public void Scan()
	{
		if (Process == null)
			return;

		OperatingSystemUtils.ReadMemory(Process, _memoryBlockAddress, Buffer, _bufferSize);

		Marker.Scan();
		if (Marker != _markerValue)
			return;

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

		ReplayBase.Scan();
		ReplayLength.Scan();
		PlayReplayFromMemory.Scan();
		GameMode.Scan();
		TimeAttackOrRaceFinished.Scan();

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

	public AddGameData GetGameData()
	{
		if (Process == null)
			return new();

		byte[] buffer = new byte[_statesBufferSize * StatsCount];
		OperatingSystemUtils.ReadMemory(Process, StatsBase.Value, buffer, buffer.Length);

		AddGameData gameData = new();

		using MemoryStream ms = new(buffer);
		using BinaryReader br = new(ms);
		for (int i = 0; i < StatsCount; i++)
		{
			(gameData.GemsCollected ??= new()).Add(br.ReadInt32());
			(gameData.EnemiesKilled ??= new()).Add(br.ReadInt32());
			(gameData.DaggersFired ??= new()).Add(br.ReadInt32());
			(gameData.DaggersHit ??= new()).Add(br.ReadInt32());
			(gameData.EnemiesAlive ??= new()).Add(br.ReadInt32());
			_ = br.ReadInt32(); // Skip level gems.
			(gameData.HomingDaggers ??= new()).Add(br.ReadInt32());
			(gameData.GemsDespawned ??= new()).Add(br.ReadInt32());
			(gameData.GemsEaten ??= new()).Add(br.ReadInt32());
			(gameData.GemsTotal ??= new()).Add(br.ReadInt32());
			(gameData.HomingDaggersEaten ??= new()).Add(br.ReadInt32());

			(gameData.Skull1sAlive ??= new()).Add(br.ReadInt16());
			(gameData.Skull2sAlive ??= new()).Add(br.ReadInt16());
			(gameData.Skull3sAlive ??= new()).Add(br.ReadInt16());
			(gameData.SpiderlingsAlive ??= new()).Add(br.ReadInt16());
			(gameData.Skull4sAlive ??= new()).Add(br.ReadInt16());
			(gameData.Squid1sAlive ??= new()).Add(br.ReadInt16());
			(gameData.Squid2sAlive ??= new()).Add(br.ReadInt16());
			(gameData.Squid3sAlive ??= new()).Add(br.ReadInt16());
			(gameData.CentipedesAlive ??= new()).Add(br.ReadInt16());
			(gameData.GigapedesAlive ??= new()).Add(br.ReadInt16());
			(gameData.Spider1sAlive ??= new()).Add(br.ReadInt16());
			(gameData.Spider2sAlive ??= new()).Add(br.ReadInt16());
			(gameData.LeviathansAlive ??= new()).Add(br.ReadInt16());
			(gameData.OrbsAlive ??= new()).Add(br.ReadInt16());
			(gameData.ThornsAlive ??= new()).Add(br.ReadInt16());
			(gameData.GhostpedesAlive ??= new()).Add(br.ReadInt16());
			(gameData.SpiderEggsAlive ??= new()).Add(br.ReadInt16());

			(gameData.Skull1sKilled ??= new()).Add(br.ReadInt16());
			(gameData.Skull2sKilled ??= new()).Add(br.ReadInt16());
			(gameData.Skull3sKilled ??= new()).Add(br.ReadInt16());
			(gameData.SpiderlingsKilled ??= new()).Add(br.ReadInt16());
			(gameData.Skull4sKilled ??= new()).Add(br.ReadInt16());
			(gameData.Squid1sKilled ??= new()).Add(br.ReadInt16());
			(gameData.Squid2sKilled ??= new()).Add(br.ReadInt16());
			(gameData.Squid3sKilled ??= new()).Add(br.ReadInt16());
			(gameData.CentipedesKilled ??= new()).Add(br.ReadInt16());
			(gameData.GigapedesKilled ??= new()).Add(br.ReadInt16());
			(gameData.Spider1sKilled ??= new()).Add(br.ReadInt16());
			(gameData.Spider2sKilled ??= new()).Add(br.ReadInt16());
			(gameData.LeviathansKilled ??= new()).Add(br.ReadInt16());
			(gameData.OrbsKilled ??= new()).Add(br.ReadInt16());
			(gameData.ThornsKilled ??= new()).Add(br.ReadInt16());
			(gameData.GhostpedesKilled ??= new()).Add(br.ReadInt16());
			(gameData.SpiderEggsKilled ??= new()).Add(br.ReadInt16());
		}

		return gameData;
	}

	public byte[] GetReplay()
	{
		if (Process == null)
			return Array.Empty<byte>();

		byte[] buffer = new byte[ReplayLength];
		OperatingSystemUtils.ReadMemory(Process, ReplayBase, buffer, buffer.Length);

		return buffer;
	}
}
