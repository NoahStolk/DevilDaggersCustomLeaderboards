using DevilDaggersCore.Spawnsets;
using DevilDaggersCustomLeaderboards.Memory;
using System;
using System.IO;

namespace DevilDaggersCustomLeaderboards
{
	public static class HashUtils
	{
		public static string CalculateSpawnsetHash()
		{
			try
			{
				using (FileStream fs = new FileStream(Path.Combine(Path.GetDirectoryName(Scanner.Instance.Process.MainModule.FileName) ?? throw new Exception("Could not retrieve process directory name."), "dd", "survival"), FileMode.Open, FileAccess.Read))
				{
					if (Spawnset.TryParse(fs, out Spawnset spawnset))
						return spawnset.GetHashString();

					Program.Log.Error($"Failed to calculate spawnset hash because the survival file could not be parsed to a {nameof(Spawnset)} object.");
				}

				return string.Empty;
			}
			catch (Exception ex)
			{
				Program.Log.Error("Failed to calculate spawnset hash.", ex);

				return string.Empty;
			}
		}
	}
}