#if UNITASK
using System;

namespace GothmogToolkit.Tools.Core.StatesHandler
{
	public interface IStateResult
	{
	}

	public interface IStateMachineTerminalResult : IStateResult
	{
	}

	public sealed class StateCompleted : IStateResult
	{
		public static StateCompleted Instance { get; } = new();

		private StateCompleted()
		{
		}
	}

	public sealed class StateCompleted<TPayload> : IStateResult
	{
		public TPayload Payload { get; }

		public StateCompleted(TPayload payload)
		{
			Payload = payload;
		}
	}

	public sealed class StateFailed : IStateResult
	{
		public static StateFailed Instance { get; } = new();

		private StateFailed()
		{
		}
	}

	public sealed class StateFailed<TPayload> : IStateResult
	{
		public TPayload Payload { get; }

		public StateFailed(TPayload payload)
		{
			Payload = payload;
		}
	}

	public sealed class StateMachineCompleted : IStateMachineTerminalResult
	{
		public static StateMachineCompleted Instance { get; } = new();

		private StateMachineCompleted()
		{
		}
	}

	public sealed class StateMachineCompleted<TPayload> : IStateMachineTerminalResult
	{
		public TPayload Payload { get; }

		public StateMachineCompleted(TPayload payload)
		{
			Payload = payload;
		}
	}
}
#endif
