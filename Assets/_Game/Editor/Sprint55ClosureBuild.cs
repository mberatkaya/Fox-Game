using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TilkiOyunu.Foundation.Editor
{
    // Development-only closure check; this is not the Sprint 6 release pipeline.
    public static class Sprint55ClosureBuild
    {
        public static void ValidateAssets()
        {
            try { ValidateProductionAssets(); EditorApplication.Exit(0); }
            catch (Exception exception) { Debug.LogException(exception); EditorApplication.Exit(1); }
        }

        private static void ValidateProductionAssets()
        {
            if (!Sprint5Validation.ValidateProject(out var errors))
                throw new InvalidOperationException(string.Join("\n", errors));
            foreach (EditorBuildSettingsScene entry in EditorBuildSettings.scenes.Where(s => s.enabled))
            {
                var scene = EditorSceneManager.OpenScene(entry.path, OpenSceneMode.Single);
                foreach (GameObject root in scene.GetRootGameObjects())
                foreach (Transform node in root.GetComponentsInChildren<Transform>(true))
                {
                    if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(node.gameObject) != 0)
                        throw new InvalidOperationException($"Missing script: {entry.path}/{node.name}");
                    if (PrefabUtility.IsPrefabAssetMissing(node.gameObject))
                        throw new InvalidOperationException($"Missing prefab: {entry.path}/{node.name}");
                    foreach (Renderer renderer in node.GetComponents<Renderer>())
                    foreach (Material material in renderer.sharedMaterials)
                        if (material == null || material.shader == null || material.shader.name == "Hidden/InternalErrorShader")
                            throw new InvalidOperationException($"Missing material/shader: {entry.path}/{node.name}");
                    foreach (Component component in node.GetComponents<Component>())
                    {
                        if (component == null) continue;
                        var serialized = new SerializedObject(component);
                        var property = serialized.GetIterator();
                        while (property.Next(true))
                            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null && property.objectReferenceInstanceIDValue != 0)
                                throw new InvalidOperationException($"Broken reference: {entry.path}/{node.name}/{property.propertyPath}");
                    }
                }
            }
            Debug.Log("SPRINT55_CLOSURE_ASSETS PASS: enabled scenes have no missing scripts, prefabs, renderer materials/shaders or serialized component references.");
        }

        public static void BuildWindowsSmoke()
        {
            try
            {
                string[] scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
                if (scenes.Length == 0 || scenes[0] != SceneIds.BootstrapPath || !scenes.Contains(SceneIds.ForestPath))
                    throw new InvalidOperationException("Smoke build requires Bootstrap first and Forest enabled.");
                ValidateProductionAssets();
                string output = Environment.GetEnvironmentVariable("TILKI_SMOKE_OUTPUT");
                if (string.IsNullOrEmpty(output)) output = "Builds/Sprint55Closure/TilkiOyunu.exe";
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));
                BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = scenes,
                    locationPathName = output,
                    target = BuildTarget.StandaloneWindows64,
                    options = BuildOptions.Development
                });
                Debug.Log($"SPRINT55_CLOSURE_BUILD result={report.summary.result} errors={report.summary.totalErrors} warnings={report.summary.totalWarnings} bytes={report.summary.totalSize} scenes={string.Join(",", scenes)} output={output}");
                EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 1);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }
    }
}
