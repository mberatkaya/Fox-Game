using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

namespace TilkiOyunu.Foundation.Tests
{
    public sealed class Sprint55E2SettingsTests
    {
        private string directory, path;
        private SettingsService Create() => new(path, GameSettingsData.Defaults(1920, 1080));
        [SetUp] public void Setup()
        {
            directory = Path.Combine(Path.GetTempPath(), "TilkiSettingsTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory); path = Path.Combine(directory, SettingsService.FileName);
        }
        [TearDown] public void Cleanup() => Directory.Delete(directory, true);

        [Test] public void FirstRunDefaultsPreserveApprovedExperience()
        {
            var data = Create().Current;
            Assert.That(data.version, Is.EqualTo(1)); Assert.That(data.quality, Is.EqualTo(2));
            Assert.That(data.sensitivity, Is.EqualTo(1)); Assert.That(data.brightness, Is.EqualTo(1));
            Assert.That(data.minimap && data.objectiveMarkers && data.vsync, Is.True);
            Assert.That(data.invertY, Is.False); Assert.That(data.fps, Is.EqualTo(60));
            Assert.That(data.master, Is.EqualTo(1)); // Mixer snapshot is 0dB; source levels retain mix.
        }

        [Test] public void DraftIsIsolatedUntilAtomicCommitAndReload()
        {
            var service = Create(); var draft = service.Current; draft.music = .37f;
            Assert.That(service.Current.music, Is.EqualTo(1));
            Assert.That(service.Commit(draft, out _), Is.True);
            draft.music = .8f;
            Assert.That(Create().Current.music, Is.EqualTo(.37f));
            Assert.That(File.Exists(path + ".tmp"), Is.False);
        }

        [Test] public void ResetToExplicitDefaultsPersists()
        {
            var service = Create(); var data = service.Current;
            data.minimap = false; data.sensitivity = 1.7f;
            service.Commit(data, out _); service.Commit(service.Defaults, out _);
            Assert.That(Create().Current.minimap, Is.True); Assert.That(Create().Current.sensitivity, Is.EqualTo(1));
        }

        [TestCase("garbage")]
        [TestCase("{}")]
        [TestCase("{\"version\":999}")]
        public void CorruptOrUnsupportedVersionFallsBack(string contents)
        {
            File.WriteAllText(path, contents);
            Assert.That(Create().Current.width, Is.EqualTo(1920));
            Assert.That(Create().Current.minimap, Is.True);
        }

        [Test] public void NonFiniteAndOutOfRangeValuesAreSafe()
        {
            var d = new GameSettingsData { width = -1, height = int.MaxValue, quality = 99, fps = 13,
                music = float.NaN, master = float.NegativeInfinity, brightness = 400, sensitivity = -20, uiScale = 9 };
            d.Normalize();
            Assert.That(d.width, Is.EqualTo(640)); Assert.That(d.height, Is.EqualTo(8640));
            Assert.That(d.quality, Is.EqualTo(3)); Assert.That(d.fps, Is.EqualTo(60));
            Assert.That(d.music, Is.EqualTo(1)); Assert.That(d.master, Is.EqualTo(1));
            Assert.That(d.brightness, Is.EqualTo(1.2f)); Assert.That(d.sensitivity, Is.EqualTo(.25f));
            Assert.That(d.uiScale, Is.EqualTo(1.2f));
        }

        [TestCase(0, -80)] [TestCase(.1f, -20)] [TestCase(1, 0)]
        public void VolumeMappingIsFinite(float input, float expected) =>
            Assert.That(SettingsService.Decibels(input), Is.EqualTo(expected).Within(.001));

        [TestCase(0, FullScreenMode.ExclusiveFullScreen)]
        [TestCase(1, FullScreenMode.FullScreenWindow)]
        [TestCase(2, FullScreenMode.Windowed)]
        public void DisplayModesAreMappedWithoutChangingEditor(int value, FullScreenMode expected) =>
            Assert.That(SettingsService.DisplayMode(value), Is.EqualTo(expected));

        [Test] public void QuestSaveDeletionDoesNotDeletePreferences()
        {
            var service = Create(); var d = service.Current; d.sfx = .4f; service.Commit(d, out _);
            var save = new SaveService(Path.Combine(directory, SaveService.DefaultFileName));
            save.Save(save.CreateNewSave()); save.DeleteSave();
            Assert.That(Create().Current.sfx, Is.EqualTo(.4f));
        }

        [Test] public void KeyboardOverridesPersistResetAndRejectConflicts()
        {
            var source = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/_Game/Settings/TilkiInputActions.inputactions");
            var asset = UnityEngine.Object.Instantiate(source);
            try
            {
                var jump = asset.FindAction("Player/Jump"); int index = 0;
                Assert.That(SettingsMenuUI.HasConflict(asset, jump, index, "<Keyboard>/w"), Is.True);
                Assert.That(SettingsMenuUI.HasConflict(asset, jump, index, "<Keyboard>/escape"), Is.True);
                Assert.That(SettingsMenuUI.HasConflict(asset, jump, index, "<Keyboard>/j"), Is.False);
                jump.ApplyBindingOverride(index, "<Keyboard>/j");
                var d = Create().Current; d.bindingOverrides = asset.SaveBindingOverridesAsJson(); Create().Commit(d, out _);
                asset.RemoveAllBindingOverrides(); asset.LoadBindingOverridesFromJson(Create().Current.bindingOverrides);
                Assert.That(jump.bindings[index].effectivePath, Is.EqualTo("<Keyboard>/j"));
                asset.RemoveAllBindingOverrides(); Assert.That(jump.bindings[index].effectivePath, Is.EqualTo("<Keyboard>/space"));
                Assert.That(asset.FindAction("Player/WorldMap").bindings[0].effectivePath, Is.EqualTo("<Keyboard>/m"));
            }
            finally { UnityEngine.Object.DestroyImmediate(asset); }
        }
    }
}
