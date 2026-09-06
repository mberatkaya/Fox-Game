using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TilkiOyunu.Foundation.Editor
{
    public static class Sprint1Validation
    {
        private const string PlayerPrefabPath = "Assets/_Game/Prefabs/Characters/PlayerFox.prefab";
        private const string InputAssetPath = "Assets/_Game/Settings/TilkiInputActions.inputactions";

        public static void RunBatch()
        {
            try
            {
                ValidateBuildSettings();
                ValidateInputAsset();
                ValidateBootstrapScene();
                ValidateForestScene();
                ValidateNewGameplayInputUsage();
                Debug.Log("[BOOT] Sprint 1 validation passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError($"[BOOT] Sprint 1 validation failed: {exception}");
                EditorApplication.Exit(1);
            }
        }

        private static void ValidateBuildSettings()
        {
            EditorBuildSettingsScene[] enabledScenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).ToArray();
            Require(enabledScenes.Length >= 2, "Bootstrap and Forest must be enabled in Build Settings.");
            Require(enabledScenes[0].path == SceneIds.BootstrapPath, "Bootstrap scene must be the first enabled build scene.");
            Require(enabledScenes[1].path == SceneIds.ForestPath, "Forest scene must be the second enabled build scene.");
        }

        private static void ValidateInputAsset()
        {
            InputActionAsset inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputAssetPath);
            Require(inputActions != null, "TilkiInputActions asset is missing.");
            InputActionMap player = inputActions.FindActionMap(InputActionIds.MapPlayer, false);
            Require(player != null, "Player action map is missing.");
            Require(player.FindAction(InputActionIds.Move, false) != null, "Move action is missing.");
            Require(player.FindAction(InputActionIds.Look, false) != null, "Look action is missing.");
            Require(player.FindAction(InputActionIds.Jump, false) != null, "Jump action is missing.");
            Require(player.FindAction(InputActionIds.Sprint, false) != null, "Sprint action is missing.");
            Require(player.FindAction(InputActionIds.Interact, false) != null, "Interact action is missing.");
            Require(player.FindAction(InputActionIds.Pause, false) != null, "Pause action is missing.");
        }

        private static void ValidateBootstrapScene()
        {
            Scene scene = EditorSceneManager.OpenScene(SceneIds.BootstrapPath);
            RequireMissingScripts(scene);
            GameBootstrap bootstrap = UnityEngine.Object.FindFirstObjectByType<GameBootstrap>();
            Require(bootstrap != null, "Bootstrap scene must contain GameBootstrap.");
            Require(GetBool(bootstrap, "loadForestOnStart"), "GameBootstrap must load Forest on start.");
        }

        private static void ValidateForestScene()
        {
            Scene scene = EditorSceneManager.OpenScene(SceneIds.ForestPath);
            RequireMissingScripts(scene);

            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            Require(playerPrefab != null, "PlayerFox prefab is missing.");
            Require(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(playerPrefab) == 0, "PlayerFox prefab has missing scripts.");

            FoxController player = UnityEngine.Object.FindFirstObjectByType<FoxController>();
            PlayerInteractor interactor = UnityEngine.Object.FindFirstObjectByType<PlayerInteractor>();
            ThirdPersonCameraController camera = UnityEngine.Object.FindFirstObjectByType<ThirdPersonCameraController>();
            InteractionPromptUI prompt = UnityEngine.Object.FindFirstObjectByType<InteractionPromptUI>();
            TestInteractable testInteractable = UnityEngine.Object.FindFirstObjectByType<TestInteractable>();
            PlayerSpawnPoint spawnPoint = UnityEngine.Object.FindFirstObjectByType<PlayerSpawnPoint>();
            ForestGameplayBootstrap gameplayBootstrap = UnityEngine.Object.FindFirstObjectByType<ForestGameplayBootstrap>();

            Require(player != null, "Forest must contain a FoxController.");
            Require(player.GetComponent<CharacterController>() != null, "FoxController must use CharacterController.");
            Require(interactor != null, "Forest must contain a PlayerInteractor.");
            Require(camera != null, "Forest must contain a ThirdPersonCameraController.");
            Require(prompt != null, "Forest must contain an InteractionPromptUI.");
            Require(testInteractable != null, "Forest must contain a TestInteractable.");
            Require(spawnPoint != null, "Forest must contain a PlayerSpawnPoint.");
            Require(gameplayBootstrap != null, "Forest must contain ForestGameplayBootstrap.");

            Require(GetObject(player, "cameraTransform") != null, "FoxController cameraTransform is not wired.");
            Require(GetObject(interactor, "promptUI") != null, "PlayerInteractor promptUI is not wired.");
            Require(GetObject(interactor, "origin") != null, "PlayerInteractor origin is not wired.");
            Require(GetObject(camera, "target") != null, "ThirdPersonCameraController target is not wired.");

            Require(GameObject.Find("Small Bridge") != null, "Forest blockout must contain Small Bridge.");
            Require(GameObject.Find("Blocked Placeholder Pond") != null, "Forest blockout must contain Blocked Placeholder Pond.");
            Require(GameObject.Find("Camp Placeholder") != null, "Forest blockout must contain Camp Placeholder.");
            Require(GameObject.FindObjectsByType<Collider>(FindObjectsSortMode.None).Length > 20, "Forest blockout should have traversal/blocker colliders.");
            Require(GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None).Count(t => t.name.StartsWith("Traversal Tree", StringComparison.Ordinal)) >= 10, "Forest traversal tree area is incomplete.");
        }

        private static void ValidateNewGameplayInputUsage()
        {
            string[] files =
            {
                "Assets/_Game/Scripts/Character/FoxController.cs",
                "Assets/_Game/Scripts/Camera/ThirdPersonCameraController.cs",
                "Assets/_Game/Scripts/Interaction/PlayerInteractor.cs"
            };

            foreach (string file in files)
            {
                string source = File.ReadAllText(file);
                Require(!source.Contains("Input.Get", StringComparison.Ordinal), $"{file} must not use legacy Input.Get* APIs.");
            }
        }

        private static void RequireMissingScripts(Scene scene)
        {
            int missingScripts = scene.GetRootGameObjects()
                .Sum(root => root.GetComponentsInChildren<Transform>(true).Sum(item => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(item.gameObject)));

            Require(missingScripts == 0, $"Scene has missing scripts: {scene.path}");
        }

        private static UnityEngine.Object GetObject(UnityEngine.Object target, string propertyName)
        {
            return new SerializedObject(target).FindProperty(propertyName).objectReferenceValue;
        }

        private static bool GetBool(UnityEngine.Object target, string propertyName)
        {
            return new SerializedObject(target).FindProperty(propertyName).boolValue;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
