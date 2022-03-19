using System;

namespace DevilDaggersCustomLeaderboards.Utils;

public static class HashUtils
{
	public static string ByteArrayToHexString(byte[] byteArray)
		=> BitConverter.ToString(byteArray).Replace("-", string.Empty);
}
