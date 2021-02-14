using DevilDaggersCustomLeaderboards.Clients;
using System;
using System.Threading.Tasks;

namespace DevilDaggersCustomLeaderboards.Network
{
	public sealed class NetworkHandler
	{
#if TESTING
		public static readonly string BaseUrl = "http://localhost:2963";
#else
		public static readonly string BaseUrl = "https://devildaggers.info";
#endif

		private static readonly Lazy<NetworkHandler> _lazy = new(() => new());

		private NetworkHandler()
		{
			ApiClient = new(new() { BaseAddress = new(BaseUrl) });
		}

		public static NetworkHandler Instance => _lazy.Value;

		public DevilDaggersInfoApiClient ApiClient { get; }

		public Tool? Tool { get; private set; }

		public async Task GetOnlineTool()
		{
			try
			{
				Tool = (await ApiClient.Tools_GetToolsAsync(Program.ApplicationName))[0];
			}
			catch (Exception ex)
			{
				Program.Log.Error("An error occurred while attempting to retrieve tool information from the API.", ex);
			}
		}
	}
}
