#if UNITASK
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GothmogToolkit.Tools.UI.Windows
{
	public interface IWindowStack
	{
		int Count { get; }
		WindowBase Top { get; }
		bool IsAnyWindowOpen { get; }

		UniTask<TResult> PushAsync<TResult>(
			WindowBase<TResult> window,
			CancellationToken cancellationToken);

		UniTask<bool> DismissTopAsync(CancellationToken cancellationToken);
	}
}
#endif
