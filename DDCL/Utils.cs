using DDCL.MemoryHandling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace DDCL
{
	public static class Utils
	{
		private static readonly List<string> Deaths = new List<string>()
		{
			"FALLEN", "SWARMED", "IMPALED", "GORED", "INFESTED", "OPENED", "PURGED", "DESECRATED", "SACRIFICED", "EVISCERATED", "ANNIHILATED", "INTOXICATED", "ENVENOMATED", "INCARNATED", "DISCARNATED", "BARBED"
		};

		private static string clientVersion;

		public static Version GetClientVersion()
		{
			if (string.IsNullOrEmpty(clientVersion))
			{
				Assembly assembly = Assembly.GetExecutingAssembly();
				clientVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
			}
			return Version.Parse(clientVersion);
		}

		public static string CalculateSpawnsetHash()
		{
			try
			{
				using (MD5 md5 = MD5.Create())
				{
					using (FileStream stream = File.OpenRead(Path.Combine(Path.GetDirectoryName(Scanner.Instance.Process.MainModule.FileName), "dd", "survival")))
					{
						byte[] hash = md5.ComputeHash(stream);
						return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
					}
				}
			}
			catch
			{
				return string.Empty;
			}
		}

		public static string GetDeathName(int value)
		{
			if (value < 0 || value > 15)
				return "N/A";
			return Deaths[value];
		}
	}
}