using DevilDaggersCustomLeaderboards.Variables;
using System;
using System.Threading;

namespace DevilDaggersCustomLeaderboards
{
	public static class Program
	{
		public static void Main()
		{
			Console.CursorVisible = false;

			Scanner scanner = Scanner.Instance;

			for (; ; )
			{
				scanner.FindWindow();

				if (scanner.Memory.ReadProcess == null)
				{
					Console.WriteLine($"Process '{Scanner.ProcessNameToFind}' not found");
					Thread.Sleep(50);
					Console.Clear();
					continue;
				}

				Console.WriteLine($"Scanning process '{scanner.Memory.ReadProcess.ProcessName}' ({scanner.Memory.ReadProcess.MainWindowTitle})");

				scanner.Memory.Open();

				Write();
				Write("PlayerID", scanner.PlayerID);
				Write();

				Write("Time", scanner.Time);
				Write("Gems", scanner.Gems);
				Write("Kills", scanner.Kills);
				Write("Daggers Fired", scanner.DaggersFired);
				Write("Daggers Hit", scanner.DaggersHit);
				Write("Enemies Alive", scanner.EnemiesAlive);
				Write("Alive", scanner.IsAlive);
				Write("Replay", scanner.IsReplay);
				Write();

				Write("Accuracy", $"{scanner.DaggersHit.Value / (float)scanner.DaggersFired.Value * 100}%");

				Thread.Sleep(50);
				Console.SetCursorPosition(0, 0);
			}
		}

		private static void Write<T>(string name, AbstractVariable<T> gameVariable)
		{
			Console.WriteLine($"{name.PadRight(20)}{gameVariable.ToString().PadRight(20)}");
		}

		private static void Write()
		{
			Console.WriteLine(new string(' ', 40));
		}

		private static void Write(string text)
		{
			Console.WriteLine(text.PadRight(40));
		}

		private static void Write(string textLeft, string textRight)
		{
			Console.WriteLine($"{textLeft.PadRight(20)}{textRight.PadRight(20)}");
		}
	}
}