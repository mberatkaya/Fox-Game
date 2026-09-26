using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TilkiOyunu.Foundation.PlayModeTests
{
    public sealed class Sprint55E1MapPlayModeTests
    {
        private Keyboard keyboard;
        private Mouse mouse;
        private InputSettings.EditorInputBehaviorInPlayMode previousEditorInput;
        private InputSettings.BackgroundBehavior previousBackground;
        [SetUp]
        public void SetupInput()
        {
            previousEditorInput = InputSystem.settings.editorInputBehaviorInPlayMode;
            previousBackground = InputSystem.settings.backgroundBehavior;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
        }
        private static T Find<T>() where T : Object => Object.FindFirstObjectByType<T>();

        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            if (keyboard != null) InputSystem.RemoveDevice(keyboard);
            if (mouse != null) InputSystem.RemoveDevice(mouse);
            InputSystem.settings.editorInputBehaviorInPlayMode = previousEditorInput;
            InputSystem.settings.backgroundBehavior = previousBackground;
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

        [UnityTest]
        public IEnumerator MapKeysLockMovementLookAndRestoreOwnedStateEvenOnDisable()
        {
            new SaveService().DeleteSave(); // Suite-level ProductionSaveProtection restores user bytes.
            yield return SceneManager.LoadSceneAsync(SceneIds.Bootstrap);
            float deadline = Time.realtimeSinceStartup + 20;
            while (Find<QuestMapUI>() == null && Time.realtimeSinceStartup < deadline) yield return null;
            yield return null; yield return null;
            var map = Find<QuestMapUI>(); Assert.That(map, Is.Not.Null);
            var inputLock = Find<GameplayInputLock>(); var camera = Find<ThirdPersonCameraController>();
            var fox = Find<FoxController>(); var lights = Find<LightPathController>();
            Assert.That(camera.InputActions.FindAction("Player/WorldMap", false), Is.Not.Null);
            map.SetOpen(true);
            Assert.That(map.IsOpen, Is.True, "Map initialization and lock availability");
            map.SetOpen(false);
            keyboard = InputSystem.AddDevice<Keyboard>();
            mouse = InputSystem.AddDevice<Mouse>();
            Cursor.lockState = CursorLockMode.None; Cursor.visible = false;
            yield return Press(Key.M);
            Assert.That(map.IsOpen, Is.True); Assert.That(inputLock.IsLocked, Is.True);
            Assert.That(camera.IsLookInputLocked, Is.True); Assert.That(Cursor.visible, Is.True);
            var lockedRotation = camera.transform.rotation;
            InputSystem.QueueDeltaStateEvent(mouse.delta, new Vector2(180, 80));
            yield return null; yield return null;
            Assert.That(Quaternion.Angle(lockedRotation, camera.transform.rotation), Is.LessThan(.01f));
            yield return Press(Key.W);
            Assert.That(fox.MoveInput, Is.EqualTo(Vector2.zero));
            yield return Press(Key.M);
            Assert.That(map.IsOpen, Is.False); Assert.That(inputLock.IsLocked, Is.False);
            Assert.That(camera.IsLookInputLocked, Is.False); Assert.That(Cursor.visible, Is.False);
            Assert.That(Cursor.lockState, Is.EqualTo(CursorLockMode.None));
            InputSystem.QueueDeltaStateEvent(mouse.delta, new Vector2(180, 80));
            yield return null; yield return null;
            Assert.That(Quaternion.Angle(lockedRotation, camera.transform.rotation), Is.GreaterThan(1f), "camera look restored");
            yield return Press(Key.M); yield return Press(Key.Escape);
            Assert.That(map.IsOpen, Is.False); Assert.That(inputLock.IsLocked, Is.False);
            camera.SetLookInputLocked(true); map.SetOpen(true); map.enabled = false;
            Assert.That(inputLock.IsLocked, Is.False); Assert.That(camera.IsLookInputLocked, Is.True);
            Assert.That(lights.NavigationPaused, Is.False); camera.SetLookInputLocked(false);
            map.enabled = true;
            inputLock.Acquire(); map.SetOpen(true); Assert.That(map.IsOpen, Is.False); inputLock.Release();
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.W));
            yield return null; yield return null;
            Assert.That(fox.MoveInput.y, Is.GreaterThan(.9f), "movement action restored");
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            Assert.That(lights.DurationSeconds, Is.EqualTo(75));
            Assert.That(lights.State, Is.EqualTo(LightPathRunState.Inactive));
        }

        [UnityTest]
        public IEnumerator AcceptanceWaitsForExplicitStartAndMapPreservesTimerAndNextNode()
        {
            new SaveService().DeleteSave();
            yield return SceneManager.LoadSceneAsync(SceneIds.Bootstrap);
            float deadline = Time.realtimeSinceStartup + 20;
            while (Find<QuestMapUI>() == null && Time.realtimeSinceStartup < deadline) yield return null;
            yield return null; yield return null;
            var quests = GameServices.Current.Quest;
            var memoryQuest = quests.FindQuest("collect_memories");
            quests.StartQuest(memoryQuest); quests.AddProgress(memoryQuest, 5); quests.TurnInQuest(memoryQuest);
            var light = Find<LightPathController>();
            quests.StartQuest(light.Quest);
            yield return new WaitForSeconds(.2f);
            Assert.That(light.State, Is.EqualTo(LightPathRunState.Inactive));
            Assert.That(light.RemainingSeconds, Is.Zero);
            Find<LightPathStart>().Interact(new InteractionContext(Find<FoxController>().gameObject));
            Assert.That(light.State, Is.EqualTo(LightPathRunState.Running));
            Assert.That(light.RemainingSeconds, Is.EqualTo(75).Within(.1f));
            var map = Find<QuestMapUI>(); map.SetOpen(true);
            float time = light.RemainingSeconds;
            yield return new WaitForSeconds(.2f);
            Assert.That(light.RemainingSeconds, Is.EqualTo(time));
            map.SetOpen(false);
            yield return new WaitForSeconds(.2f);
            Assert.That(light.RemainingSeconds, Is.LessThan(time));
            foreach (var node in Object.FindObjectsByType<LightPathNode>(FindObjectsSortMode.None))
            {
                var marker = node.GetComponent<MapMarker>();
                Assert.That(marker.IsVisible(quests, false, light), Is.EqualTo(node.SequenceIndex == 0));
                Assert.That(marker.IsVisible(quests, true, light), Is.True);
            }
        }
    }
}
