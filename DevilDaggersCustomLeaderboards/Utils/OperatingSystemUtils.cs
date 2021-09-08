using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Exceptions;
using System.Runtime.InteropServices;

namespace DevilDaggersCustomLeaderboards.Utils
{
	public static class OperatingSystemUtils
	{
		static OperatingSystemUtils()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				OperatingSystem = OperatingSystem.Windows;
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				OperatingSystem = OperatingSystem.Linux;
			else
				OperatingSystem = OperatingSystem.None;
		}

		public static OperatingSystem OperatingSystem { get; }

		public static long GetMarker(DdclSettings ddclSettings) => OperatingSystem switch
		{
			OperatingSystem.Windows => ddclSettings.MarkerWindowsSteam,
			OperatingSystem.Linux => ddclSettings.MarkerLinuxSteam,
			_ => throw new OperatingSystemNotSupportedException(),
		};
	}
}
