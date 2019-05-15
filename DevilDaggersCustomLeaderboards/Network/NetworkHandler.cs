using DevilDaggersCustomLeaderboards.MemoryHandling;
using Newtonsoft.Json;
using System;
using System.Net;

namespace DevilDaggersCustomLeaderboards.Network
{
	public sealed class NetworkHandler
	{
		private const string BaseURL = "http://localhost:2963";

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

				JsonResult jsonResult;
				string query = $"spawnsetHash={Utils.CalculateSpawnsetHash()}&playerID={scanner.PlayerID}&username={scanner.PlayerName}&time={scanner.Time}&gems={scanner.Gems}&kills={scanner.Kills}&deathType={scanner.DeathType}&shotsHit={scanner.ShotsHit}&shotsFired={scanner.ShotsFired}&enemiesAlive={scanner.EnemiesAlive}&homing={Program.homing}&levelUpTime2={Program.levelUpTimes[0]}&levelUpTime3={Program.levelUpTimes[1]}&levelUpTime4={Program.levelUpTimes[2]}";
				using (WebClient wc = new WebClient())
				{
					jsonResult = JsonConvert.DeserializeObject<JsonResult>(wc.DownloadString($"{BaseURL}/CustomLeaderboards/Upload?{query}"));
				}

				return jsonResult;
			}
			catch (Exception ex)
			{
				return new JsonResult(false, $"Error uploading score\n\nDetails:\n\n{ex}");
			}
		}
	}
}