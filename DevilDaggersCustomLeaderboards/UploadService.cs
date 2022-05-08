using DevilDaggersCustomLeaderboards.Clients;
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

		byte[] timeAsBytes = BitConverter.GetBytes(_memoryService.MainBlock.Time);
		byte[] levelUpTime2AsBytes = BitConverter.GetBytes(_memoryService.MainBlock.LevelUpTime2);
		byte[] levelUpTime3AsBytes = BitConverter.GetBytes(_memoryService.MainBlock.LevelUpTime3);
		byte[] levelUpTime4AsBytes = BitConverter.GetBytes(_memoryService.MainBlock.LevelUpTime4);

		string toEncrypt = string.Join(
			";",
			_memoryService.MainBlock.ReplayPlayerId,
			HashUtils.ByteArrayToHexString(timeAsBytes),
			_memoryService.MainBlock.GemsCollected,
			_memoryService.MainBlock.GemsDespawned,
			_memoryService.MainBlock.GemsEaten,
			_memoryService.MainBlock.GemsTotal,
			_memoryService.MainBlock.EnemiesAlive,
			_memoryService.MainBlock.EnemiesKilled,
			_memoryService.MainBlock.DeathType,
			_memoryService.MainBlock.DaggersHit,
			_memoryService.MainBlock.DaggersFired,
			_memoryService.MainBlock.HomingStored,
			_memoryService.MainBlock.HomingEaten,
			_memoryService.MainBlock.IsReplay,
			4,
			HashUtils.ByteArrayToHexString(_memoryService.MainBlock.SurvivalHashMd5),
			HashUtils.ByteArrayToHexString(levelUpTime2AsBytes),
			HashUtils.ByteArrayToHexString(levelUpTime3AsBytes),
			HashUtils.ByteArrayToHexString(levelUpTime4AsBytes),
			_memoryService.MainBlock.GameMode,
			_memoryService.MainBlock.TimeAttackOrRaceFinished,
			_memoryService.MainBlock.ProhibitedMods);
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
			HomingStored = _memoryService.MainBlock.HomingStored,
			HomingEaten = _memoryService.MainBlock.HomingEaten,
			EnemiesKilled = _memoryService.MainBlock.EnemiesKilled,
			LevelUpTime2InSeconds = _memoryService.MainBlock.LevelUpTime2,
			LevelUpTime3InSeconds = _memoryService.MainBlock.LevelUpTime3,
			LevelUpTime4InSeconds = _memoryService.MainBlock.LevelUpTime4,
			LevelUpTime2AsBytes = levelUpTime2AsBytes,
			LevelUpTime3AsBytes = levelUpTime3AsBytes,
			LevelUpTime4AsBytes = levelUpTime4AsBytes,
			PlayerId = _memoryService.MainBlock.ReplayPlayerId,
			SurvivalHashMd5 = _memoryService.MainBlock.SurvivalHashMd5,
			TimeInSeconds = _memoryService.MainBlock.Time,
			TimeAsBytes = timeAsBytes,
			PlayerName = _memoryService.MainBlock.ReplayPlayerName,
			IsReplay = _memoryService.MainBlock.IsReplay,
			Validation = HttpUtility.HtmlEncode(validation),
			ValidationVersion = 2,
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
			Status = 4,
			ReplayPlayerId = _memoryService.MainBlock.ReplayPlayerId,
			GameMode = _memoryService.MainBlock.GameMode,
			TimeAttackOrRaceFinished = _memoryService.MainBlock.TimeAttackOrRaceFinished,
		};

		return await _networkService.SubmitScore(uploadRequest);
	}
}
