#if UNITASK
using UnityEngine.Pool;

namespace GothmogToolkit.Tools.Core.StatesHandler
{
	[System.Obsolete("Use result-driven transitions instead.")]
	public class StateTransitionsPool: ObjectPool<StateTransition>
	{
		public StateTransitionsPool() : base(createFunc: OnCreate, actionOnRelease: OnRelease)
		{
		}

		private static StateTransition OnCreate() => new();

		private static void OnRelease(StateTransition transition) => transition.Release();

	}
}
#endif
