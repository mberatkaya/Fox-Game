using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TilkiOyunu.Foundation;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public static class Sprint0ProjectSetup
{
    private const string PipelineAssetPath = "Assets/_Game/Settings/TilkiURPAsset.asset";
    private const string RendererAssetPath = "Assets/_Game/Settings/TilkiUniversalRenderer.asset";
    private const string AudioMixerPath = "Assets/_Game/Audio/Mixers/TilkiAudioMixer.mixer";
    private const string ContentConfigPath = "Assets/_Game/Data/Config/GameContentConfig.asset";
    private const string FinalMessagePath = "Assets/_Game/Data/Config/FinalMessage.asset";

    [MenuItem("Tools/Tilki Oyunu/Sprint 0/Apply Project Foundation")]
    public static void ApplyProjectFoundation()
    {
        EnsureFolders();
        EnsureInputSystemPackage();
        EnsureRenderPipeline();

        AudioMixer mixer = EnsureAudioMixer();
        FinalMessageDefinition finalMessage = EnsureFinalMessage();
        QuestDefinition[] quests = EnsureQuestDefinitions();
        MemoryDefinition[] memories = EnsureMemoryDefinitions();
        DialogueDefinition[] dialogues = EnsureDialogueDefinitions();
        GameContentConfig contentConfig = EnsureContentConfig(finalMessage, quests, memories, dialogues);

        EnsureBootstrapScene(contentConfig, mixer);
        EnsureForestScene();
        EnsureSystemsTestScene(contentConfig, mixer);
        EnsureBuildSettings();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[BOOT] Sprint 0 project foundation applied.");
    }

    public static void ApplyProjectFoundationBatch()
    {
        ApplyProjectFoundation();
        EditorApplication.Exit(0);
    }

    private static void EnsureFolders()
    {
        string[] folders =
        {
            "Assets/_Game/Art/Characters",
            "Assets/_Game/Art/Environment",
            "Assets/_Game/Art/Props",
            "Assets/_Game/Art/UI",
            "Assets/_Game/Audio/Music",
            "Assets/_Game/Audio/SFX",
            "Assets/_Game/Audio/Mixers",
            "Assets/_Game/Data/Dialogues",
            "Assets/_Game/Data/Quests",
            "Assets/_Game/Data/Memories",
            "Assets/_Game/Data/Config",
            "Assets/_Game/Materials",
            "Assets/_Game/Prefabs/Characters",
            "Assets/_Game/Prefabs/Environment",
            "Assets/_Game/Prefabs/Gameplay",
            "Assets/_Game/Prefabs/UI",
            "Assets/_Game/Scenes/Bootstrap",
            "Assets/_Game/Scenes/Gameplay",
            "Assets/_Game/Scenes/Testing",
            "Assets/_Game/Scripts/Core",
            "Assets/_Game/Scripts/Gameplay",
            "Assets/_Game/Scripts/Character",
            "Assets/_Game/Scripts/Interaction",
            "Assets/_Game/Scripts/Dialogue",
            "Assets/_Game/Scripts/Quests",
            "Assets/_Game/Scripts/Collectibles",
            "Assets/_Game/Scripts/Minigames",
            "Assets/_Game/Scripts/UI",
            "Assets/_Game/Scripts/Audio",
            "Assets/_Game/Scripts/Save",
            "Assets/_Game/Scripts/Data",
            "Assets/_Game/Scripts/Utilities",
            "Assets/_Game/Scripts/Settings",
            "Assets/_Game/Tests/EditMode",
            "Assets/_Game/Tests/PlayMode",
            "ThirdParty",
            "Documentation"
        };

        foreach (string folder in folders)
        {
            Directory.CreateDirectory(folder);
        }
    }

    private static void EnsureInputSystemPackage()
    {
        const string packageName = "com.unity.inputsystem";
        ListRequest listRequest = Client.List(true, false);
        WaitForPackageRequest(listRequest);

        bool installed = listRequest.Status == StatusCode.Success &&
            listRequest.Result.Any(package => package.name == packageName);

        if (installed)
        {
            return;
        }

        AddRequest addRequest = Client.Add(packageName);
        WaitForPackageRequest(addRequest);
        if (addRequest.Status != StatusCode.Success)
        {
            Debug.LogWarning($"[INPUT] Input System package could not be added: {addRequest.Error?.message}");
        }
    }

    private static void WaitForPackageRequest(Request request)
    {
        while (!request.IsCompleted)
        {
            System.Threading.Thread.Sleep(100);
        }
    }

    private static void EnsureRenderPipeline()
    {
        UniversalRenderPipelineAsset pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelineAssetPath);
        if (pipeline == null)
        {
            UniversalRendererData rendererData = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RendererAssetPath);
            if (rendererData == null)
            {
                rendererData = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(rendererData, RendererAssetPath);
            }

            pipeline = UniversalRenderPipelineAsset.Create(rendererData);
            AssetDatabase.CreateAsset(pipeline, PipelineAssetPath);
        }

        GraphicsSettings.defaultRenderPipeline = pipeline;
        QualitySettings.renderPipeline = pipeline;
    }

    private static AudioMixer EnsureAudioMixer()
    {
        AudioMixer mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(AudioMixerPath);
        if (mixer != null)
        {
            return mixer;
        }

        Type controllerType = Type.GetType("UnityEditor.Audio.AudioMixerController, UnityEditor");
        MethodInfo createMethod = controllerType?.GetMethod("CreateMixerControllerAtPath", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (createMethod == null)
        {
            Debug.LogWarning("[AUDIO] Could not find AudioMixerController.CreateMixerControllerAtPath. Audio mixer asset was not created.");
            return null;
        }

        createMethod.Invoke(null, new object[] { AudioMixerPath });
        mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(AudioMixerPath);
        CreateAudioMixerGroups(mixer, controllerType);
        return mixer;
    }

    private static void CreateAudioMixerGroups(AudioMixer mixer, Type controllerType)
    {
        if (mixer == null)
        {
            return;
        }

        UnityEngine.Object controller = mixer as UnityEngine.Object;
        PropertyInfo masterGroupProperty = controllerType.GetProperty("masterGroup", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        object masterGroup = masterGroupProperty?.GetValue(controller);

        foreach (string groupName in new[] { "Music", "SFX" })
        {
            if (mixer.FindMatchingGroups(groupName).Length > 0)
            {
                continue;
            }

            MethodInfo method = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(candidate => candidate.Name == "CreateNewGroup" && candidate.GetParameters().Any(parameter => parameter.ParameterType == typeof(string)));

            if (method == null)
            {
                continue;
            }

            object[] args = method.GetParameters()
                .Select(parameter => BuildMixerGroupArgument(parameter, groupName, masterGroup))
                .ToArray();
            method.Invoke(controller, args);
        }

        EditorUtility.SetDirty(mixer);
    }

    private static object BuildMixerGroupArgument(ParameterInfo parameter, string groupName, object masterGroup)
    {
        if (parameter.ParameterType == typeof(string))
        {
            return groupName;
        }

        if (parameter.ParameterType == typeof(bool))
        {
            return false;
        }

        return masterGroup != null && parameter.ParameterType.IsInstanceOfType(masterGroup)
            ? masterGroup
            : null;
    }

    private static FinalMessageDefinition EnsureFinalMessage()
    {
        FinalMessageDefinition asset = LoadOrCreate<FinalMessageDefinition>(FinalMessagePath);
        SerializedObject serialized = new SerializedObject(asset);
        serialized.FindProperty("title").stringValue = "Final Mesaji";
        serialized.FindProperty("body").stringValue = "Bu kucuk yolculuk, beraber biriktirilecek guzel anilar icin hazirlandi.";
        serialized.FindProperty("signature").stringValue = "Tilki";
        serialized.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(asset);
        return asset;
    }

    private static QuestDefinition[] EnsureQuestDefinitions()
    {
        return new[]
        {
            EnsureQuest("01_AnilariTopla", "collect_memories", "Anilari Topla", "Ormanda saklanan 5 ani objesini bul.", QuestObjectiveType.CollectMemory, "memory", 5),
            EnsureQuest("02_IsikYolu", "complete_light_path", "Isik Yolu", "Isiklari dogru sirayla tamamla.", QuestObjectiveType.CompleteLightPath, "light_path", 1),
            EnsureQuest("03_KalpBahcesi", "complete_card_match", "Kalp Bahcesi", "Kart eslestirme oyununu tamamla.", QuestObjectiveType.CompleteCardMatch, "heart_garden", 1),
            EnsureQuest("04_FinalKampi", "reach_final_camp", "Final Kampi", "Final kampina ulas.", QuestObjectiveType.ReachFinalCamp, "final_camp", 1)
        };
    }

    private static QuestDefinition EnsureQuest(string assetName, string id, string title, string description, QuestObjectiveType type, string targetId, int requiredAmount)
    {
        QuestDefinition asset = LoadOrCreate<QuestDefinition>($"Assets/_Game/Data/Quests/{assetName}.asset");
        SerializedObject serialized = new SerializedObject(asset);
        serialized.FindProperty("id").stringValue = id;
        serialized.FindProperty("title").stringValue = title;
        serialized.FindProperty("description").stringValue = description;
        serialized.FindProperty("objectiveType").enumValueIndex = (int)type;
        serialized.FindProperty("targetId").stringValue = targetId;
        serialized.FindProperty("requiredAmount").intValue = requiredAmount;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(asset);
        return asset;
    }

    private static MemoryDefinition[] EnsureMemoryDefinitions()
    {
        MemoryDefinition[] memories = new MemoryDefinition[5];
        for (int i = 0; i < memories.Length; i++)
        {
            MemoryDefinition asset = LoadOrCreate<MemoryDefinition>($"Assets/_Game/Data/Memories/Memory_{i + 1:00}.asset");
            SerializedObject serialized = new SerializedObject(asset);
            serialized.FindProperty("id").stringValue = $"memory_{i + 1:00}";
            serialized.FindProperty("displayName").stringValue = $"Ani {i + 1}";
            serialized.FindProperty("shortText").stringValue = "Sprint 1'de gercek icerikle degistirilecek sicak ama ozel bilgi icermeyen placeholder metin.";
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            memories[i] = asset;
        }

        return memories;
    }

    private static DialogueDefinition[] EnsureDialogueDefinitions()
    {
        DialogueDefinition intro = LoadOrCreate<DialogueDefinition>("Assets/_Game/Data/Dialogues/IntroDialogue.asset");
        SerializedObject serialized = new SerializedObject(intro);
        serialized.FindProperty("id").stringValue = "intro";
        serialized.FindProperty("speaker").stringValue = "Anlatici";
        SerializedProperty lines = serialized.FindProperty("lines");
        lines.arraySize = 2;
        lines.GetArrayElementAtIndex(0).stringValue = "Orman bugun sessiz ama yolunu biliyor.";
        lines.GetArrayElementAtIndex(1).stringValue = "Kucuk tilki, ilk isigi bulmak icin hazir.";
        serialized.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(intro);
        return new[] { intro };
    }

    private static GameContentConfig EnsureContentConfig(FinalMessageDefinition finalMessage, QuestDefinition[] quests, MemoryDefinition[] memories, DialogueDefinition[] dialogues)
    {
        GameContentConfig config = LoadOrCreate<GameContentConfig>(ContentConfigPath);
        SerializedObject serialized = new SerializedObject(config);
        serialized.FindProperty("finalMessage").objectReferenceValue = finalMessage;
        AssignObjectArray(serialized.FindProperty("quests"), quests);
        AssignObjectArray(serialized.FindProperty("memories"), memories);
        AssignObjectArray(serialized.FindProperty("dialogues"), dialogues);
        serialized.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(config);
        return config;
    }

    private static void AssignObjectArray(SerializedProperty property, UnityEngine.Object[] values)
    {
        property.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++)
        {
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }
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

    private static void EnsureBootstrapScene(GameContentConfig contentConfig, AudioMixer mixer)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = SceneIds.Bootstrap;
        CreateWarmLight("Bootstrap Light", new Vector3(35f, -30f, 0f));
        CreateCamera("Bootstrap Camera", new Vector3(0f, 2.5f, -6f), new Vector3(20f, 0f, 0f));

        GameObject bootstrap = new GameObject("Game Bootstrap");
        GameBootstrap component = bootstrap.AddComponent<GameBootstrap>();
        SerializedObject serialized = new SerializedObject(component);
        serialized.FindProperty("contentConfig").objectReferenceValue = contentConfig;
        serialized.FindProperty("audioMixer").objectReferenceValue = mixer;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.SaveScene(scene, SceneIds.BootstrapPath);
    }

    private static void EnsureForestScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = SceneIds.Forest;
        RenderSettings.ambientLight = new Color(0.48f, 0.52f, 0.44f);
        CreateWarmLight("Forest Sun", new Vector3(48f, -35f, 0f));
        CreateCamera("Main Camera", new Vector3(0f, 4f, -8f), new Vector3(24f, 0f, 0f));

        Material groundMaterial = EnsureMaterial("Assets/_Game/Materials/PlaceholderGround.mat", new Color(0.25f, 0.42f, 0.22f));
        Material playerMaterial = EnsureMaterial("Assets/_Game/Materials/PlaceholderPlayer.mat", new Color(0.93f, 0.38f, 0.12f));

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Placeholder Forest Ground";
        ground.transform.localScale = new Vector3(4f, 1f, 4f);
        ground.GetComponent<Renderer>().sharedMaterial = groundMaterial;

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Placeholder Fox Player";
        player.transform.position = new Vector3(0f, 1f, -4f);
        player.GetComponent<Renderer>().sharedMaterial = playerMaterial;

        EditorSceneManager.SaveScene(scene, SceneIds.ForestPath);
    }

    private static void EnsureSystemsTestScene(GameContentConfig contentConfig, AudioMixer mixer)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = SceneIds.SystemsTest;
        CreateWarmLight("Systems Test Light", new Vector3(35f, -30f, 0f));
        CreateCamera("Systems Test Camera", new Vector3(0f, 3f, -6f), new Vector3(25f, 0f, 0f));
        GameObject bootstrap = new GameObject("Game Bootstrap");
        GameBootstrap component = bootstrap.AddComponent<GameBootstrap>();
        SerializedObject serialized = new SerializedObject(component);
        serialized.FindProperty("contentConfig").objectReferenceValue = contentConfig;
        serialized.FindProperty("audioMixer").objectReferenceValue = mixer;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        EditorSceneManager.SaveScene(scene, SceneIds.SystemsTestPath);
    }

    private static void CreateWarmLight(string name, Vector3 eulerAngles)
    {
        GameObject lightObject = new GameObject(name);
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(1f, 0.88f, 0.72f);
        light.intensity = 1.1f;
        lightObject.transform.rotation = Quaternion.Euler(eulerAngles);
    }

    private static void CreateCamera(string name, Vector3 position, Vector3 eulerAngles)
    {
        GameObject cameraObject = new GameObject(name, typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = position;
        cameraObject.transform.rotation = Quaternion.Euler(eulerAngles);
    }

    private static Material EnsureMaterial(string path, Color color)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            material = new Material(shader);
            AssetDatabase.CreateAsset(material, path);
        }

        material.color = color;
        EditorUtility.SetDirty(material);
        return material;
    }

    private static void EnsureBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>
        {
            new EditorBuildSettingsScene(SceneIds.BootstrapPath, true),
            new EditorBuildSettingsScene(SceneIds.ForestPath, true),
            new EditorBuildSettingsScene(SceneIds.SystemsTestPath, false)
        };

        if (File.Exists("Assets/Scenes/Main.unity"))
        {
            scenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Main.unity", false));
        }

        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
