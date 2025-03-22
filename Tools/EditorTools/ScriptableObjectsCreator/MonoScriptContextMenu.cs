#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace GothmogToolkit.Tools.EditorTools.ScriptableObjectsCreator
{
    /// <summary>
    /// Adds Create Scriptable Object button to mono script context.
    /// </summary>
    [CustomEditor(typeof(MonoScript))]
    public class MonoScriptContextMenu : Editor
    {
        private const string ButtonName = "Create Scriptable Object";
        private const float SpaceHeight = 20;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            var monoScript = target as MonoScript;
            var type = monoScript?.GetClass();
        
            if (type == null || !type.IsSubclassOf(typeof(ScriptableObject)))
            {   
                GUILayout.Space(SpaceHeight);
                GUILayout.TextArea(monoScript?.text);
                return;
            }

            if (GUILayout.Button(ButtonName))
            {
                var example = CreateInstance(type.FullName);
                var instance = Activator.CreateInstance(type) as ScriptableObject;

                var monoScriptPath = AssetDatabase.GetAssetPath(monoScript);
                var pathDirectory = Path.GetDirectoryName(monoScriptPath);
                Debug.Assert(pathDirectory != null, $"{nameof(pathDirectory)} != null");
                var assetName = Path.Combine(pathDirectory, $"{type.Name}.asset");

                AssetDatabase.CreateAsset(instance, assetName);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = example;
            }
            GUILayout.Space(SpaceHeight);
            GUILayout.TextArea(monoScript.text);
        }
    }
}
#endif