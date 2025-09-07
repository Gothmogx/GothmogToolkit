#if UNITASK
using Cysharp.Threading.Tasks;
using GothmogToolkit.Tools.Core.OperationResults;
using UnityEngine.SceneManagement;

namespace GothmogToolkit.Tools.ScenesManager
{
	public interface IScenesManager
	{
		UniTask<OperationData> LoadScene(string sceneKey, LoadSceneMode mode = LoadSceneMode.Single,
			bool activateOnLoad = true, UniTask onBeforeLoad = default, UniTask onAfterLoad = default);
		(string key, Scene scene) GetActiveScene();
		bool TryUnloadScene(string sceneKey);
		void UnloadAllScenes();
	}
}
#endif