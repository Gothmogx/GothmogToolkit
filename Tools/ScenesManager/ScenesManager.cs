#if UNITASK && ADDRESSABLES
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GothmogToolkit.Tools.Core.OperationResults;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace GothmogToolkit.Tools.ScenesManager
{
	public class ScenesManager : IScenesManager
	{
		private string _activeScene;
		private readonly Dictionary<string, AsyncOperationHandle> _loadedScenes = new();
		public async UniTask<OperationData> LoadScene(string sceneKey, LoadSceneMode mode = LoadSceneMode.Single,
			bool activateOnLoad = true, UniTask onBeforeLoad = default, UniTask onAfterLoad = default)
		{
			try
			{
				if (_loadedScenes.ContainsKey(sceneKey) && _loadedScenes[sceneKey].IsValid())
					throw new InvalidOperationException($"Scene {sceneKey} is already loaded");

				await onBeforeLoad;
				var handle = Addressables.LoadSceneAsync(sceneKey, mode, activateOnLoad);
				await handle.Task;
				_loadedScenes[sceneKey] = handle;
				_activeScene = sceneKey;
				await onAfterLoad;
			}
			catch (OperationCanceledException)
			{
				_activeScene = null;
				return new OperationData(OperationResult.Cancelled);
			}
			catch (Exception exception)
			{
				_activeScene = null;
				return new OperationData(OperationResult.Failure, exception, $"Failed to load scene {sceneKey}");
			}
			return new OperationData(OperationResult.Success);
		}
		public (string key, Scene scene) GetActiveScene() => (_activeScene, SceneManager.GetActiveScene());

		public bool TryUnloadScene(string sceneKey)
		{
			if (!_loadedScenes.TryGetValue(sceneKey, out var handle))
				return false;

			handle.Release();
			_loadedScenes.Remove(sceneKey);
			return true;
		}

		public void UnloadAllScenes()
		{
			foreach (var handle in _loadedScenes.Values)
			{
				handle.Release();
			}
		}
	}
}
#endif