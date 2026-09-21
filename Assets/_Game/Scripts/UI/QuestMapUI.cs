using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TilkiOyunu.Foundation
{
    /// <summary>North-up, terrain-derived map. No second camera, render pass or saved state.</summary>
    public sealed class QuestMapUI : MonoBehaviour
    {
        private readonly List<MarkerView> markers = new();
        private readonly List<Object> ownedAssets = new();
        private GameplayInputLock inputLock;
        private ThirdPersonCameraController cameraController;
        private LightPathController lightPath;
        private RectTransform mini, full;
        private GameObject overlay;
        private InputAction toggleAction, cancelAction;
        private CursorLockMode previousCursorLock;
        private bool previousCursorVisible, previousCameraLock;
        private Rect worldBounds;
        private Texture2D mapTexture;
        private Sprite roundSprite;
        private RawImage miniTerrain;
        private TMP_Text destinationText;
        private Transform playerTransform;
        private Vector2 miniCenter;
        private MapMarker lastDestination;
        private int lastDistance = -1;
        public bool IsOpen { get; private set; }
        public Rect WorldBounds => worldBounds;

        private sealed class MarkerView
        {
            public MapMarker Target;
            public RectTransform Mini, Full;
            public Image MiniIcon, FullIcon;
            public TMP_Text Label;
            public string ShortLabel;
        }

        private void Start()
        {
            inputLock = FindFirstObjectByType<GameplayInputLock>();
            cameraController = FindFirstObjectByType<ThirdPersonCameraController>();
            lightPath = FindFirstObjectByType<LightPathController>();
            if (inputLock == null || cameraController == null) { enabled = false; return; }
            toggleAction = cameraController.InputActions?.FindAction("Player/WorldMap", false)?.Clone();
            cancelAction = cameraController.InputActions?.FindAction("Player/Pause", false)?.Clone();
            toggleAction?.Enable();
            cancelAction?.Enable();
            RegisterTargets();
            BuildUI();
        }

        private void RegisterTargets()
        {
            var player = FindFirstObjectByType<FoxController>();
            playerTransform = player != null ? player.transform : null;
            Add(player, MapMarkerType.Player, "Sen");
            Add(FindFirstObjectByType<GuideNpc>(), MapMarkerType.NPC, "Rehber");
            foreach (var item in FindObjectsByType<MemoryCollectible>(FindObjectsSortMode.None))
                if (item.Memory != null) Add(item, MapMarkerType.QuestObjective, item.Memory.DisplayName, item.Memory.Id);
            Add(FindFirstObjectByType<LightPathStart>(), MapMarkerType.LightPath, "Işık Korusu · F ile başlat");
            foreach (var node in FindObjectsByType<LightPathNode>(FindObjectsSortMode.None))
                Add(node, MapMarkerType.LightPath, $"Işık {node.SequenceIndex + 1}", "", node.SequenceIndex);
            Add(FindFirstObjectByType<CardMatchingStart>(), MapMarkerType.CardQuest, "Kalp Bahçesi · F ile oyna");
            Add(FindFirstObjectByType<FinalCampController>(), MapMarkerType.FinalCamp, "Final Kampı");
            // Fit production anchors and targets, not the empty 512m terrain buffer.
            Vector2 min = new(float.PositiveInfinity, float.PositiveInfinity), max = -min;
            foreach (var marker in markers) Encapsulate(marker.Target.transform.position, ref min, ref max);
            foreach (var anchor in FindObjectsByType<Transform>(FindObjectsSortMode.None))
                if (anchor.name.StartsWith("LM_")) Encapsulate(anchor.position, ref min, ref max);
            foreach (var water in FindObjectsByType<CalmWaterMotion>(FindObjectsSortMode.None))
            {
                var surface = water.GetComponent<Renderer>();
                if (surface == null) continue;
                Encapsulate(surface.bounds.min, ref min, ref max);
                Encapsulate(surface.bounds.max, ref min, ref max);
            }
            float size = Mathf.Max(max.x - min.x, max.y - min.y) + 48f;
            Vector2 center = (min + max) / 2;
            worldBounds = new Rect(center - Vector2.one * size / 2, Vector2.one * size);
            if (playerTransform != null)
                miniCenter = new Vector2(playerTransform.position.x, playerTransform.position.z);
        }

        private static void Encapsulate(Vector3 p, ref Vector2 min, ref Vector2 max)
        {
            var xz = new Vector2(p.x, p.z);
            min = Vector2.Min(min, xz); max = Vector2.Max(max, xz);
        }

        private void Add(Component target, MapMarkerType type, string label, string id = "", int index = -1)
        {
            if (target == null) return;
            var marker = target.GetComponent<MapMarker>() ?? target.gameObject.AddComponent<MapMarker>();
            marker.Configure(type, label, id, index);
            markers.Add(new MarkerView { Target = marker, ShortLabel = label.Split('·')[0].Trim() });
        }

        public Vector2 WorldToMap(Vector3 position)
        {
            return new Vector2((position.x - worldBounds.xMin) / worldBounds.width,
                (position.z - worldBounds.yMin) / worldBounds.height);
        }

        private void BuildUI()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 45;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            roundSprite = MakeIcon(-1);
            mapTexture = BuildTerrainMap();
            mini = MapPanel(transform, "Minimap", 254);
            mini.anchorMin = mini.anchorMax = mini.pivot = Vector2.zero;
            mini.anchoredPosition = new Vector2(26, 28);
            miniTerrain = mini.GetComponentInChildren<RawImage>();
            Text(mini, "M · Harita     K ↑", new Vector2(.5f, 1), new Vector2(0, 18), 20, new Vector2(254, 28));
            destinationText = Text(mini, "", new Vector2(.5f, 0), new Vector2(0, -14), 17, new Vector2(310, 26));

            overlay = new GameObject("World map overlay", typeof(RectTransform), typeof(Image));
            overlay.transform.SetParent(transform, false);
            var shade = overlay.GetComponent<RectTransform>();
            shade.anchorMin = Vector2.zero; shade.anchorMax = Vector2.one; shade.offsetMin = shade.offsetMax = Vector2.zero;
            overlay.GetComponent<Image>().color = new Color(0.035f, 0.065f, 0.04f, 0.94f);
            full = MapPanel(overlay.transform, "World map", 720);
            full.anchorMin = full.anchorMax = full.pivot = new Vector2(0.5f, 0.5f);
            full.anchoredPosition = Vector2.zero;
            Text(full, "ORMANIN HATIRASI", new Vector2(0.5f, 1), new Vector2(0, 64), 30, new Vector2(720, 44));
            Text(full, "Kuzey ↑     M / Esc · Oyuna dön", new Vector2(0.5f, 1), new Vector2(0, 28), 22, new Vector2(720, 32));
            string[] legend = { "Rehber", "Malzeme", "Işık", "Kart", "Final" };
            for (int i = 0; i < legend.Length; i++)
            {
                var icon = Icon(full, MakeIcon(i + 1), 19, out var legendImage);
                icon.anchorMin = icon.anchorMax = new Vector2(.5f, 0);
                icon.anchoredPosition = new Vector2(-315 + i * 140, -34);
                legendImage.color = new Color(1, .93f, .65f);
                Text(full, legend[i], new Vector2(.5f, 0), new Vector2(-265 + i * 140, -34), 20, new Vector2(110, 28));
            }
            Text(full, "Parlak işaret: sıradaki hedefin", new Vector2(0.5f, 0), new Vector2(0, -67), 20, new Vector2(720, 32));
            AddLandmarks();
            foreach (var view in markers)
            {
                int iconType = (int)view.Target.Type;
                if (view.Target.Type == MapMarkerType.QuestObjective && int.TryParse(view.Target.PersistentId.Replace("memory_", ""), out int ingredient))
                    iconType = 10 + ingredient;
                var sprite = MakeIcon(iconType);
                view.Mini = Icon(mini, sprite, 18, out view.MiniIcon);
                view.Full = Icon(full, sprite, 24, out view.FullIcon);
                view.Label = Text(view.Full, view.Target.Label, new Vector2(0.5f, 0), new Vector2(0, -18), 18, new Vector2(260, 28));
                view.Label.gameObject.SetActive(view.Target.Type != MapMarkerType.Player);
            }
            overlay.SetActive(false);
        }

        private RectTransform MapPanel(Transform parent, string name, float size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>(); rect.sizeDelta = Vector2.one * size;
            var frame = go.GetComponent<Image>(); frame.sprite = roundSprite; frame.type = Image.Type.Sliced;
            frame.color = new Color(0.79f, 0.69f, 0.48f);
            var inner = new GameObject("Terrain", typeof(RectTransform), typeof(RawImage));
            inner.transform.SetParent(rect, false);
            var r = inner.GetComponent<RectTransform>(); r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = Vector2.one * 7; r.offsetMax = Vector2.one * -7;
            inner.GetComponent<RawImage>().texture = mapTexture;
            return rect;
        }

        private void AddLandmarks()
        {
            var names = new Dictionary<string, string> { ["LM_SpawnMeadow"] = "Başlangıç", ["LM_NPCGrove"] = "Rehber Korusu",
                ["LM_Lake"] = "Göl", ["LM_LightGrove"] = "Işık Korusu", ["LM_HeartGarden"] = "Kalp Bahçesi", ["LM_FinalHill"] = "Final Tepesi", ["LM_Bridge"] = "Köprü" };
            foreach (var t in FindObjectsByType<Transform>(FindObjectsSortMode.None))
                if (names.TryGetValue(t.name, out string label))
                {
                    var text = Text(full, label, MapAnchor(full, WorldToMap(t.position)), new Vector2(0, 28), 17, new Vector2(180, 26));
                    text.color = new Color(0.88f, 0.88f, 0.70f, 0.75f);
                }
        }

        private static TMP_Text Text(Transform parent, string content, Vector2 anchor, Vector2 offset, int size, Vector2 dimensions)
        {
            var go = new GameObject(content, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            var r = go.GetComponent<RectTransform>(); r.anchorMin = r.anchorMax = anchor; r.anchoredPosition = offset; r.sizeDelta = dimensions;
            var text = go.GetComponent<TextMeshProUGUI>(); text.text = content; text.fontSize = size;
            text.alignment = TextAlignmentOptions.Center; text.color = new Color(1, 0.94f, 0.78f);
            text.textWrappingMode = TextWrappingModes.NoWrap; text.raycastTarget = false;
            return text;
        }

        private static RectTransform Icon(Transform parent, Sprite sprite, float size, out Image image)
        {
            var go = new GameObject("Marker", typeof(RectTransform), typeof(Image)); go.transform.SetParent(parent, false);
            var r = go.GetComponent<RectTransform>(); r.sizeDelta = Vector2.one * size;
            image = go.GetComponent<Image>(); image.sprite = sprite; image.raycastTarget = false;
            var outline = go.AddComponent<Outline>(); outline.effectColor = new Color(0.13f, 0.10f, 0.05f); outline.effectDistance = new Vector2(1.5f, -1.5f);
            return r;
        }

        private Sprite MakeIcon(int type)
        {
            const int n = 40;
            var texture = new Texture2D(n, n, TextureFormat.RGBA32, false) { filterMode = FilterMode.Bilinear };
            for (int y = 0; y < n; y++) for (int x = 0; x < n; x++)
            {
                float a = (x + 0.5f) / n * 2 - 1, b = (y + 0.5f) / n * 2 - 1;
                bool on = type switch
                {
                    -1 => new Vector2(Mathf.Max(0, Mathf.Abs(a) - .72f), Mathf.Max(0, Mathf.Abs(b) - .72f)).magnitude < .27f,
                    0 => b > -.7f && b < .95f && Mathf.Abs(a) < (.95f - b) * .48f && (b > -.25f || Mathf.Abs(a) > -b * .32f),
                    1 => (a * a + (b - .45f) * (b - .45f) < .18f) || (b < .0f && b > -.8f && Mathf.Abs(a) < .62f),
                    2 => Mathf.Abs(a) + Mathf.Abs(b) < .95f,
                    3 => Mathf.Abs(a) * .7f + Mathf.Abs(b) * .7f < .65f && (Mathf.Abs(a) < .25f || Mathf.Abs(b) < .25f || Mathf.Abs(a) + Mathf.Abs(b) < .58f),
                    4 => (a * a + b * b - .5f) * (a * a + b * b - .5f) * (a * a + b * b - .5f) - a * a * b * b * b < 0,
                    11 => Mathf.Abs(a) < .65f && b > -.8f && b < .8f && !(Mathf.Abs(b - .5f) < .06f && Mathf.Abs(a) < .5f),
                    12 => b > -.85f && b < .9f && Mathf.Abs(a) < (b > .35f ? .26f : .59f),
                    13 => a * a / .46f + b * b / .82f < 1,
                    14 => Mathf.Abs(a) < .9f && Mathf.Abs(b) < .46f,
                    15 => (b < .65f && b > -.85f && Mathf.Abs(a) < (.95f + b) * .65f) || (Mathf.Abs(a) < .16f && b > .5f),
                    _ => b > -.75f && Mathf.Abs(a) < (.95f - b) * .48f
                };
                texture.SetPixel(x, y, on ? Color.white : Color.clear);
            }
            texture.Apply(); ownedAssets.Add(texture);
            var sprite = Sprite.Create(texture, new Rect(0, 0, n, n), Vector2.one / 2, 100, 0, SpriteMeshType.FullRect, Vector4.one * 10);
            ownedAssets.Add(sprite); return sprite;
        }

        private Texture2D BuildTerrainMap()
        {
            const int n = 256;
            var texture = new Texture2D(n, n, TextureFormat.RGB24, false);
            Terrain terrain = Terrain.activeTerrain;
            float[,,] alpha = terrain != null ? terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight) : null;
            int pathLayer = -1;
            if (terrain != null)
            {
                var layers = terrain.terrainData.terrainLayers;
                for (int i = 0; i < layers.Length; i++)
                    if (layers[i] != null && layers[i].name.IndexOf("Path", System.StringComparison.OrdinalIgnoreCase) >= 0)
                    { pathLayer = i; break; }
            }
            for (int y = 0; y < n; y++) for (int x = 0; x < n; x++)
            {
                var p = new Vector3(worldBounds.xMin + worldBounds.width * x / (n - 1), 0, worldBounds.yMin + worldBounds.height * y / (n - 1));
                float path = 0, height = 0;
                if (terrain != null)
                {
                    Vector3 local = p - terrain.transform.position;
                    int ax = Mathf.Clamp(Mathf.RoundToInt(local.x / terrain.terrainData.size.x * (alpha.GetLength(1) - 1)), 0, alpha.GetLength(1) - 1);
                    int ay = Mathf.Clamp(Mathf.RoundToInt(local.z / terrain.terrainData.size.z * (alpha.GetLength(0) - 1)), 0, alpha.GetLength(0) - 1);
                    if (pathLayer >= 0 && pathLayer < alpha.GetLength(2)) path = Mathf.Clamp01(alpha[ay, ax, pathLayer] * 1.6f);
                    height = terrain.SampleHeight(p) / terrain.terrainData.size.y;
                }
                Color c = Color.Lerp(new Color(.24f, .36f, .25f), new Color(.60f, .57f, .36f), path);
                c *= .85f + height * .8f;
                texture.SetPixel(x, y, c);
            }
            // Rasterize actual water surfaces once, so the lake and creek agree with the terrain.
            foreach (var water in FindObjectsByType<CalmWaterMotion>(FindObjectsSortMode.None))
            {
                var filter = water.GetComponent<MeshFilter>();
                if (filter == null || filter.sharedMesh == null || !filter.sharedMesh.isReadable) continue;
                var vertices = filter.sharedMesh.vertices; var indices = filter.sharedMesh.triangles;
                for (int i = 0; i < indices.Length; i += 3)
                {
                    Vector2 a = WorldToMap(filter.transform.TransformPoint(vertices[indices[i]])) * (n - 1);
                    Vector2 b = WorldToMap(filter.transform.TransformPoint(vertices[indices[i + 1]])) * (n - 1);
                    Vector2 c = WorldToMap(filter.transform.TransformPoint(vertices[indices[i + 2]])) * (n - 1);
                    int minX = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(a.x, b.x, c.x)), 0, n - 1);
                    int maxX = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(a.x, b.x, c.x)), 0, n - 1);
                    int minY = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(a.y, b.y, c.y)), 0, n - 1);
                    int maxY = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(a.y, b.y, c.y)), 0, n - 1);
                    for (int y = minY; y <= maxY; y++) for (int x = minX; x <= maxX; x++)
                    {
                        var p = new Vector2(x, y);
                        float ab = Cross(b - a, p - a), bc = Cross(c - b, p - b), ca = Cross(a - c, p - c);
                        if ((ab >= 0 && bc >= 0 && ca >= 0) || (ab <= 0 && bc <= 0 && ca <= 0))
                            texture.SetPixel(x, y, new Color(.25f, .46f, .49f));
                    }
                }
            }
            texture.Apply(); ownedAssets.Add(texture); return texture;
        }

        private static float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

        private static Vector2 MapAnchor(RectTransform panel, Vector2 uv)
        {
            float inset = 7f / panel.sizeDelta.x;
            return Vector2.one * inset + uv * (1 - inset * 2);
        }

        private void LateUpdate()
        {
            if (mini == null || !GameServices.HasCurrent) return;
            if (toggleAction != null && toggleAction.WasPressedThisFrame()) SetOpen(!IsOpen);
            else if (IsOpen && cancelAction != null && cancelAction.WasPressedThisFrame()) SetOpen(false);
            var quests = GameServices.Current.Quest;
            Vector3 playerPosition = playerTransform != null ? playerTransform.position : Vector3.zero;
            float span = Mathf.Min(150f, worldBounds.width);
            // Keep the north-up neighborhood stable while the fox moves within it.
            // Scroll only at the inner edge, retaining local objectives and edge guidance.
            float travel = span * .3f;
            miniCenter.x = Mathf.Clamp(miniCenter.x, playerPosition.x - travel, playerPosition.x + travel);
            miniCenter.y = Mathf.Clamp(miniCenter.y, playerPosition.z - travel, playerPosition.z + travel);
            Vector2 center = new(Mathf.Clamp(miniCenter.x, worldBounds.xMin + span / 2, worldBounds.xMax - span / 2),
                Mathf.Clamp(miniCenter.y, worldBounds.yMin + span / 2, worldBounds.yMax - span / 2));
            var nearbyBounds = new Rect(center - Vector2.one * span / 2, Vector2.one * span);
            miniTerrain.uvRect = new Rect((nearbyBounds.xMin - worldBounds.xMin) / worldBounds.width,
                (nearbyBounds.yMin - worldBounds.yMin) / worldBounds.height, span / worldBounds.width, span / worldBounds.height);
            MarkerView nearest = null;
            float nearestDistance = float.PositiveInfinity;
            foreach (var view in markers)
            {
                if (view.Target == null || view.Target.Type == MapMarkerType.Player || !view.Target.IsVisible(quests, false, lightPath)) continue;
                Vector3 delta = view.Target.transform.position - playerPosition; delta.y = 0;
                if (delta.sqrMagnitude < nearestDistance) { nearest = view; nearestDistance = delta.sqrMagnitude; }
            }
            int distance = nearest != null ? Mathf.RoundToInt(Mathf.Sqrt(nearestDistance)) : -1;
            if (lastDestination != nearest?.Target || lastDistance != distance)
            {
                lastDestination = nearest?.Target; lastDistance = distance;
                destinationText.text = nearest != null ? $"{nearest.ShortLabel} · {distance} m" : "";
            }
            foreach (var view in markers)
            {
                if (view.Target == null) continue;
                var position = view.Target.transform.position;
                var localUV = new Vector2((position.x - nearbyBounds.xMin) / span, (position.z - nearbyBounds.yMin) / span);
                bool nearby = localUV.x >= 0 && localUV.x <= 1 && localUV.y >= 0 && localUV.y <= 1;
                view.Mini.gameObject.SetActive(view.Target.IsVisible(quests, false, lightPath)
                    && (nearby || view == nearest || view.Target.Type == MapMarkerType.Player));
                view.Full.gameObject.SetActive(view.Target.IsVisible(quests, true, lightPath));
                var uv = WorldToMap(position);
                uv.x = Mathf.Clamp(uv.x, .035f, .965f); uv.y = Mathf.Clamp(uv.y, .035f, .965f);
                localUV.x = Mathf.Clamp(localUV.x, .055f, .945f); localUV.y = Mathf.Clamp(localUV.y, .055f, .945f);
                view.Mini.anchorMin = view.Mini.anchorMax = MapAnchor(mini, localUV);
                view.Full.anchorMin = view.Full.anchorMax = MapAnchor(full, uv);
                bool highlighted = view.Target.IsHighlighted(quests, lightPath);
                Color color = view.Target.Type == MapMarkerType.Player ? new Color(1, .46f, .16f) : new Color(1, .93f, .65f);
                bool futureLight = view.Target.Type == MapMarkerType.LightPath && view.Target.SequenceIndex >= 0 && !highlighted;
                if (futureLight) color.a = .4f;
                view.Full.sizeDelta = Vector2.one * (futureLight ? 12 : 24);
                view.Label.enabled = !futureLight;
                view.MiniIcon.color = view.FullIcon.color = color;
                float scale = highlighted ? 1.35f + Mathf.Sin(Time.unscaledTime * 3) * .08f : 1;
                view.Mini.localScale = view.Full.localScale = Vector3.one * scale;
                if (view.Target.Type == MapMarkerType.Player)
                    view.Mini.localEulerAngles = view.Full.localEulerAngles = new Vector3(0, 0, -view.Target.transform.eulerAngles.y);
                view.Label.text = highlighted && view.Target.Type == MapMarkerType.NPC ? "NPC’ye geri dön" : view.Target.Label;
                if (highlighted) view.Full.SetAsLastSibling();
            }
        }

        public void SetOpen(bool open)
        {
            if (open == IsOpen || (open && overlay == null)) return;
            if (open)
            {
                // Do not stack map on dialogue, cards or the final cinematic.
                if (inputLock == null || inputLock.IsLocked || cameraController == null || cameraController.IsExternalControlActive) return;
                previousCameraLock = cameraController.IsLookInputLocked;
                previousCursorLock = Cursor.lockState; previousCursorVisible = Cursor.visible;
                inputLock.Acquire(); cameraController.SetLookInputLocked(true);
                Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
            }
            else
            {
                inputLock?.Release(); cameraController?.SetLookInputLocked(previousCameraLock);
                Cursor.lockState = previousCursorLock; Cursor.visible = previousCursorVisible;
            }
            IsOpen = open;
            if (overlay != null) overlay.SetActive(open);
            if (lightPath != null) lightPath.NavigationPaused = open;
        }

        private void OnDisable()
        {
            SetOpen(false); toggleAction?.Disable(); cancelAction?.Disable();
        }

        private void OnEnable() { toggleAction?.Enable(); cancelAction?.Enable(); }
        private void OnDestroy()
        {
            toggleAction?.Dispose(); cancelAction?.Dispose();
            foreach (var asset in ownedAssets) if (asset != null) Destroy(asset);
        }
    }
}
