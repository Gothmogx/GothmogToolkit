#if UNITASK
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GothmogToolkit.Tools.UI.Windows
{
	public sealed class WindowStack : IWindowStack
	{
		private readonly List<WindowBase> _windows = new();

		public int Count => _windows.Count;
		public WindowBase Top => _windows.Count > 0 ? _windows[^1] : null;
		public bool IsAnyWindowOpen => _windows.Count > 0;

		public async UniTask<TResult> PushAsync<TResult>(
			WindowBase<TResult> window,
			CancellationToken cancellationToken)
		{
			if (window == null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			if (!window)
			{
				throw new ArgumentException("A destroyed window cannot be pushed.", nameof(window));
			}

			cancellationToken.ThrowIfCancellationRequested();
			window.ValidateConfiguration();
			var destroyCancellationToken = window.DestroyCancellationToken;
			using (var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
			       cancellationToken,
			       destroyCancellationToken))
			{
				var operationToken = linkedCancellationTokenSource.Token;
				var isClaimed = false;
				var isAdded = false;

				try
				{
					window.Claim(this);
					isClaimed = true;
					_windows.Add(window);
					isAdded = true;
					UpdateTopInteraction();

					window.BeginOpening();
					await window.FinishOpeningAsync(operationToken);
					UpdateTopInteraction();

					var result = await window.WaitForCloseResultAsync(operationToken);
					window.BeginClosing();
					await window.FinishClosingAsync(operationToken);
					return result;
				}
				finally
				{
					if (isAdded)
					{
						window.ForceClosed();
						RemoveByReference(window);
					}

					if (isClaimed)
					{
						window.Release(this);
					}

					if (isAdded)
					{
						UpdateTopInteraction();
						window.CompleteClosure();
					}
				}
			}
		}

		public async UniTask<bool> DismissTopAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var window = Top;
			if (window == null)
			{
				return false;
			}

			var closedTask = window.WaitUntilClosedAsync(cancellationToken);
			if (!window.TryDismiss())
			{
				return false;
			}

			await closedTask;
			return true;
		}

		private void UpdateTopInteraction()
		{
			for (var index = 0; index < _windows.Count; index++)
			{
				var window = _windows[index];
				var isTop = index == _windows.Count - 1;
				var isInteractable = isTop &&
					window != null &&
					window.State == WindowState.Open &&
					!window.HasCloseResult;
				window?.SetStackInteractable(isInteractable);
			}
		}

		private void RemoveByReference(WindowBase target)
		{
			for (var index = _windows.Count - 1; index >= 0; index--)
			{
				if (ReferenceEquals(_windows[index], target))
				{
					_windows.RemoveAt(index);
					return;
				}
			}
		}
	}
}
#endif
