using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TilkiOyunu.Foundation.PlayModeTests
{
    public sealed class Sprint55E11RuntimeTests
    {
        private Keyboard keyboard;
        private InputSettings.EditorInputBehaviorInPlayMode editorInput;
        private InputSettings.BackgroundBehavior background;
        private static T Find<T>() where T : Object => Object.FindFirstObjectByType<T>();
        private static T Field<T>(object owner, string name) => (T)owner.GetType()
            .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(owner);

        [SetUp]
        public void Setup()
        {
            editorInput = InputSystem.settings.editorInputBehaviorInPlayMode;
            background = InputSystem.settings.backgroundBehavior;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            keyboard = InputSystem.AddDevice<Keyboard>();
            new SaveService().DeleteSave(); // Protected by ProductionSaveProtection.
        }

        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            InputSystem.RemoveDevice(keyboard);
            InputSystem.settings.editorInputBehaviorInPlayMode = editorInput;
            InputSystem.settings.backgroundBehavior = background;
            foreach (var bootstrap in Object.FindObjectsByType<GameBootstrap>(FindObjectsSortMode.None)) Object.Destroy(bootstrap.gameObject);
            yield return null;
            GameServices.Shutdown();
        }

        private IEnumerator Press(Key key)
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(key));
            yield return null; yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return null;
        }

        [UnityTest] public IEnumerator DirectForestSupportsProductionInput() => Exercise(SceneIds.Forest);
        [UnityTest] public IEnumerator BootstrapSupportsProductionInput() => Exercise(SceneIds.Bootstrap);
        [UnityTest] public IEnumerator BootstrapMapAndInteractionBindings() => Exercise(SceneIds.Bootstrap, false);

        private IEnumerator Exercise(string entry, bool checkTracking = true)
        {
            yield return SceneManager.LoadSceneAsync(entry);
            float deadline = Time.realtimeSinceStartup + 30;
            while ((SceneManager.GetActiveScene().name != SceneIds.Forest || Find<QuestMapUI>() == null) && Time.realtimeSinceStartup < deadline) yield return null;
            yield return null; yield return null;
            var map = Find<QuestMapUI>();
            var fox = Find<FoxController>();
            var camera = Find<ThirdPersonCameraController>();
            var interactor = fox.GetComponent<PlayerInteractor>();
            var guide = Find<GuideNpc>();
            var inputLock = Find<GameplayInputLock>();
            Assert.That(GameServices.HasCurrent, Is.True, "Forest must initialize production services even when played directly");
            Assert.That(Object.FindObjectsByType<GameBootstrap>(FindObjectsSortMode.None).Length, Is.EqualTo(1));
            Assert.That(inputLock.IsLocked, Is.False);
            Assert.That(Field<InputActionAsset>(fox, "inputActions"), Is.SameAs(camera.InputActions));
            Assert.That(Field<InputActionAsset>(interactor, "inputActions"), Is.SameAs(camera.InputActions));
            foreach (string action in new[] { "Move", "Look", "Jump", "Sprint", "Interact", "WorldMap", "Pause" })
                Assert.That(camera.InputActions.FindAction("Player/" + action).enabled, Is.True, action);
            Assert.That(Field<Transform>(map, "playerTransform"), Is.SameAs(fox.transform));
            var markerViews = Field<IList>(map, "markers");
            RectTransform playerIcon = null;
            foreach (var view in markerViews)
                if (Field<MapMarker>(view, "Target").Type == MapMarkerType.Player) playerIcon = Field<RectTransform>(view, "Mini");
            Assert.That(playerIcon, Is.Not.Null);
            Vector3 spawn = fox.transform.position;
            Vector2 initial = playerIcon.anchorMin;
            yield return Capture("01_minimap_spawn", entry);
            foreach (Vector3 offset in checkTracking ? new[] { Vector3.right * 10, Vector3.left * 10, Vector3.forward * 10, Vector3.back * 10 } : new Vector3[0])
            {
                Pose(fox, spawn + offset, 90);
                yield return null; yield return null;
                Vector2 delta = playerIcon.anchorMin - initial;
                Assert.That(Vector2.Dot(delta, new Vector2(offset.x, offset.z)), Is.GreaterThan(.1f), "Live X/Z must move icon on the corresponding UI axis: " + offset);
                Assert.That(Mathf.DeltaAngle(playerIcon.localEulerAngles.z, -fox.transform.eulerAngles.y), Is.EqualTo(0).Within(.1f));
                if (offset.x > 0) yield return Capture("02_minimap_after_movement", entry);
            }
            Assert.That(Field<GameObject>(map, "overlay").activeSelf, Is.False);
            yield return Press(Key.M);
            Assert.That(map.IsOpen, Is.True); Assert.That(inputLock.IsLocked, Is.True);
            Assert.That(camera.IsLookInputLocked, Is.True);
            Assert.That(Field<GameObject>(map, "overlay").activeSelf, Is.True);
            yield return Capture("03_world_map_open", entry);
            yield return Press(Key.W); Assert.That(fox.MoveInput, Is.EqualTo(Vector2.zero));
            yield return Press(Key.M); Assert.That(map.IsOpen, Is.False);
            yield return Press(Key.M); yield return Press(Key.Escape);
            Assert.That(map.IsOpen, Is.False); Assert.That(inputLock.IsLocked, Is.False);
            Assert.That(camera.IsLookInputLocked, Is.False);
            map.enabled = false; map.enabled = true;
            yield return Press(Key.M); Assert.That(map.IsOpen, Is.True);
            map.enabled = false;
            Assert.That(inputLock.IsLocked, Is.False);
            map.enabled = true;
            yield return Press(Key.W); // Other consumers must still have their actions enabled.
            Assert.That(camera.InputActions.FindAction("Player/Move").enabled, Is.True);
            Vector3 nearGuide = guide.transform.position + Vector3.back * 2;
            nearGuide.y = Terrain.activeTerrain.SampleHeight(nearGuide) + Terrain.activeTerrain.transform.position.y + .1f;
            Pose(fox, nearGuide, 0);
            yield return null; yield return null;
            Assert.That(guide.GetComponent<Collider>().isTrigger, Is.True);
            Assert.That(interactor.Focused, Is.SameAs(guide), "Production range/angle/collider chain must focus NPC_Guide");
            Assert.That(Field<string>(interactor, "bindingLabel"), Does.Contain("F"));
            nearGuide = guide.transform.position + Vector3.back * 1.3f;
            nearGuide.y = Terrain.activeTerrain.SampleHeight(nearGuide) + Terrain.activeTerrain.transform.position.y + .1f;
            Pose(fox, nearGuide, 0);
            yield return null; yield return null;
            Assert.That(interactor.Focused, Is.SameAs(guide), "Close approach must not reject the tall NPC due to vertical view angle");
            yield return Capture("04_npc_interaction_prompt", entry);
            yield return Press(Key.F);
            Assert.That(Find<DialoguePanelUI>().IsOpen, Is.True, "F must reach the production interactor and open dialogue");
            yield return Capture("05_npc_dialogue", entry);
            Find<DialoguePanelUI>().Close();
            Assert.That(inputLock.IsLocked, Is.False);
        }

        private static void Pose(FoxController fox, Vector3 position, float yaw)
        {
            var pose = new GameObject("Test pose");
            pose.transform.SetPositionAndRotation(position, Quaternion.Euler(0, yaw, 0));
            fox.TeleportTo(pose.transform);
            Object.Destroy(pose);
        }

        private static IEnumerator Capture(string name, string entry)
        {
            if (entry != SceneIds.Forest || System.Environment.GetEnvironmentVariable("TILKI_E11_SCREENSHOTS") != "1") yield break;
            const string folder = "Documentation/Sprint55E11/";
            Directory.CreateDirectory(folder);
            yield return new WaitForEndOfFrame();
            System.Type.GetType("UnityEngine.ScreenCapture, UnityEngine.ScreenCaptureModule", true)
                .GetMethod("CaptureScreenshot", new[] { typeof(string) }).Invoke(null, new object[] { folder + name + ".png" });
            yield return new WaitForEndOfFrame();
            yield return null; yield return null;
        }
    }
}
