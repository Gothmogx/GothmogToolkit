using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GothmogToolkit.GothmogToolkitParent.GothmogToolkit.Tools.Core.Services;
using GothmogToolkit.Tools.Core.StatesHandler;
using VContainer;

namespace GothmogToolkit.Tools.Core.Bootstrap
{
    public class BootstrapInitializationState : AsyncState
    {
        private readonly IReadOnlyList<IBootstrapInitializableService> _services;

        [Inject]
        public BootstrapInitializationState(IReadOnlyList<IBootstrapInitializableService> services)
        {
            _services = services;
        }

        protected override async UniTask<IStateResult> Process(CancellationToken cancellationToken)
        {
            foreach (var service in _services)
            {
                await service.InitializeAsync(cancellationToken);
            }

            return StateCompleted.Instance;
        }
    }
}
