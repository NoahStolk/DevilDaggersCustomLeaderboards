using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Memory;
using DevilDaggersCustomLeaderboards.Native;
using DevilDaggersCustomLeaderboards.Utils;
using System;
using System.Diagnostics;
using System.IO;

namespace DevilDaggersCustomLeaderboards;

public class ScannerService
{
	private const int _bufferSize = 319;
	private const int _statesBufferSize = 112;

	private long _memoryBlockAddress;

	public bool IsInitialized { get; set; }

	public Process? Process { get; private set; }

	public byte[] Buffer { get; } = new byte[_bufferSize];

	public MainBlock MainBlockPrevious { get; private set; }
	public MainBlock MainBlock { get; private set; }

	public void FindWindow()
	{
		Process = GetDevilDaggersProcess();
	}

	public void Initialize(long ddstatsMarkerOffset)
	{
		if (IsInitialized || Process?.MainModule == null)
			return;

		long? memoryBlockAddress = GetMemoryBlockAddress(Process, ddstatsMarkerOffset);
		if (!memoryBlockAddress.HasValue)
			return;

		_memoryBlockAddress = memoryBlockAddress.Value;

		IsInitialized = true;
	}

	public void Scan()
	{
		if (Process == null)
			return;

		ReadMemory(Process, _memoryBlockAddress, Buffer, _bufferSize);

		MainBlockPrevious = MainBlock;
		MainBlock = new(Buffer);
	}

	public AddGameData GetGameData()
	{
		if (Process == null)
			return new();

		byte[] buffer = new byte[_statesBufferSize * MainBlock.StatsCount];
		ReadMemory(Process, MainBlock.StatsBase, buffer, buffer.Length);

		AddGameData gameData = new();

		using MemoryStream ms = new(buffer);
		using BinaryReader br = new(ms);
		for (int i = 0; i < MainBlock.StatsCount; i++)
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

		byte[] buffer = new byte[MainBlock.ReplayLength];
		ReadMemory(Process, MainBlock.ReplayBase, buffer, buffer.Length);

		return buffer;
	}

	private static void ReadMemory(Process process, long address, byte[] bytes, int size)
		=> NativeMethods.ReadProcessMemory(process.Handle, new(address), bytes, (uint)size, out _);

	private static long? GetMemoryBlockAddress(Process process, long ddstatsMarkerOffset)
	{
		if (process.MainModule == null)
			return null;

		byte[] pointerBytes = new byte[sizeof(long)];
		ReadMemory(process, process.MainModule.BaseAddress.ToInt64() + ddstatsMarkerOffset, pointerBytes, sizeof(long));
		return BitConverter.ToInt64(pointerBytes);
	}

	private static Process? GetDevilDaggersProcess()
		=> Array.Find(Process.GetProcessesByName("dd"), p => p.MainWindowTitle == "Devil Daggers");
}
