#if UNITASK
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GothmogToolkit.StatesHandler
{
	public abstract class State : IState
	{
		public virtual Type Type => GetType();
		public bool IsActive { get; protected set; }

		public event Action StateEntered;
		public event Action StateExited;

		public async UniTask<StateTransition> Execute(CancellationToken cancellationToken,
			object stateTransitionArgs = null)
		{
			IsActive = true;
			StateEntered?.Invoke();
			OnEntered();

			cancellationToken.ThrowIfCancellationRequested();
			var task = await Process(cancellationToken, stateTransitionArgs);

			IsActive = false;
			StateExited?.Invoke();
			OnExited();
			return task;
		}

		protected abstract UniTask<StateTransition> Process(CancellationToken cancellationToken,
			object stateEnterArgs = null);

		protected virtual void OnEntered()
		{
		}

		protected virtual void OnExited()
		{
		}

		public override string ToString() => GetType().Name;
	}
}
#endif