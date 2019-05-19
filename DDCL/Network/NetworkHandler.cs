using DDCL.MemoryHandling;
using Newtonsoft.Json;
using System;
using System.Net;

namespace DDCL.Network
{
	public sealed class NetworkHandler
	{
		private const string BaseURL = "https://devildaggers.info";

		private static readonly Lazy<NetworkHandler> lazy = new Lazy<NetworkHandler>(() => new NetworkHandler());
		public static NetworkHandler Instance => lazy.Value;

		private NetworkHandler()
		{
		}

		public JsonResult Upload()
		{
			try
			{
				Scanner scanner = Scanner.Instance;

				if (string.IsNullOrEmpty(scanner.SpawnsetHash))
					return new JsonResult(false, "Not the entire run has been recorded. Unable to validate.");

				string query = $"spawnsetHash={scanner.SpawnsetHash}&playerID={scanner.PlayerID}&username={scanner.PlayerName}&time={scanner.Time}&gems={scanner.Gems}&kills={scanner.Kills}&deathType={scanner.DeathType}&shotsHit={scanner.ShotsHit}&shotsFired={scanner.ShotsFired}&enemiesAlive={scanner.EnemiesAlive}&homing={scanner.Homing}&levelUpTime2={scanner.LevelUpTimes[0]}&levelUpTime3={scanner.LevelUpTimes[1]}&levelUpTime4={scanner.LevelUpTimes[2]}&ddclClientVersion={Utils.GetVersion()}";
				using (WebClient wc = new WebClient())
					return JsonConvert.DeserializeObject<JsonResult>(wc.DownloadString($"{BaseURL}/CustomLeaderboards/Upload?{query}"));
			}
			catch (Exception ex)
			{
				Program.logger.Error("Error trying to submit score", ex);
				return new JsonResult(false, $"Error uploading score\n\nDetails:\n\n{ex}");
			}
		}
	}
}