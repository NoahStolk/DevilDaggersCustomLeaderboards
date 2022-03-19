using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Extensions;
using DevilDaggersCustomLeaderboards.Network;
using DevilDaggersCustomLeaderboards.Utils;
using System;
using System.Threading.Tasks;
using System.Web;

namespace DevilDaggersCustomLeaderboards;

public class UploadService
{
	private readonly NetworkService _networkService;
	private readonly ScannerService _scannerService;

	public UploadService(NetworkService networkService, ScannerService scannerService)
	{
		_networkService = networkService;
		_scannerService = scannerService;
	}

	public async Task<GetUploadSuccess?> UploadRun()
	{
		Console.Clear();
		Cmd.WriteLine("Checking if this spawnset has a leaderboard...");
		Cmd.WriteLine();

		if (!await _networkService.CheckIfLeaderboardExists(_scannerService.SurvivalHashMd5))
			return null;

		Console.Clear();
		Cmd.WriteLine("Uploading...");
		Cmd.WriteLine();

		string toEncrypt = string.Join(
			";",
			_scannerService.PlayerId,
			_scannerService.Time.ConvertToTimeInt(),
			_scannerService.GemsCollected,
			_scannerService.GemsDespawned,
			_scannerService.GemsEaten,
			_scannerService.GemsTotal,
			_scannerService.EnemiesKilled,
			_scannerService.DeathType,
			_scannerService.DaggersHit,
			_scannerService.DaggersFired,
			_scannerService.EnemiesAlive,
			_scannerService.HomingDaggers,
			_scannerService.HomingDaggersEaten,
			_scannerService.IsReplay ? 1 : 0,
			HashUtils.ByteArrayToHexString(_scannerService.SurvivalHashMd5),
			string.Join(",", new[] { _scannerService.LevelUpTime2.ConvertToTimeInt(), _scannerService.LevelUpTime3.ConvertToTimeInt(), _scannerService.LevelUpTime4.ConvertToTimeInt() }));
		string validation = Secrets.EncryptionWrapper.EncryptAndEncode(toEncrypt);

		AddUploadRequest uploadRequest = new()
		{
			DaggersFired = _scannerService.DaggersFired,
			DaggersHit = _scannerService.DaggersHit,
			ClientVersion = Constants.LocalVersion.ToString(),
			DeathType = _scannerService.DeathType,
			EnemiesAlive = _scannerService.EnemiesAlive,
			GemsCollected = _scannerService.GemsCollected,
			GemsDespawned = _scannerService.GemsDespawned,
			GemsEaten = _scannerService.GemsEaten,
			GemsTotal = _scannerService.GemsTotal,
			HomingDaggers = _scannerService.HomingDaggers,
			HomingDaggersEaten = _scannerService.HomingDaggersEaten,
			EnemiesKilled = _scannerService.EnemiesKilled,
			LevelUpTime2 = _scannerService.LevelUpTime2.ConvertToTimeInt(),
			LevelUpTime3 = _scannerService.LevelUpTime3.ConvertToTimeInt(),
			LevelUpTime4 = _scannerService.LevelUpTime4.ConvertToTimeInt(),
			PlayerId = _scannerService.PlayerId,
			SurvivalHashMd5 = _scannerService.SurvivalHashMd5,
			Time = _scannerService.Time.ConvertToTimeInt(),
			PlayerName = _scannerService.PlayerName,
			IsReplay = _scannerService.IsReplay,
			Validation = HttpUtility.HtmlEncode(validation),
			GameData = _scannerService.GetGameData(),
#if DEBUG
			BuildMode = "Debug",
#else
			BuildMode = "Release",
#endif
			OperatingSystem = "Windows",
			ProhibitedMods = _scannerService.ProhibitedMods,
			Client = "DevilDaggersCustomLeaderboards",
			ReplayData = _scannerService.GetReplay(),
			Status = _scannerService.Status,
			ReplayPlayerId = _scannerService.ReplayPlayerId,
			GameMode = _scannerService.GameMode,
			TimeAttackOrRaceFinished = _scannerService.TimeAttackOrRaceFinished,
		};

		return await _networkService.SubmitScore(uploadRequest);
	}
}
