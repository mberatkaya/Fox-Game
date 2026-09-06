using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TilkiOyunu.Foundation.Editor
{
    public static class Sprint2SceneBuilder
    {
        private const string QuestPath = "Assets/_Game/Data/Quests/01_AnilariTopla.asset";
        private const string MemoryPrefabPath = "Assets/_Game/Prefabs/Gameplay/MemoryCollectible.prefab";
        private const string WarmGlowMaterialPath = "Assets/Art/Materials/Warm Glow.mat";

        private static readonly string[] DialoguePaths =
        {
            "Assets/_Game/Data/Dialogues/Dialogue_Guide_Intro.asset",
            "Assets/_Game/Data/Dialogues/Dialogue_Guide_Active.asset",
            "Assets/_Game/Data/Dialogues/Dialogue_Guide_TurnIn.asset",
            "Assets/_Game/Data/Dialogues/Dialogue_Guide_Completed.asset"
        };

        private static readonly string[] MemoryPaths =
        {
            "Assets/_Game/Data/Memories/Memory_01.asset",
            "Assets/_Game/Data/Memories/Memory_02.asset",
            "Assets/_Game/Data/Memories/Memory_03.asset",
            "Assets/_Game/Data/Memories/Memory_04.asset",
            "Assets/_Game/Data/Memories/Memory_05.asset"
        };

        private static readonly Vector3[] MemoryPositions =
        {
            new(1.6f, 0.8f, -6.2f),
            new(-1.35f, 0.95f, -0.8f),
            new(7.35f, 0.75f, -2.7f),
            new(-7.6f, 0.75f, 2.15f),
            new(0.7f, 0.85f, 8.6f)
        };

        [MenuItem("Tilki Oyunu/Sprint 2/Build Quest Dialogue Memories")]
        public static void BuildSprint2Content()
        {
            AssetDatabase.StartAssetEditing();
            try
            {
                EnsureDataDirectories();
                QuestDefinition quest = UpdateQuestAsset();
                DialogueDefinition[] dialogues = UpdateDialogueAssets();
                MemoryDefinition[] memories = UpdateMemoryAssets();
                GameObject memoryPrefab = BuildMemoryPrefab();
                AssignWorldPrefab(memories, memoryPrefab);
                UpdateContentConfig(quest, dialogues, memories);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EnhanceForestScene();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void EnsureDataDirectories()
        {
            EnsureFolder("Assets/_Game/Data", "Dialogues");
            EnsureFolder("Assets/_Game/Data", "Quests");
            EnsureFolder("Assets/_Game/Data", "Memories");
            EnsureFolder("Assets/_Game/Prefabs", "Gameplay");
        }

        private static QuestDefinition UpdateQuestAsset()
        {
            QuestDefinition quest = LoadOrCreate<QuestDefinition>(QuestPath);
            SerializedObject serializedObject = new(quest);
            SetString(serializedObject, "id", QuestService.CollectMemoriesQuestId);
            SetString(serializedObject, "title", "Dağılmış Anılar");
            SetString(serializedObject, "description", "Ormanda saklanan 5 küçük anıyı bul.");
            serializedObject.FindProperty("objectiveType").enumValueIndex = (int)QuestObjectiveType.CollectMemory;
            SetString(serializedObject, "targetId", "memory");
            serializedObject.FindProperty("requiredAmount").intValue = 5;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(quest);
            return quest;
        }

        private static DialogueDefinition[] UpdateDialogueAssets()
        {
            string[][] lines =
            {
                new[]
                {
                    "Buralarda birkaç küçük şey kaybolmuş.",
                    "Belki de kaybolmadılar... sadece bulunmayı bekliyorlar.",
                    "Ormanda beş küçük anı var. Hepsini bulabilir misin?"
                },
                new[]
                {
                    "Anılar bazen yaprakların arasına saklanır.",
                    "Acele etme. Orman sana yol gösterecek."
                },
                new[]
                {
                    "Hepsini bulmuşsun.",
                    "Şimdi yerlerine döndüler. Orman biraz daha hafifledi."
                },
                new[]
                {
                    "Bugün iyi bir iz bıraktın.",
                    "İstersen biraz daha dolaş. Orman seni tanıyor artık."
                }
            };

            string[] ids =
            {
                "guide_intro",
                "guide_active",
                "guide_turn_in",
                "guide_completed"
            };

            DialogueDefinition[] dialogues = new DialogueDefinition[DialoguePaths.Length];
            for (int i = 0; i < DialoguePaths.Length; i++)
            {
                DialogueDefinition dialogue = LoadOrCreate<DialogueDefinition>(DialoguePaths[i]);
                SerializedObject serializedObject = new(dialogue);
                SetString(serializedObject, "id", ids[i]);
                SetString(serializedObject, "speaker", "Rehber");
                SerializedProperty lineProperty = serializedObject.FindProperty("lines");
                lineProperty.arraySize = lines[i].Length;
                for (int lineIndex = 0; lineIndex < lines[i].Length; lineIndex++)
                {
                    lineProperty.GetArrayElementAtIndex(lineIndex).stringValue = lines[i][lineIndex];
                }

                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(dialogue);
                dialogues[i] = dialogue;
            }

            return dialogues;
        }

        private static MemoryDefinition[] UpdateMemoryAssets()
        {
            string[] displayNames =
            {
                "Küçük bir macera",
                "Birlikte gülmek",
                "Sessiz bir an",
                "Yolculuk",
                "Güzel bir gün"
            };

            string[] shortTexts =
            {
                "Küçük bir adım, büyük bir keşif gibi parladı.",
                "Sıcak bir gülüş yaprakların arasından geri geldi.",
                "Sessizlik bile bazen yanında biri varmış gibi hissettirir.",
                "Yol uzundu ama izler kaybolmadı.",
                "Güneş kısa bir an için her şeyi yumuşattı."
            };

            MemoryDefinition[] memories = new MemoryDefinition[MemoryPaths.Length];
            for (int i = 0; i < MemoryPaths.Length; i++)
            {
                MemoryDefinition memory = LoadOrCreate<MemoryDefinition>(MemoryPaths[i]);
                SerializedObject serializedObject = new(memory);
                SetString(serializedObject, "id", $"memory_{i + 1:00}");
                SetString(serializedObject, "displayName", displayNames[i]);
                SetString(serializedObject, "shortText", shortTexts[i]);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(memory);
                memories[i] = memory;
            }

            return memories;
        }

        private static void UpdateContentConfig(QuestDefinition collectQuest, DialogueDefinition[] dialogues, MemoryDefinition[] memories)
        {
            GameContentConfig config = AssetDatabase.LoadAssetAtPath<GameContentConfig>("Assets/_Game/Data/Config/GameContentConfig.asset");
            if (config == null)
            {
                return;
            }

            SerializedObject serializedObject = new(config);
            SerializedProperty questsProperty = serializedObject.FindProperty("quests");
            bool hasQuest = false;
            for (int i = 0; i < questsProperty.arraySize; i++)
            {
                if (questsProperty.GetArrayElementAtIndex(i).objectReferenceValue == collectQuest)
                {
                    hasQuest = true;
                    break;
                }
            }

            if (!hasQuest)
            {
                questsProperty.InsertArrayElementAtIndex(0);
                questsProperty.GetArrayElementAtIndex(0).objectReferenceValue = collectQuest;
            }

            SetObjectArray(serializedObject.FindProperty("memories"), memories);
            SetObjectArray(serializedObject.FindProperty("dialogues"), dialogues);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(config);
        }

        private static GameObject BuildMemoryPrefab()
        {
            Material glow = AssetDatabase.LoadAssetAtPath<Material>(WarmGlowMaterialPath);
            GameObject root = new("MemoryCollectible");
            SphereCollider trigger = root.AddComponent<SphereCollider>();
            trigger.radius = 0.85f;
            trigger.isTrigger = true;

            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.name = "Glowing Memory Token";
            visual.transform.SetParent(root.transform, false);
            visual.transform.localScale = Vector3.one * 0.48f;
            if (glow != null)
            {
                visual.GetComponent<Renderer>().sharedMaterial = glow;
            }

            Object.DestroyImmediate(visual.GetComponent<Collider>());

            Light light = new GameObject("Memory Glow").AddComponent<Light>();
            light.transform.SetParent(root.transform, false);
            light.transform.localPosition = Vector3.up * 0.15f;
            light.color = new Color(1f, 0.78f, 0.36f);
            light.range = 2.8f;
            light.intensity = 1.4f;

            root.AddComponent<MemoryCollectible>();
            PrefabUtility.SaveAsPrefabAsset(root, MemoryPrefabPath);
            Object.DestroyImmediate(root);
            return AssetDatabase.LoadAssetAtPath<GameObject>(MemoryPrefabPath);
        }

        private static void AssignWorldPrefab(MemoryDefinition[] memories, GameObject memoryPrefab)
        {
            if (memoryPrefab == null)
            {
                return;
            }

            foreach (MemoryDefinition memory in memories)
            {
                SerializedObject serializedObject = new(memory);
                serializedObject.FindProperty("worldPrefab").objectReferenceValue = memoryPrefab;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(memory);
            }
        }

        private static void EnhanceForestScene()
        {
            Scene scene = EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            FoxController player = Object.FindFirstObjectByType<FoxController>();
            PlayerInteractor interactor = Object.FindFirstObjectByType<PlayerInteractor>();
            QuestDefinition quest = AssetDatabase.LoadAssetAtPath<QuestDefinition>(QuestPath);
            DialogueDefinition[] dialogues = LoadDialogues();
            MemoryDefinition[] memories = LoadMemories();
            GameObject memoryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(MemoryPrefabPath);

            GameObject uiRoot = EnsureGameObject("Sprint 2 UI Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            ConfigureCanvas(uiRoot);
            GameplayInputLock inputLock = EnsureComponent<GameplayInputLock>(uiRoot);
            SetObject(inputLock, "player", player);
            SetObject(inputLock, "interactor", interactor);

            DialoguePanelUI dialogueUI = BuildDialogueUI(uiRoot.transform, inputLock);
            MemoryFeedbackUI feedbackUI = BuildMemoryFeedbackUI(uiRoot.transform);
            BuildQuestHUD(uiRoot.transform, quest);
            BuildGuideNpc(quest, dialogues, dialogueUI);
            BuildMemoryInstances(memoryPrefab, memories, quest, feedbackUI);

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }

            EditorSceneManager.SaveScene(scene);
        }

        private static DialoguePanelUI BuildDialogueUI(Transform parent, GameplayInputLock inputLock)
        {
            GameObject panel = EnsureChild(parent, "Dialogue Panel", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.18f, 0.05f);
            panelRect.anchorMax = new Vector2(0.82f, 0.26f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.09f, 0.08f, 0.07f, 0.86f);

            TMP_Text speaker = BuildText(panel.transform, "Speaker", new Vector2(0.04f, 0.62f), new Vector2(0.96f, 0.92f), 26f, FontStyles.Bold, TextAlignmentOptions.Left);
            TMP_Text line = BuildText(panel.transform, "Line", new Vector2(0.04f, 0.24f), new Vector2(0.96f, 0.66f), 30f, FontStyles.Normal, TextAlignmentOptions.Left);
            TMP_Text hint = BuildText(panel.transform, "Continue Hint", new Vector2(0.72f, 0.05f), new Vector2(0.96f, 0.22f), 20f, FontStyles.Normal, TextAlignmentOptions.Right);

            DialoguePanelUI dialogueUI = EnsureComponent<DialoguePanelUI>(panel);
            SetObject(dialogueUI, "panel", panel.GetComponent<CanvasGroup>());
            SetObject(dialogueUI, "speakerText", speaker);
            SetObject(dialogueUI, "lineText", line);
            SetObject(dialogueUI, "hintText", hint);
            SetObject(dialogueUI, "inputLock", inputLock);
            return dialogueUI;
        }

        private static MemoryFeedbackUI BuildMemoryFeedbackUI(Transform parent)
        {
            GameObject panel = EnsureChild(parent, "Memory Feedback Panel", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.33f, 0.72f);
            panelRect.anchorMax = new Vector2(0.67f, 0.91f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.12f, 0.1f, 0.07f, 0.82f);

            TMP_Text title = BuildText(panel.transform, "Title", new Vector2(0.06f, 0.54f), new Vector2(0.94f, 0.9f), 28f, FontStyles.Bold, TextAlignmentOptions.Center);
            TMP_Text body = BuildText(panel.transform, "Body", new Vector2(0.08f, 0.12f), new Vector2(0.92f, 0.54f), 22f, FontStyles.Normal, TextAlignmentOptions.Center);

            MemoryFeedbackUI feedbackUI = EnsureComponent<MemoryFeedbackUI>(panel);
            SetObject(feedbackUI, "panel", panel.GetComponent<CanvasGroup>());
            SetObject(feedbackUI, "titleText", title);
            SetObject(feedbackUI, "bodyText", body);
            return feedbackUI;
        }

        private static QuestHUD BuildQuestHUD(Transform parent, QuestDefinition quest)
        {
            GameObject panel = EnsureChild(parent, "Quest HUD Panel", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.02f, 0.76f);
            panelRect.anchorMax = new Vector2(0.31f, 0.94f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.08f, 0.11f, 0.09f, 0.8f);

            TMP_Text title = BuildText(panel.transform, "Title", new Vector2(0.07f, 0.54f), new Vector2(0.93f, 0.88f), 25f, FontStyles.Bold, TextAlignmentOptions.Left);
            TMP_Text objective = BuildText(panel.transform, "Objective", new Vector2(0.07f, 0.17f), new Vector2(0.93f, 0.53f), 22f, FontStyles.Normal, TextAlignmentOptions.Left);

            QuestHUD hud = EnsureComponent<QuestHUD>(panel);
            SetObject(hud, "quest", quest);
            SetObject(hud, "panel", panel.GetComponent<CanvasGroup>());
            SetObject(hud, "titleText", title);
            SetObject(hud, "objectiveText", objective);
            return hud;
        }

        private static void BuildGuideNpc(QuestDefinition quest, DialogueDefinition[] dialogues, DialoguePanelUI dialogueUI)
        {
            GameObject npc = EnsureGameObject("NPC_Guide");
            npc.transform.SetPositionAndRotation(new Vector3(-1.85f, 0f, -7.4f), Quaternion.Euler(0f, 145f, 0f));
            CapsuleCollider collider = EnsureComponent<CapsuleCollider>(npc);
            collider.radius = 0.65f;
            collider.height = 1.75f;
            collider.center = new Vector3(0f, 0.85f, 0f);
            collider.isTrigger = true;

            Material glow = AssetDatabase.LoadAssetAtPath<Material>(WarmGlowMaterialPath);
            GameObject body = EnsurePrimitiveChild(npc.transform, "Guide Body", PrimitiveType.Capsule, new Vector3(0f, 0.85f, 0f), new Vector3(0.6f, 0.85f, 0.6f), glow);
            Object.DestroyImmediate(body.GetComponent<Collider>());
            GameObject head = EnsurePrimitiveChild(npc.transform, "Guide Head", PrimitiveType.Sphere, new Vector3(0f, 1.58f, 0f), new Vector3(0.5f, 0.5f, 0.5f), glow);
            Object.DestroyImmediate(head.GetComponent<Collider>());

            GuideNpc guide = EnsureComponent<GuideNpc>(npc);
            SetObject(guide, "collectMemoriesQuest", quest);
            SetObject(guide, "introDialogue", dialogues[0]);
            SetObject(guide, "activeDialogue", dialogues[1]);
            SetObject(guide, "readyToTurnInDialogue", dialogues[2]);
            SetObject(guide, "completedDialogue", dialogues[3]);
            SetObject(guide, "dialogueUI", dialogueUI);
        }

        private static void BuildMemoryInstances(GameObject prefab, MemoryDefinition[] memories, QuestDefinition quest, MemoryFeedbackUI feedbackUI)
        {
            if (prefab == null)
            {
                return;
            }

            for (int i = 0; i < memories.Length; i++)
            {
                string name = $"MemoryCollectible_{i + 1:00}";
                GameObject instance = GameObject.Find(name);
                if (instance == null)
                {
                    instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    instance.name = name;
                }

                instance.transform.position = MemoryPositions[i];
                MemoryCollectible collectible = EnsureComponent<MemoryCollectible>(instance);
                SetObject(collectible, "memory", memories[i]);
                SetObject(collectible, "quest", quest);
                SetObject(collectible, "feedbackUI", feedbackUI);
            }
        }

        private static TMP_Text BuildText(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, float fontSize, FontStyles style, TextAlignmentOptions alignment)
        {
            GameObject textObject = EnsureChild(parent, name, typeof(RectTransform), typeof(TextMeshProUGUI));
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            TMP_Text text = textObject.GetComponent<TMP_Text>();
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = new Color(1f, 0.94f, 0.82f);
            text.raycastTarget = false;
            return text;
        }

        private static GameObject EnsurePrimitiveChild(Transform parent, string name, PrimitiveType primitiveType, Vector3 localPosition, Vector3 localScale, Material material)
        {
            Transform existing = parent.Find(name);
            GameObject child = existing != null ? existing.gameObject : GameObject.CreatePrimitive(primitiveType);
            child.name = name;
            child.transform.SetParent(parent, false);
            child.transform.localPosition = localPosition;
            child.transform.localScale = localScale;
            if (material != null && child.TryGetComponent(out Renderer renderer))
            {
                renderer.sharedMaterial = material;
            }

            return child;
        }

        private static void ConfigureCanvas(GameObject canvasObject)
        {
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
        }

        private static DialogueDefinition[] LoadDialogues()
        {
            DialogueDefinition[] dialogues = new DialogueDefinition[DialoguePaths.Length];
            for (int i = 0; i < DialoguePaths.Length; i++)
            {
                dialogues[i] = AssetDatabase.LoadAssetAtPath<DialogueDefinition>(DialoguePaths[i]);
            }

            return dialogues;
        }

        private static MemoryDefinition[] LoadMemories()
        {
            MemoryDefinition[] memories = new MemoryDefinition[MemoryPaths.Length];
            for (int i = 0; i < MemoryPaths.Length; i++)
            {
                memories[i] = AssetDatabase.LoadAssetAtPath<MemoryDefinition>(MemoryPaths[i]);
            }

            return memories;
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static GameObject EnsureGameObject(string name, params System.Type[] components)
        {
            GameObject gameObject = GameObject.Find(name);
            if (gameObject == null)
            {
                gameObject = new GameObject(name, components);
            }

            return gameObject;
        }

        private static GameObject EnsureChild(Transform parent, string name, params System.Type[] components)
        {
            Transform child = parent.Find(name);
            if (child != null)
            {
                return child.gameObject;
            }

            GameObject gameObject = new(name, components);
            gameObject.transform.SetParent(parent, false);
            return gameObject;
        }

        private static T EnsureComponent<T>(GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }

        private static void EnsureFolder(string parent, string name)
        {
            string path = $"{parent}/{name}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, name);
            }
        }

        private static void SetString(SerializedObject serializedObject, string propertyName, string value)
        {
            serializedObject.FindProperty(propertyName).stringValue = value;
        }

        private static void SetObject(Object target, string propertyName, Object value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetObjectArray(SerializedProperty property, Object[] values)
        {
            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }
        }
    }
}
