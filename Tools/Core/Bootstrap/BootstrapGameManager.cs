using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GothmogToolkit.Tools.Core.StatesHandler;
using GothmogToolkit.Tools.ScenesManager;
using VContainer;
using VContainer.Unity;

namespace GothmogToolkit.Tools.Core.Bootstrap
{
    public abstract class BootstrapGameManager<TState, TFirstState> : IDisposable, IInitializable
        where TState : AsyncState where TFirstState : TState
    {
        private readonly IReadOnlyList<TState> _gameStates;
        protected readonly AsyncStateMachine<TState> _stateMachine;
        private readonly CancellationTokenSource _bootstrapCancellationToken = new();

        [Inject]
        public BootstrapGameManager(AsyncStateMachine<TState> stateMachine,
            IReadOnlyList<TState> gameStates)
        {
            _stateMachine = stateMachine;
            _gameStates = gameStates;
        }

        public void Initialize()
        {
            _stateMachine.Initialize(_gameStates);
            OnAfterStateMachineInitialized();
            Run().Forget();
        }

        protected abstract void OnAfterStateMachineInitialized();

        private async UniTask Run()
        {
            await _stateMachine.Run<TFirstState>(_bootstrapCancellationToken.Token);
        }

        public virtual void Dispose()
        {
            _bootstrapCancellationToken?.Cancel();
            _bootstrapCancellationToken?.Dispose();
        }
    }
}
