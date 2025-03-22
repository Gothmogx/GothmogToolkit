#if UNITASK
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GothmogToolkit.Tools.Core.StatesHandler
{
	public interface IState
	{
		/// <summary>
		/// Used as a key of the dictionary.
		/// </summary>
		Type Type { get; }

		/// <summary>
		/// Shows if the state is active.
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Invoked after entering the state.
		/// </summary>
		event Action StateEntered;

		/// <summary>
		/// Invoked before exiting the state.
		/// </summary>
		event Action StateExited;

		/// <summary>
		/// Executing state operations.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <param name="stateTransitionArgs">Transition args possibly passed from the previous state</param>
		/// <returns></returns>
		UniTask<StateTransition> Execute(CancellationToken cancellationToken, object stateTransitionArgs = null);
	}
}
#endif