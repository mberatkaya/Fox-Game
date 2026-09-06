using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace TilkiOyunu.Foundation.Editor
{
    public static class Sprint3SceneBuilder
    {
        private const string CollectQuestPath = "Assets/_Game/Data/Quests/01_AnilariTopla.asset";
        private const string LightPathQuestPath = "Assets/_Game/Data/Quests/Quest_LightPath.asset";
        private const string WarmGlowMaterialPath = "Assets/Art/Materials/Warm Glow.mat";

        private static readonly string[] LightPathDialoguePaths =
        {
            "Assets/_Game/Data/Dialogues/Dialogue_Guide_LightPath_Intro.asset",
            "Assets/_Game/Data/Dialogues/Dialogue_Guide_LightPath_Active.asset",
            "Assets/_Game/Data/Dialogues/Dialogue_Guide_LightPath_TurnIn.asset",
            "Assets/_Game/Data/Dialogues/Dialogue_Guide_LightPath_Completed.asset"
        };

        private static readonly Vector3[] NodePositions =
        {
            new(4.2f, 0.65f, 7.4f),
            new(7.2f, 0.65f, 9.6f),
            new(10.1f, 0.65f, 7.1f),
            new(12.0f, 0.65f, 3.9f),
            new(9.0f, 0.65f, 1.0f)
        };

        [MenuItem("Tilki Oyunu/Sprint 3/Build Light Path Quest")]
        public static void BuildSprint3Content()
        {
            AssetDatabase.StartAssetEditing();
            QuestDefinition lightPathQuest;
            DialogueDefinition[] lightPathDialogues;
            try
            {
                EnsureFolder("Assets/_Game/Data", "Quests");
                EnsureFolder("Assets/_Game/Data", "Dialogues");
                lightPathQuest = UpdateLightPathQuest();
                lightPathDialogues = UpdateLightPathDialogues();
                UpdateContentConfig(lightPathQuest, lightPathDialogues);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EnhanceForestScene(lightPathQuest, lightPathDialogues);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static QuestDefinition UpdateLightPathQuest()
        {
            QuestDefinition quest = LoadOrCreate<QuestDefinition>(LightPathQuestPath);
            SerializedObject serializedObject = new(quest);
            SetString(serializedObject, "id", QuestService.LightPathQuestId);
            SetString(serializedObject, "title", "Işıkların İzinde");
            SetString(serializedObject, "description", "Ormandaki ışıkları doğru sırayla takip et.");
            serializedObject.FindProperty("objectiveType").enumValueIndex = (int)QuestObjectiveType.CompleteLightPath;
            SetString(serializedObject, "targetId", "light_path");
            serializedObject.FindProperty("requiredAmount").intValue = NodePositions.Length;

            SerializedProperty prerequisites = serializedObject.FindProperty("prerequisiteQuestIds");
            prerequisites.arraySize = 1;
            prerequisites.GetArrayElementAtIndex(0).stringValue = QuestService.CollectMemoriesQuestId;

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(quest);
            return quest;
        }

        private static DialogueDefinition[] UpdateLightPathDialogues()
        {
            string[][] lines =
            {
                new[]
                {
                    "Güzel... hepsi yerini buldu.",
                    "Ama ormanın biraz ilerisinde başka bir şey uyandı.",
                    "Işıkları takip et. Sana yolu gösterebilirler."
                },
                new[]
                {
                    "Işıklar sırayla yanıyor.",
                    "Başladığın yolu sonuna kadar takip et."
                },
                new[]
                {
                    "Yolu bulmuşsun.",
                    "Bazen doğru yere varmak için yalnızca küçük bir ışığı takip etmek yeter."
                },
                new[]
                {
                    "Ormanın ışıkları artık sakin."
                }
            };

            string[] ids =
            {
                "guide_light_path_intro",
                "guide_light_path_active",
                "guide_light_path_turn_in",
                "guide_light_path_completed"
            };

            DialogueDefinition[] dialogues = new DialogueDefinition[LightPathDialoguePaths.Length];
            for (int i = 0; i < LightPathDialoguePaths.Length; i++)
            {
                DialogueDefinition dialogue = LoadOrCreate<DialogueDefinition>(LightPathDialoguePaths[i]);
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

        private static void UpdateContentConfig(QuestDefinition lightPathQuest, DialogueDefinition[] lightPathDialogues)
        {
            GameContentConfig config = AssetDatabase.LoadAssetAtPath<GameContentConfig>("Assets/_Game/Data/Config/GameContentConfig.asset");
            if (config == null)
            {
                return;
            }

            SerializedObject serializedObject = new(config);
            AddObjectReference(serializedObject.FindProperty("quests"), lightPathQuest);
            SerializedProperty dialogues = serializedObject.FindProperty("dialogues");
            for (int i = 0; i < lightPathDialogues.Length; i++)
            {
                AddObjectReference(dialogues, lightPathDialogues[i]);
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(config);
        }

        private static DialogueDefinition[] LoadLightPathDialogues()
        {
            DialogueDefinition[] dialogues = new DialogueDefinition[LightPathDialoguePaths.Length];
            for (int i = 0; i < LightPathDialoguePaths.Length; i++)
            {
                dialogues[i] = AssetDatabase.LoadAssetAtPath<DialogueDefinition>(LightPathDialoguePaths[i]);
            }

            return dialogues;
        }

        private static void EnhanceForestScene(QuestDefinition lightPathQuest, DialogueDefinition[] lightPathDialogues)
        {
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            QuestDefinition collectQuest = AssetDatabase.LoadAssetAtPath<QuestDefinition>(CollectQuestPath);
            lightPathQuest = AssetDatabase.LoadAssetAtPath<QuestDefinition>(LightPathQuestPath);
            lightPathDialogues = LoadLightPathDialogues();
            Material glow = AssetDatabase.LoadAssetAtPath<Material>(WarmGlowMaterialPath);

            LightPathController controller = BuildLightPathArea(lightPathQuest, glow);
            GuideNpc guide = Object.FindFirstObjectByType<GuideNpc>();
            if (guide != null)
            {
                SetObject(guide, "lightPathQuest", lightPathQuest);
                SetObject(guide, "lightPathIntroDialogue", lightPathDialogues[0]);
                SetObject(guide, "lightPathActiveDialogue", lightPathDialogues[1]);
                SetObject(guide, "lightPathTurnInDialogue", lightPathDialogues[2]);
                SetObject(guide, "lightPathCompletedDialogue", lightPathDialogues[3]);
            }

            QuestHUD questHUD = Object.FindFirstObjectByType<QuestHUD>();
            if (questHUD != null)
            {
                SetObjectArray(questHUD, "quests", new Object[] { collectQuest, lightPathQuest });
            }

            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                BuildLightPathHUD(canvas.transform, controller);
            }

            EditorSceneManager.SaveScene(scene);
        }

        private static LightPathController BuildLightPathArea(QuestDefinition quest, Material glow)
        {
            GameObject area = EnsureGameObject("Light Path Area");
            LightPathController controller = EnsureComponent<LightPathController>(area);
            SetObject(controller, "quest", quest);
            SetFloat(controller, "durationSeconds", 40f);

            BuildStartObject(area.transform, controller, glow);

            List<LightPathNode> nodes = new();
            for (int i = 0; i < NodePositions.Length; i++)
            {
                nodes.Add(BuildNode(area.transform, i, NodePositions[i], controller, glow));
            }

            SetObjectArray(controller, "nodes", nodes.ToArray());
            return controller;
        }

        private static void BuildStartObject(Transform parent, LightPathController controller, Material glow)
        {
            GameObject start = EnsureChild(parent, "LightPathStart");
            start.transform.position = new Vector3(1.5f, 0.55f, 5.1f);
            SphereCollider collider = EnsureComponent<SphereCollider>(start);
            collider.radius = 1.25f;
            collider.isTrigger = true;

            LightPathStart startComponent = EnsureComponent<LightPathStart>(start);
            SetObject(startComponent, "controller", controller);

            GameObject visual = EnsurePrimitiveChild(start.transform, "Start Glow", PrimitiveType.Cylinder, Vector3.zero, new Vector3(1.2f, 0.1f, 1.2f), glow);
            Object.DestroyImmediate(visual.GetComponent<Collider>());
            Light light = EnsureLightChild(start.transform, "Start Light", new Color(0.95f, 0.85f, 0.48f), 1.5f, 3.2f);
            light.transform.localPosition = Vector3.up * 0.55f;
        }

        private static LightPathNode BuildNode(Transform parent, int sequenceIndex, Vector3 position, LightPathController controller, Material glow)
        {
            GameObject nodeObject = EnsureChild(parent, $"LightPathNode_{sequenceIndex:00}");
            nodeObject.transform.position = position;
            nodeObject.transform.rotation = Quaternion.identity;
            nodeObject.transform.localScale = Vector3.one;

            SphereCollider collider = EnsureComponent<SphereCollider>(nodeObject);
            collider.radius = 1.35f;
            collider.isTrigger = true;

            GameObject visual = EnsurePrimitiveChild(nodeObject.transform, "Node Glow", PrimitiveType.Sphere, Vector3.zero, Vector3.one * 0.55f, glow);
            Object.DestroyImmediate(visual.GetComponent<Collider>());
            Light light = EnsureLightChild(nodeObject.transform, "Node Light", new Color(0.68f, 0.9f, 1f), 0.25f, 3.4f);
            light.transform.localPosition = Vector3.up * 0.25f;

            LightPathNode node = EnsureComponent<LightPathNode>(nodeObject);
            SetInt(node, "sequenceIndex", sequenceIndex);
            SetObject(node, "controller", controller);
            SetObject(node, "visualRoot", visual.transform);
            return node;
        }

        private static LightPathHUD BuildLightPathHUD(Transform parent, LightPathController controller)
        {
            GameObject panel = EnsureChild(parent, "Light Path HUD Panel", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.02f, 0.56f);
            panelRect.anchorMax = new Vector2(0.31f, 0.74f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.07f, 0.1f, 0.12f, 0.78f);

            TMP_Text title = BuildText(panel.transform, "Title", new Vector2(0.07f, 0.68f), new Vector2(0.93f, 0.92f), 24f, FontStyles.Bold, TextAlignmentOptions.Left);
            TMP_Text progress = BuildText(panel.transform, "Progress", new Vector2(0.07f, 0.45f), new Vector2(0.93f, 0.68f), 20f, FontStyles.Normal, TextAlignmentOptions.Left);
            TMP_Text timer = BuildText(panel.transform, "Timer", new Vector2(0.07f, 0.24f), new Vector2(0.93f, 0.45f), 20f, FontStyles.Normal, TextAlignmentOptions.Left);
            TMP_Text feedback = BuildText(panel.transform, "Feedback", new Vector2(0.07f, 0.05f), new Vector2(0.93f, 0.24f), 18f, FontStyles.Normal, TextAlignmentOptions.Left);

            LightPathHUD hud = EnsureComponent<LightPathHUD>(panel);
            SetObject(hud, "controller", controller);
            SetObject(hud, "panel", panel.GetComponent<CanvasGroup>());
            SetObject(hud, "titleText", title);
            SetObject(hud, "progressText", progress);
            SetObject(hud, "timerText", timer);
            SetObject(hud, "feedbackText", feedback);
            return hud;
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
            text.color = new Color(1f, 0.95f, 0.82f);
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

        private static void EnsureFolder(string parent, string name)
        {
            string path = $"{parent}/{name}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, name);
            }
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

        private static void SetObjectArray(Object target, string propertyName, Object[] values)
        {
            SerializedObject serializedObject = new(target);
            SetObjectArray(serializedObject.FindProperty(propertyName), values);
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
