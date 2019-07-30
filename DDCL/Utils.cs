using DDCL.MemoryHandling;
using DevilDaggersCore.Spawnsets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DDCL
{
	public static class Utils
	{
		// TODO: Use DevilDaggersCore
		private static readonly List<string> Deaths = new List<string>()
		{
			"FALLEN",
			"SWARMED",
			"IMPALED",
			"GORED",
			"INFESTED",
			"OPENED",
			"PURGED",
			"DESECRATED",
			"SACRIFICED",
			"EVISCERATED",
			"ANNIHILATED",
			"INTOXICATED",
			"ENVENOMATED",
			"INCARNATED",
			"DISCARNATED",
			"BARBED"
		};

		// TODO: Use DevilDaggersCore
		public static string GetDeathName(int value)
		{
			if (value < 0 || value > 15)
				return "N/A";
			return Deaths[value];
		}

		private static string clientVersion;

		public static Version GetClientVersion()
		{
			if (string.IsNullOrEmpty(clientVersion))
			{
				Assembly assembly = Assembly.GetExecutingAssembly();
				clientVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
			}
			return Version.Parse(clientVersion);
		}

		public static string CalculateSpawnsetHash()
		{
			try
			{
				using (FileStream fs = new FileStream(Path.Combine(Path.GetDirectoryName(Scanner.Instance.Process.MainModule.FileName), "dd", "survival"), FileMode.Open, FileAccess.Read))
					if (Spawnset.TryParse(fs, out Spawnset spawnsetObject))
						return spawnsetObject.GetHashString();

				return string.Empty;
			}
			catch
			{
				return string.Empty;
			}
		}
	}
}