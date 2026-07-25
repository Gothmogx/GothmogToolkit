#if UNITASK
using System;

namespace GothmogToolkit.Tools.Core.StatesHandler
{
	[Obsolete("Use IStateResult and result-driven transitions instead.")]
	public class StateTransition
	{
		private static StateTransitionsPool Pool { get; } = new();
		public State PreviousState { get; private set; }
		public Type NextStateType { get; private set; }
		public object TransitionArgs { get; private set; }
		public bool IsYieldRequired { get; private set; }

		private void Initialize(State previousState = null, Type nextStateType = null, object transitionArgs = null, bool isYieldRequired = false)
		{
			PreviousState = previousState;
			NextStateType = nextStateType;
			TransitionArgs = transitionArgs;
			IsYieldRequired = isYieldRequired;
		}

		public void Release()
		{
			PreviousState = null;
			NextStateType = null;
			TransitionArgs = null;
		}
		
		public static StateTransition GetTransition(State previousState, Type nextStateType = null, object transitionArgs = null, bool isYieldRequired = false)
		{
			var state = Pool.Get();
			state.Initialize(previousState, nextStateType, transitionArgs);
			return state;
		}

	}
}
#endif
