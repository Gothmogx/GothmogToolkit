#if UNITASK
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GothmogToolkit.Tools.Core.StatesHandler
{
	[Obsolete("Use AsyncState instead.")]
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
		
		protected StateTransition TransitTo(Type nextStateType, object stateEnterArgs = null, bool isYieldRequired = false)
		{
			return StateTransition.GetTransition(this, nextStateType, stateEnterArgs, isYieldRequired);
		}
	}
}
#endif
