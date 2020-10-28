﻿using DevilDaggersCore.Utils;
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
		private const int _magicStatic = 0x001F30C0;
		private const int _magicDynamic = 0x001F8084;

		private static readonly Lazy<Scanner> _lazy = new Lazy<Scanner>(() => new Scanner());

		private Scanner()
		{
		}

		public static Scanner Instance => _lazy.Value;

		public Process? Process { get; private set; } = ProcessUtils.GetDevilDaggersProcess();

		public IntPtr ProcessAddress { get; private set; } = IntPtr.Zero;

		public string SpawnsetHash { get; private set; } = string.Empty;

		public IntVariable PlayerId { get; private set; } = new IntVariable(_magicStatic, 0x5C);
		public StringVariable Username { get; private set; } = new StringVariable(_magicStatic, 32, 0x60); // TODO: Use 16? Strings longer than 16 characters are stored differently (which isn't yet supported).
		public FloatVariable TimeFloat { get; private set; } = new FloatVariable(_magicStatic, 0x1A0);
		public IntVariable Gems { get; private set; } = new IntVariable(_magicStatic, 0x1C0);
		public IntVariable Kills { get; private set; } = new IntVariable(_magicStatic, 0x1BC);
		public IntVariable DeathType { get; private set; } = new IntVariable(_magicStatic, 0x1C4);
		public IntVariable DaggersFired { get; private set; } = new IntVariable(_magicStatic, 0x1B4);
		public IntVariable DaggersHit { get; private set; } = new IntVariable(_magicStatic, 0x1B8);
		public IntVariable EnemiesAlive { get; private set; } = new IntVariable(_magicStatic, 0x1FC);
		public BoolVariable IsAlive { get; private set; } = new BoolVariable(_magicStatic, 0x1A4);
		public BoolVariable IsReplay { get; private set; } = new BoolVariable(_magicStatic, 0x35D);

		public IntVariable LevelGems { get; private set; } = new IntVariable(_magicDynamic, 0, 0x218);
		public IntVariable Homing { get; private set; } = new IntVariable(_magicDynamic, 0, 0x224);

		public int Time => (int)(TimeFloat * 10000);
		public int LevelUpTime2 { get; private set; }
		public int LevelUpTime3 { get; private set; }
		public int LevelUpTime4 { get; private set; }

		public List<GameState> GameStates { get; } = new List<GameState>();

		public void FindWindow()
			=> Process = ProcessUtils.GetDevilDaggersProcess();

		public void RestartScan()
		{
			Homing.HardReset();
			LevelUpTime2 = 0;
			LevelUpTime3 = 0;
			LevelUpTime4 = 0;

			GameStates.Clear();
		}

		public void Open()
		{
			if (Process == null)
				return;

			ProcessAccessType access = ProcessAccessType.PROCESS_VM_READ | ProcessAccessType.PROCESS_VM_WRITE | ProcessAccessType.PROCESS_VM_OPERATION;
			ProcessAddress = NativeMethods.OpenProcess((uint)access, 1, (uint)Process.Id);
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

			LevelGems.PreScan();
			Homing.PreScan();
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

				// Only calculate the spawnset in lobby, because the game does this as well.
				// Otherwise you can first normally load a spawnset to set the hash, exit and load an empty spawnset in the menu/lobby, then during playing the empty spawnset change it back to the same original spawnset and upload a cheated score.
				if (IsInLobby())
				{
#if DEBUG
					Console.WriteLine("RELOADING HASH");
#endif
					SpawnsetHash = HashUtils.CalculateCurrentSurvivalHash();
				}

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
					LevelGems.Scan();

					if (LevelGems != 0)
					{
						Homing.Scan();

						if (LevelUpTime2 == 0 && LevelGems >= 10 && LevelGems < 70)
							LevelUpTime2 = Time;
						if (LevelUpTime3 == 0 && LevelGems == 70)
							LevelUpTime3 = Time;
						if (LevelUpTime4 == 0 && LevelGems == 71)
							LevelUpTime4 = Time;
					}

					if (TimeFloat >= GameStates.Count && TimeFloat > 0)
					{
						GameStates.Add(new GameState
						{
							DaggersFired = DaggersFired,
							DaggersHit = DaggersHit,
							EnemiesAlive = EnemiesAlive,
							Gems = Gems,
							Homing = Homing,
							Kills = Kills,
						});
					}
				}
				else
				{
					// Only scan death type when dead.
					DeathType.Scan();
				}

				if (string.IsNullOrEmpty(SpawnsetHash))
				{
#if DEBUG
					Console.WriteLine("RELOADING HASH");
#endif
					SpawnsetHash = HashUtils.CalculateCurrentSurvivalHash();
				}
			}
			catch (Exception ex)
			{
				Program.Log.Error("Scan failed", ex);
			}
		}

		public bool IsInLobby()
			=> TimeFloat == 0 && TimeFloat.ValuePrevious == 0 && EnemiesAlive == 0;

		public bool IsInMenu()
			=> TimeFloat == 0 && TimeFloat.ValuePrevious == 0 && EnemiesAlive > 0;
	}
}