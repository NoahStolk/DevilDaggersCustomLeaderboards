using DevilDaggersCustomLeaderboards.Clients;
using System;
using System.Runtime.InteropServices;

namespace DevilDaggersCustomLeaderboards.Utils
{
	public static class OperatingSystemUtils
	{
		public static bool IsWindows() =>
			RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

		public static bool IsMacOS() =>
			RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

		public static bool IsLinux() =>
			RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

		public static Clients.OperatingSystem GetOperatingSystem()
		{
			if (IsWindows())
				return Clients.OperatingSystem.Windows;
			if (IsLinux())
				return Clients.OperatingSystem.Linux;
			return Clients.OperatingSystem.None;
		}

		public static long GetMarker(DdclSettings ddclSettings)
		{
			Clients.OperatingSystem os = GetOperatingSystem();
			return os switch
			{
				Clients.OperatingSystem.Windows => ddclSettings.MarkerWindowsSteam,
				Clients.OperatingSystem.Linux => ddclSettings.MarkerLinuxSteam,
				_ => throw new NotSupportedException($"Retrieving marker for operating system '{os}' is not supported."),
			};
		}
	}
}
