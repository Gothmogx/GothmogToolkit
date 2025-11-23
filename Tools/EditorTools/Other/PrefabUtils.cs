using UnityEngine;

namespace GothmogToolkit.Tools.EditorTools.Other
{
	public static class PrefabUtils
	{
		#if UNITY_EDITOR
		public static T InstantiatePrefab<T>(T prefab, Transform transform) where T : Component
			=> UnityEditor.PrefabUtility.InstantiatePrefab(prefab, transform) as T;
		#else 
		public static T InstantiatePrefab<T>(T prefab, Transform transform) where T : Component  => null;
		#endif
	}
}