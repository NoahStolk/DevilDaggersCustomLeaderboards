using System;
using System.IO;
using System.Text;

namespace DevilDaggersCustomLeaderboards.Memory;

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable S1104 // Fields should not have public accessibility
public struct MainBlock
{
	public string Marker;
	public int FormatVersion;

	public int PlayerId;
	public string PlayerName;
	public float Time;
	public int GemsCollected;
	public int EnemiesKilled;
	public int DaggersFired;
	public int DaggersHit;
	public int EnemiesAlive;
	public int LevelGems;
	public int HomingDaggers;
	public int GemsDespawned;
	public int GemsEaten;
	public int GemsTotal;
	public int HomingDaggersEaten;

	public short Skull1sAlive;
	public short Skull2sAlive;
	public short Skull3sAlive;
	public short SpiderlingsAlive;
	public short Skull4sAlive;
	public short Squid1sAlive;
	public short Squid2sAlive;
	public short Squid3sAlive;
	public short CentipedesAlive;
	public short GigapedesAlive;
	public short Spider1sAlive;
	public short Spider2sAlive;
	public short LeviathansAlive;
	public short OrbsAlive;
	public short ThornsAlive;
	public short GhostpedesAlive;
	public short SpiderEggsAlive;

	public short Skull1sKilled;
	public short Skull2sKilled;
	public short Skull3sKilled;
	public short SpiderlingsKilled;
	public short Skull4sKilled;
	public short Squid1sKilled;
	public short Squid2sKilled;
	public short Squid3sKilled;
	public short CentipedesKilled;
	public short GigapedesKilled;
	public short Spider1sKilled;
	public short Spider2sKilled;
	public short LeviathansKilled;
	public short OrbsKilled;
	public short ThornsKilled;
	public short GhostpedesKilled;
	public short SpiderEggsKilled;

	public bool IsPlayerAlive;
	public bool IsReplay;
	public byte DeathType;
	public bool IsInGame;

	public int ReplayPlayerId;
	public string ReplayPlayerName;

	public byte[] SurvivalHashMd5;

	public float LevelUpTime2;
	public float LevelUpTime3;
	public float LevelUpTime4;

	public float LeviathanDownTime;
	public float OrbDownTime;

	public int Status;

	public int HomingMax;
	public float HomingMaxTime;
	public int EnemiesAliveMax;
	public float EnemiesAliveMaxTime;
	public float MaxTime;

	public long StatsBase;
	public int StatsCount;
	public bool StatsLoaded;

	public int StartHandLevel;
	public int StartAdditionalGems;
	public float StartTimer;

	public bool ProhibitedMods;

	public long ReplayBase;
	public int ReplayLength;

	public bool PlayReplayFromMemory;
	public byte GameMode;
	public bool TimeAttackOrRaceFinished;

	public MainBlock(byte[] buffer)
	{
		using MemoryStream ms = new(buffer);
		using BinaryReader br = new(ms);
		Marker = GetUtf8StringFromBytes(br.ReadBytes(12));
		FormatVersion = br.ReadInt32();

		PlayerId = br.ReadInt32();
		PlayerName = GetUtf8StringFromBytes(br.ReadBytes(32));
		Time = br.ReadSingle();
		GemsCollected = br.ReadInt32();
		EnemiesKilled = br.ReadInt32();
		DaggersFired = br.ReadInt32();
		DaggersHit = br.ReadInt32();
		EnemiesAlive = br.ReadInt32();
		LevelGems = br.ReadInt32();
		HomingDaggers = br.ReadInt32();
		GemsDespawned = br.ReadInt32();
		GemsEaten = br.ReadInt32();
		GemsTotal = br.ReadInt32();
		HomingDaggersEaten = br.ReadInt32();

		Skull1sAlive = br.ReadInt16();
		Skull2sAlive = br.ReadInt16();
		Skull3sAlive = br.ReadInt16();
		SpiderlingsAlive = br.ReadInt16();
		Skull4sAlive = br.ReadInt16();
		Squid1sAlive = br.ReadInt16();
		Squid2sAlive = br.ReadInt16();
		Squid3sAlive = br.ReadInt16();
		CentipedesAlive = br.ReadInt16();
		GigapedesAlive = br.ReadInt16();
		Spider1sAlive = br.ReadInt16();
		Spider2sAlive = br.ReadInt16();
		LeviathansAlive = br.ReadInt16();
		OrbsAlive = br.ReadInt16();
		ThornsAlive = br.ReadInt16();
		GhostpedesAlive = br.ReadInt16();
		SpiderEggsAlive = br.ReadInt16();

		Skull1sKilled = br.ReadInt16();
		Skull2sKilled = br.ReadInt16();
		Skull3sKilled = br.ReadInt16();
		SpiderlingsKilled = br.ReadInt16();
		Skull4sKilled = br.ReadInt16();
		Squid1sKilled = br.ReadInt16();
		Squid2sKilled = br.ReadInt16();
		Squid3sKilled = br.ReadInt16();
		CentipedesKilled = br.ReadInt16();
		GigapedesKilled = br.ReadInt16();
		Spider1sKilled = br.ReadInt16();
		Spider2sKilled = br.ReadInt16();
		LeviathansKilled = br.ReadInt16();
		OrbsKilled = br.ReadInt16();
		ThornsKilled = br.ReadInt16();
		GhostpedesKilled = br.ReadInt16();
		SpiderEggsKilled = br.ReadInt16();

		IsPlayerAlive = br.ReadBoolean();
		IsReplay = br.ReadBoolean();
		DeathType = br.ReadByte();
		IsInGame = br.ReadBoolean();

		ReplayPlayerId = br.ReadInt32();
		ReplayPlayerName = GetUtf8StringFromBytes(br.ReadBytes(32));

		SurvivalHashMd5 = br.ReadBytes(16);

		LevelUpTime2 = br.ReadSingle();
		LevelUpTime3 = br.ReadSingle();
		LevelUpTime4 = br.ReadSingle();

		LeviathanDownTime = br.ReadSingle();
		OrbDownTime = br.ReadSingle();

		Status = br.ReadInt32();

		HomingMax = br.ReadInt32();
		HomingMaxTime = br.ReadSingle();
		EnemiesAliveMax = br.ReadInt32();
		EnemiesAliveMaxTime = br.ReadSingle();
		MaxTime = br.ReadSingle();

		br.BaseStream.Seek(4, SeekOrigin.Current);

		StatsBase = br.ReadInt64();
		StatsCount = br.ReadInt32();
		StatsLoaded = br.ReadBoolean();

		br.BaseStream.Seek(3, SeekOrigin.Current);

		StartHandLevel = br.ReadInt32();
		StartAdditionalGems = br.ReadInt32();
		StartTimer = br.ReadSingle();

		ProhibitedMods = br.ReadBoolean();

		br.BaseStream.Seek(3, SeekOrigin.Current);

		ReplayBase = br.ReadInt64();
		ReplayLength = br.ReadInt32();

		PlayReplayFromMemory = br.ReadBoolean();
		GameMode = br.ReadByte();
		TimeAttackOrRaceFinished = br.ReadBoolean();
	}

	private static string GetUtf8StringFromBytes(byte[] bytes)
		=> Encoding.UTF8.GetString(bytes[0..Array.IndexOf(bytes, (byte)0)]);
}
