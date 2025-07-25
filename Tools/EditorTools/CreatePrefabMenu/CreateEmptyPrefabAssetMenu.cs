#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GothmogToolkit.Tools.EditorTools
{
	public static class CreateEmptyPrefabAssetMenu
	{
		[MenuItem("Assets/Create/Empty Prefab", false, 10)]
		public static void CreatePrefab()
		{
			try
			{
				var path = GetSelectedPathOrFallback();
				var prefabPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, "New Empty Prefab.prefab"));

				var go = new GameObject("New Empty Prefab");
				PrefabUtility.SaveAsPrefabAsset(go, prefabPath);

				Object.DestroyImmediate(go);
				Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
			}
			catch (System.Exception e)
			{
				Debug.LogError($"Failed to create prefab: {e.Message}");
			}
		}
	
		private static string GetSelectedPathOrFallback()
		{
			var path = "Assets";

			foreach (var obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
			{
				var tempPath = AssetDatabase.GetAssetPath(obj);
				path = string.IsNullOrEmpty(tempPath) switch
				{
					false when File.Exists(tempPath) => Path.GetDirectoryName(tempPath),
					false => tempPath,
					_ => path
				};
			}

			return path;
		}
	}
}
#endif