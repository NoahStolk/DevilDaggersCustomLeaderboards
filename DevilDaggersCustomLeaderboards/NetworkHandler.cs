using DevilDaggersCore.CustomLeaderboards;
using DevilDaggersCore.Utils;
using DevilDaggersCustomLeaderboards.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace DevilDaggersCustomLeaderboards
{
	internal sealed class NetworkHandler
	{
		private const float minimalTime = 1f;

		private static readonly Lazy<NetworkHandler> lazy = new Lazy<NetworkHandler>(() => new NetworkHandler());
		internal static NetworkHandler Instance => lazy.Value;

		private NetworkHandler()
		{
		}

		internal UploadResult Upload()
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
					return new UploadResult(false, $"Timer is under {minimalTime:0.0000}. Unable to validate.", 1);

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
					string.Join(",", new[] { scanner.LevelUpTime2, scanner.LevelUpTime3, scanner.LevelUpTime4 }));
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

				using WebClient wc = new WebClient();
				return JsonConvert.DeserializeObject<UploadResult>(wc.DownloadString($"{UrlUtils.BaseUrl}/CustomLeaderboards/Upload?{string.Join("&", queryValues)}"));
			}
			catch (Exception ex)
			{
				Program.Log.Error("Error trying to submit score", ex);
				return new UploadResult(false, $"Error uploading score\n\nDetails:\n\n{ex}");
			}
		}

		internal void FakeUpload(int id, float seconds)
		{
			try
			{
				string hash = "7A427E3149DBD1307B9F730BECA13EE9007FC1CEC0B23E493B236DFA8747ED4A"; // DaggerLobby
				string username = "xvlv testing";
				int gems = 1;
				int kills = 14;
				int deathType = 3;
				int shotsHit = 666;
				int shotsFired = 6667;
				int enemiesAlive = 54;
				int homing = 3;
				float levelUpTime2 = 0.5f;
				float levelUpTime3 = 1.5f;
				float levelUpTime4 = 2.5f;

				string toEncrypt = string.Join(";",
					id,
					username,
					seconds,
					gems,
					kills,
					deathType,
					shotsHit,
					shotsFired,
					enemiesAlive,
					homing,
					string.Join(",", new[] { levelUpTime2, levelUpTime3, levelUpTime4 }));
				string validation = Secrets.EncryptionWrapper.EncryptAndEncode(toEncrypt);

				List<string> queryValues = new List<string>
				{
					$"spawnsetHash={hash}",
					$"playerId={id}",
					$"username={username}",
					$"time={seconds}",
					$"gems={gems}",
					$"kills={kills}",
					$"deathType={deathType}",
					$"shotsHit={shotsHit}",
					$"shotsFired={shotsFired}",
					$"enemiesAlive={enemiesAlive}",
					$"homing={homing}",
					$"levelUpTime2={levelUpTime2}",
					$"levelUpTime3={levelUpTime3}",
					$"levelUpTime4={levelUpTime4}",
					$"ddclClientVersion={Program.LocalVersion}",
					$"v={HttpUtility.HtmlEncode(validation)}"
				};

				using WebClient wc = new WebClient();
				UploadResult result = JsonConvert.DeserializeObject<UploadResult>(wc.DownloadString($"{UrlUtils.BaseUrl}/CustomLeaderboards/Upload?{string.Join("&", queryValues)}"));
				Console.WriteLine(result.Message);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error uploading score\n\nDetails:\n\n{ex.Message}");
			}
		}
	}
}