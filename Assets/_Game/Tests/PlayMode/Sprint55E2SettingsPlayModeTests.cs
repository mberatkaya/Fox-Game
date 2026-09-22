using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TilkiOyunu.Foundation.PlayModeTests
{
    public sealed class Sprint55E2SettingsPlayModeTests
    {
        private sealed class FakeDisplay : ISettingsDisplay
        {
            public int Count;
            public GameSettingsData Current = GameSettingsData.Defaults(1600, 900);
            public GameSettingsData Capture() => Current.Copy();
            public void Apply(GameSettingsData data) { Current = data.Copy(); Count++; }
        }
        private static T Find<T>() where T : Object => Object.FindFirstObjectByType<T>();

        [UnityTearDown] public IEnumerator Cleanup()
        {
            GameBootstrap.ShowMainMenuOnStart = false; Time.timeScale = 1;
            if (SettingsRuntime.Instance != null)
                SettingsRuntime.Instance.Service.Commit(SettingsRuntime.Instance.Service.Defaults, out _);
            foreach (var bootstrap in Object.FindObjectsByType<GameBootstrap>(FindObjectsSortMode.None)) Object.Destroy(bootstrap.gameObject);
            yield return null; GameServices.Shutdown();
        }

        internal static IEnumerator Capture(string name)
        {
            if (!Environment.GetCommandLineArgs().Contains("-tilkiE2Screenshots")) yield break;
            yield return null; yield return null;
            Directory.CreateDirectory("Documentation/Sprint55E2/Screenshots");
            ScreenCapture.CaptureScreenshot("Documentation/Sprint55E2/Screenshots/" + name + ".png");
            Debug.Log("E2 screenshot requested: " + name);
            for (int i = 0; i < 8; i++) yield return null;
        }

        [UnityTest] public IEnumerator FreshMainMenuSharedSettingsRuntimeEffectsAndRestartPersistence()
        {
            GameBootstrap.ShowMainMenuOnStart = true;
            yield return SceneManager.LoadSceneAsync(SceneIds.Bootstrap);
            yield return null; yield return null;
            var menu = Find<SettingsMenuUI>(); var runtime = Find<SettingsRuntime>();
            Assert.That(menu.Page, Is.EqualTo("main"));
            yield return Capture("01_main_menu_settings");
            menu.OpenSettings();
            for (int i = 1; i <= 4; i++)
            {
                menu.SelectCategory(i);
                yield return Capture(new[] { "", "02_display_settings", "03_audio_settings", "04_control_settings", "05_accessibility_settings" }[i]);
            }
            menu.SelectCategory(3);
            var rebindButton = menu.GetComponentsInChildren<Button>().First(b => b.name.StartsWith("İleri"));
            rebindButton.onClick.Invoke();
            yield return Capture("06_rebind_prompt");
            menu.Back(); // Cancels capture safely before leaving.
            menu.OpenSettings();
            menu.Draft.master = .8f; menu.Draft.music = .35f; menu.Draft.ambience = .55f; menu.Draft.sfx = .65f;
            menu.Draft.sensitivity = 1.4f; menu.Draft.invertY = true; menu.Draft.minimap = false;
            menu.Draft.objectiveMarkers = false; menu.Draft.uiScale = 1.1f; menu.Draft.dialogueScale = 1.15f;
            menu.Draft.vsync = false; menu.Draft.fps = 120;
            menu.ApplyDraft();
            Assert.That(runtime.Service.Current.sensitivity, Is.EqualTo(1.4f));
            Assert.That(QualitySettings.vSyncCount, Is.Zero); Assert.That(Application.targetFrameRate, Is.EqualTo(120));
            var mixer = Resources.FindObjectsOfTypeAll<AudioMixer>().First(m => m.name == "TilkiAudioMixer");
            foreach (var pair in new[] { ("MasterVolume", .8f), ("MusicVolume", .35f), ("AmbienceVolume", .55f), ("SFXVolume", .65f) })
            {
                Assert.That(mixer.GetFloat(pair.Item1, out float db), Is.True);
                Assert.That(db, Is.EqualTo(SettingsService.Decibels(pair.Item2)).Within(.01));
            }
            // A new bootstrap/service instance reads the committed bytes (same as restart).
            Object.Destroy(runtime.gameObject); yield return null;
            yield return SceneManager.LoadSceneAsync(SceneIds.Bootstrap); yield return null; yield return null;
            runtime = Find<SettingsRuntime>(); menu = Find<SettingsMenuUI>();
            Assert.That(runtime.Service.Current.music, Is.EqualTo(.35f));
            Assert.That(runtime.Service.Current.invertY, Is.True);
            Assert.That(runtime.Service.Current.minimap, Is.False);
            menu.Resume();
            yield return SceneManager.LoadSceneAsync(SceneIds.Forest); yield return null; yield return null;
            var map = Find<QuestMapUI>();
            Assert.That(map.transform.Find("Minimap").gameObject.activeSelf, Is.False);
            menu.ShowPause(); menu.OpenSettings();
            yield return Capture("07_pause_settings");
            Assert.That(Find<GameplayInputLock>().IsLocked, Is.True);
            Assert.That(Find<ThirdPersonCameraController>().IsLookInputLocked, Is.True);
            menu.Draft.minimap = true; menu.Draft.objectiveMarkers = true;
            menu.ApplyDraft(); menu.Back();
            Assert.That(Find<GameplayInputLock>().IsLocked, Is.True, "Settings returns to still-owned Pause");
            menu.Resume(); yield return null;
            Assert.That(Find<GameplayInputLock>().IsLocked, Is.False);
            Assert.That(Find<ThirdPersonCameraController>().IsLookInputLocked, Is.False);
            Assert.That(map.transform.Find("Minimap").gameObject.activeSelf, Is.True);
            map.SetOpen(true); Assert.That(map.IsOpen, Is.True); map.SetOpen(false);
            runtime.Service.Commit(runtime.Service.Defaults, out _);
            Assert.That(runtime.Applied.sensitivity, Is.EqualTo(1));
        }

        [UnityTest] public IEnumerator MixerChannelsRemainIndependentAndMuteIsFinite()
        {
            yield return SceneManager.LoadSceneAsync(SceneIds.Bootstrap);
            yield return null; yield return null;
            var runtime = Find<SettingsRuntime>();
            var mixer = Resources.FindObjectsOfTypeAll<AudioMixer>().First(m => m.name == "TilkiAudioMixer");
            var data = runtime.Service.Defaults;
            runtime.Apply(data);
            data.music = .2f; runtime.Apply(data);
            Assert.That(mixer.GetFloat("MusicVolume", out float music), Is.True);
            Assert.That(music, Is.EqualTo(SettingsService.Decibels(.2f)).Within(.01));
            foreach (string channel in new[] { "MasterVolume", "AmbienceVolume", "SFXVolume" })
            {
                mixer.GetFloat(channel, out float value); Assert.That(value, Is.EqualTo(0).Within(.01));
            }
            data.master = data.ambience = data.music = data.sfx = 0; runtime.Apply(data);
            foreach (string channel in new[] { "MasterVolume", "MusicVolume", "AmbienceVolume", "SFXVolume" })
            {
                mixer.GetFloat(channel, out float value); Assert.That(value, Is.EqualTo(-80).Within(.01));
            }
        }

        [UnityTest] public IEnumerator DisplayTimeoutRestoresActualPreviousModeWithoutSavingOrChangingEditor()
        {
            GameBootstrap.ShowMainMenuOnStart = true;
            yield return SceneManager.LoadSceneAsync(SceneIds.Bootstrap); yield return null; yield return null;
            var menu = Find<SettingsMenuUI>(); var runtime = Find<SettingsRuntime>();
            var display = new FakeDisplay(); menu.Display = display;
            menu.OpenSettings(); int savedWidth = runtime.Service.Current.width;
            menu.Draft.width = savedWidth == 1280 ? 1920 : 1280;
            menu.Draft.windowMode = 2;
            menu.ApplyDraft();
            Assert.That(display.Count, Is.EqualTo(1));
            Assert.That(runtime.Service.Current.width, Is.EqualTo(savedWidth), "unconfirmed mode must not be saved");
            yield return new WaitForSecondsRealtime(12.2f);
            Assert.That(display.Count, Is.EqualTo(2));
            Assert.That(display.Current.width, Is.EqualTo(1600));
            Assert.That(display.Current.height, Is.EqualTo(900));
            Assert.That(runtime.Service.Current.width, Is.EqualTo(savedWidth));
            menu.Back(); menu.Resume();
        }

        [UnityTest] public IEnumerator AllGraphicsPresetsAndBrightnessChangeRuntimeAssetsOnly()
        {
            yield return SceneManager.LoadSceneAsync(SceneIds.Bootstrap); yield return null; yield return null;
            var runtime = Find<SettingsRuntime>();
            var original = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
            float originalScale = original.renderScale, originalShadow = original.shadowDistance;
            var data = runtime.Service.Defaults;
            for (int i = 0; i < 4; i++)
            {
                data.quality = i; runtime.Apply(data);
                var applied = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
                Assert.That(applied, Is.Not.SameAs(original));
                Assert.That(applied.renderScale, Is.EqualTo(new[] { .75f, .85f, 1f, 1f }[i]));
                Assert.That(applied.shadowDistance, Is.EqualTo(new[] { 25f, 45f, 65f, 85f }[i]));
                Assert.That(applied.mainLightShadowmapResolution, Is.EqualTo(new[] { 512, 1024, 2048, 4096 }[i]));
            }
            var volume = runtime.GetComponentInChildren<Volume>();
            Assert.That(volume.sharedProfile.TryGet<ColorAdjustments>(out var color), Is.True);
            data.brightness = 1.2f; runtime.Apply(data);
            Assert.That(color.postExposure.value, Is.EqualTo(Mathf.Log(1.2f, 2)).Within(.001f));
            data.brightness = .8f; runtime.Apply(data);
            Assert.That(color.postExposure.value, Is.EqualTo(Mathf.Log(.8f, 2)).Within(.001f));
            Assert.That(original.renderScale, Is.EqualTo(originalScale));
            Assert.That(original.shadowDistance, Is.EqualTo(originalShadow));
        }

        [UnityTest] public IEnumerator InteractiveRebindCameraPreferencesAndEscapePriorityUseLiveInput()
        {
            var oldEditor = InputSystem.settings.editorInputBehaviorInPlayMode;
            var oldBackground = InputSystem.settings.backgroundBehavior;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            var keyboard = InputSystem.AddDevice<Keyboard>(); var mouse = InputSystem.AddDevice<Mouse>();
            try
            {
                yield return SceneManager.LoadSceneAsync(SceneIds.Bootstrap);
                float deadline = Time.realtimeSinceStartup + 20;
                while (Find<QuestMapUI>() == null && Time.realtimeSinceStartup < deadline) yield return null;
                yield return null; yield return null;
                var menu = Find<SettingsMenuUI>(); var runtime = Find<SettingsRuntime>(); var map = Find<QuestMapUI>();
                menu.ShowPause(); menu.OpenSettings(); menu.SelectCategory(3);
                menu.GetComponentsInChildren<Button>().First(b => b.name.StartsWith("Zıpla")).onClick.Invoke();
                yield return Press(keyboard, Key.J);
                yield return new WaitForSecondsRealtime(.2f);
                Assert.That(menu.Draft.bindingOverrides, Does.Contain("<Keyboard>/j"));
                menu.ApplyDraft(); menu.Back(); menu.Resume();
                Assert.That(runtime.Actions.FindAction("Player/Jump").bindings[0].effectivePath, Is.EqualTo("<Keyboard>/j"));
                Assert.That(runtime.Actions.FindAction("Player/Interact").bindings[0].effectivePath, Is.EqualTo("<Keyboard>/f"));
                yield return Press(keyboard, Key.M); Assert.That(map.IsOpen, Is.True);
                yield return Press(keyboard, Key.Escape);
                Assert.That(map.IsOpen, Is.False); Assert.That(menu.Page, Is.Empty, "one Escape closes only the map");
                yield return Press(keyboard, Key.Escape); Assert.That(menu.Page, Is.EqualTo("pause"));
                yield return Press(keyboard, Key.Escape); Assert.That(menu.Page, Is.Empty);
                var camera = Find<ThirdPersonCameraController>();
                var data = runtime.Service.Current; data.sensitivity = .5f; data.invertY = false; runtime.Apply(data);
                var first = camera.transform.eulerAngles;
                InputSystem.QueueDeltaStateEvent(mouse.delta, new Vector2(100, 20)); yield return null; yield return null;
                var second = camera.transform.eulerAngles;
                float halfYaw = Mathf.DeltaAngle(first.y, second.y), normalPitch = Mathf.DeltaAngle(first.x, second.x);
                data.sensitivity = 1; data.invertY = true; runtime.Apply(data);
                InputSystem.QueueDeltaStateEvent(mouse.delta, new Vector2(100, 20)); yield return null; yield return null;
                var third = camera.transform.eulerAngles;
                Assert.That(Mathf.DeltaAngle(second.y, third.y), Is.EqualTo(halfYaw * 2).Within(.2f));
                Assert.That(normalPitch, Is.LessThan(0)); Assert.That(Mathf.DeltaAngle(second.x, third.x), Is.GreaterThan(0));
                data.objectiveMarkers = true; runtime.Apply(data); yield return null;
                var mini = map.transform.Find("Minimap");
                int shown = mini.GetComponentsInChildren<Image>().Length;
                data.objectiveMarkers = false; runtime.Apply(data); yield return null;
                Assert.That(mini.GetComponentsInChildren<Image>().Length, Is.LessThan(shown));
                runtime.Service.Commit(runtime.Service.Defaults, out _);
                Assert.That(runtime.Actions.FindAction("Player/Jump").bindings[0].effectivePath, Is.EqualTo("<Keyboard>/space"));
            }
            finally
            {
                InputSystem.RemoveDevice(keyboard); InputSystem.RemoveDevice(mouse);
                InputSystem.settings.editorInputBehaviorInPlayMode = oldEditor;
                InputSystem.settings.backgroundBehavior = oldBackground;
            }
        }

        private static IEnumerator Press(Keyboard keyboard, Key key)
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(key)); yield return null; yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
        }
    }
}
