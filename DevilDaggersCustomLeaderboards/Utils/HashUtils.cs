using DevilDaggersCore.Spawnsets;
using DevilDaggersCustomLeaderboards.Memory;
using System;
using System.IO;

namespace DevilDaggersCustomLeaderboards.Utils
{
	public static class HashUtils
	{
		public static string CalculateCurrentSurvivalHash()
		{
			try
			{
				if (Spawnset.TryParse(File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(Scanner.Instance.Process?.MainModule?.FileName) ?? throw new("Could not retrieve process file name."), "dd", "survival")), out Spawnset spawnset))
					return spawnset.GetHashString();

				Program.Log.Error($"Failed to calculate spawnset hash because the survival file could not be parsed to a {nameof(Spawnset)} object.");
			}
			catch (Exception ex)
			{
				Program.Log.Error("Failed to calculate spawnset hash.", ex);
			}

			return string.Empty;
		}
	}
}
