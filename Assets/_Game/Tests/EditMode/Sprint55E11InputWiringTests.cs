using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TilkiOyunu.Foundation.Tests
{
    public sealed class Sprint55E11InputWiringTests
    {
        [Test]
        public void ProductionScenesAndPrefabUseCanonicalActionsAndContent()
        {
            var previous = EditorSceneManager.GetSceneManagerSetup();
            try
            {
                var asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/_Game/Settings/TilkiInputActions.inputactions");
                Assert.That(asset.actionMaps.Count, Is.EqualTo(1));
                string[] names = { "Move", "Look", "Jump", "Sprint", "Interact", "WorldMap", "Pause" };
                foreach (string name in names) Assert.That(asset.FindAction("Player/" + name), Is.Not.Null);
                foreach (var binding in asset.bindings)
                    if (binding.path.StartsWith("<Keyboard>") || binding.path.StartsWith("<Mouse>"))
                        Assert.That(binding.groups, Does.Contain("KeyboardMouse"), binding.path);
                Assert.That(asset.FindAction("Player/Interact").GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse")), Does.Contain("F"));
                Assert.That(asset.FindAction("Player/WorldMap").bindings[0].path, Is.EqualTo("<Keyboard>/m"));
                EditorSceneManager.OpenScene(SceneIds.BootstrapPath);
                var content = Object.FindFirstObjectByType<GameBootstrap>().ContentConfig;
                EditorSceneManager.OpenScene(SceneIds.ForestPath);
                Assert.That(Object.FindFirstObjectByType<GameBootstrap>().ContentConfig, Is.SameAs(content));
                Assert.That(Object.FindObjectsByType<PlayerInput>(FindObjectsSortMode.None), Is.Empty);
                // Opening a scene can reload a managed asset wrapper; compare serialized identity.
                Assert.That(AssetDatabase.GetAssetPath(Object.FindFirstObjectByType<ThirdPersonCameraController>().InputActions),
                    Is.EqualTo("Assets/_Game/Settings/TilkiInputActions.inputactions"));
                var fox = Object.FindFirstObjectByType<FoxController>();
                AssertAsset(fox, asset);
                AssertAsset(fox.GetComponent<PlayerInteractor>(), asset);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/Characters/PlayerFox.prefab");
                AssertAsset(prefab.GetComponent<FoxController>(), asset);
                AssertAsset(prefab.GetComponent<PlayerInteractor>(), asset);
                Assert.That(Object.FindFirstObjectByType<GuideNpc>().GetComponent<Collider>().isTrigger, Is.True);
            }
            finally { EditorSceneManager.RestoreSceneManagerSetup(previous); }
        }

        private static void AssertAsset(Object component, InputActionAsset asset)
        {
            Assert.That(AssetDatabase.GetAssetPath(new SerializedObject(component).FindProperty("inputActions").objectReferenceValue),
                Is.EqualTo("Assets/_Game/Settings/TilkiInputActions.inputactions"));
        }
    }
}
