using DevilDaggersCore.CustomLeaderboards;
using DevilDaggersCore.Tools;
using DevilDaggersCustomLeaderboards.Gui;
using DevilDaggersCustomLeaderboards.Memory;
using log4net;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Cmd = DevilDaggersCustomLeaderboards.Gui.ConsoleUtils;

namespace DevilDaggersCustomLeaderboards
{
	public static class Program
	{
#pragma warning disable IDE1006
#pragma warning disable SA1310 // Field names should not contain underscore
		private const int MF_BYCOMMAND = 0x00000000;
		private const int SC_MINIMIZE = 0xF020;
		private const int SC_MAXIMIZE = 0xF030;
		private const int SC_SIZE = 0xF000;
#pragma warning restore IDE1006
#pragma warning restore SA1310 // Field names should not contain underscore

		private static readonly CultureInfo culture = CultureInfo.InvariantCulture;

		private static readonly Scanner scanner = Scanner.Instance;

		private static bool recording = true;

		public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? throw new Exception("Could not retrieve logger declaring type."));

		public static string ApplicationName => "DevilDaggersCustomLeaderboards";
		public static string ApplicationDisplayName => "Devil Daggers Custom Leaderboards";

		public static Assembly Assembly { get; private set; } = Assembly.GetExecutingAssembly();
		public static Version LocalVersion { get; private set; } = VersionHandler.GetLocalVersion(Assembly);

		public static void Main()
		{
			Console.CursorVisible = false;
			try
			{
				Console.WindowHeight = 40;
				Console.WindowWidth = 170;
			}
			catch
			{
			}

			DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
			DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
			DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);

			Console.Title = $"{ApplicationDisplayName} {LocalVersion}";

			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;

			Cmd.WriteLine("Checking for updates...");

			VersionHandler.Instance.GetOnlineVersion(ApplicationName, LocalVersion);
			VersionResult versionResult = VersionHandler.Instance.VersionResult;
			Console.Clear();
			if (versionResult.IsUpToDate.HasValue)
			{
				if (LocalVersion < versionResult.Tool.VersionNumberRequired)
				{
					Cmd.WriteLine($"You are using an unsupported and outdated version of {ApplicationDisplayName}. Please update the program.\n(Press any key to continue.)", ConsoleColor.Red);
					Console.ReadKey();
				}
				else if (LocalVersion < versionResult.Tool.VersionNumber)
				{
					Cmd.WriteLine($"An update for {ApplicationDisplayName} is available.\n(Press any key to continue.)", ConsoleColor.Yellow);
					Console.ReadKey();
				}
			}
			else
			{
				Cmd.WriteLine("Failed to check for updates.\n(Press any key to continue.)", ConsoleColor.Red);
				Console.ReadKey();
			}

			Console.Clear();
			while (true)
			{
				scanner.FindWindow();

				if (scanner.Process == null)
				{
					Cmd.WriteLine($"Devil Daggers not found. Make sure the game is running. Retrying in a second...");
					Thread.Sleep(1000);
					Console.Clear();
					continue;
				}

				scanner.Memory.ReadProcess = scanner.Process;
				scanner.Memory.Open();

				scanner.PreScan();
				scanner.Scan();

				if (recording)
				{
					scanner.WriteRecording();

					Thread.Sleep(50);
					Console.SetCursorPosition(0, 0);

					// If player just died
					if (!scanner.IsAlive && scanner.IsAlive.ValuePrevious)
					{
						recording = false;

						int tries = 0;
						UploadResult uploadResult;
						do
						{
							Console.Clear();
							Cmd.WriteLine("Uploading...");
							Cmd.WriteLine();

							// Thread is being blocked by the upload.
							uploadResult = NetworkHandler.Upload();

							if (uploadResult.Success)
							{
								Cmd.WriteLine("Upload successful", ConsoleColor.Green);
								Cmd.WriteLine(uploadResult.Message);
								Cmd.WriteLine();
								if (uploadResult.SubmissionInfo != null)
								{
									uploadResult.SubmissionInfo.WriteLeaderboard(scanner.PlayerId);

									Cmd.WriteLine();

									if (uploadResult.SubmissionInfo.IsHighscore())
										uploadResult.SubmissionInfo.WriteHighscoreStats();
									else
										scanner.WriteStats(uploadResult.SubmissionInfo.Leaderboard, uploadResult.SubmissionInfo.Category, uploadResult.SubmissionInfo.Entries.FirstOrDefault(e => e.PlayerId == scanner.PlayerId));
								}

								Cmd.WriteLine();
							}
							else
							{
								Cmd.WriteLine("Upload failed", ConsoleColor.Red);
								Cmd.WriteLine(uploadResult.Message);
								tries++;
								if (uploadResult.TryCount > 1)
									Cmd.WriteLine($"Retrying (attempt {tries} / {uploadResult.TryCount})");
								Log.Warn($"Upload failed - {uploadResult.Message}");

								Thread.Sleep(500);
							}
						}
						while (!uploadResult.Success && tries < uploadResult.TryCount);

						Console.SetCursorPosition(0, 0);
						Cmd.WriteLine("Ready to restart");
						Cmd.WriteLine();
					}
				}
				else if (scanner.TimeFloat < scanner.TimeFloat.ValuePrevious)
				{
					Console.Clear();
					recording = true;
					scanner.RestartScan();
				}
			}
		}

		[DllImport("user32.dll")]
		internal static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

		[DllImport("user32.dll")]
		private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("kernel32.dll", ExactSpelling = true)]
		private static extern IntPtr GetConsoleWindow();
	}
}