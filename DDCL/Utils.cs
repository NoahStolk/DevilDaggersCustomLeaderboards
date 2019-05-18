using DDCL.MemoryHandling;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace DDCL
{
	public static class Utils
	{
		private static string version;
		
		public static string GetVersion()
		{
			if (string.IsNullOrEmpty(version))
			{
				Assembly assembly = Assembly.GetExecutingAssembly();
				version = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
			}
			return version;
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
	}
}