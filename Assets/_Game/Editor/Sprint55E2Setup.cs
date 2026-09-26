using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TilkiOyunu.Foundation.Editor
{
    public static class Sprint55E2Setup
    {
        public static void Configure()
        {
            const string directory = "Assets/_Game/Resources";
            if (!AssetDatabase.IsValidFolder(directory)) AssetDatabase.CreateFolder("Assets/_Game", "Resources");
            const string path = directory + "/TilkiSettings.asset";
            var config = AssetDatabase.LoadAssetAtPath<SettingsConfiguration>(path);
            if (config == null) { config = ScriptableObject.CreateInstance<SettingsConfiguration>(); AssetDatabase.CreateAsset(config, path); }
            config.inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/_Game/Settings/TilkiInputActions.inputactions");
            EditorUtility.SetDirty(config); AssetDatabase.SaveAssets();
            Debug.Log("Sprint 5.5 E.2 settings configuration ready.");
        }
    }
}
