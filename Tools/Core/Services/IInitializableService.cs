using System.Threading;
#if UNITASK
using Cysharp.Threading.Tasks;
#endif
using GothmogToolkit.Tools.Core.OperationResults;

namespace GothmogToolkit.GothmogToolkitParent.GothmogToolkit.Tools.Core.Services
{
    public interface IInitializableService
    {
#if UNITASK
        UniTask<OperationResult> InitializeAsync(CancellationToken cancellationToken = default);
#else
        Task<OperationResult> InitializeAsync(CancellationToken cancellationToken = default);
#endif
    }
}
