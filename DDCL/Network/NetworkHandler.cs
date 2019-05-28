using DDCL.MemoryHandling;
using Newtonsoft.Json;
using NetBase.Encryption;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace DDCL.Network
{
	public sealed class NetworkHandler
	{
		private const string BaseURL = "https://devildaggers.info";
		//private const string BaseURL = "http://localhost:2963/";

		public Version ServerVersion { get; private set; }
		public Version ServerVersionRequired { get; private set; }

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
					$"ddclClientVersion={Utils.GetClientVersion()}",
					$"v={HttpUtility.HtmlEncode(validation)}"
				};

				using (WebClient wc = new WebClient())
					return JsonConvert.DeserializeObject<JsonResult>(wc.DownloadString($"{BaseURL}/CustomLeaderboards/Upload?{string.Join("&", queryValues)}"));
			}
			catch (Exception ex)
			{
				Program.logger.Error("Error trying to submit score", ex);
				return new JsonResult(false, $"Error uploading score\n\nDetails:\n\n{ex}");
			}
		}

		public bool GetVersionNumberFromServer()
		{
			string url = $"{BaseURL}/API/GetToolVersions";

			try
			{
				using (WebClient client = new WebClient())
				{
					using (MemoryStream stream = new MemoryStream(client.DownloadData(url)))
					{
						byte[] byteArray = new byte[1024];
						int count = stream.Read(byteArray, 0, 1024);
						string str = Encoding.UTF8.GetString(byteArray);
						dynamic json = JsonConvert.DeserializeObject(str);
						foreach (dynamic tool in json)
						{
							if ((string)tool.Name == "DDCL")
							{
								ServerVersion = Version.Parse((string)tool.VersionNumber);
								ServerVersionRequired = Version.Parse((string)tool.VersionNumberRequired);
								return true;
							}
						}
					}
				}

				Program.logger.Error($"DDCL not found in {url}.");
				return false;
			}
			catch (Exception ex)
			{
				Program.logger.Error("Failed to check for updates.", ex);
				return false;
			}
		}
	}
}