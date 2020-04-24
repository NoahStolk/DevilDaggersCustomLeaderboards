using DevilDaggersCore.Game;
using DevilDaggersCustomLeaderboards.Memory;
using System;
using Cmd = DevilDaggersCustomLeaderboards.Gui.ConsoleUtils;

namespace DevilDaggersCustomLeaderboards.Gui
{
	internal static class ScannerGuiExtensions
	{
		internal static void WriteRecording(this Scanner scanner)
		{
			Cmd.WriteLine($"Scanning process '{scanner.Process.ProcessName}' ({scanner.Process.MainWindowTitle})");
			Cmd.WriteLine("Recording...");
			Cmd.WriteLine();

			Cmd.WriteLine("Player ID", scanner.PlayerId);
			Cmd.WriteLine("Username", scanner.Username);
			Cmd.WriteLine();

			Cmd.WriteLine("Time", scanner.Time.Value.ToString("0.0000"));
			Cmd.WriteLine("Gems", scanner.Gems);
			Cmd.WriteLine("Kills", scanner.Kills);
			Cmd.WriteLine("Shots Hit", scanner.ShotsHit);
			Cmd.WriteLine("Shots Fired", scanner.ShotsFired);
			Cmd.WriteLine("Accuracy", $"{(scanner.ShotsFired == 0 ? 0 : scanner.ShotsHit / (float)scanner.ShotsFired * 100):0.00}%");
			Cmd.WriteLine("Enemies Alive", scanner.EnemiesAlive);
			Cmd.WriteLine("Death Type", GameInfo.GetDeathFromDeathType(scanner.DeathType).Name);
			Cmd.WriteLine("Alive", scanner.IsAlive);
			Cmd.WriteLine("Replay", scanner.IsReplay);
			Cmd.WriteLine();

			if (scanner.LevelGems == 0 && scanner.Gems != 0 && scanner.IsAlive && !scanner.IsReplay)
			{
				Cmd.WriteLine("WARNING: Level up times and homing count are not being detected.\nRestart Devil Daggers to fix this issue.", ConsoleColor.Red);
				// TODO: Log addresses.
			}

			Cmd.WriteLine("Hand", GetHand(scanner.LevelGems));
			int GetHand(int levelGems)
			{
				if (levelGems < 10)
					return 1;
				if (levelGems < 70)
					return 2;
				if (levelGems == 70)
					return 3;
				return 4;
			}

			Cmd.WriteLine("Homing", scanner.Homing);
			Cmd.WriteLine();

			Cmd.WriteLine("Level 2", scanner.LevelUpTime2.ToString("0.0000"));
			Cmd.WriteLine("Level 3", scanner.LevelUpTime3.ToString("0.0000"));
			Cmd.WriteLine("Level 4", scanner.LevelUpTime4.ToString("0.0000"));
			Cmd.WriteLine();
		}
	}
}