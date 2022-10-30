using DevilDaggersCustomLeaderboards.Clients;
using DevilDaggersCustomLeaderboards.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DevilDaggersCustomLeaderboards.Network;

public class NetworkService
{
#if TESTING
	public static readonly string BaseUrl = "https://localhost:44318";
#else
	public static readonly string BaseUrl = "https://devildaggers.info";
#endif

	private readonly ILogger<NetworkService> _logger;
	private readonly DevilDaggersInfoApiClient _apiClient;

	public NetworkService(ILogger<NetworkService> logger)
	{
		_logger = logger;
		_apiClient = new(new() { BaseAddress = new(BaseUrl) });
	}

	public async Task CheckForUpdates()
	{
		Cmd.WriteLine("Checking for updates...");

		GetTool? tool = await GetTool();
		GetToolDistribution? distribution = await GetDistribution();
		Console.Clear();

		if (tool == null || distribution == null)
		{
			Cmd.WriteLine($"Failed to check for updates (host: {BaseUrl}).\n\n(Press any key to continue.)", ColorUtils.Error);
			Console.ReadKey();
			return;
		}

		if (Constants.LocalVersion < Version.Parse(tool.VersionNumberRequired))
		{
			Cmd.WriteLine($"You are using an unsupported and outdated version of {Constants.ApplicationDisplayName} ({Constants.LocalVersion}).\n\nYou must use version {tool.VersionNumberRequired} or higher in order to submit scores.\n\nPlease update the program.\n\n(Press any key to continue.)", ColorUtils.Error);
			Console.ReadKey();
		}
		else if (Constants.LocalVersion < Version.Parse(distribution.VersionNumber))
		{
			Cmd.WriteLine($"{Constants.ApplicationDisplayName} version {distribution.VersionNumber} is available.\n\n(Press any key to continue.)", ColorUtils.Warning);
			Console.ReadKey();
		}
	}

	private async Task<GetTool?> GetTool()
	{
		const int maxAttempts = 5;
		for (int i = 0; i < maxAttempts; i++)
		{
			try
			{
				return await _apiClient.Tools_GetToolAsync(Constants.ApplicationName);
			}
			catch (Exception ex)
			{
				_logger.LogError("Error while trying to retrieve tool.", ex);
				string message = $"An error occurred while trying to check for updates. Retrying in 1 second... (attempt {i + 1} out of {maxAttempts})";
				Cmd.WriteLine(message, string.Empty, ColorUtils.Error);

				await Task.Delay(TimeSpan.FromSeconds(1));
			}
		}

		return null;
	}

	private async Task<GetToolDistribution?> GetDistribution()
	{
		const int maxAttempts = 5;
		for (int i = 0; i < maxAttempts; i++)
		{
			try
			{
				// TODO: Use ToolPublishMethod.Default for Windows 7.
				return await _apiClient.Tools_GetLatestToolDistributionAsync(Constants.ApplicationName, ToolPublishMethod.SelfContained, ToolBuildType.WindowsConsole);
			}
			catch (Exception ex)
			{
				_logger.LogError("Error while trying to retrieve distribution.", ex);
				string message = $"An error occurred while trying to check for updates. Retrying in 1 second... (attempt {i + 1} out of {maxAttempts})";
				Cmd.WriteLine(message, string.Empty, ColorUtils.Error);

				await Task.Delay(TimeSpan.FromSeconds(1));
			}
		}

		return null;
	}

	public async Task<long> GetMarker()
	{
		while (true)
		{
			try
			{
				Marker marker = await _apiClient.ProcessMemory_GetMarkerAsync(SupportedOperatingSystem.Windows);
				return marker.Value;
			}
			catch (Exception ex)
			{
				_logger.LogError("Error while trying to get marker.", ex);
				const string message = "An error occurred while trying to retrieve marker. Retrying in 1 second...";
				Cmd.WriteLine(message, string.Empty, ColorUtils.Error);

				await Task.Delay(TimeSpan.FromSeconds(1));
			}
		}
	}

	public async Task<bool> CheckIfLeaderboardExists(byte[] survivalHashMd5)
	{
		const int maxAttempts = 5;
		for (int i = 0; i < maxAttempts; i++)
		{
			try
			{
				await _apiClient.CustomLeaderboards_CustomLeaderboardExistsBySpawnsetHashAsync(survivalHashMd5);

				return true;
			}
			catch (DevilDaggersInfoApiException ex) when (ex.StatusCode == 404)
			{
				Cmd.WriteLine("This spawnset does not have a leaderboard.", string.Empty, ColorUtils.Warning);

				return false;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while trying to check for existing leaderboard.");
				string message = $"An error occurred while trying to check for existing leaderboard. Retrying in 1 second... (attempt {i + 1} out of {maxAttempts})";
				Cmd.WriteLine(message, string.Empty, ColorUtils.Error);

				await Task.Delay(TimeSpan.FromSeconds(1));
			}
		}

		return false;
	}

	public async Task<GetUploadSuccess?> SubmitScore(AddUploadRequest uploadRequest)
	{
		try
		{
			return await _apiClient.CustomEntries_SubmitScoreForDdclAsync(uploadRequest);
		}
		catch (DevilDaggersInfoApiException<ProblemDetails> ex)
		{
			Cmd.WriteLine("Upload failed", ex.Result?.Title ?? "Empty response", ColorUtils.Error);
			return null;
		}
		catch (Exception ex)
		{
			Cmd.WriteLine("Upload failed", ex.Message, ColorUtils.Error);
			_logger.LogError(ex, "Error trying to submit score");
			return null;
		}
	}

	public async Task<byte[]?> GetReplay(int customEntryId)
	{
		try
		{
			FileResponse fr = await _apiClient.CustomEntries_GetCustomEntryReplayByIdAsync(customEntryId);

			using MemoryStream ms = new();
			fr.Stream.CopyTo(ms);
			return ms.ToArray();
		}
		catch (DevilDaggersInfoApiException ex) when (ex.StatusCode == 404)
		{
			return null;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error while trying to download replay.");
			return null;
		}
	}
}
