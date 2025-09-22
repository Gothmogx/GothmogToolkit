#if ADDRESSABLES
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GothmogToolkit.Tools.Core.Id;
using GothmogToolkit.Tools.Core.OperationResults;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GothmogToolkit.Tools.Core.AddressablesTools
{
	public abstract class AddressableEntityDatabase<TType, TId> : IDisposable
		where TType : IIdContainer<TId>
	{
		private readonly Dictionary<TId, TType> _types = new();
		private AsyncOperationHandle<IList<TType>> _handles;
		public bool TryGetType(TId typeId, out TType type) => _types.TryGetValue(typeId, out type);
		public IEnumerable<TType> GetTypes() => _types.Values;
		public int Count => _types.Count;
		protected abstract List<string> Keys { get; }
		protected virtual Addressables.MergeMode MergeMode => Addressables.MergeMode.Intersection;

		public async UniTask<OperationResult> Initialize()
		{
			try
			{
				_handles = Addressables.LoadAssetsAsync<TType>(Keys, OnLoaded, MergeMode);
				await _handles.ToUniTask();
				OnAllResourcesLoaded();

			}
			catch (Exception)
			{
				return OperationResult.Failure;
			}

			return OperationResult.Success;
		}
		protected virtual void OnAllResourcesLoaded() { }

		protected virtual void OnLoaded(TType resource)
		{
			_types.TryAdd(resource.Id, resource);
		}
		public void Dispose()
		{
			if (_handles.IsValid() && _handles.IsDone)
				_handles.Release();
		}
	}
}
#endif