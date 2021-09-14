#if LINUX
using System;
using System.IO;

namespace DevilDaggersCustomLeaderboards.Memory.Linux
{
	public class LinuxHeapAccessor : IDisposable
	{
		private bool _disposedValue;

		public LinuxHeapAccessor(string memoryFilePath)
		{
			Stream = new(memoryFilePath, FileMode.Open);
		}

		public FileStream Stream { get; }

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
					Stream.Dispose();

				_disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
#endif
