#if UNITASK
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GothmogToolkit.Tools.UI.Windows
{
	public abstract class WindowBase : MonoBehaviour
	{
		[SerializeField] private CanvasGroup _canvasGroup;

		private WindowStack _owner;
		private UniTaskCompletionSource<bool> _closedCompletionSource;

		public WindowState State { get; private set; } = WindowState.Closed;
		public bool IsInteractable => _canvasGroup != null && _canvasGroup.interactable;

		internal CancellationToken DestroyCancellationToken => this.GetCancellationTokenOnDestroy();

		internal void ValidateConfiguration()
		{
			if (_canvasGroup == null)
			{
				_canvasGroup = GetComponent<CanvasGroup>();
			}

			if (_canvasGroup == null)
			{
				throw new InvalidOperationException(
					$"Window '{name}' requires a {nameof(CanvasGroup)} on the same GameObject " +
					"or an assigned CanvasGroup reference.");
			}
		}

		internal void Claim(WindowStack owner)
		{
			if (_owner != null)
			{
				var relationship = ReferenceEquals(_owner, owner)
					? "is already in this window stack"
					: "already belongs to another window stack";
				throw new InvalidOperationException($"Window '{name}' {relationship}.");
			}

			_owner = owner;
		}

		internal void Release(WindowStack owner)
		{
			if (ReferenceEquals(_owner, owner))
			{
				_owner = null;
			}
		}

		internal void BeginOpening()
		{
			if (State != WindowState.Closed)
			{
				throw new InvalidOperationException(
					$"Window '{name}' cannot open while it is {State}.");
			}

			_closedCompletionSource = new UniTaskCompletionSource<bool>();
			PrepareCloseResult();
			State = WindowState.Opening;
			gameObject.SetActive(true);
			SetStackInteractable(false);
		}

		internal async UniTask FinishOpeningAsync(CancellationToken cancellationToken)
		{
			await PlayShowAnimationAsync(cancellationToken);
			cancellationToken.ThrowIfCancellationRequested();
			State = WindowState.Open;
		}

		internal void BeginClosing()
		{
			if (State != WindowState.Open)
			{
				throw new InvalidOperationException(
					$"Window '{name}' cannot close while it is {State}.");
			}

			State = WindowState.Closing;
			SetStackInteractable(false);
		}

		internal async UniTask FinishClosingAsync(CancellationToken cancellationToken)
		{
			await PlayHideAnimationAsync(cancellationToken);
			cancellationToken.ThrowIfCancellationRequested();
		}

		internal void SetStackInteractable(bool isInteractable)
		{
			if (_canvasGroup == null)
			{
				return;
			}

			_canvasGroup.interactable = isInteractable;
			_canvasGroup.blocksRaycasts = isInteractable;
		}

		internal async UniTask WaitUntilClosedAsync(CancellationToken cancellationToken)
		{
			var completionSource = _closedCompletionSource;
			if (completionSource == null)
			{
				return;
			}

			await completionSource.Task.AttachExternalCancellation(cancellationToken);
		}

		internal void ForceClosed()
		{
			SetStackInteractable(false);
			CancelCloseResult();
			State = WindowState.Closed;

			if (this != null && gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}

		internal void CompleteClosure()
		{
			var completionSource = _closedCompletionSource;
			_closedCompletionSource = null;
			completionSource?.TrySetResult(true);
		}

		internal abstract bool TryDismiss();

		internal abstract bool HasCloseResult { get; }

		internal abstract void PrepareCloseResult();

		internal abstract void CancelCloseResult();

		protected virtual UniTask PlayShowAnimationAsync(CancellationToken cancellationToken)
		{
			gameObject.SetActive(true);
			return UniTask.CompletedTask;
		}

		protected virtual UniTask PlayHideAnimationAsync(CancellationToken cancellationToken)
		{
			gameObject.SetActive(false);
			return UniTask.CompletedTask;
		}

		private void Reset()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
		}

		protected virtual void OnDisable()
		{
			if (State == WindowState.Opening || State == WindowState.Open)
			{
				CancelCloseResult();
			}
		}

		protected virtual void OnDestroy()
		{
			SetStackInteractable(false);
			CancelCloseResult();
			State = WindowState.Closed;
		}
	}

	public abstract class WindowBase<TResult> : WindowBase
	{
		private UniTaskCompletionSource<TResult> _closeCompletionSource;

		protected abstract TResult DismissResult { get; }

		internal override bool HasCloseResult =>
			_closeCompletionSource != null && _closeCompletionSource.Task.Status.IsCompleted();

		public bool TryClose(TResult result)
		{
			if (State != WindowState.Opening && State != WindowState.Open)
			{
				return false;
			}

			return _closeCompletionSource != null && _closeCompletionSource.TrySetResult(result);
		}

		internal async UniTask<TResult> WaitForCloseResultAsync(CancellationToken cancellationToken)
		{
			if (_closeCompletionSource == null)
			{
				throw new InvalidOperationException($"Window '{name}' has not started opening.");
			}

			return await _closeCompletionSource.Task.AttachExternalCancellation(cancellationToken);
		}

		internal override bool TryDismiss()
		{
			return TryClose(DismissResult);
		}

		internal override void PrepareCloseResult()
		{
			_closeCompletionSource = new UniTaskCompletionSource<TResult>();
		}

		internal override void CancelCloseResult()
		{
			var completionSource = _closeCompletionSource;
			_closeCompletionSource = null;
			completionSource?.TrySetCanceled();
		}
	}
}
#endif
