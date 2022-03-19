using System.Diagnostics;
using System.IO;
using System.Reflection;
using System;

namespace DevilDaggersCustomLeaderboards;

public static class Constants
{
	public const string ApplicationName = "DevilDaggersCustomLeaderboards";
	public const string ApplicationDisplayName = "Devil Daggers Custom Leaderboards";

	public static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
	public static Version LocalVersion { get; } = Version.Parse(FileVersionInfo.GetVersionInfo(Path.Combine(AppContext.BaseDirectory, $"{ApplicationName}.exe")).FileVersion ?? throw new("Could not get file version from current assembly."));
}
