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
	}
}