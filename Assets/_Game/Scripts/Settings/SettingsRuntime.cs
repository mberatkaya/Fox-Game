using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TilkiOyunu.Foundation
{
    public sealed class SettingsRuntime : MonoBehaviour
    {
        public static SettingsRuntime Instance { get; private set; }
        public static bool ApplyDisplayOnStartup { get; set; } = true;
        public static string SettingsPathOverride { get; set; }
        public SettingsService Service { get; private set; }
        public InputActionAsset Actions { get; private set; }
        public GameSettingsData Applied { get; private set; }
        private UniversalRenderPipelineAsset originalPipeline, runtimePipeline;
        private RenderPipelineAsset originalQualityPipeline;
        private Volume brightnessVolume;
        private ColorAdjustments brightness;
        private int originalQuality, originalVsync, originalFps;
        private readonly Dictionary<CanvasScaler, Vector2> canvasSizes = new();
        private readonly Dictionary<TMP_Text, float> dialogueSizes = new();
        private string originalBindings;

        private void Awake()
        {
            Instance = this;
            var resolution = Screen.currentResolution;
            Service = new SettingsService(SettingsPathOverride ?? Path.Combine(Application.persistentDataPath, SettingsService.FileName),
                GameSettingsData.Defaults(resolution.width, resolution.height));
            Applied = Service.Current;
            Actions = Resources.Load<SettingsConfiguration>("TilkiSettings")?.inputActions;
            originalBindings = Actions != null ? Actions.SaveBindingOverridesAsJson() : "";
            originalQuality = QualitySettings.GetQualityLevel(); originalVsync = QualitySettings.vSyncCount;
            originalFps = Application.targetFrameRate;
            originalQualityPipeline = QualitySettings.renderPipeline;
            originalPipeline = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
            if (originalPipeline != null)
            {
                runtimePipeline = Instantiate(originalPipeline);
                runtimePipeline.name = "User graphics (runtime only)";
            }
            var volumeObject = new GameObject("User display brightness");
            volumeObject.transform.SetParent(transform);
            brightnessVolume = volumeObject.AddComponent<Volume>();
            brightnessVolume.isGlobal = true; brightnessVolume.priority = 1000;
            brightnessVolume.sharedProfile = ScriptableObject.CreateInstance<VolumeProfile>();
            brightness = brightnessVolume.sharedProfile.Add<ColorAdjustments>();
            Service.Changed += ApplyCurrent;
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void Start()
        {
            ApplyCurrent();
            if (ApplyDisplayOnStartup) ApplyDisplay(Service.Current);
            StartCoroutine(ApplyScene());
        }
        private void SceneLoaded(Scene scene, LoadSceneMode mode) => StartCoroutine(ApplyScene());
        private IEnumerator ApplyScene() { yield return null; ApplyCurrent(); }

        public void ApplyCurrent() => Apply(Service.Current);

        public void Apply(GameSettingsData data)
        {
            Applied = data.Copy(); Applied.Normalize();
            var audio = GameServices.HasCurrent ? GameServices.Current.Audio : null;
            audio?.SetMasterVolume(Applied.master); audio?.SetMusicVolume(Applied.music);
            audio?.SetAmbienceVolume(Applied.ambience); audio?.SetSfxVolume(Applied.sfx);
            // Preserve approved High look; presets change real LOD/shadow/render budgets.
            int[] levels = { 1, 2, 5, 5 };
            QualitySettings.SetQualityLevel(Mathf.Min(levels[Applied.quality], QualitySettings.names.Length - 1), true);
            QualitySettings.vSyncCount = Applied.vsync ? 1 : 0;
            Application.targetFrameRate = Applied.vsync ? -1 : Applied.fps;
            if (runtimePipeline != null)
            {
                QualitySettings.renderPipeline = runtimePipeline;
                runtimePipeline.renderScale = new[] { .75f, .85f, 1f, 1f }[Applied.quality];
                runtimePipeline.shadowDistance = new[] { 25f, 45f, 65f, 85f }[Applied.quality];
                runtimePipeline.mainLightShadowmapResolution = new[] { 512, 1024, 2048, 4096 }[Applied.quality];
                runtimePipeline.msaaSampleCount = Applied.quality == 3 ? 4 : 1;
            }
            brightness.postExposure.Override(Mathf.Log(Applied.brightness, 2));
            if (Actions == null) Actions = FindFirstObjectByType<ThirdPersonCameraController>()?.InputActions;
            if (Actions != null)
            {
                Actions.RemoveAllBindingOverrides();
                if (!string.IsNullOrWhiteSpace(Applied.bindingOverrides))
                {
                    try { Actions.LoadBindingOverridesFromJson(Applied.bindingOverrides); }
                    catch (System.ArgumentException) { Actions.RemoveAllBindingOverrides(); }
                }
            }
            foreach (var scaler in FindObjectsByType<CanvasScaler>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (scaler.GetComponent<Canvas>().renderMode == RenderMode.WorldSpace) continue;
                if (!canvasSizes.ContainsKey(scaler)) canvasSizes[scaler] = scaler.referenceResolution;
                scaler.referenceResolution = canvasSizes[scaler] / Applied.uiScale;
            }
            foreach (var dialogue in FindObjectsByType<DialoguePanelUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            foreach (var text in dialogue.GetComponentsInChildren<TMP_Text>(true))
            {
                if (!dialogueSizes.ContainsKey(text)) dialogueSizes[text] = text.fontSize;
                text.fontSize = dialogueSizes[text] * Applied.dialogueScale;
                text.fontSizeMax = text.fontSize;
                text.fontSizeMin = text.fontSize * .8f;
                text.enableAutoSizing = true;
            }
        }

        public static void ApplyDisplay(GameSettingsData data)
        {
            Screen.SetResolution(data.width, data.height, SettingsService.DisplayMode(data.windowMode), Screen.currentResolution.refreshRateRatio);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
            if (Service != null) Service.Changed -= ApplyCurrent;
            if (Actions != null)
            {
                Actions.RemoveAllBindingOverrides();
                if (!string.IsNullOrEmpty(originalBindings)) Actions.LoadBindingOverridesFromJson(originalBindings);
            }
            QualitySettings.SetQualityLevel(originalQuality, true);
            QualitySettings.renderPipeline = originalQualityPipeline;
            QualitySettings.vSyncCount = originalVsync; Application.targetFrameRate = originalFps;
            if (runtimePipeline != null) Destroy(runtimePipeline);
            if (brightnessVolume != null) Destroy(brightnessVolume.sharedProfile);
            if (Instance == this) Instance = null;
        }
    }
}
