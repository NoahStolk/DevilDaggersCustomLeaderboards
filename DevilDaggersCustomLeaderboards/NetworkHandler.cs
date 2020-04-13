using DevilDaggersCore.CustomLeaderboards;
using DevilDaggersCore.MemoryHandling;
using DevilDaggersCore.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace DevilDaggersCustomLeaderboards
{
	public sealed class NetworkHandler
	{
		private const float minimalTime = 2.5f;

		private static readonly Lazy<NetworkHandler> lazy = new Lazy<NetworkHandler>(() => new NetworkHandler());
		public static NetworkHandler Instance => lazy.Value;

		private NetworkHandler()
		{
		}

		public UploadResult Upload()
		{
			try
			{
				Scanner scanner = Scanner.Instance;

				if (scanner.PlayerId <= 0)
				{
					Program.Log.Warn($"Invalid player ID: {scanner.PlayerId}");
					return new UploadResult(false, "Invalid player ID.", 3);
				}

				if (scanner.IsReplay)
					return new UploadResult(false, "Run is replay. Unable to validate.", 3);

				// This should fix the broken submissions that occasionally get sent for some reason.
				if (scanner.Time < minimalTime)
					return new UploadResult(false, $"Timer is under {minimalTime:0.0000}. Unable to validate.", 3);

				if (string.IsNullOrEmpty(scanner.SpawnsetHash))
				{
					Program.Log.Warn("Spawnset hash has not been calculated.");
					return new UploadResult(false, "Spawnset hash has not been calculated.");
				}

				// This is to prevent people from initially starting an easy spawnset to get e.g. 800 seconds, then change the survival file during the run to a different (harder) spawnset to trick the application into uploading it to the wrong leaderboard.
				if (Utils.CalculateSpawnsetHash() != scanner.SpawnsetHash)
					return new UploadResult(false, "Cheats suspected. Spawnset hash has been changed since the run was started.");

				string toEncrypt = string.Join(";",
					scanner.PlayerId,
					scanner.Username,
					scanner.Time,
					scanner.Gems,
					scanner.Kills,
					scanner.DeathType,
					scanner.ShotsHit,
					scanner.ShotsFired,
					scanner.EnemiesAlive,
					scanner.Homing,
					scanner.LevelUpTime2,
					scanner.LevelUpTime3,
					scanner.LevelUpTime4);
				string validation = Secrets.EncryptionWrapper.EncryptAndEncode(toEncrypt);

				List<string> queryValues = new List<string>
				{
					$"spawnsetHash={scanner.SpawnsetHash}",
					$"playerId={scanner.PlayerId}",
					$"username={scanner.Username}",
					$"time={scanner.Time}",
					$"gems={scanner.Gems}",
					$"kills={scanner.Kills}",
					$"deathType={scanner.DeathType}",
					$"shotsHit={scanner.ShotsHit}",
					$"shotsFired={scanner.ShotsFired}",
					$"enemiesAlive={scanner.EnemiesAlive}",
					$"homing={scanner.Homing}",
					$"levelUpTime2={scanner.LevelUpTime2}",
					$"levelUpTime3={scanner.LevelUpTime3}",
					$"levelUpTime4={scanner.LevelUpTime4}",
					$"ddclClientVersion={Program.LocalVersion}",
					$"v={HttpUtility.HtmlEncode(validation)}"
				};

				using (WebClient wc = new WebClient())
					return JsonConvert.DeserializeObject<UploadResult>(wc.DownloadString($"{UrlUtils.BaseUrl}/CustomLeaderboards/Upload?{string.Join("&", queryValues)}"));
			}
			catch (Exception ex)
			{
				Program.Log.Error("Error trying to submit score", ex);
				return new UploadResult(false, $"Error uploading score\n\nDetails:\n\n{ex}");
			}
		}

		public void FakeUpload()
		{
			string toEncrypt = string.Join(";",
				21854,
				"xvlv testing",
				11.3,
				1,
				14,
				3,
				666,
				6667,
				54,
				3,
				string.Join(",", new[] { 5, 6, 7.5f }));
			string validation = Secrets.EncryptionWrapper.EncryptAndEncode(toEncrypt);

			List<string> queryValues = new List<string>
			{
				$"spawnsetHash=abc",
				$"playerId=21854",
				$"username=xvlv testing",
				$"time=11.3",
				$"gems=1",
				$"kills=14",
				$"deathType=3",
				$"shotsHit=666",
				$"shotsFired=6667",
				$"enemiesAlive=54",
				$"homing=3",
				$"levelUpTime2=5",
				$"levelUpTime3=6",
				$"levelUpTime4=7.5",
				$"ddclClientVersion={Program.LocalVersion}",
				$"v={HttpUtility.HtmlEncode(validation)}"
			};

			using (WebClient wc = new WebClient())
				Console.WriteLine(JsonConvert.DeserializeObject<UploadResult>(wc.DownloadString($"{UrlUtils.BaseUrl}/CustomLeaderboards/Upload?{string.Join("&", queryValues)}")));
		}
	}
}