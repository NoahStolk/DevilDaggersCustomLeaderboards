using Newtonsoft.Json;
using NetBase.Encryption;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using DevilDaggersCore.CustomLeaderboards;
using DevilDaggersCore.MemoryHandling;
using DevilDaggersCore.Tools;

namespace DevilDaggersCustomLeaderboards
{
	public sealed class NetworkHandler
	{
		private const float MinimalTime = 2.5f;

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

				if (scanner.PlayerID.Value <= 0)
				{
					Program.Log.Warn($"Invalid player ID: {scanner.PlayerID.Value}");
					return new UploadResult(false, "Invalid player ID.", 3);
				}

				if (scanner.IsReplay.Value)
					return new UploadResult(false, "Run is replay. Unable to validate.", 3);

				// This should fix the broken submissions that occasionally get sent for some reason.
				if (scanner.Time.Value < MinimalTime)
					return new UploadResult(false, $"Timer is under {MinimalTime.ToString("0.0000")}. Unable to validate.", 3);

				if (string.IsNullOrEmpty(scanner.SpawnsetHash))
				{
					Program.Log.Warn("Spawnset hash has not been calculated.");
					return new UploadResult(false, "Spawnset hash has not been calculated.");
				}

				// This is to prevent people from initially starting an easy spawnset to get e.g. 800 seconds, then change the survival file during the run to a different (harder) spawnset to trick the application into uploading it to the wrong leaderboard.
				if (Utils.CalculateSpawnsetHash() != scanner.SpawnsetHash)
					return new UploadResult(false, "Cheats suspected. Spawnset hash has been changed since the run was started.");

				string toEncrypt = string.Join(";",
					scanner.PlayerID.Value,
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
					$"playerID={scanner.PlayerID}",
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
	}
}