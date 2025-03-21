#if UNITASK
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GothmogToolkit.StatesHandler
{
	public class StatesHandlerMachine
	{
		private readonly Dictionary<Type, IState> _states = new(8);

		public event Action<IState> EnteringState;
		public event Action<IState> ExitedState;

		public void Initialize(IEnumerable<IState> sessionStates)
		{
			foreach (var state in sessionStates)
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

		public async UniTask Run<TFirstState>(CancellationToken cancellationToken, bool shouldYield)
			where TFirstState : State
		{
			var nextState = typeof(TFirstState);
			StateTransition lastTransition = null;

			while (nextState != null)
			{
				if (!TryGetState(nextState, out var state))
					throw new ArgumentException($"No state of type {nextState} found");

				cancellationToken.ThrowIfCancellationRequested();

				EnteringState?.Invoke(state);
				lastTransition = await state.Execute(cancellationToken, lastTransition?.TransitionArgs);
				ExitedState?.Invoke(state);

				nextState = lastTransition.NextStateType;
				if (lastTransition?.NextStateType == null)
					return;
				await UniTask.Yield();
			}
		}

		public bool TryGetState(Type type, out IState state) => _states.TryGetValue(type, out state);
	}
}
#endif