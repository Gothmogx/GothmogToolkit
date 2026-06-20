#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace GothmogToolkit.Tools.EditorTools.TextAssetCreator
{
    public class CreateTextAssetAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var content = string.Empty;
            var extension = Path.GetExtension(pathName).ToLower();
            if (extension == ".json")
            {
                content = "{\n\t\n}";
            }

            File.WriteAllText(pathName, content);
            AssetDatabase.ImportAsset(pathName);
            var asset = AssetDatabase.LoadAssetAtPath<Object>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }

    public static class TextAssetCreatorMenu
    {
        [MenuItem("Assets/Create/Gothmog/TextAsset/JSON", false, 80)]
        public static void CreateJsonFile()
        {
            var icon = EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D;
            var action = ScriptableObject.CreateInstance<CreateTextAssetAction>();
            var directory = GetSelectedPath();
            var path = Path.Combine(directory, "NewJson.json");
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                action,
                path,
                icon,
                null
            );
        }

        [MenuItem("Assets/Create/Gothmog/TextAsset/Text", false, 81)]
        public static void CreateTxtFile()
        {
            var icon = EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D;
            var action = ScriptableObject.CreateInstance<CreateTextAssetAction>();
            var directory = GetSelectedPath();
            var path = Path.Combine(directory, "NewText.txt");
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                action,
                path,
                icon,
                null
            );
        }

        private static string GetSelectedPath()
        {
            var path = "Assets";
            foreach (var obj in Selection.GetFiltered<Object>(SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                }
                break;
            }
            return path;
        }
    }
}
#endif
