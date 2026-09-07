using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace TilkiOyunu.Foundation.Editor
{
    public static class Sprint5PolishBuilder
    {
        private const string FoxFbxPath = "Assets/ThirdParty/Quaternius/UltimateAnimatedAnimals/Fox/Fox.fbx";
        private const string PlayerPrefabPath = "Assets/_Game/Prefabs/Characters/PlayerFox.prefab";
        private const string AnimatorPath = "Assets/_Game/Art/Characters/FoxAnimatorController.controller";
        private const string ThemePath = "Assets/_Game/Art/UI/GameUITheme.asset";
        private const string FinalMessagePath = "Assets/_Game/Data/Config/FinalMessage.asset";
        private const string ContentConfigPath = "Assets/_Game/Data/Config/GameContentConfig.asset";
        private const string InputActionsPath = "Assets/_Game/Settings/TilkiInputActions.inputactions";
        private const string MixerPath = "Assets/_Game/Audio/Mixers/TilkiAudioMixer.mixer";

        private static readonly string[] NatureAssetPaths =
        {
            "Assets/ThirdParty/Quaternius/UltimateStylizedNature/BirchTree_1.fbx",
            "Assets/ThirdParty/Quaternius/UltimateStylizedNature/BirchTree_3.fbx",
            "Assets/ThirdParty/Quaternius/UltimateStylizedNature/MapleTree_1.fbx",
            "Assets/ThirdParty/Quaternius/UltimateStylizedNature/NormalTree_1.fbx",
            "Assets/ThirdParty/Quaternius/UltimateStylizedNature/PineTree_1.fbx",
            "Assets/ThirdParty/Quaternius/UltimateStylizedNature/Bush.fbx",
            "Assets/ThirdParty/Quaternius/UltimateStylizedNature/Bush_Flowers.fbx",
            "Assets/ThirdParty/Quaternius/UltimateStylizedNature/Bush_Large.fbx",
            "Assets/ThirdParty/Quaternius/UltimateStylizedNature/Grass_Small.fbx",
            "Assets/ThirdParty/Quaternius/UltimateStylizedNature/Grass_Large.fbx",
            "Assets/ThirdParty/Quaternius/UltimateStylizedNature/Flower_1_Clump.fbx",
            "Assets/ThirdParty/Quaternius/UltimateStylizedNature/Flower_2_Clump.fbx",
            "Assets/ThirdParty/Quaternius/UltimateStylizedNature/Rock_1.fbx",
            "Assets/ThirdParty/Quaternius/UltimateStylizedNature/Rock_2.fbx",
            "Assets/ThirdParty/Quaternius/UltimateStylizedNature/Rock_3.fbx"
        };

        [MenuItem("Tilki Oyunu/Sprint 5/Apply Production Polish")]
        public static void ApplyProductionPolish()
        {
            ImportThirdPartyAssets();
            GameUITheme theme = EnsureTheme();
            AnimatorController animator = EnsureFoxAnimatorController();
            UpdateFinalMessage();
            UpdateNarrativeAssets();
            UpdatePlayerPrefab(animator);
            PolishForestScene(theme);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void ImportThirdPartyAssets()
        {
            AssetDatabase.ImportAsset(FoxFbxPath, ImportAssetOptions.ForceUpdate);
            for (int i = 0; i < NatureAssetPaths.Length; i++)
            {
                AssetDatabase.ImportAsset(NatureAssetPaths[i], ImportAssetOptions.ForceUpdate);
            }

            AssetDatabase.ImportAsset("Assets/ThirdParty/OpenGameArt", ImportAssetOptions.ImportRecursive);
            AssetDatabase.ImportAsset("Assets/ThirdParty/Kenney", ImportAssetOptions.ImportRecursive);
        }

        private static GameUITheme EnsureTheme()
        {
            GameUITheme theme = AssetDatabase.LoadAssetAtPath<GameUITheme>(ThemePath);
            if (theme == null)
            {
                EnsureFolder("Assets/_Game/Art/UI");
                theme = ScriptableObject.CreateInstance<GameUITheme>();
                AssetDatabase.CreateAsset(theme, ThemePath);
            }

            EditorUtility.SetDirty(theme);
            return theme;
        }

        private static AnimatorController EnsureFoxAnimatorController()
        {
            EnsureFolder("Assets/_Game/Art/Characters");
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(AnimatorPath);
            if (controller == null || controller.layers == null || controller.layers.Length == 0)
            {
                if (controller != null)
                {
                    AssetDatabase.DeleteAsset(AnimatorPath);
                }

                controller = AnimatorController.CreateAnimatorControllerAtPath(AnimatorPath);
            }

            controller.parameters = new AnimatorControllerParameter[0];
            AddAnimatorParameter(controller, "Speed", AnimatorControllerParameterType.Float);
            AddAnimatorParameter(controller, "Grounded", AnimatorControllerParameterType.Bool);
            AddAnimatorParameter(controller, "VerticalVelocity", AnimatorControllerParameterType.Float);

            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            ClearStateMachine(stateMachine);

            AnimationClip idle = FindClip("idle") ?? FirstClip();
            AnimationClip walk = FindClip("walk") ?? idle;
            AnimationClip run = FindClip("run", "gallop") ?? walk;
            AnimationClip jump = FindClip("jump") ?? idle;

            AnimatorState idleState = AddState(stateMachine, "Idle", idle, new Vector3(240f, 80f, 0f));
            AnimatorState walkState = AddState(stateMachine, "Walk", walk, new Vector3(240f, 180f, 0f));
            AnimatorState runState = AddState(stateMachine, "Run", run, new Vector3(240f, 280f, 0f));
            AnimatorState jumpState = AddState(stateMachine, "Jump", jump, new Vector3(520f, 180f, 0f));
            stateMachine.defaultState = idleState;

            AddSpeedTransition(idleState, walkState, AnimatorConditionMode.Greater, 0.08f);
            AddSpeedTransition(walkState, idleState, AnimatorConditionMode.Less, 0.06f);
            AddSpeedTransition(walkState, runState, AnimatorConditionMode.Greater, 0.62f);
            AddSpeedTransition(runState, walkState, AnimatorConditionMode.Less, 0.58f);
            AddGroundedTransition(idleState, jumpState);
            AddGroundedTransition(walkState, jumpState);
            AddGroundedTransition(runState, jumpState);
            AnimatorStateTransition jumpReturn = jumpState.AddTransition(idleState);
            jumpReturn.hasExitTime = false;
            jumpReturn.duration = 0.12f;
            jumpReturn.AddCondition(AnimatorConditionMode.If, 0f, "Grounded");

            EditorUtility.SetDirty(controller);
            return controller;
        }

        private static void UpdatePlayerPrefab(AnimatorController animatorController)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
            try
            {
                FoxController controller = root.GetComponent<FoxController>();
                Transform visualRoot = root.transform.Find("VisualRoot") ?? root.transform.Find("Visual");
                if (visualRoot == null)
                {
                    visualRoot = new GameObject("VisualRoot").transform;
                    visualRoot.SetParent(root.transform, false);
                }

                visualRoot.name = "VisualRoot";
                for (int i = visualRoot.childCount - 1; i >= 0; i--)
                {
                    Object.DestroyImmediate(visualRoot.GetChild(i).gameObject);
                }

                GameObject foxVisual = new("QuaterniusFox");
                foxVisual.transform.SetParent(visualRoot, false);
                foxVisual.transform.localPosition = new Vector3(0f, -0.88f, 0f);
                foxVisual.transform.localRotation = Quaternion.identity;
                foxVisual.transform.localScale = Vector3.one * 0.62f;

                GameObject foxAsset = AssetDatabase.LoadAssetAtPath<GameObject>(FoxFbxPath);
                if (foxAsset != null)
                {
                    GameObject model = (GameObject)PrefabUtility.InstantiatePrefab(foxAsset, foxVisual.transform);
                    model.name = "FoxModel";
                    model.transform.localPosition = Vector3.zero;
                    model.transform.localRotation = Quaternion.identity;
                    model.transform.localScale = Vector3.one;
                }

                Animator animator = foxVisual.AddComponent<Animator>();
                animator.runtimeAnimatorController = animatorController;
                animator.applyRootMotion = false;

                FoxAnimationDriver animationDriver = EnsureComponent<FoxAnimationDriver>(foxVisual);
                SetObject(animationDriver, "controller", controller);
                SetObject(animationDriver, "animator", animator);

                AudioSource source = EnsureComponent<AudioSource>(root);
                FootstepAudio footsteps = EnsureComponent<FootstepAudio>(root);
                SetObject(footsteps, "controller", controller);
                SetObject(footsteps, "source", source);
                SetObjectArray(footsteps, "clips", LoadAudioClips("Assets/ThirdParty/OpenGameArt/SFX/Footsteps/leaves01.ogg", "Assets/ThirdParty/OpenGameArt/SFX/Footsteps/leaves02.ogg"));
                SetFloat(footsteps, "volume", 0.24f);

                CharacterController characterController = root.GetComponent<CharacterController>();
                if (characterController != null)
                {
                    characterController.height = 1.45f;
                    characterController.radius = 0.36f;
                    characterController.center = new Vector3(0f, 0.72f, 0f);
                }

                PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void PolishForestScene(GameUITheme theme)
        {
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            GameplayInputLock inputLock = Object.FindFirstObjectByType<GameplayInputLock>();
            Camera camera = Object.FindFirstObjectByType<Camera>();

            EnsureSceneAudio();
            AddNatureVisuals();
            PolishQuestHud(theme);
            PolishDialoguePanel(theme);
            PolishMemoryFeedback(theme);
            PolishCardMatching(theme);
            PolishLightPath();
            WireMemoryAudio();
            WireFinalCamp(canvas, inputLock, camera, theme);

            EditorSceneManager.SaveScene(scene);
        }

        private static void EnsureSceneAudio()
        {
            GameObject audioRoot = EnsureSceneObject("Sprint 5 Scene Audio");
            AudioSource music = EnsureChild(audioRoot.transform, "Music Loop", typeof(AudioSource)).GetComponent<AudioSource>();
            SceneLoopAudio musicLoop = EnsureComponent<SceneLoopAudio>(music.gameObject);
            music.spatialBlend = 0f;
            SetObject(musicLoop, "source", music);
            SetObject(musicLoop, "clip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/OpenGameArt/Music/SunsetWalk.ogg"));
            SetFloat(musicLoop, "volume", 0.18f);

            AudioSource ambience = EnsureChild(audioRoot.transform, "Forest Ambience Loop", typeof(AudioSource)).GetComponent<AudioSource>();
            SceneLoopAudio ambienceLoop = EnsureComponent<SceneLoopAudio>(ambience.gameObject);
            ambience.spatialBlend = 0f;
            SetObject(ambienceLoop, "source", ambience);
            SetObject(ambienceLoop, "clip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/OpenGameArt/Ambience/Forest_Ambience.mp3"));
            SetFloat(ambienceLoop, "volume", 0.12f);
        }

        private static void AddNatureVisuals()
        {
            GameObject parent = EnsureSceneObject("Environment_Visuals");
            for (int i = parent.transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(parent.transform.GetChild(i).gameObject);
            }

            string[] trees =
            {
                NatureAssetPaths[0], NatureAssetPaths[1], NatureAssetPaths[2], NatureAssetPaths[3], NatureAssetPaths[4]
            };
            Vector3[] positions =
            {
                new(-8f, 0f, -5f), new(-5f, 0f, 2.2f), new(-9.5f, 0f, 8f), new(6f, 0f, -4f), new(8.5f, 0f, 3f),
                new(4f, 0f, 9f), new(-2f, 0f, -8f), new(10f, 0f, -10f), new(-12f, 0f, -1.5f), new(1f, 0f, 11f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                PlaceAsset(parent.transform, trees[i % trees.Length], $"Quaternius Tree {i + 1:00}", positions[i], Quaternion.Euler(0f, i * 37f, 0f), Vector3.one * (0.9f + (i % 3) * 0.1f));
            }

            PlaceAsset(parent.transform, NatureAssetPaths[5], "Quaternius Bush Bridge", new Vector3(-2.5f, 0f, 2.4f), Quaternion.Euler(0f, 24f, 0f), Vector3.one);
            PlaceAsset(parent.transform, NatureAssetPaths[6], "Quaternius Bush Flowers Camp", new Vector3(3.2f, 0f, 7.2f), Quaternion.Euler(0f, 90f, 0f), Vector3.one);
            PlaceAsset(parent.transform, NatureAssetPaths[7], "Quaternius Bush Garden", new Vector3(-8.8f, 0f, 5.2f), Quaternion.Euler(0f, 12f, 0f), Vector3.one);
            PlaceAsset(parent.transform, NatureAssetPaths[8], "Quaternius Grass Camp", new Vector3(1.8f, 0f, 6.5f), Quaternion.identity, Vector3.one * 1.2f);
            PlaceAsset(parent.transform, NatureAssetPaths[9], "Quaternius Grass Pond", new Vector3(5.1f, 0f, 1.5f), Quaternion.Euler(0f, 140f, 0f), Vector3.one);
            PlaceAsset(parent.transform, NatureAssetPaths[10], "Quaternius Flowers Memory", new Vector3(-4.8f, 0f, -3.3f), Quaternion.identity, Vector3.one);
            PlaceAsset(parent.transform, NatureAssetPaths[11], "Quaternius Flowers Garden", new Vector3(-7.9f, 0f, 7.8f), Quaternion.Euler(0f, 70f, 0f), Vector3.one);
            PlaceAsset(parent.transform, NatureAssetPaths[12], "Quaternius Rock Path 01", new Vector3(-1.2f, 0f, -5.5f), Quaternion.Euler(0f, 31f, 0f), Vector3.one);
            PlaceAsset(parent.transform, NatureAssetPaths[13], "Quaternius Rock Path 02", new Vector3(5.8f, 0f, 4.6f), Quaternion.Euler(0f, 78f, 0f), Vector3.one * 0.9f);
            PlaceAsset(parent.transform, NatureAssetPaths[14], "Quaternius Rock Camp", new Vector3(2.4f, 0f, 8.5f), Quaternion.Euler(0f, 110f, 0f), Vector3.one);

            Collider[] colliders = parent.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                Object.DestroyImmediate(colliders[i]);
            }
        }

        private static void PolishQuestHud(GameUITheme theme)
        {
            QuestHUD hud = Object.FindFirstObjectByType<QuestHUD>(FindObjectsInactive.Include);
            if (hud == null)
            {
                return;
            }

            Transform transform = hud.transform;
            Image image = transform.GetComponent<Image>();
            if (image != null)
            {
                image.color = theme.ForestDark;
            }

            if (transform is RectTransform rect)
            {
                rect.anchorMin = new Vector2(0.66f, 0.78f);
                rect.anchorMax = new Vector2(0.97f, 0.96f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
            }

            foreach (TMP_Text text in hud.GetComponentsInChildren<TMP_Text>(true))
            {
                text.color = theme.Cream;
                text.fontSize = Mathf.Min(text.fontSize, text.fontStyle.HasFlag(FontStyles.Bold) ? 24f : 18f);
            }
        }

        private static void PolishDialoguePanel(GameUITheme theme)
        {
            DialoguePanelUI panel = Object.FindFirstObjectByType<DialoguePanelUI>(FindObjectsInactive.Include);
            if (panel == null)
            {
                return;
            }

            Image image = panel.GetComponent<Image>();
            if (image != null)
            {
                image.color = theme.ForestDark;
            }

            foreach (TMP_Text text in panel.GetComponentsInChildren<TMP_Text>(true))
            {
                text.color = text.name.Contains("Speaker") ? theme.GoldLight : theme.Cream;
            }
        }

        private static void PolishMemoryFeedback(GameUITheme theme)
        {
            MemoryFeedbackUI feedback = Object.FindFirstObjectByType<MemoryFeedbackUI>(FindObjectsInactive.Include);
            if (feedback == null)
            {
                return;
            }

            Image image = feedback.GetComponent<Image>();
            if (image != null)
            {
                image.color = new Color(theme.Wood.r, theme.Wood.g, theme.Wood.b, 0.9f);
            }

            SetFloat(feedback, "visibleSeconds", 2.1f);
            foreach (TMP_Text text in feedback.GetComponentsInChildren<TMP_Text>(true))
            {
                text.color = theme.Cream;
            }
        }

        private static void PolishCardMatching(GameUITheme theme)
        {
            CardMatchingPanelUI panel = Object.FindFirstObjectByType<CardMatchingPanelUI>(FindObjectsInactive.Include);
            if (panel != null)
            {
                Image image = panel.GetComponent<Image>();
                if (image != null)
                {
                    image.color = theme.ForestDark;
                }

                SetObject(panel, "inputActions", AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>(InputActionsPath));
                foreach (TMP_Text text in panel.GetComponentsInChildren<TMP_Text>(true))
                {
                    text.color = theme.Cream;
                }
            }

            foreach (CardMatchingCardButton card in Object.FindObjectsByType<CardMatchingCardButton>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                SetColor(card, "hiddenColor", theme.Wood);
                SetColor(card, "revealedColor", theme.Cream);
                SetColor(card, "matchedColor", theme.FoxAccent);
            }

            CardMatchingController controller = Object.FindFirstObjectByType<CardMatchingController>(FindObjectsInactive.Include);
            if (controller != null)
            {
                AudioSource source = EnsureComponent<AudioSource>(controller.gameObject);
                CardMatchingAudioFeedback audio = EnsureComponent<CardMatchingAudioFeedback>(controller.gameObject);
                SetObject(audio, "controller", controller);
                SetObject(audio, "source", source);
                SetObject(audio, "openClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Cards/shuffle.wav"));
                SetObject(audio, "revealClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Cards/contact1.wav"));
                SetObject(audio, "matchClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Chimes/bell_ding2.wav"));
                SetObject(audio, "completeClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Chimes/bell_ding3.wav"));
            }
        }

        private static void PolishLightPath()
        {
            LightPathController controller = Object.FindFirstObjectByType<LightPathController>(FindObjectsInactive.Include);
            if (controller == null)
            {
                return;
            }

            AudioSource source = EnsureComponent<AudioSource>(controller.gameObject);
            LightPathAudioFeedback audio = EnsureComponent<LightPathAudioFeedback>(controller.gameObject);
            SetObject(audio, "controller", controller);
            SetObject(audio, "source", source);
            SetObject(audio, "activateClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Chimes/bell_ding1.wav"));
            SetObject(audio, "completedClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Chimes/bell_ding3.wav"));
            SetObject(audio, "failedClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/Kenney/InterfaceSounds/close_001.ogg"));

            foreach (LightPathNode node in Object.FindObjectsByType<LightPathNode>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                SetColor(node, "inactiveColor", new Color(0.26f, 0.29f, 0.21f));
                SetColor(node, "availableColor", new Color(1f, 0.78f, 0.28f));
                SetColor(node, "completedColor", new Color(1f, 0.55f, 0.22f));
            }
        }

        private static void WireMemoryAudio()
        {
            AudioClip pickup = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Chimes/bell_ding1.wav");
            foreach (MemoryCollectible collectible in Object.FindObjectsByType<MemoryCollectible>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                AudioSource source = EnsureComponent<AudioSource>(collectible.gameObject);
                MemoryAudioFeedback audio = EnsureComponent<MemoryAudioFeedback>(collectible.gameObject);
                SetObject(audio, "source", source);
                SetObject(audio, "pickupClip", pickup);
                SetObject(collectible, "audioFeedback", audio);
            }
        }

        private static void WireFinalCamp(Canvas canvas, GameplayInputLock inputLock, Camera camera, GameUITheme theme)
        {
            GameObject camp = GameObject.Find("Camp Placeholder");
            if (camp == null)
            {
                return;
            }

            FinalMessagePanelUI panel = EnsureFinalPanel(canvas, theme);
            FinalSequenceController sequence = EnsureComponent<FinalSequenceController>(camp);
            AudioSource source = EnsureComponent<AudioSource>(camp);
            source.spatialBlend = 0.65f;
            Transform focus = camp.transform.Find("Final Camera Focus") ?? new GameObject("Final Camera Focus").transform;
            focus.SetParent(camp.transform, false);
            focus.localPosition = new Vector3(0f, 1.1f, 0f);
            Light campLight = camp.GetComponentInChildren<Light>(true);

            SetObject(sequence, "finalMessage", AssetDatabase.LoadAssetAtPath<FinalMessageDefinition>(FinalMessagePath));
            SetObject(sequence, "finalPanel", panel);
            SetObject(sequence, "inputLock", inputLock);
            SetObject(sequence, "gameplayCamera", camera);
            SetObject(sequence, "cameraFocus", focus);
            SetObject(sequence, "campLight", campLight);
            SetObject(sequence, "audioSource", source);
            SetObject(sequence, "completionClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Chimes/bell_ding3.wav"));

            FinalCampController finalCamp = EnsureComponent<FinalCampController>(camp);
            SetObject(finalCamp, "finalSequence", sequence);
            SetString(finalCamp, "lockedInteractionLabel", "Henüz zamanı değil");
            SetString(finalCamp, "unlockedInteractionLabel", "Son anıyı aç");

            AudioSource campfire = EnsureChild(camp.transform, "Campfire Loop", typeof(AudioSource)).GetComponent<AudioSource>();
            campfire.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Campfire/fire.wav");
            campfire.loop = true;
            campfire.playOnAwake = true;
            campfire.spatialBlend = 1f;
            campfire.volume = 0.18f;
            campfire.minDistance = 1.5f;
            campfire.maxDistance = 9f;
        }

        private static FinalMessagePanelUI EnsureFinalPanel(Canvas canvas, GameUITheme theme)
        {
            if (canvas == null)
            {
                return null;
            }

            GameObject panelObject = EnsureChild(canvas.transform, "Final Message Panel", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            RectTransform rect = panelObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.28f, 0.3f);
            rect.anchorMax = new Vector2(0.72f, 0.74f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            panelObject.GetComponent<Image>().color = new Color(theme.ForestDark.r, theme.ForestDark.g, theme.ForestDark.b, 0.92f);

            TMP_Text title = EnsureText(panelObject.transform, "Title", new Vector2(0.09f, 0.76f), new Vector2(0.91f, 0.93f), 28f, FontStyles.Bold, TextAlignmentOptions.Center, theme.GoldLight);
            TMP_Text body = EnsureText(panelObject.transform, "Body", new Vector2(0.11f, 0.29f), new Vector2(0.89f, 0.73f), 21f, FontStyles.Normal, TextAlignmentOptions.Center, theme.Cream);
            TMP_Text signature = EnsureText(panelObject.transform, "Signature", new Vector2(0.14f, 0.16f), new Vector2(0.86f, 0.26f), 19f, FontStyles.Italic, TextAlignmentOptions.Center, theme.Cream);
            Button close = EnsureButton(panelObject.transform, "Close", new Vector2(0.35f, 0.05f), new Vector2(0.65f, 0.14f), "Kapat", theme);

            FinalMessagePanelUI panel = EnsureComponent<FinalMessagePanelUI>(panelObject);
            SetObject(panel, "panel", panelObject.GetComponent<CanvasGroup>());
            SetObject(panel, "titleText", title);
            SetObject(panel, "bodyText", body);
            SetObject(panel, "signatureText", signature);
            SetObject(panel, "closeButton", close);
            return panel;
        }

        private static void UpdateFinalMessage()
        {
            FinalMessageDefinition message = AssetDatabase.LoadAssetAtPath<FinalMessageDefinition>(FinalMessagePath);
            if (message == null)
            {
                message = ScriptableObject.CreateInstance<FinalMessageDefinition>();
                AssetDatabase.CreateAsset(message, FinalMessagePath);
            }

            SerializedObject serializedObject = new(message);
            serializedObject.FindProperty("title").stringValue = "Buraya kadar geldiğin için...";
            serializedObject.FindProperty("body").stringValue = "Bu küçük yolculuk, sakince gülümseyen anılar için hazırlandı. Her ışık, her kart ve her durak biraz daha sıcak bir yere çıksın istedim.";
            serializedObject.FindProperty("signature").stringValue = "Tilki";
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(message);
        }

        private static void UpdateNarrativeAssets()
        {
            UpdateMemory("Assets/_Game/Data/Memories/Memory_01.asset", "Küçük An", "Bazen en küçük anlar en uzun süre kalıyor.");
            UpdateMemory("Assets/_Game/Data/Memories/Memory_02.asset", "Güzel Yol", "Birlikte gülmek yolu biraz daha güzel yapıyor.");
            UpdateMemory("Assets/_Game/Data/Memories/Memory_03.asset", "Sessiz Işık", "Bazı sessizlikler bile güzel bir anıya dönüşüyor.");
            UpdateMemory("Assets/_Game/Data/Memories/Memory_04.asset", "Saklı Söz", "İyi gelen şeyler bazen usulca yanında yürür.");
            UpdateMemory("Assets/_Game/Data/Memories/Memory_05.asset", "Sıcak İz", "Hatırlamak, içinden küçük bir ışık geçirmek gibi.");

            UpdateDialogue("Assets/_Game/Data/Dialogues/IntroDialogue.asset", "Rehber", "Hoş geldin.", "Orman bugün sakin. Tilki, seni küçük anıların peşine çağırıyor.");
            UpdateDialogue("Assets/_Game/Data/Dialogues/Dialogue_Guide_Intro.asset", "Rehber", "Birkaç anı ormana dağılmış.", "Onları bulursan yol biraz daha aydınlanacak.");
            UpdateDialogue("Assets/_Game/Data/Dialogues/Dialogue_Guide_Active.asset", "Rehber", "Anılar yakında. Yavaş bak, acele etme.");
            UpdateDialogue("Assets/_Game/Data/Dialogues/Dialogue_Guide_TurnIn.asset", "Rehber", "Hepsini buldun.", "Küçük şeylerin nasıl da iz bıraktığını gördün mü?");
            UpdateDialogue("Assets/_Game/Data/Dialogues/Dialogue_Guide_Completed.asset", "Rehber", "Anılar yerini buldu. Şimdi ışık yoluna bakabiliriz.");
            UpdateDialogue("Assets/_Game/Data/Dialogues/Dialogue_Guide_LightPath_Intro.asset", "Rehber", "Şimdi ışıkları takip et.", "Her doğru adım seni biraz daha yaklaştıracak.");
            UpdateDialogue("Assets/_Game/Data/Dialogues/Dialogue_Guide_LightPath_Active.asset", "Rehber", "Sıradaki ışık seni bekliyor.");
            UpdateDialogue("Assets/_Game/Data/Dialogues/Dialogue_Guide_LightPath_TurnIn.asset", "Rehber", "Yolu kaybetmeden geçtin.", "Güzel. Orman bunu hatırlayacak.");
            UpdateDialogue("Assets/_Game/Data/Dialogues/Dialogue_Guide_LightPath_Completed.asset", "Rehber", "Işık yolu tamam. Bahçe artık hazır.");
            UpdateDialogue("Assets/_Game/Data/Dialogues/Dialogue_Guide_CardMatching_Intro.asset", "Rehber", "Bahçede küçük bir oyun var.", "Eşini bulan kartlar gibi, bazı anlar da yerini bulur.");
            UpdateDialogue("Assets/_Game/Data/Dialogues/Dialogue_Guide_CardMatching_Active.asset", "Rehber", "Kartların eşlerini bul. Sakince, birer birer.");
            UpdateDialogue("Assets/_Game/Data/Dialogues/Dialogue_Guide_CardMatching_TurnIn.asset", "Rehber", "Hepsi tamamlandı.", "Kampın sıcak ışığı artık seni çağırıyor.");
            UpdateDialogue("Assets/_Game/Data/Dialogues/Dialogue_Guide_CardMatching_Completed.asset", "Rehber", "Bahçe sakin. Son anı kampta bekliyor.");
        }

        private static void UpdateMemory(string path, string displayName, string shortText)
        {
            MemoryDefinition memory = AssetDatabase.LoadAssetAtPath<MemoryDefinition>(path);
            if (memory == null)
            {
                return;
            }

            SerializedObject serializedObject = new(memory);
            serializedObject.FindProperty("displayName").stringValue = displayName;
            serializedObject.FindProperty("shortText").stringValue = shortText;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(memory);
        }

        private static void UpdateDialogue(string path, string speaker, params string[] lines)
        {
            DialogueDefinition dialogue = AssetDatabase.LoadAssetAtPath<DialogueDefinition>(path);
            if (dialogue == null)
            {
                return;
            }

            SerializedObject serializedObject = new(dialogue);
            serializedObject.FindProperty("speaker").stringValue = speaker;
            SerializedProperty lineProperty = serializedObject.FindProperty("lines");
            lineProperty.arraySize = lines.Length;
            for (int i = 0; i < lines.Length; i++)
            {
                lineProperty.GetArrayElementAtIndex(i).stringValue = lines[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(dialogue);
        }

        private static AnimationClip FindClip(params string[] tokens)
        {
            Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(FoxFbxPath);
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] is not AnimationClip clip || clip.name.StartsWith("__preview__", System.StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string lower = clip.name.ToLowerInvariant();
                for (int token = 0; token < tokens.Length; token++)
                {
                    if (lower.Contains(tokens[token]))
                    {
                        return clip;
                    }
                }
            }

            return null;
        }

        private static AnimationClip FirstClip()
        {
            Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(FoxFbxPath);
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] is AnimationClip clip && !clip.name.StartsWith("__preview__", System.StringComparison.OrdinalIgnoreCase))
                {
                    return clip;
                }
            }

            return null;
        }

        private static void PlaceAsset(Transform parent, string path, string name, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset == null)
            {
                return;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(asset, parent);
            instance.name = name;
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.transform.localScale = scale;
        }

        private static TMP_Text EnsureText(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, float fontSize, FontStyles style, TextAlignmentOptions alignment, Color color)
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
            text.color = color;
            text.raycastTarget = false;
            return text;
        }

        private static Button EnsureButton(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, string label, GameUITheme theme)
        {
            GameObject buttonObject = EnsureChild(parent, name, typeof(RectTransform), typeof(Image), typeof(Button));
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            buttonObject.GetComponent<Image>().color = theme.FoxAccent;
            TMP_Text text = EnsureText(buttonObject.transform, "Label", Vector2.zero, Vector2.one, 18f, FontStyles.Bold, TextAlignmentOptions.Center, theme.Cream);
            text.text = label;
            return buttonObject.GetComponent<Button>();
        }

        private static GameObject EnsureSceneObject(string name)
        {
            GameObject existing = GameObject.Find(name);
            return existing != null ? existing : new GameObject(name);
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
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }

        private static void ClearStateMachine(AnimatorStateMachine stateMachine)
        {
            foreach (ChildAnimatorState state in stateMachine.states)
            {
                stateMachine.RemoveState(state.state);
            }
        }

        private static AnimatorState AddState(AnimatorStateMachine stateMachine, string name, Motion motion, Vector3 position)
        {
            AnimatorState state = stateMachine.AddState(name, position);
            state.motion = motion;
            return state;
        }

        private static void AddSpeedTransition(AnimatorState from, AnimatorState to, AnimatorConditionMode mode, float threshold)
        {
            AnimatorStateTransition transition = from.AddTransition(to);
            transition.hasExitTime = false;
            transition.duration = 0.12f;
            transition.AddCondition(mode, threshold, "Speed");
        }

        private static void AddGroundedTransition(AnimatorState from, AnimatorState to)
        {
            AnimatorStateTransition transition = from.AddTransition(to);
            transition.hasExitTime = false;
            transition.duration = 0.08f;
            transition.AddCondition(AnimatorConditionMode.IfNot, 0f, "Grounded");
        }

        private static void AddAnimatorParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
        {
            controller.AddParameter(name, type);
        }

        private static AudioClip[] LoadAudioClips(params string[] paths)
        {
            List<AudioClip> clips = new();
            for (int i = 0; i < paths.Length; i++)
            {
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(paths[i]);
                if (clip != null)
                {
                    clips.Add(clip);
                }
            }

            return clips.ToArray();
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
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
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetString(Object target, string propertyName, string value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).stringValue = value;
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

        private static void SetColor(Object target, string propertyName, Color value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).colorValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }
}
