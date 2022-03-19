using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Extensions;
using DevilDaggersCustomLeaderboards.Network;
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

		if (!await _networkService.CheckIfLeaderboardExists(_scannerService.MainBlock.SurvivalHashMd5))
			return null;

		Console.Clear();
		Cmd.WriteLine("Uploading...");
		Cmd.WriteLine();

		string toEncrypt = string.Join(
			";",
			_scannerService.MainBlock.PlayerId,
			_scannerService.MainBlock.Time.ConvertToTimeInt(),
			_scannerService.MainBlock.GemsCollected,
			_scannerService.MainBlock.GemsDespawned,
			_scannerService.MainBlock.GemsEaten,
			_scannerService.MainBlock.GemsTotal,
			_scannerService.MainBlock.EnemiesKilled,
			_scannerService.MainBlock.DeathType,
			_scannerService.MainBlock.DaggersHit,
			_scannerService.MainBlock.DaggersFired,
			_scannerService.MainBlock.EnemiesAlive,
			_scannerService.MainBlock.HomingDaggers,
			_scannerService.MainBlock.HomingDaggersEaten,
			_scannerService.MainBlock.IsReplay ? 1 : 0,
			ByteArrayToHexString(_scannerService.MainBlock.SurvivalHashMd5),
			string.Join(",", new[] { _scannerService.MainBlock.LevelUpTime2.ConvertToTimeInt(), _scannerService.MainBlock.LevelUpTime3.ConvertToTimeInt(), _scannerService.MainBlock.LevelUpTime4.ConvertToTimeInt() }));
		string validation = Secrets.EncryptionWrapper.EncryptAndEncode(toEncrypt);

		AddUploadRequest uploadRequest = new()
		{
			DaggersFired = _scannerService.MainBlock.DaggersFired,
			DaggersHit = _scannerService.MainBlock.DaggersHit,
			ClientVersion = Constants.LocalVersion.ToString(),
			DeathType = _scannerService.MainBlock.DeathType,
			EnemiesAlive = _scannerService.MainBlock.EnemiesAlive,
			GemsCollected = _scannerService.MainBlock.GemsCollected,
			GemsDespawned = _scannerService.MainBlock.GemsDespawned,
			GemsEaten = _scannerService.MainBlock.GemsEaten,
			GemsTotal = _scannerService.MainBlock.GemsTotal,
			HomingDaggers = _scannerService.MainBlock.HomingDaggers,
			HomingDaggersEaten = _scannerService.MainBlock.HomingDaggersEaten,
			EnemiesKilled = _scannerService.MainBlock.EnemiesKilled,
			LevelUpTime2 = _scannerService.MainBlock.LevelUpTime2.ConvertToTimeInt(),
			LevelUpTime3 = _scannerService.MainBlock.LevelUpTime3.ConvertToTimeInt(),
			LevelUpTime4 = _scannerService.MainBlock.LevelUpTime4.ConvertToTimeInt(),
			PlayerId = _scannerService.MainBlock.PlayerId,
			SurvivalHashMd5 = _scannerService.MainBlock.SurvivalHashMd5,
			Time = _scannerService.MainBlock.Time.ConvertToTimeInt(),
			PlayerName = _scannerService.MainBlock.PlayerName,
			IsReplay = _scannerService.MainBlock.IsReplay,
			Validation = HttpUtility.HtmlEncode(validation),
			GameData = _scannerService.GetGameData(),
#if DEBUG
			BuildMode = "Debug",
#else
			BuildMode = "Release",
#endif
			OperatingSystem = "Windows",
			ProhibitedMods = _scannerService.MainBlock.ProhibitedMods,
			Client = "DevilDaggersCustomLeaderboards",
			ReplayData = _scannerService.GetReplay(),
			Status = _scannerService.MainBlock.Status,
			ReplayPlayerId = _scannerService.MainBlock.ReplayPlayerId,
			GameMode = _scannerService.MainBlock.GameMode,
			TimeAttackOrRaceFinished = _scannerService.MainBlock.TimeAttackOrRaceFinished,
		};

		return await _networkService.SubmitScore(uploadRequest);
	}

	private static string ByteArrayToHexString(byte[] byteArray)
		=> BitConverter.ToString(byteArray).Replace("-", string.Empty);
}
