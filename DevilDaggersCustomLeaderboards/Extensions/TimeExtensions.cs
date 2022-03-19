namespace DevilDaggersCustomLeaderboards.Extensions;

public static class TimeExtensions
{
	public static int ConvertToTimeInt(this float timeFloat)
		=> (int)(timeFloat * 10000);
}
