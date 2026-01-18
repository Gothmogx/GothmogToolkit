#if UNITY_EDITOR && EDITOR_BOOTSTRAPPER
using UnityEditor;
using UnityEditor.SceneManagement;

namespace GothmogToolkit.Tools.EditorTools.Bootstrapper
{
	[InitializeOnLoad]
	public class EditorBootstrapper
	{
		static EditorBootstrapper()
		{
			EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
		}
	}
}
#endif