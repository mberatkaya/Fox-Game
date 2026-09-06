using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace TilkiOyunu.Foundation.Editor
{
    public static class Sprint4SceneBuilder
    {
        private const string CollectQuestPath = "Assets/_Game/Data/Quests/01_AnilariTopla.asset";
        private const string LightPathQuestPath = "Assets/_Game/Data/Quests/Quest_LightPath.asset";
        private const string LegacyCardQuestPath = "Assets/_Game/Data/Quests/03_KalpBahcesi.asset";
        private const string CardQuestPath = "Assets/_Game/Data/Quests/Quest_CardMatching.asset";
        private const string MossMaterialPath = "Assets/Art/Materials/Moss.mat";
        private const string FlowerPinkMaterialPath = "Assets/Art/Materials/Flower Pink.mat";
        private const string FlowerBlueMaterialPath = "Assets/Art/Materials/Flower Blue.mat";
        private const string WarmGlowMaterialPath = "Assets/Art/Materials/Warm Glow.mat";

        private static readonly string[] CardDialoguePaths =
        {
            "Assets/_Game/Data/Dialogues/Dialogue_Guide_CardMatching_Intro.asset",
            "Assets/_Game/Data/Dialogues/Dialogue_Guide_CardMatching_Active.asset",
            "Assets/_Game/Data/Dialogues/Dialogue_Guide_CardMatching_TurnIn.asset",
            "Assets/_Game/Data/Dialogues/Dialogue_Guide_CardMatching_Completed.asset"
        };

        private static readonly Vector3 GardenPosition = new(-7.3f, 0.52f, 6.4f);

        [MenuItem("Tilki Oyunu/Sprint 4/Build Card Matching Quest")]
        public static void BuildSprint4Content()
        {
            AssetDatabase.StartAssetEditing();
            QuestDefinition cardQuest;
            DialogueDefinition[] cardDialogues;
            try
            {
                EnsureCardQuestAssetPath();
                cardQuest = UpdateCardQuest();
                cardDialogues = UpdateCardDialogues();
                UpdateContentConfig(cardQuest, cardDialogues);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EnhanceForestScene(cardQuest, cardDialogues);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void EnsureCardQuestAssetPath()
        {
            if (AssetDatabase.LoadAssetAtPath<QuestDefinition>(CardQuestPath) != null)
            {
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<QuestDefinition>(LegacyCardQuestPath) != null)
            {
                AssetDatabase.MoveAsset(LegacyCardQuestPath, CardQuestPath);
                return;
            }

            QuestDefinition quest = ScriptableObject.CreateInstance<QuestDefinition>();
            AssetDatabase.CreateAsset(quest, CardQuestPath);
        }

        private static QuestDefinition UpdateCardQuest()
        {
            QuestDefinition quest = LoadOrCreate<QuestDefinition>(CardQuestPath);
            SerializedObject serializedObject = new(quest);
            SetString(serializedObject, "id", QuestService.CardMatchingQuestId);
            SetString(serializedObject, "title", "Kalp Bahçesi");
            SetString(serializedObject, "description", "Bahçedeki kartların eşlerini bul.");
            serializedObject.FindProperty("objectiveType").enumValueIndex = (int)QuestObjectiveType.CompleteCardMatch;
            SetString(serializedObject, "targetId", "heart_garden");
            serializedObject.FindProperty("requiredAmount").intValue = 4;

            SerializedProperty prerequisites = serializedObject.FindProperty("prerequisiteQuestIds");
            prerequisites.arraySize = 1;
            prerequisites.GetArrayElementAtIndex(0).stringValue = QuestService.LightPathQuestId;

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(quest);
            return quest;
        }

        private static DialogueDefinition[] UpdateCardDialogues()
        {
            string[][] lines =
            {
                new[]
                {
                    "Işıkların sonuna kadar gittin.",
                    "Şimdi bahçede seni küçük bir oyun bekliyor.",
                    "Bazı şeyler ancak eşini bulduğunda tamamlanıyor."
                },
                new[]
                {
                    "Bahçedeki kartların eşlerini bul."
                },
                new[]
                {
                    "Hepsini eşleştirdin.",
                    "Demek ki bazı parçalar gerçekten birbirini buluyor."
                },
                new[]
                {
                    "Bahçe şimdilik sakin."
                }
            };

            string[] ids =
            {
                "guide_card_matching_intro",
                "guide_card_matching_active",
                "guide_card_matching_turn_in",
                "guide_card_matching_completed"
            };

            DialogueDefinition[] dialogues = new DialogueDefinition[CardDialoguePaths.Length];
            for (int i = 0; i < CardDialoguePaths.Length; i++)
            {
                DialogueDefinition dialogue = LoadOrCreate<DialogueDefinition>(CardDialoguePaths[i]);
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

        private static void UpdateContentConfig(QuestDefinition cardQuest, DialogueDefinition[] cardDialogues)
        {
            GameContentConfig config = AssetDatabase.LoadAssetAtPath<GameContentConfig>("Assets/_Game/Data/Config/GameContentConfig.asset");
            if (config == null)
            {
                return;
            }

            SerializedObject serializedObject = new(config);
            AddObjectReference(serializedObject.FindProperty("quests"), cardQuest);
            SerializedProperty dialogues = serializedObject.FindProperty("dialogues");
            for (int i = 0; i < cardDialogues.Length; i++)
            {
                AddObjectReference(dialogues, cardDialogues[i]);
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(config);
        }

        private static void EnhanceForestScene(QuestDefinition cardQuest, DialogueDefinition[] cardDialogues)
        {
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            QuestDefinition collectQuest = AssetDatabase.LoadAssetAtPath<QuestDefinition>(CollectQuestPath);
            QuestDefinition lightPathQuest = AssetDatabase.LoadAssetAtPath<QuestDefinition>(LightPathQuestPath);
            cardQuest = AssetDatabase.LoadAssetAtPath<QuestDefinition>(CardQuestPath);
            cardDialogues = LoadCardDialogues();

            Material moss = AssetDatabase.LoadAssetAtPath<Material>(MossMaterialPath);
            Material flowerPink = AssetDatabase.LoadAssetAtPath<Material>(FlowerPinkMaterialPath);
            Material flowerBlue = AssetDatabase.LoadAssetAtPath<Material>(FlowerBlueMaterialPath);
            Material warmGlow = AssetDatabase.LoadAssetAtPath<Material>(WarmGlowMaterialPath);

            CardMatchingController controller = BuildHeartGarden(cardQuest, moss, flowerPink, flowerBlue, warmGlow);
            GuideNpc guide = Object.FindFirstObjectByType<GuideNpc>();
            if (guide != null)
            {
                SetObject(guide, "cardMatchingQuest", cardQuest);
                SetObject(guide, "cardMatchingIntroDialogue", cardDialogues[0]);
                SetObject(guide, "cardMatchingActiveDialogue", cardDialogues[1]);
                SetObject(guide, "cardMatchingTurnInDialogue", cardDialogues[2]);
                SetObject(guide, "cardMatchingCompletedDialogue", cardDialogues[3]);
            }

            QuestHUD questHUD = Object.FindFirstObjectByType<QuestHUD>();
            if (questHUD != null)
            {
                SetObjectArray(questHUD, "quests", new Object[] { collectQuest, lightPathQuest, cardQuest });
            }

            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                BuildCardMatchingPanel(canvas.transform, controller);
            }

            BuildFinalCamp();
            EditorSceneManager.SaveScene(scene);
        }

        private static DialogueDefinition[] LoadCardDialogues()
        {
            DialogueDefinition[] dialogues = new DialogueDefinition[CardDialoguePaths.Length];
            for (int i = 0; i < CardDialoguePaths.Length; i++)
            {
                dialogues[i] = AssetDatabase.LoadAssetAtPath<DialogueDefinition>(CardDialoguePaths[i]);
            }

            return dialogues;
        }

        private static CardMatchingController BuildHeartGarden(QuestDefinition quest, Material moss, Material flowerPink, Material flowerBlue, Material warmGlow)
        {
            GameObject area = EnsureGameObject("Heart Garden");
            area.transform.position = GardenPosition;

            CardMatchingController controller = EnsureComponent<CardMatchingController>(area);
            SetObject(controller, "quest", quest);
            SetFloat(controller, "mismatchDelaySeconds", 0.85f);
            SetBool(controller, "shuffleOnStart", true);
            SetInt(controller, "deterministicSeed", -1);
            SetCardPairs(controller);

            GameObject platform = EnsurePrimitiveChild(area.transform, "Garden Platform", PrimitiveType.Cylinder, Vector3.zero, new Vector3(3.4f, 0.12f, 3.4f), moss);
            Object.DestroyImmediate(platform.GetComponent<Collider>());

            BuildStartObject(area.transform, controller, warmGlow);
            BuildGardenDecoration(area.transform, flowerPink, flowerBlue, warmGlow);
            return controller;
        }

        private static void BuildStartObject(Transform parent, CardMatchingController controller, Material warmGlow)
        {
            GameObject start = EnsureChild(parent, "CardMatchingStart");
            start.transform.localPosition = new Vector3(0f, 0.18f, -0.8f);
            start.transform.localRotation = Quaternion.identity;
            start.transform.localScale = Vector3.one;

            SphereCollider collider = EnsureComponent<SphereCollider>(start);
            collider.radius = 1.35f;
            collider.isTrigger = true;

            CardMatchingStart startComponent = EnsureComponent<CardMatchingStart>(start);
            SetObject(startComponent, "controller", controller);

            GameObject visual = EnsurePrimitiveChild(start.transform, "Start Marker", PrimitiveType.Cube, Vector3.zero, new Vector3(1.1f, 0.16f, 0.8f), warmGlow);
            Object.DestroyImmediate(visual.GetComponent<Collider>());
            EnsureLightChild(start.transform, "Garden Start Light", new Color(1f, 0.62f, 0.44f), 1.1f, 3.2f).transform.localPosition = Vector3.up * 0.55f;
        }

        private static void BuildGardenDecoration(Transform parent, Material flowerPink, Material flowerBlue, Material warmGlow)
        {
            Vector3[] positions =
            {
                new(-1.3f, 0.12f, 1.15f),
                new(1.25f, 0.12f, 1.05f),
                new(-1.4f, 0.12f, -1.25f),
                new(1.45f, 0.12f, -1.1f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                Material material = i % 2 == 0 ? flowerPink : flowerBlue;
                GameObject flower = EnsurePrimitiveChild(parent, $"Garden Flower {i + 1:00}", PrimitiveType.Sphere, positions[i], new Vector3(0.32f, 0.18f, 0.32f), material);
                Object.DestroyImmediate(flower.GetComponent<Collider>());
            }

            Light light = EnsureLightChild(parent, "Heart Garden Warm Light", new Color(1f, 0.58f, 0.42f), 1.4f, 5.2f);
            light.transform.localPosition = new Vector3(0f, 1.5f, 0.15f);

            GameObject center = EnsurePrimitiveChild(parent, "Garden Center Glow", PrimitiveType.Sphere, new Vector3(0f, 0.22f, 0.25f), new Vector3(0.45f, 0.14f, 0.45f), warmGlow);
            Object.DestroyImmediate(center.GetComponent<Collider>());
        }

        private static CardMatchingPanelUI BuildCardMatchingPanel(Transform parent, CardMatchingController controller)
        {
            GameObject panel = EnsureChild(parent, "Card Matching Panel", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.31f, 0.15f);
            panelRect.anchorMax = new Vector2(0.69f, 0.86f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.1f, 0.12f, 0.14f, 0.94f);

            TMP_Text title = BuildText(panel.transform, "Title", new Vector2(0.08f, 0.86f), new Vector2(0.78f, 0.97f), 30f, FontStyles.Bold, TextAlignmentOptions.Left);
            TMP_Text progress = BuildText(panel.transform, "Progress", new Vector2(0.08f, 0.78f), new Vector2(0.92f, 0.86f), 20f, FontStyles.Normal, TextAlignmentOptions.Left);
            TMP_Text success = BuildText(panel.transform, "Success", new Vector2(0.08f, 0.05f), new Vector2(0.74f, 0.14f), 20f, FontStyles.Bold, TextAlignmentOptions.Left);
            Button close = BuildButton(panel.transform, "Close", new Vector2(0.82f, 0.88f), new Vector2(0.94f, 0.96f), "X", 22f);

            CardMatchingCardButton[] cardButtons = new CardMatchingCardButton[8];
            for (int i = 0; i < cardButtons.Length; i++)
            {
                int row = i / 4;
                int column = i % 4;
                Vector2 anchorMin = new(0.08f + column * 0.215f, 0.45f - row * 0.25f);
                Vector2 anchorMax = anchorMin + new Vector2(0.16f, 0.19f);
                Button button = BuildButton(panel.transform, $"Card {i + 1:00}", anchorMin, anchorMax, "?", 34f);
                cardButtons[i] = EnsureComponent<CardMatchingCardButton>(button.gameObject);
            }

            GameplayInputLock inputLock = Object.FindFirstObjectByType<GameplayInputLock>();
            CardMatchingPanelUI ui = EnsureComponent<CardMatchingPanelUI>(panel);
            SetObject(ui, "panel", panel.GetComponent<CanvasGroup>());
            SetObject(ui, "titleText", title);
            SetObject(ui, "progressText", progress);
            SetObject(ui, "successText", success);
            SetObjectArray(ui, "cardButtons", cardButtons);
            SetObject(ui, "closeButton", close);
            SetObject(ui, "inputLock", inputLock);
            SetObject(controller, "panel", ui);
            return ui;
        }

        private static void BuildFinalCamp()
        {
            GameObject camp = GameObject.Find("Camp Placeholder");
            if (camp == null)
            {
                return;
            }

            FinalCampController controller = EnsureComponent<FinalCampController>(camp);
            Renderer[] renderers = camp.GetComponentsInChildren<Renderer>();
            Light[] lights = camp.GetComponentsInChildren<Light>();
            SetObjectArray(controller, "renderers", renderers);
            SetObjectArray(controller, "lights", lights);
        }

        private static Button BuildButton(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, string label, float fontSize)
        {
            GameObject buttonObject = EnsureChild(parent, name, typeof(RectTransform), typeof(Image), typeof(Button));
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            buttonObject.GetComponent<Image>().color = new Color(0.2f, 0.23f, 0.26f);

            TMP_Text text = BuildText(buttonObject.transform, "Label", Vector2.zero, Vector2.one, fontSize, FontStyles.Bold, TextAlignmentOptions.Center);
            text.text = label;
            text.color = Color.white;
            return buttonObject.GetComponent<Button>();
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
            text.color = new Color(1f, 0.95f, 0.84f);
            text.raycastTarget = false;
            return text;
        }

        private static void SetCardPairs(CardMatchingController controller)
        {
            SerializedObject serializedObject = new(controller);
            SerializedProperty pairs = serializedObject.FindProperty("pairs");
            pairs.arraySize = 4;
            string[] ids = { "pair_01", "pair_02", "pair_03", "pair_04" };
            string[] symbols = { "*", "Ay", "Yaprak", "Kalp" };
            Color[] colors =
            {
                new(0.93f, 0.62f, 0.48f),
                new(0.52f, 0.72f, 0.95f),
                new(0.58f, 0.82f, 0.55f),
                new(0.95f, 0.52f, 0.62f)
            };

            for (int i = 0; i < pairs.arraySize; i++)
            {
                SerializedProperty pair = pairs.GetArrayElementAtIndex(i);
                pair.FindPropertyRelative("pairId").stringValue = ids[i];
                pair.FindPropertyRelative("symbol").stringValue = symbols[i];
                pair.FindPropertyRelative("color").colorValue = colors[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(controller);
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

        private static Light EnsureLightChild(Transform parent, string name, Color color, float intensity, float range)
        {
            Transform existing = parent.Find(name);
            Light light = existing != null ? existing.GetComponent<Light>() : null;
            if (light == null)
            {
                light = new GameObject(name).AddComponent<Light>();
            }

            light.transform.SetParent(parent, false);
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            return light;
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

        private static void AddObjectReference(SerializedProperty property, Object value)
        {
            if (value == null)
            {
                return;
            }

            for (int i = 0; i < property.arraySize; i++)
            {
                if (property.GetArrayElementAtIndex(i).objectReferenceValue == value)
                {
                    return;
                }
            }

            property.InsertArrayElementAtIndex(property.arraySize);
            property.GetArrayElementAtIndex(property.arraySize - 1).objectReferenceValue = value;
        }

        private static void SetString(SerializedObject serializedObject, string propertyName, string value)
        {
            serializedObject.FindProperty(propertyName).stringValue = value;
        }

        private static void SetBool(Object target, string propertyName, bool value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetInt(Object target, string propertyName, int value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).intValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetFloat(Object target, string propertyName, float value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).floatValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetObject(Object target, string propertyName, Object value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetObjectArray<T>(Object target, string propertyName, T[] values) where T : Object
        {
            SerializedObject serializedObject = new(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetObjectArray(Object target, string propertyName, Object[] values)
        {
            SerializedObject serializedObject = new(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }
}
