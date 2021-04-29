using DevilDaggersCustomLeaderboards.Clients;
using System;

namespace DevilDaggersCustomLeaderboards.Network
{
	public sealed class NetworkHandler
	{
#if TESTING
		public static readonly string BaseUrl = "http://localhost:2964";
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
	}
}
