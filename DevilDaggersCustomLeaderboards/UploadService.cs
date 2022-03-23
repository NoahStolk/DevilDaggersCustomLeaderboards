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
	private readonly MemoryService _memoryService;

	public UploadService(NetworkService networkService, MemoryService memoryService)
	{
		_networkService = networkService;
		_memoryService = memoryService;
	}

	public async Task<GetUploadSuccess?> UploadRun()
	{
		Console.Clear();
		Cmd.WriteLine("Checking if this spawnset has a leaderboard...");
		Cmd.WriteLine();

		if (!await _networkService.CheckIfLeaderboardExists(_memoryService.MainBlock.SurvivalHashMd5))
			return null;

		Console.Clear();
		Cmd.WriteLine("Uploading...");
		Cmd.WriteLine();

		string toEncrypt = string.Join(
			";",
			_memoryService.MainBlock.PlayerId,
			_memoryService.MainBlock.Time.ConvertToTimeInt(),
			_memoryService.MainBlock.GemsCollected,
			_memoryService.MainBlock.GemsDespawned,
			_memoryService.MainBlock.GemsEaten,
			_memoryService.MainBlock.GemsTotal,
			_memoryService.MainBlock.EnemiesKilled,
			_memoryService.MainBlock.DeathType,
			_memoryService.MainBlock.DaggersHit,
			_memoryService.MainBlock.DaggersFired,
			_memoryService.MainBlock.EnemiesAlive,
			_memoryService.MainBlock.HomingDaggers,
			_memoryService.MainBlock.HomingDaggersEaten,
			_memoryService.MainBlock.IsReplay ? 1 : 0,
			HashUtils.ByteArrayToHexString(_memoryService.MainBlock.SurvivalHashMd5),
			string.Join(",", new[] { _memoryService.MainBlock.LevelUpTime2.ConvertToTimeInt(), _memoryService.MainBlock.LevelUpTime3.ConvertToTimeInt(), _memoryService.MainBlock.LevelUpTime4.ConvertToTimeInt() }));
		string validation = Secrets.EncryptionWrapper.EncryptAndEncode(toEncrypt);

		AddUploadRequest uploadRequest = new()
		{
			DaggersFired = _memoryService.MainBlock.DaggersFired,
			DaggersHit = _memoryService.MainBlock.DaggersHit,
			ClientVersion = Constants.LocalVersion.ToString(),
			DeathType = _memoryService.MainBlock.DeathType,
			EnemiesAlive = _memoryService.MainBlock.EnemiesAlive,
			GemsCollected = _memoryService.MainBlock.GemsCollected,
			GemsDespawned = _memoryService.MainBlock.GemsDespawned,
			GemsEaten = _memoryService.MainBlock.GemsEaten,
			GemsTotal = _memoryService.MainBlock.GemsTotal,
			HomingDaggers = _memoryService.MainBlock.HomingDaggers,
			HomingDaggersEaten = _memoryService.MainBlock.HomingDaggersEaten,
			EnemiesKilled = _memoryService.MainBlock.EnemiesKilled,
			LevelUpTime2InSeconds = _memoryService.MainBlock.LevelUpTime2,
			LevelUpTime3InSeconds = _memoryService.MainBlock.LevelUpTime3,
			LevelUpTime4InSeconds = _memoryService.MainBlock.LevelUpTime4,
			PlayerId = _memoryService.MainBlock.PlayerId,
			SurvivalHashMd5 = _memoryService.MainBlock.SurvivalHashMd5,
			TimeInSeconds = _memoryService.MainBlock.Time,
			PlayerName = _memoryService.MainBlock.PlayerName,
			IsReplay = _memoryService.MainBlock.IsReplay,
			Validation = HttpUtility.HtmlEncode(validation),
			GameData = _memoryService.GetGameDataForUpload(),
#if DEBUG
			BuildMode = "Debug",
#else
			BuildMode = "Release",
#endif
			OperatingSystem = "Windows",
			ProhibitedMods = _memoryService.MainBlock.ProhibitedMods,
			Client = "DevilDaggersCustomLeaderboards",
			ReplayData = _memoryService.GetReplayForUpload(),
			Status = _memoryService.MainBlock.Status,
			ReplayPlayerId = _memoryService.MainBlock.ReplayPlayerId,
			GameMode = _memoryService.MainBlock.GameMode,
			TimeAttackOrRaceFinished = _memoryService.MainBlock.TimeAttackOrRaceFinished,
		};

		return await _networkService.SubmitScore(uploadRequest);
	}
}
