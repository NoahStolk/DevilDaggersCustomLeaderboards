using DDCL.MemoryHandling;
using System;
using System.IO;
using System.Security.Cryptography;

namespace DDCL
{
	public static class Utils
	{
		public static string SurvivalFilePath = Path.Combine(Path.GetDirectoryName(Scanner.Instance.Process.MainModule.FileName), "dd", "survival");

		public static string CalculateSpawnsetHash()
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