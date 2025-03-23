using System;
using System.Threading;

namespace GothmogToolkit.Tools.Helpers.Extensions
{
	public static class AsyncExtensions
	{
		public static void CancelAndDispose(this CancellationTokenSource cts)
		{
			if (cts == null)
				return;
			
			try
			{
				cts.Cancel();
				cts.Dispose();
			}
			catch (ObjectDisposedException) { }
		}
	}
}