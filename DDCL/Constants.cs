using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace DDCL
{
	public static class Constants
	{
		public static string Version;
		public static CultureInfo Culture = new CultureInfo("en-US");

		static Constants()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			Version = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
		}
	}
}