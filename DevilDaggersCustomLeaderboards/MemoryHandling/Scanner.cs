using DevilDaggersCustomLeaderboards.Variables;
using System;
using System.Diagnostics;

namespace DevilDaggersCustomLeaderboards.MemoryHandling
{
	public sealed class Scanner
	{
		public const string ProcessNameToFind = "dd";
		private const int Magic = 0x001F30C0;

		public Process Process { get; set; }
		public Memory Memory { get; private set; } = new Memory();

		public IntVariable PlayerID { get; private set; } = new IntVariable(Magic, 0x5C);
		public StringVariable PlayerName { get; private set; } = new StringVariable(Magic, 0x60);
		public FloatVariable Time { get; private set; } = new FloatVariable(Magic, 0x1A0);
		public IntVariable Gems { get; private set; } = new IntVariable(Magic, 0x1C0);
		public IntVariable Kills { get; private set; } = new IntVariable(Magic, 0x1BC);
		public IntVariable DeathType { get; private set; } = new IntVariable(Magic, 0x1C4);
		public IntVariable ShotsFired { get; private set; } = new IntVariable(Magic, 0x1B4);
		public IntVariable ShotsHit { get; private set; } = new IntVariable(Magic, 0x1B8);
		public IntVariable EnemiesAlive { get; private set; } = new IntVariable(Magic, 0x1FC);
		public BoolVariable IsAlive { get; private set; } = new BoolVariable(Magic, 0x1A4);
		public BoolVariable IsReplay { get; private set; } = new BoolVariable(Magic, 0x35D);

		private static readonly Lazy<Scanner> lazy = new Lazy<Scanner>(() => new Scanner());
		public static Scanner Instance => lazy.Value;

		private Scanner()
		{
		}

		public void FindWindow()
		{
			foreach (Process proc in Process.GetProcesses())
			{
				if (proc.ProcessName.Contains(ProcessNameToFind))
				{
					Process = proc;
					return;
				}
				else
				{
					Process = null;
				}
			}
		}
	}
}