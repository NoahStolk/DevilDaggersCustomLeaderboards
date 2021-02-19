using DevilDaggersCustomLeaderboards.Memory.Variables;

namespace DevilDaggersCustomLeaderboards.Utils
{
	public static class TimeExtensions
	{
		public static int ConvertToTimeInt(this float timeFloat)
			=> (int)(timeFloat * 10000);

		public static int ConvertToTimeInt(this FloatVariable timeFloat)
			=> (int)(timeFloat * 10000);
	}
}
