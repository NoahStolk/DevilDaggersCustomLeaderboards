using DDCL.MemoryHandling;
using Newtonsoft.Json;
using System;
using System.Net;

namespace DDCL.Network
{
	public sealed class NetworkHandler
	{
		private const string BaseURL = "https://devildaggers.info";
		//private const string BaseURL = "http://localhost:2963/";

		/// <summary>
		/// This should fix the broken submissions that occasionally get sent for some reason.
		/// </summary>
		private const float MinimalTime = 2.5f;

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

				if (scanner.PlayerID.Value <= 0)
				{
					Program.logger.Warn($"Invalid player ID: {scanner.PlayerID.Value}");
					return new JsonResult(false, "Invalid player ID.", 3);
				}

				if (scanner.IsReplay.Value)
					return new JsonResult(false, "Run is replay. Unable to validate.", 3);
				if (scanner.Time.Value < MinimalTime)
					return new JsonResult(false, $"Timer is under {MinimalTime.ToString("0.0000")}. Unable to validate.", 3);
				if (string.IsNullOrEmpty(scanner.SpawnsetHash))
					return new JsonResult(false, $"Not the entire run has been recorded. You must start recording before the timer reaches {Scanner.MaxHashTime.ToString("0.0000")}. Unable to validate.", 3);

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