using System;
using System.IO;
using System.Linq;
using TilkiOyunu.Foundation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class Sprint0Validation
{
    public static void RunBatch()
    {
        try
        {
            ValidateBuildSettings();
            ValidateRenderPipeline();
            ValidateScene(SceneIds.BootstrapPath);
            ValidateScene(SceneIds.ForestPath);
            ValidateScene(SceneIds.SystemsTestPath);
            ValidateSaveRoundtrip();
            Debug.Log("[BOOT] Sprint 0 validation passed.");
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogError($"[BOOT] Sprint 0 validation failed: {exception}");
            EditorApplication.Exit(1);
        }
    }

    private static void ValidateBuildSettings()
    {
        string firstEnabledScene = EditorBuildSettings.scenes.FirstOrDefault(scene => scene.enabled)?.path;
        Require(firstEnabledScene == SceneIds.BootstrapPath, "Bootstrap scene must be the first enabled build scene.");
    }

    private static void ValidateRenderPipeline()
    {
        Require(GraphicsSettings.defaultRenderPipeline is UniversalRenderPipelineAsset, "Default render pipeline must be URP.");
        Require(QualitySettings.renderPipeline is UniversalRenderPipelineAsset, "Quality render pipeline must be URP.");
    }

    private static void ValidateScene(string scenePath)
    {
        Require(File.Exists(scenePath), $"Scene does not exist: {scenePath}");
        EditorSceneManager.OpenScene(scenePath);

        int missingScripts = Resources.FindObjectsOfTypeAll<GameObject>()
            .Where(gameObject => gameObject.scene.IsValid())
            .Sum(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount);

        Require(missingScripts == 0, $"Scene has missing scripts: {scenePath}");
    }

    private static void ValidateSaveRoundtrip()
    {
        string directory = Path.Combine(Path.GetTempPath(), "TilkiOyunuSprint0Validation", Guid.NewGuid().ToString("N"));
        string savePath = Path.Combine(directory, SaveService.DefaultFileName);
        SaveService service = new SaveService(savePath);
        SaveGameData data = service.CreateNewSave();
        data.gameState = GameState.Playing;
        data.collectedMemoryIds.Add("memory_01");

        Require(service.Save(data), "SaveService.Save returned false.");
        SaveGameData loaded = service.Load();
        Require(loaded.gameState == GameState.Playing, "Loaded game state did not match.");
        Require(loaded.collectedMemoryIds.Contains("memory_01"), "Loaded collected memory ids did not match.");
        Require(service.DeleteSave(), "SaveService.DeleteSave returned false.");

        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
