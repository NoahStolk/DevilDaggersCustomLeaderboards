using DevilDaggersCustomLeaderboards.Variables;
using System;
using System.Net;
using System.Threading;

namespace DevilDaggersCustomLeaderboards
{
	/// <summary>
	/// Handles the main program and GUI-related tasks.
	/// Special Write methods are used to output to the console, as clearing the console after every update makes everything flicker which is ugly.
	/// So instead of clearing the console using Console.Clear(), we just reset the cursor to the top-left, and then overwrite everything from the previous update using the special Write methods.
	/// </summary>
	public static class Program
	{
		private static bool wasAlive;
		private static bool recording = true;

		public static void Main()
		{
			Console.CursorVisible = false;

			Scanner scanner = Scanner.Instance;

			for (; ; )
			{
				scanner.FindWindow();

				if (scanner.Process == null)
				{
					Write($"Process '{Scanner.ProcessNameToFind}' not found. Retrying in a second.");
					Thread.Sleep(1000);
					Console.Clear();
					continue;
				}

				if (recording)
				{
					scanner.Memory.ReadProcess = scanner.Process;
					scanner.Memory.Open();

					Write($"Scanning process '{scanner.Process.ProcessName}' ({scanner.Process.MainWindowTitle})");
					Write("Recording...");
					Write();

					Write("PlayerID", scanner.PlayerID);
					Write();

					Write("Time", scanner.Time);
					Write("Gems", scanner.Gems);
					Write("Kills", scanner.Kills);
					Write("Shots Hit", scanner.ShotsHit);
					Write("Shots Fired", scanner.ShotsFired);
					Write("Enemies Alive", scanner.EnemiesAlive);
					Write("Alive", scanner.IsAlive);
					Write("Replay", scanner.IsReplay);
					Write();

					Write("Accuracy", $"{(scanner.ShotsHit.Value / (float)scanner.ShotsFired.Value * 100).ToString("0.00")}%");

					Thread.Sleep(50);
					Console.SetCursorPosition(0, 0);
				}

				// If player just died
				if (!scanner.IsAlive.Value && wasAlive)
				{
					Console.Clear();
					recording = false;
					Write("Uploading...");

					// Block the thread because this is easy and doesn't matter anyway
					string query = $"spawnsetHash={Utils.CalculateSpawnsetHash()}&playerID={scanner.PlayerID}&username=WIP&time={scanner.Time}&gems={scanner.Gems}&kills={scanner.Kills}&deathType={scanner.DeathType}&shotsHit={scanner.ShotsHit}&shotsFired={scanner.ShotsFired}&enemiesAlive={scanner.EnemiesAlive}&homing=0&levelUpTime2=0&levelUpTime3=0&levelUpTime4=0&submitDate={DateTime.Now}";
					string jsonResult;
					using (WebClient wc = new WebClient())
					{
						jsonResult = wc.DownloadString($"http://localhost:2963/CustomLeaderboards/Upload?{query}");
					}

					Write(jsonResult);
				}

				// On restart
				if (scanner.IsAlive.Value && !wasAlive)
				{
					Console.Clear();
					recording = true;
				}

				wasAlive = scanner.IsAlive.Value;
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