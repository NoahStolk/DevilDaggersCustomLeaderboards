using System;
using System.IO;
using System.Security.Cryptography;

namespace DevilDaggersCustomLeaderboards
{
	public static class Utils
	{
		public const string V3Hash = "569fead87abf4d30fdee4231a6398051";

		public static string SurvivalFilePath = Path.Combine(Path.GetDirectoryName(Scanner.Instance.Process.MainModule.FileName), "dd", "survival");

		public static string CalculateSurvivalHash()
		{
			using (MD5 md5 = MD5.Create())
			{
				using (FileStream stream = File.OpenRead(SurvivalFilePath))
				{
					byte[] hash = md5.ComputeHash(stream);
					return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
				}
			}
		}
	}
}