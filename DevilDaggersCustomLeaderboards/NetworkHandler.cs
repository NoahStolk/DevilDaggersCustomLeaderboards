using DevilDaggersCore.CustomLeaderboards;
using DevilDaggersCore.MemoryHandling;
using DevilDaggersCore.Tools;
using EncryptionUtils;
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

				if (scanner.PlayerId.Value <= 0)
				{
					Program.Log.Warn($"Invalid player ID: {scanner.PlayerId.Value}");
					return new UploadResult(false, "Invalid player ID.", 3);
				}

				if (scanner.IsReplay.Value)
					return new UploadResult(false, "Run is replay. Unable to validate.", 3);

				// This should fix the broken submissions that occasionally get sent for some reason.
				if (scanner.Time.Value < minimalTime)
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
					scanner.PlayerId.Value,
					scanner.Username.Value,
					scanner.Time.Value,
					scanner.Gems.Value,
					scanner.Kills.Value,
					scanner.DeathType.Value,
					scanner.ShotsHit.Value,
					scanner.ShotsFired.Value,
					scanner.EnemiesAlive.Value,
					scanner.Homing,
					string.Join(",", scanner.LevelUpTimes));
				AesBase32Wrapper aes = new AesBase32Wrapper("4GDdtUpDelr2wIae", "xx7SXitvxQh4tJzn", "K0sfsKXLZKmKs929");
				string validation = aes.EncryptAndEncode(toEncrypt);

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
					$"levelUpTime2={scanner.LevelUpTimes[0]}",
					$"levelUpTime3={scanner.LevelUpTimes[1]}",
					$"levelUpTime4={scanner.LevelUpTimes[2]}",
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
			AesBase32Wrapper aes = new AesBase32Wrapper("4GDdtUpDelr2wIae", "xx7SXitvxQh4tJzn", "K0sfsKXLZKmKs929");
			string validation = aes.EncryptAndEncode(toEncrypt);

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