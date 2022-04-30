using DevilDaggersCustomLeaderboards.Native;
using DevilDaggersCustomLeaderboards.Network;
using DevilDaggersCustomLeaderboards.Utils;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace DevilDaggersCustomLeaderboards;

public static class Program
{
	public static async Task Main()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

		InitializeConsole();

		ServiceProvider serviceProvider = new ServiceCollection()
			.AddLogging(lb => lb.AddSerilog(new LoggerConfiguration().WriteTo.File("DDCL.log").CreateLogger()))
			.AddSingleton<NetworkService>()
			.AddSingleton<RecorderService>()
			.AddSingleton<MemoryService>()
			.AddSingleton<UploadService>()
			.BuildServiceProvider();

		NetworkService networkService = serviceProvider.GetRequiredService<NetworkService>();
		await networkService.CheckForUpdates();

		RecorderService recorderService = serviceProvider.GetRequiredService<RecorderService>();
		await recorderService.Record();
	}

	private static void InitializeConsole()
	{
		Console.CursorVisible = false;

		try
		{
#pragma warning disable CA1416 // Validate platform compatibility
#if DEBUG
			Console.WindowHeight = 80;
#else
			Console.WindowHeight = 60;
#endif
			Console.WindowWidth = 170;
#pragma warning restore CA1416 // Validate platform compatibility
		}
		catch
		{
			// Do nothing if resizing the console failed. It usually means a very large custom font caused the window to be too large which throws an exception.
		}

#pragma warning disable CA1806 // Do not ignore method results
		const int MF_BYCOMMAND = 0x00000000;
		const int SC_MINIMIZE = 0xF020;
		const int SC_MAXIMIZE = 0xF030;
		const int SC_SIZE = 0xF000;
		NativeMethods.DeleteMenu(NativeMethods.GetSystemMenu(NativeMethods.GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
		NativeMethods.DeleteMenu(NativeMethods.GetSystemMenu(NativeMethods.GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
		NativeMethods.DeleteMenu(NativeMethods.GetSystemMenu(NativeMethods.GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);
#pragma warning restore CA1806 // Do not ignore method results

		ColorUtils.ModifyConsoleColor(2, 0x47, 0x8B, 0x41);
		ColorUtils.ModifyConsoleColor(3, 0xCD, 0x7F, 0x32);
		ColorUtils.ModifyConsoleColor(4, 0x77, 0x1D, 0x00);
		ColorUtils.ModifyConsoleColor(5, 0xAF, 0x6B, 0x00);
		ColorUtils.ModifyConsoleColor(6, 0x97, 0x6E, 0x2E);
		ColorUtils.ModifyConsoleColor(7, 0xDD, 0xDD, 0xDD);
		ColorUtils.ModifyConsoleColor(9, 0xC8, 0xA2, 0xC8);
		ColorUtils.ModifyConsoleColor(11, 0x80, 0x06, 0x00);
		ColorUtils.ModifyConsoleColor(14, 0xFF, 0xDF, 0x00);

#if DEBUG
		Console.Title = $"{Constants.ApplicationDisplayName} {Constants.LocalVersion} DEBUG";
#else
		Console.Title = $"{Constants.ApplicationDisplayName} {Constants.LocalVersion}";
#endif
	}
}
