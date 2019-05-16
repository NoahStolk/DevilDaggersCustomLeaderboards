using DevilDaggersCustomLeaderboards.MemoryHandling;
using DevilDaggersCustomLeaderboards.Network;
using DevilDaggersCustomLeaderboards.Variables;
using System;
using System.Globalization;
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
		private static readonly Version version = new Version(0, 1, 0, 2);

		private static readonly Scanner scanner = Scanner.Instance;
		private static bool recording = true;

		private static bool wasAlive;
		public static int homing;
		public static readonly float[] levelUpTimes = new float[3] { 0, 0, 0 };
		private static int handCurrent = 1;
		private static int handPrevious = 1;

		public static void Main()
		{
			Console.CursorVisible = false;
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

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

					Write($"DevilDaggersCustomLeaderboards version {version}\n\nScanning process '{scanner.Process.ProcessName}' ({scanner.Process.MainWindowTitle})");
					Write("Recording...");
					Write();

					Write("PlayerID", scanner.PlayerID);
					Write("Player name", scanner.PlayerName);
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

					byte[] bytes = scanner.Memory.Read(scanner.Process.MainModule.BaseAddress + 0x001F8084, 4, out _);
					int ptr = AddressUtils.ToDec(AddressUtils.MakeAddress(bytes));
					bytes = scanner.Memory.Read(new IntPtr(ptr), 4, out _);
					ptr = AddressUtils.ToDec(AddressUtils.MakeAddress(bytes));
					bytes = scanner.Memory.Read(new IntPtr(ptr) + 0x218, 4, out _);
					int levelGems = BitConverter.ToInt32(bytes);

					bytes = scanner.Memory.Read(new IntPtr(ptr) + 0x224, 4, out _);
					homing = BitConverter.ToInt32(bytes);

					handCurrent = GetHand(levelGems);
					if (handCurrent > handPrevious)
						levelUpTimes[handPrevious - 1] = scanner.Time.Value;

					Write("Hand", handCurrent.ToString());
					Write("Homing", homing.ToString());
					Write();

					//Write("HASH", Utils.CalculateSpawnsetHash());
					//Write();

					Write("Accuracy", $"{(scanner.ShotsFired.Value == 0 ? 0 : scanner.ShotsHit.Value / (float)scanner.ShotsFired.Value * 100).ToString("0.00")}%");

					Thread.Sleep(50);
					Console.SetCursorPosition(0, 0);
				}

				// If player just died
				if (!scanner.IsAlive.Value && wasAlive)
				{
					recording = false;

					JsonResult jsonResult;
					do
					{
						jsonResult = NetworkHandler.Instance.Upload();
						Console.Clear();
						Write("Uploading...");

						Console.Clear();
						if (jsonResult.success)
							Write("Upload successful", ConsoleColor.Green);
						else
							Write("Upload failed", ConsoleColor.Red);
						Write(jsonResult.message);

						Thread.Sleep(500);
					}
					while (!jsonResult.success);
				}

				// On restart
				if (scanner.IsAlive.Value && !wasAlive)
				{
					Console.Clear();
					recording = true;
				}

				wasAlive = scanner.IsAlive.Value;
				handPrevious = handCurrent;
			}
		}

		private static int GetHand(int gems)
		{
			if (gems < 10)
				return 1;
			if (gems < 70)
				return 2;
			if (gems == 70)
				return 3;
			return 4;
		}

		private static void Write<T>(string name, AbstractVariable<T> gameVariable, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine($"{name.PadRight(20)}{gameVariable.ToString().PadRight(20)}");
			Console.ForegroundColor = ConsoleColor.White;
		}

		private static void Write()
		{
			Console.WriteLine(new string(' ', 40));
		}

		private static void Write(string text, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(text.PadRight(40));
			Console.ForegroundColor = ConsoleColor.White;
		}

		private static void Write(string textLeft, string textRight, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine($"{textLeft.PadRight(20)}{textRight.PadRight(20)}");
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}