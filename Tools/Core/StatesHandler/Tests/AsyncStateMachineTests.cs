using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace GothmogToolkit.Tools.Core.StatesHandler.Tests
{
	public class AsyncStateMachineTests
	{
		[Test]
		public void RoutesByResultAndForwardsTypedPayload()
		{
			var source = new SourceState();
			var target = new InputState();
			var machine = new AsyncStateMachine<AsyncState>();
			machine.Initialize(new AsyncState[] { source, target });
			machine.From<SourceState>()
				.On<StateCompleted<int>>()
				.To<InputState, int>(result => result.Payload);

			var terminal = machine.Run<SourceState>(CancellationToken.None)
				.GetAwaiter()
				.GetResult();

			Assert.That(target.Received, Is.EqualTo(42));
			Assert.That(terminal, Is.TypeOf<StateMachineCompleted>());
		}

		[Test]
		public void PredicateRoutesAreEvaluatedInDeclarationOrder()
		{
			var source = new ResultState();
			var firstTarget = new FirstTerminalState();
			var fallbackTarget = new FallbackTerminalState();
			var machine = new AsyncStateMachine<AsyncState>();
			machine.Initialize(new AsyncState[] { source, firstTarget, fallbackTarget });
			machine.From<ResultState>()
				.On<StateCompleted<int>>(result => result.Payload == 42)
				.To<FirstTerminalState>();
			machine.From<ResultState>()
				.On<StateCompleted<int>>()
				.To<FallbackTerminalState>();

			Assert.DoesNotThrow(() =>
				machine.Run<ResultState>(CancellationToken.None).GetAwaiter().GetResult());
			Assert.That(firstTarget.Reached, Is.True);
			Assert.That(fallbackTarget.Reached, Is.False);
		}

		[Test]
		public void ValidateRejectsFallbackBeforePredicate()
		{
			var source = new ResultState();
			var target = new FirstTerminalState();
			var machine = new AsyncStateMachine<AsyncState>();
			machine.Initialize(new AsyncState[] { source, target });
			machine.From<ResultState>()
				.On<StateCompleted<int>>()
				.To<TerminalState>();
			machine.From<ResultState>()
				.On<StateCompleted<int>>(result => result.Payload == 42)
				.To<TerminalState>();

			Assert.That(
				() => machine.Validate(),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public void UnmatchedResultThrows()
		{
			var source = new UnmatchedState();
			var machine = new AsyncStateMachine<AsyncState>();
			machine.Initialize(new AsyncState[] { source });

			Assert.That(
				() => machine.Run<UnmatchedState>(CancellationToken.None).GetAwaiter().GetResult(),
				Throws.TypeOf<InvalidOperationException>());
		}

		private sealed class SourceState : AsyncState
		{
			protected override UniTask<IStateResult> Process(CancellationToken cancellationToken)
			{
				return UniTask.FromResult<IStateResult>(new StateCompleted<int>(42));
			}
		}

		private sealed class ResultState : AsyncState
		{
			protected override UniTask<IStateResult> Process(CancellationToken cancellationToken)
			{
				return UniTask.FromResult<IStateResult>(new StateCompleted<int>(42));
			}
		}

		private sealed class InputState : AsyncState<int>
		{
			public int Received { get; private set; }

			protected override UniTask<IStateResult> Process(
				CancellationToken cancellationToken,
				int input)
			{
				Received = input;
				return UniTask.FromResult<IStateResult>(StateMachineCompleted.Instance);
			}
		}

		private abstract class TerminalState : AsyncState
		{
			public bool Reached { get; private set; }

			protected override UniTask<IStateResult> Process(CancellationToken cancellationToken)
			{
				Reached = true;
				return UniTask.FromResult<IStateResult>(StateMachineCompleted.Instance);
			}
		}

		private sealed class FirstTerminalState : TerminalState
		{
		}

		private sealed class FallbackTerminalState : TerminalState
		{
		}

		private sealed class UnmatchedState : AsyncState
		{
			protected override UniTask<IStateResult> Process(CancellationToken cancellationToken)
			{
				return UniTask.FromResult<IStateResult>(StateFailed.Instance);
			}
		}
	}
}
