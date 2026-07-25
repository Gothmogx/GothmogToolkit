#if UNITASK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GothmogToolkit.Tools.Core.StatesHandler
{
	public sealed class AsyncStateMachine<TState> where TState : AsyncState
	{
		private readonly Dictionary<Type, TState> _states = new(8);
		private readonly List<TransitionRule> _transitions = new(16);
		private bool _initialized;
		private bool _hasRun;

		public event Action<TState> EnteringState;
		public event Action<TState> ExitedState;

		public void Initialize(IEnumerable<TState> states)
		{
			if (_initialized)
				throw new InvalidOperationException("The state machine has already been initialized.");
			if (states == null)
				throw new ArgumentNullException(nameof(states));

			foreach (var state in states)
			{
				RegisterState(state);
			}

			_initialized = true;
		}

		public StateSource<TSource> From<TSource>() where TSource : TState
		{
			EnsureCanConfigure();
			return new StateSource<TSource>(this);
		}

		public void Validate()
		{
			if (!_initialized)
				throw new InvalidOperationException("Initialize the state machine before validating it.");

			foreach (var transition in _transitions)
			{
				if (!_states.ContainsKey(transition.SourceStateType))
				{
					throw new InvalidOperationException(
						$"Transition originates from unregistered state " +
						$"{transition.SourceStateType}.");
				}

				if (!_states.ContainsKey(transition.TargetStateType))
				{
					throw new InvalidOperationException(
						$"Transition from {transition.SourceStateType} targets unregistered state " +
						$"{transition.TargetStateType}.");
				}
			}

			foreach (var group in _transitions.GroupBy(
				transition => new { transition.SourceStateType, transition.ResultType }))
			{
				var fallbackIndex = group
					.Select((transition, index) => new { transition, index })
					.FirstOrDefault(item => item.transition.Predicate == null);

				if (fallbackIndex != null && fallbackIndex.index != group.Count() - 1)
				{
					throw new InvalidOperationException(
						$"The unconditional transition for {group.Key.SourceStateType} and " +
						$"{group.Key.ResultType} must be declared last.");
				}

				if (group.Count(item => item.Predicate == null) > 1)
				{
					throw new InvalidOperationException(
						$"Only one unconditional transition may be declared for " +
						$"{group.Key.SourceStateType} and {group.Key.ResultType}.");
				}
			}
		}

		public UniTask<IStateResult> Run<TFirstState>(
			CancellationToken cancellationToken,
			bool shouldYieldBetweenStates = false)
			where TFirstState : TState
		{
			return RunCore(typeof(TFirstState), null, false, cancellationToken, shouldYieldBetweenStates);
		}

		public UniTask<IStateResult> Run<TFirstState, TInput>(
			TInput input,
			CancellationToken cancellationToken,
			bool shouldYieldBetweenStates = false)
			where TFirstState : AsyncState<TInput>
		{
			return RunCore(typeof(TFirstState), input, true, cancellationToken, shouldYieldBetweenStates);
		}

		private async UniTask<IStateResult> RunCore(
			Type firstStateType,
			object input,
			bool hasInput,
			CancellationToken cancellationToken,
			bool shouldYieldBetweenStates)
		{
			if (!_initialized)
				throw new InvalidOperationException("Initialize the state machine before running it.");
			if (_hasRun)
				throw new InvalidOperationException("The state machine can only be run once.");

			Validate();
			_hasRun = true;

			var nextStateType = firstStateType;
			var nextInput = input;
			var nextHasInput = hasInput;

			while (nextStateType != null)
			{
				cancellationToken.ThrowIfCancellationRequested();

				if (!_states.TryGetValue(nextStateType, out var state))
					throw new ArgumentException($"No state of type {nextStateType} found.");

				EnteringState?.Invoke(state);
				var result = await state.Execute(cancellationToken, nextInput, nextHasInput);
				ExitedState?.Invoke(state);

				if (result == null)
					throw new InvalidOperationException($"State {state} returned a null result.");

				if (result is IStateMachineTerminalResult)
					return result;

				var transition = FindTransition(state.Type, result);
				nextStateType = transition.TargetStateType;
				nextInput = transition.GetInput(result);
				nextHasInput = transition.HasInput;

				if (shouldYieldBetweenStates)
					await UniTask.Yield(cancellationToken: cancellationToken);
			}

			throw new InvalidOperationException("The state machine stopped without a terminal result.");
		}

		private TransitionRule FindTransition(Type sourceStateType, IStateResult result)
		{
			foreach (var transition in _transitions)
			{
				if (transition.SourceStateType != sourceStateType ||
					transition.ResultType != result.GetType())
				{
					continue;
				}

				if (transition.Predicate == null || transition.Predicate(result))
					return transition;
			}

			throw new InvalidOperationException(
				$"No transition was configured from {sourceStateType} for result {result.GetType()}.");
		}

		private void RegisterState(TState state)
		{
			if (state == null)
				throw new ArgumentNullException(nameof(state), "Failed to register a null state.");
			if (state.Type == null)
				throw new ArgumentException($"Failed to register {state}: its Type is null.");
			if (!_states.TryAdd(state.Type, state))
				throw new ArgumentException($"State of type {state.Type} is already registered.");
		}

		private void AddTransition(
			Type sourceStateType,
			Type resultType,
			Type targetStateType,
			Func<IStateResult, bool> predicate,
			Func<IStateResult, object> inputSelector,
			bool hasInput)
		{
			EnsureCanConfigure();
			_transitions.Add(new TransitionRule(
				sourceStateType,
				resultType,
				targetStateType,
				predicate,
				inputSelector,
				hasInput));
		}

		private void EnsureCanConfigure()
		{
			if (_hasRun)
				throw new InvalidOperationException("Transitions cannot be changed after the machine has run.");
		}

		public sealed class StateSource<TSource> where TSource : TState
		{
			private readonly AsyncStateMachine<TState> _machine;

			internal StateSource(AsyncStateMachine<TState> machine)
			{
				_machine = machine;
			}

			public ResultTransition<TResult> On<TResult>(
				Func<TResult, bool> predicate = null)
				where TResult : IStateResult
			{
				return new ResultTransition<TResult>(_machine, typeof(TSource), predicate);
			}
		}

		public sealed class ResultTransition<TResult> where TResult : IStateResult
		{
			private readonly AsyncStateMachine<TState> _machine;
			private readonly Type _sourceStateType;
			private readonly Func<TResult, bool> _predicate;

			internal ResultTransition(
				AsyncStateMachine<TState> machine,
				Type sourceStateType,
				Func<TResult, bool> predicate)
			{
				_machine = machine;
				_sourceStateType = sourceStateType;
				_predicate = predicate;
			}

			public void To<TTarget>() where TTarget : AsyncState
			{
				_machine.AddTransition(
					_sourceStateType,
					typeof(TResult),
					typeof(TTarget),
					_predicate == null ? null : result => _predicate((TResult)result),
					null,
					false);
			}

			public void To<TTarget, TInput>(Func<TResult, TInput> inputSelector)
				where TTarget : AsyncState<TInput>
			{
				if (inputSelector == null)
					throw new ArgumentNullException(nameof(inputSelector));

				_machine.AddTransition(
					_sourceStateType,
					typeof(TResult),
					typeof(TTarget),
					_predicate == null ? null : result => _predicate((TResult)result),
					result => inputSelector((TResult)result),
					true);
			}
		}

		private sealed class TransitionRule
		{
			public Type SourceStateType { get; }
			public Type ResultType { get; }
			public Type TargetStateType { get; }
			public Func<IStateResult, bool> Predicate { get; }
			public bool HasInput { get; }

			private readonly Func<IStateResult, object> _inputSelector;

			public TransitionRule(
				Type sourceStateType,
				Type resultType,
				Type targetStateType,
				Func<IStateResult, bool> predicate,
				Func<IStateResult, object> inputSelector,
				bool hasInput)
			{
				SourceStateType = sourceStateType;
				ResultType = resultType;
				TargetStateType = targetStateType;
				Predicate = predicate;
				_inputSelector = inputSelector;
				HasInput = hasInput;
			}

			public object GetInput(IStateResult result) => _inputSelector?.Invoke(result);
		}
	}
}
#endif
