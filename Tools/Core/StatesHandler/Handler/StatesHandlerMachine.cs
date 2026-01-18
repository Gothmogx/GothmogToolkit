#if UNITASK
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GothmogToolkit.Tools.Core.StatesHandler
{
	[Obsolete("Use generic version")]
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

		public async UniTask Run<TFirstState>(CancellationToken cancellationToken)
			where TFirstState : IState
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
				if (lastTransition is { IsYieldRequired: true })
					await UniTask.Yield();
			}
		}

		public bool TryGetState(Type type, out IState state) => _states.TryGetValue(type, out state);
	}

	public class StatesHandlerMachine<TState> where TState : IState
	{
		private readonly Dictionary<Type, TState> _states = new(8);

		public event Action<TState> EnteringState;
		public event Action<TState> ExitedState;

		public void Initialize(IEnumerable<TState> states)
		{
			foreach (var state in states)
			{
				RegisterState(state);
			}
		}

		private void RegisterState(TState state)
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
			where TFirstState : TState
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

				if (shouldYieldBetweenStates)
					await UniTask.Yield();
			}
		}

		public bool TryGetState(Type type, out TState state) => _states.TryGetValue(type, out state);
	}

}
#endif