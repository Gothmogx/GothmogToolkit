#if UNITASK
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GothmogToolkit.Tools.Core.StatesHandler
{
	public class StatesHandlerMachine
	{
		private readonly Dictionary<Type, IState> _states = new(8);

		public event Action<IState> EnteringState;
		public event Action<IState> ExitedState;

		public void Initialize(IEnumerable<IState> states)
		{
			foreach (var state in states)
			{
				RegisterState(state);
			}
		}

		private void RegisterState(IState state)
		{
			if (state == null)
				throw new ArgumentNullException($"Failed to register state. The state is null.");
			if (state.Type == null)
				throw new ArgumentNullException($"Failed to register state. Type of the state {state} is null.");
			if (!_states.TryAdd(state.Type, state))
				throw new ArgumentException(
					$"Failed to register state. State of type {state.Type} is already registered");
		}

		public async UniTask Run<TFirstState>(CancellationToken cancellationToken, bool shouldYieldBetweenStates = false)
			where TFirstState : State
		{
			var nextState = typeof(TFirstState);
			StateTransition lastTransition = null;

			while (nextState != null)
			{
				if (!TryGetState(nextState, out var state))
					throw new ArgumentException($"No state of type {nextState} found");

				try
				{
					cancellationToken.ThrowIfCancellationRequested();
					
					EnteringState?.Invoke(state);
					lastTransition = await state.Execute(cancellationToken, lastTransition?.TransitionArgs);
					ExitedState?.Invoke(state);
				}
				catch (OperationCanceledException)
				{
					return;
				}

				nextState = lastTransition.NextStateType;
				if (lastTransition?.NextStateType == null)
					return;
				
				if(shouldYieldBetweenStates)
					await UniTask.Yield();
			}
		}

		public bool TryGetState(Type type, out IState state) => _states.TryGetValue(type, out state);
	}
}
#endif