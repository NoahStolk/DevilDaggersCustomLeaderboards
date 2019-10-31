using DevilDaggersCore.MemoryHandling;
using DevilDaggersCore.Spawnsets;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DevilDaggersCustomLeaderboards
{
	public static class Utils
	{
		private static Version clientVersion;
		public static Version ClientVersion
		{
			get
			{
				if (clientVersion == null)
					clientVersion = Version.Parse(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
				return clientVersion;
			}
		}

		public static string CalculateSpawnsetHash()
		{
			try
			{
				using (FileStream fs = new FileStream(Path.Combine(Path.GetDirectoryName(Scanner.Instance.Process.MainModule.FileName), "dd", "survival"), FileMode.Open, FileAccess.Read))
				{
					if (Spawnset.TryParse(fs, out Spawnset spawnset))
						return spawnset.GetHashString();

					Program.logger.Error($"Failed to calculate spawnset hash because the survival file could not be parsed to a {nameof(Spawnset)} object.");
				}

				return string.Empty;
			}
			catch (Exception ex)
			{
				Program.logger.Error("Failed to calculate spawnset hash.", ex);
				
				return string.Empty;
			}
		}
	}
}