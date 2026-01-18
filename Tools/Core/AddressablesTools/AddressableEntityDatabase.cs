#if ADDRESSABLES
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GothmogToolkit.Tools.Core.Id;
using GothmogToolkit.Tools.Core.OperationResults;
using GothmogToolkit.Tools.Core.Types;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace GothmogToolkit.Tools.Core.AddressablesTools
{
	public abstract class AddressablePrefabDatabase<TType, TId> : AddressableEntityDatabase<TType, GameObject, TId>
		where TType : Object, ITypeContainer<TId>
	{
		protected override async UniTask LoadResources()
		{
			_handles = Addressables.LoadAssetsAsync<GameObject>(Keys, OnLoaded, MergeMode);
			await _handles.ToUniTask();
		}
		protected override void OnLoaded(GameObject resource)
		{
			if (resource.TryGetComponent<TType>(out var type))
				TryAdd(type);
		}
	}

	public abstract class AddressableObjectDatabase<TType, TId> : AddressableEntityDatabase<TType, TType, TId>
		where TType : Object,  ITypeContainer<TId>
	{
		protected override async UniTask LoadResources()
		{
			_handles = Addressables.LoadAssetsAsync<TType>(Keys, OnLoaded, MergeMode);
			await _handles.ToUniTask();
		}

		protected override void OnLoaded(TType resource)
		{
			TryAdd(resource);
		}
	}

	public abstract class AddressableEntityDatabase<TType, THandle, TId> : IDisposable
		where TType : Object,  ITypeContainer<TId>
	{
		private readonly Dictionary<TId, TType> _types = new();
		protected AsyncOperationHandle<IList<THandle>> _handles;
		public bool TryGetType(TId typeId, out TType type) => _types.TryGetValue(typeId, out type);
		public IEnumerable<TType> GetTypes() => _types.Values;
		public int Count => _types.Count;
		protected abstract List<string> Keys { get; }
		protected virtual Addressables.MergeMode MergeMode => Addressables.MergeMode.Intersection;

		public async UniTask<OperationResult> Initialize()
		{
			try
			{
				await LoadResources();
				OnAllResourcesLoaded();
			}
			catch (Exception)
			{
				return OperationResult.Failure;
			}

			return OperationResult.Success;
		}
		protected abstract UniTask LoadResources();


		protected virtual void OnAllResourcesLoaded()
		{
		}

		protected abstract void OnLoaded(THandle resource);

		protected bool TryAdd(TType type) => _types.TryAdd(type.Type, type);
		public void Clear()
		{
			_types?.Clear();

			if (_handles.IsValid() && _handles.IsDone)
				_handles.Release();
		}
		public void Dispose() => Clear();
	}
}
#endif