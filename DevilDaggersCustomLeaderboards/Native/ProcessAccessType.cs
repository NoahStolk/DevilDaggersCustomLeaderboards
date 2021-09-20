#pragma warning disable CA1707 // Identifiers should not contain underscores
using System;

namespace DevilDaggersCustomLeaderboards.Native
{
	[Flags]
#pragma warning disable RCS1135 // Declare enum member with zero value (when enum has FlagsAttribute).
	internal enum ProcessAccessType
#pragma warning restore RCS1135 // Declare enum member with zero value (when enum has FlagsAttribute).
	{
		PROCESS_TERMINATE = 0x0001,
		PROCESS_CREATE_THREAD = 0x0002,
		PROCESS_SET_SESSIONID = 0x0004,
		PROCESS_VM_OPERATION = 0x0008,
		PROCESS_VM_READ = 0x0010,
		PROCESS_VM_WRITE = 0x0020,
		PROCESS_DUP_HANDLE = 0x0040,
		PROCESS_CREATE_PROCESS = 0x0080,
		PROCESS_SET_QUOTA = 0x0100,
		PROCESS_SET_INFORMATION = 0x0200,
		PROCESS_QUERY_INFORMATION = 0x0400,
	}
}

#pragma warning restore CA1707 // Identifiers should not contain underscores
