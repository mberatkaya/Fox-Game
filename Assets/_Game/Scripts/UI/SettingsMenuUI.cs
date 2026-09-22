using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TilkiOyunu.Foundation
{
    [DefaultExecutionOrder(-500)]
    public sealed class SettingsMenuUI : MonoBehaviour
    {
        private static int consumedFrame = -1;
        public static bool InputConsumed => consumedFrame == Time.frameCount;
        public static void ConsumeInput() => consumedFrame = Time.frameCount;
        public bool IsOpen => page == "settings";
        public string Page => page;
        public GameSettingsData Draft => draft;
        public ISettingsDisplay Display { get; set; } = new SettingsDisplay();
        private SettingsRuntime runtime;
        private RectTransform root, body;
        private Canvas canvas;
        private TMP_Text notice;
        private string page = "", parentPage = "", status = "";
        private int category;
        private GameSettingsData draft, displayBefore;
        private float confirmDeadline;
        private InputActionAsset bindingDraft;
        private InputActionRebindingExtensions.RebindingOperation rebind;
        private GameplayInputLock inputLock;
        private ThirdPersonCameraController cameraController;
        private bool ownsLock, previousCameraLock, previousCursorVisible;
        private CursorLockMode previousCursorLock;
        private float previousTimeScale;
        private readonly Color cream = new(1, .94f, .78f);
        private readonly Color green = new(.16f, .29f, .20f, .99f);
        private readonly Color gold = new(.76f, .43f, .19f);
        private readonly List<Vector2Int> resolutions = new();

        private void Awake()
        {
            runtime = GetComponent<SettingsRuntime>();
            foreach (var r in Screen.resolutions)
                if (!resolutions.Contains(new Vector2Int(r.width, r.height))) resolutions.Add(new(r.width, r.height));
            var current = new Vector2Int(Screen.width, Screen.height);
            if (!resolutions.Contains(current)) resolutions.Add(current);
            resolutions.Sort((a, b) => a.x == b.x ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));
            BuildShell();
            canvas.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (confirmDeadline > 0)
            {
                if (Time.realtimeSinceStartup >= confirmDeadline) RevertDisplay();
                else if (notice != null) notice.text = $"Bu görüntü ayarları korunsun mu?  {Mathf.CeilToInt(confirmDeadline - Time.realtimeSinceStartup)} sn";
            }
            if (rebind != null) { ConsumeInput(); return; }
            bool escape = Keyboard.current?.escapeKey.wasPressedThisFrame == true
                || runtime.Actions?.FindAction("Player/Pause", false)?.WasPressedThisFrame() == true;
            if (!escape || InputConsumed) return;
            if (page != "")
            {
                ConsumeInput();
                if (confirmDeadline > 0) RevertDisplay();
                else if (page == "settings") Back();
                else if (page == "pause") Resume();
                return;
            }
            if (FindFirstObjectByType<QuestMapUI>()?.IsOpen == true
                || FindFirstObjectByType<CardMatchingPanelUI>()?.IsOpen == true
                || FindFirstObjectByType<DialoguePanelUI>()?.IsOpen == true) return;
            var gameLock = FindFirstObjectByType<GameplayInputLock>();
            if (gameLock != null && !gameLock.IsLocked) { ConsumeInput(); ShowPause(); }
        }

        public void ShowMainMenu() { Acquire(); page = "main"; DrawMenu(); }
        public void ShowPause() { Acquire(); page = "pause"; DrawMenu(); }

        public void OpenSettings()
        {
            Acquire();
            parentPage = page; page = "settings"; category = 0;
            draft = runtime.Service.Current; status = "Değişiklikleri kaydetmek için Uygula’yı seç.";
            ResetBindingDraft(); DrawSettings();
        }

        private void Acquire()
        {
            if (ownsLock) return;
            ownsLock = true;
            inputLock = FindFirstObjectByType<GameplayInputLock>();
            cameraController = FindFirstObjectByType<ThirdPersonCameraController>();
            previousCameraLock = cameraController != null && cameraController.IsLookInputLocked;
            previousCursorLock = Cursor.lockState; previousCursorVisible = Cursor.visible;
            previousTimeScale = Time.timeScale;
            inputLock?.Acquire(); cameraController?.SetLookInputLocked(true);
            Time.timeScale = 0; Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
            EnsureEventSystem();
        }

        private void Release()
        {
            if (!ownsLock) return;
            ownsLock = false;
            inputLock?.Release(); cameraController?.SetLookInputLocked(previousCameraLock);
            Time.timeScale = previousTimeScale;
            Cursor.lockState = previousCursorLock; Cursor.visible = previousCursorVisible;
        }

        public void Resume() { page = ""; canvas.gameObject.SetActive(false); Release(); }

        private void DrawMenu()
        {
            ClearBody(page == "main" ? "TİLKİ OYUNU" : "DURAKLATILDI");
            Text(body, "Ormanın sıcaklığına geri dön.", 23, 0, -44, 800, 45);
            if (page == "main")
            {
                Button(body, "Devam Et", 0, -160, 430, 58, () => StartGame(false), GameServices.Current.Save.HasSave());
                Button(body, "Yeni Oyun", 0, -238, 430, 58, () =>
                {
                    if (GameServices.Current.Save.HasSave()) ConfirmNewGame(); else StartGame(true);
                });
            }
            else Button(body, "Devam Et", 0, -160, 430, 58, Resume);
            Button(body, "Ayarlar", 0, -316, 430, 58, OpenSettings);
            if (page == "pause") Button(body, "Ana Menü", 0, -394, 430, 58, ReturnMainMenu);
            else Button(body, "Çıkış", 0, -394, 430, 58, Application.Quit);
        }

        private void ConfirmNewGame()
        {
            ClearBody("YENİ OYUN");
            Text(body, "Mevcut görev ilerlemesi silinecek. Yeni oyun başlatılsın mı?", 25, 0, -150, 920, 100);
            Button(body, "Yeni Oyun", -220, -320, 350, 60, () => StartGame(true));
            Button(body, "Geri", 220, -320, 350, 60, DrawMenu);
        }

        private void StartGame(bool fresh)
        {
            if (fresh)
            {
                var old = GameServices.Current;
                if (!old.Save.DeleteSave()) { status = "Kayıt silinemedi."; return; }
                GameServices.Initialize(old.Save, old.Scenes, old.Audio, new QuestService(old.Save, old.Owner.ContentConfig));
                GameServices.Current.Owner = old.Owner;
            }
            Resume(); GameServices.Current.Scenes.LoadForest();
        }

        private void ReturnMainMenu()
        {
            Resume();
            SceneManager.sceneLoaded += OnMainSceneLoaded;
            SceneManager.LoadScene(SceneIds.Bootstrap);
        }

        private void OnMainSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnMainSceneLoaded;
            ShowMainMenu();
        }

        private void ResetBindingDraft()
        {
            if (bindingDraft != null) Destroy(bindingDraft);
            if (runtime.Actions == null) return;
            bindingDraft = Instantiate(runtime.Actions); bindingDraft.Disable();
            bindingDraft.RemoveAllBindingOverrides();
            if (!string.IsNullOrEmpty(draft.bindingOverrides))
            {
                try { bindingDraft.LoadBindingOverridesFromJson(draft.bindingOverrides); }
                catch (ArgumentException) { draft.bindingOverrides = ""; }
            }
        }

        public void SelectCategory(int index) { category = Mathf.Clamp(index, 0, 4); DrawSettings(); }

        private void DrawSettings()
        {
            ClearBody("AYARLAR");
            string[] tabs = { "GENEL", "GÖRÜNTÜ", "SES", "KONTROLLER", "ERİŞİLEBİLİRLİK" };
            for (int i = 0; i < tabs.Length; i++)
            {
                int index = i;
                var b = Button(body, tabs[i], -424 + i * 212, -42, 204, 46, () => SelectCategory(index));
                b.GetComponent<Image>().color = category == i ? gold : green;
            }
            switch (category)
            {
                case 0:
                    Text(body, "Dil: Türkçe", 25, 0, -125, 960, 42);
                    ToggleRow("Mini Harita", 1, () => draft.minimap, v => draft.minimap = v);
                    ToggleRow("Görev İşaretçileri", 2, () => draft.objectiveMarkers, v => draft.objectiveMarkers = v);
                    Text(body, "Harita kuzey sabittir. Görev metinleri her zaman görünür.", 21, 0, -370, 960, 70);
                    break;
                case 1:
                    Cycle("Ekran Modu", 0, new[] { "Tam Ekran", "Kenarlıksız Pencere", "Pencere" }, () => draft.windowMode, v => draft.windowMode = v);
                    var resolution = new Vector2Int(draft.width, draft.height);
                    if (!resolutions.Contains(resolution)) resolutions.Add(resolution);
                    Cycle("Çözünürlük", 1, resolutions.Select(r => $"{r.x} x {r.y}").ToArray(), () => resolutions.IndexOf(new(draft.width, draft.height)), v => { draft.width = resolutions[v].x; draft.height = resolutions[v].y; });
                    ToggleRow("Dikey Eşitleme", 2, () => draft.vsync, v => { draft.vsync = v; DrawSettings(); });
                    int[] rates = { 30, 60, 120, 144, -1 };
                    Cycle("Kare Hızı Sınırı", 3, new[] { "30", "60", "120", "144", "Sınırsız" }, () => Array.IndexOf(rates, draft.fps), v => draft.fps = rates[v], !draft.vsync);
                    Cycle("Grafik Kalitesi", 4, new[] { "Düşük", "Orta", "Yüksek", "Çok Yüksek" }, () => draft.quality, v => draft.quality = v);
                    SliderRow("Parlaklık", 5, .8f, 1.2f, draft.brightness, v => draft.brightness = v, true);
                    Text(body, draft.vsync ? "Dikey eşitleme açık: kare hızını ekranın yenileme hızı belirler." : "Kare hızı sınırı etkin.", 19, 0, -520, 1000, 40);
                    break;
                case 2:
                    SliderRow("Ana Ses", 0, 0, 1, draft.master, v => draft.master = v, true);
                    SliderRow("Müzik", 1, 0, 1, draft.music, v => draft.music = v, true);
                    SliderRow("Ortam Sesleri", 2, 0, 1, draft.ambience, v => draft.ambience = v, true);
                    SliderRow("Efektler", 3, 0, 1, draft.sfx, v => draft.sfx = v, true);
                    Text(body, "Ses değişiklikleri Uygula ile etkinleşir.", 21, 0, -435, 960, 60);
                    break;
                case 3:
                    SliderRow("Kamera Hassasiyeti", 0, .25f, 1.75f, draft.sensitivity, v => draft.sensitivity = v, false);
                    ToggleRow("Dikey Bakışı Ters Çevir", 1, () => draft.invertY, v => draft.invertY = v);
                    DrawBindings();
                    break;
                case 4:
                    SliderRow("Arayüz Ölçeği", 0, .8f, 1.2f, draft.uiScale, v => draft.uiScale = v, true);
                    Cycle("İşaretçi Boyutu", 1, new[] { "Küçük", "Normal", "Büyük" }, () => draft.markerScale < 1 ? 0 : draft.markerScale > 1 ? 2 : 1, v => draft.markerScale = new[] { .8f, 1, 1.2f }[v]);
                    Cycle("Diyalog Metin Boyutu", 2, new[] { "Küçük", "Normal", "Büyük" }, () => draft.dialogueScale < 1 ? 0 : draft.dialogueScale > 1 ? 2 : 1, v => draft.dialogueScale = new[] { .85f, 1, 1.15f }[v]);
                    Text(body, "Görev ve diyalog bilgileri her zaman korunur.", 21, 0, -390, 960, 60);
                    break;
            }
            notice = Text(body, status, 19, 0, -579, 1050, 48);
            Button(body, "Varsayılana Döndür", -357, -640, 340, 55, () => { draft = runtime.Service.Defaults; ResetBindingDraft(); status = "Varsayılanlar hazır. Kaydetmek için Uygula."; DrawSettings(); });
            Button(body, "Geri", 15, -640, 260, 55, Back);
            Button(body, "Uygula", 350, -640, 300, 55, ApplyDraft);
        }

        private void DrawBindings()
        {
            if (bindingDraft == null) { status = "Tuş ayarları yüklenemedi."; return; }
            var map = bindingDraft.FindActionMap("Player");
            int row = 0;
            foreach (var action in map.actions)
            for (int i = 0; i < action.bindings.Count; i++)
            {
                var binding = action.bindings[i];
                if (binding.isComposite || !binding.path.StartsWith("<Keyboard>")) continue;
                int slot = i; var captured = action;
                string label = action.name switch { "Move" => binding.name switch { "up" => "İleri", "down" => "Geri", "left" => "Sol", _ => "Sağ" }, "Jump" => "Zıpla", "Sprint" => "Koş", "Interact" => "Etkileşim", "WorldMap" => "Harita", "Pause" => "Duraklat", _ => action.name };
                float x = row % 2 == 0 ? -270 : 270;
                Button(body, label + "   ·   " + action.GetBindingDisplayString(i), x, -252 - row / 2 * 49, 510, 42, () => BeginRebind(captured, slot));
                row++;
            }
            Button(body, "Tuşları Varsayılana Döndür", 270, -497, 510, 42, () => { bindingDraft.RemoveAllBindingOverrides(); draft.bindingOverrides = ""; DrawSettings(); });
        }

        private void BeginRebind(InputAction action, int index)
        {
            status = "Yeni tuşa bas...  (Escape: iptal)";
            notice.text = status;
            // Disable all other controls while capture is active.
            foreach (var selectable in body.GetComponentsInChildren<Selectable>()) selectable.interactable = false;
            string old = action.bindings[index].overridePath;
            rebind = action.PerformInteractiveRebinding(index).WithControlsHavingToMatchPath("<Keyboard>")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnCancel(op => FinishRebind("Tuş ataması iptal edildi."))
                .OnComplete(op =>
                {
                    string path = action.bindings[index].effectivePath;
                    if (HasConflict(bindingDraft, action, index, path))
                    {
                        if (old == null) action.RemoveBindingOverride(index); else action.ApplyBindingOverride(index, old);
                        FinishRebind("Bu tuş başka bir işlemde kullanılıyor. Başka bir tuş seç.");
                    }
                    else { draft.bindingOverrides = bindingDraft.SaveBindingOverridesAsJson(); FinishRebind("Tuş ataması hazır. Kaydetmek için Uygula."); }
                }).Start();
        }

        public static bool HasConflict(InputActionAsset asset, InputAction action, int index, string path)
        {
            if (string.IsNullOrEmpty(path) || !path.StartsWith("<Keyboard>") || path == "<Keyboard>/escape") return true;
            foreach (var other in asset.FindActionMap("Player").actions)
            for (int i = 0; i < other.bindings.Count; i++)
                if ((other != action || i != index) && string.Equals(other.bindings[i].effectivePath, path, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        private void FinishRebind(string message)
        {
            rebind?.Dispose(); rebind = null; ConsumeInput(); status = message; DrawSettings();
        }

        public void ApplyDraft()
        {
            var previous = runtime.Service.Current;
            if (draft.width != previous.width || draft.height != previous.height || draft.windowMode != previous.windowMode)
            {
                displayBefore = Display.Capture();
                Display.Apply(draft); confirmDeadline = Time.realtimeSinceStartup + 12;
                ClearBody("GÖRÜNTÜ AYARLARI");
                notice = Text(body, "Bu görüntü ayarları korunsun mu?", 27, 0, -200, 980, 80);
                Button(body, "Evet", -225, -340, 350, 60, Commit);
                Button(body, "Geri Al", 225, -340, 350, 60, RevertDisplay);
            }
            else Commit();
        }

        private void Commit()
        {
            if (runtime.Service.Commit(draft, out string error))
            { confirmDeadline = 0; displayBefore = null; status = "Ayarlar kaydedildi."; draft = runtime.Service.Current; }
            else { if (displayBefore != null) Display.Apply(displayBefore); confirmDeadline = 0; displayBefore = null; status = error; }
            DrawSettings();
        }

        private void RevertDisplay()
        {
            if (displayBefore != null) Display.Apply(displayBefore);
            var saved = runtime.Service.Current;
            draft.width = saved.width; draft.height = saved.height; draft.windowMode = saved.windowMode;
            confirmDeadline = 0; displayBefore = null; status = "Görüntü değişiklikleri geri alındı."; DrawSettings();
        }

        public void Back()
        {
            if (confirmDeadline > 0) RevertDisplay();
            rebind?.Cancel(); rebind?.Dispose(); rebind = null;
            if (bindingDraft != null) Destroy(bindingDraft);
            draft = null; page = parentPage;
            if (page == "") Resume(); else DrawMenu();
        }

        private void BuildShell()
        {
            var go = new GameObject("Shared Settings Menu", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.transform.SetParent(transform, false);
            canvas = go.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 150;
            var scaler = go.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            scaler.referenceResolution = new(1600, 1000); scaler.matchWidthOrHeight = .5f;
            root = Rect(go.transform, "Warm backdrop", 0, 0, 1600, 1000);
            root.anchorMin = Vector2.zero; root.anchorMax = Vector2.one; root.sizeDelta = Vector2.zero;
            root.gameObject.AddComponent<Image>().color = new(.045f, .09f, .065f, .97f);
            body = Rect(root, "Menu panel", 0, 0, 1150, 700);
            body.gameObject.AddComponent<Image>().color = green;
        }

        private void ClearBody(string title)
        {
            canvas.gameObject.SetActive(true);
            for (int i = body.childCount - 1; i >= 0; i--) { var child = body.GetChild(i).gameObject; child.SetActive(false); Destroy(child); }
            Text(body, title, 36, 0, 42, 1040, 60);
        }

        private static RectTransform Rect(Transform parent, string name, float x, float y, float width, float height)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false); rect.sizeDelta = new(width, height); rect.anchoredPosition = new(x, y);
            return rect;
        }

        private RectTransform Element(Transform parent, string name, float x, float y, float w, float h)
        {
            var rect = Rect(parent, name, x, y, w, h);
            rect.anchorMin = rect.anchorMax = new(.5f, 1); rect.pivot = new(.5f, 1);
            return rect;
        }

        private TMP_Text Text(Transform parent, string value, float size, float x, float y, float w, float h)
        {
            var text = Element(parent, value, x, y, w, h).gameObject.AddComponent<TextMeshProUGUI>();
            text.text = value; text.font = TMP_Settings.defaultFontAsset; text.fontSize = size; text.color = cream;
            text.alignment = TextAlignmentOptions.Center; text.raycastTarget = false;
            text.enableAutoSizing = true; text.fontSizeMin = size * .8f; text.fontSizeMax = size;
            return text;
        }

        private Button Button(Transform parent, string label, float x, float y, float w, float h, Action clicked, bool enabled = true)
        {
            var rect = Element(parent, label, x, y, w, h);
            var image = rect.gameObject.AddComponent<Image>(); image.color = new(.28f, .39f, .25f);
            var button = rect.gameObject.AddComponent<Button>(); button.targetGraphic = image; button.interactable = enabled;
            var colors = button.colors; colors.highlightedColor = new(1, .85f, .62f); colors.selectedColor = colors.highlightedColor; button.colors = colors;
            Text(rect, label, 22, 0, 0, w - 16, h);
            button.onClick.AddListener(() => clicked()); return button;
        }

        private void Label(string label, int row) => Text(body, label, 23, -280, -112 - row * 64, 450, 46).alignment = TextAlignmentOptions.MidlineLeft;
        private void ToggleRow(string label, int row, Func<bool> get, Action<bool> set) =>
            Cycle(label, row, new[] { "Kapalı", "Açık" }, () => get() ? 1 : 0, v => set(v == 1));

        private void Cycle(string label, int row, string[] values, Func<int> get, Action<int> set, bool enabled = true)
        {
            Label(label, row);
            Button b = null;
            b = Button(body, values[Mathf.Clamp(get(), 0, values.Length - 1)] + "  ›", 285, -112 - row * 64, 440, 46, () =>
            {
                set((get() + 1) % values.Length);
                if (b != null) b.GetComponentInChildren<TMP_Text>().text = values[Mathf.Clamp(get(), 0, values.Length - 1)] + "  ›";
            }, enabled);
        }

        private void SliderRow(string label, int row, float min, float max, float value, Action<float> set, bool percent)
        {
            Label(label, row);
            var rect = Element(body, label + " slider", 240, -124 - row * 64, 330, 25);
            var background = rect.gameObject.AddComponent<Image>(); background.color = new(.07f, .13f, .08f);
            var slider = rect.gameObject.AddComponent<Slider>(); slider.minValue = min; slider.maxValue = max;
            var handle = Rect(rect, "Handle", 0, 0, 22, 34); handle.gameObject.AddComponent<Image>().color = cream;
            slider.handleRect = handle; slider.targetGraphic = handle.GetComponent<Image>(); slider.value = value;
            var number = Text(body, "", 22, 470, -112 - row * 64, 75, 46);
            void UpdateValue(float v) { set(v); number.text = percent ? Mathf.RoundToInt(v * 100).ToString() : v.ToString("0.00"); }
            slider.onValueChanged.AddListener(v => UpdateValue(v)); UpdateValue(value);
        }

        private void EnsureEventSystem()
        {
            if (EventSystem.current != null) return;
            var events = new GameObject("Menu EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
        }

        private void OnDisable()
        {
            rebind?.Cancel(); rebind?.Dispose(); rebind = null;
            if (displayBefore != null) Display.Apply(displayBefore);
            confirmDeadline = 0; displayBefore = null; Release();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnMainSceneLoaded;
            if (bindingDraft != null) Destroy(bindingDraft);
        }
    }
}
