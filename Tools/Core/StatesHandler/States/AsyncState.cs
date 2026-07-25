#if UNITASK
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GothmogToolkit.Tools.Core.StatesHandler
{
	public abstract class AsyncState
	{
		public virtual Type Type => GetType();
		public bool IsActive { get; private set; }

		public event Action StateEntered;
		public event Action StateExited;

		internal async UniTask<IStateResult> Execute(CancellationToken cancellationToken,
			object input,
			bool hasInput)
		{
			IsActive = true;

			try
			{
				StateEntered?.Invoke();
				OnEntered();

				return hasInput
					? await ProcessWithInput(cancellationToken, input)
					: await Process(cancellationToken);
			}
			finally
			{
				IsActive = false;
				StateExited?.Invoke();
				OnExited();
			}
		}

		protected abstract UniTask<IStateResult> Process(CancellationToken cancellationToken);

		protected virtual UniTask<IStateResult> ProcessWithInput(CancellationToken cancellationToken,
			object input)
		{
			throw new InvalidOperationException(
				$"State {this} does not accept input, but a transition supplied {input ?? "null"}.");
		}

		protected virtual void OnEntered()
		{
		}

		protected virtual void OnExited()
		{
		}

		public override string ToString() => GetType().Name;
	}

	public abstract class AsyncState<TInput> : AsyncState
	{
		protected sealed override UniTask<IStateResult> Process(CancellationToken cancellationToken)
		{
			throw new InvalidOperationException(
				$"State {this} requires input of type {typeof(TInput)}.");
		}

		protected sealed override UniTask<IStateResult> ProcessWithInput(
			CancellationToken cancellationToken,
			object input)
		{
			if (input == null)
			{
				if (typeof(TInput).IsValueType && Nullable.GetUnderlyingType(typeof(TInput)) == null)
				{
					throw new ArgumentException(
						$"State {this} requires input of type {typeof(TInput)}, but received null.");
				}

				return Process(cancellationToken, default);
			}

			if (!(input is TInput typedInput))
			{
				throw new ArgumentException(
					$"State {this} requires input of type {typeof(TInput)}, but received {input.GetType()}.");
			}

			return Process(cancellationToken, typedInput);
		}

		protected abstract UniTask<IStateResult> Process(
			CancellationToken cancellationToken,
			TInput input);
	}
}
#endif
