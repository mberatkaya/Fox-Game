using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

namespace TilkiOyunu.Foundation.Editor
{
    public static class Sprint5Validation
    {
        private const string ToonFoxPrefabPath = "Assets/Fox/Prefabs/Fox.prefab";
        private const string ToonFoxModelPath = "Assets/Fox/FBXs/Fox.fbx";
        private const string ToonFoxMaterialPath = "Assets/_Game/Art/Characters/ToonFox_URP.mat";
        private const string ToonFoxBaseTexturePath = "Assets/Fox/Textures/T_Fox_BC.png";
        private const string ToonFoxNormalTexturePath = "Assets/Fox/Textures/T_Fox_Normal.png";
        private const string ToonFoxOcclusionTexturePath = "Assets/Fox/Textures/T_Fox_AO.png";
        private const string MixerPath = "Assets/_Game/Audio/Mixers/TilkiAudioMixer.mixer";

        [MenuItem("Tilki Oyunu/Sprint 5/Validate")]
        public static void ValidateFromMenu()
        {
            if (ValidateProject(out List<string> errors))
            {
                Debug.Log("Sprint 5 validation passed.");
                return;
            }

            foreach (string error in errors)
            {
                Debug.LogError(error);
            }
        }

        public static void ValidateFromCommandLine()
        {
            if (ValidateProject(out List<string> errors))
            {
                Debug.Log("Sprint 5 validation passed.");
                EditorApplication.Exit(0);
                return;
            }

            foreach (string error in errors)
            {
                Debug.LogError(error);
            }

            EditorApplication.Exit(1);
        }

        public static bool ValidateProject(out List<string> errors)
        {
            errors = new List<string>();
            ValidateImportedAssets(errors);
            ValidatePlayerPrefab(errors);
            ValidateBuildSettings(errors);
            ValidateForestScene(errors);
            ValidateDocumentation(errors);
            return errors.Count == 0;
        }

        private static void ValidateImportedAssets(List<string> errors)
        {
            RequireAsset<GameObject>(ToonFoxPrefabPath, "Pxltiger Toon Fox prefab", errors);
            RequireAsset<GameObject>(ToonFoxModelPath, "Pxltiger Toon Fox model", errors);
            RequireAsset<Texture2D>(ToonFoxBaseTexturePath, "Toon Fox base color texture", errors);
            RequireAsset<Texture2D>(ToonFoxNormalTexturePath, "Toon Fox normal texture", errors);
            RequireAsset<Texture2D>(ToonFoxOcclusionTexturePath, "Toon Fox occlusion texture", errors);
            RequireAsset<GameObject>("Assets/TreePackVol.1/Prefabs/0/Tree1.prefab", "Tree Collection Pack 2017 Tree1 prefab", errors);
            RequireAsset<GameObject>("Assets/TreePackVol.1/Prefabs/3/Tree 3.prefab", "Tree Collection Pack 2017 broadleaf prefab", errors);
            RequireAsset<Material>(ToonFoxMaterialPath, "Toon Fox URP material override", errors);
            RequireAsset<RuntimeAnimatorController>("Assets/_Game/Art/Characters/FoxAnimatorController.controller", "Fox animator controller", errors);
            RequireClip(errors, "Fox_Idle");
            RequireClip(errors, "Fox_Walk_InPlace");
            RequireClip(errors, "Fox_Run_InPlace");
            RequireClip(errors, "Fox_Jump");
            RequireClip(errors, "Fox_Falling");
            ValidateFoxAnimationImport(errors);
            ValidateFoxAnimatorController(errors);

            string[] nature =
            {
                "BirchTree_1.fbx", "MapleTree_1.fbx", "NormalTree_1.fbx", "PineTree_1.fbx",
                "Bush.fbx", "Grass_Small.fbx", "Flower_1_Clump.fbx", "Rock_1.fbx"
            };
            for (int i = 0; i < nature.Length; i++)
            {
                RequireAsset<GameObject>($"Assets/ThirdParty/Quaternius/UltimateStylizedNature/{nature[i]}", nature[i], errors);
            }

            string[] megaKit =
            {
                "CommonTree_1.fbx", "CommonTree_5.fbx", "Pine_1.fbx", "Pine_5.fbx",
                "TwistedTree_1.fbx", "TwistedTree_5.fbx", "Rock_Medium_1.fbx", "Bush_Common.fbx",
                "Grass_Common_Short.fbx", "Flower_3_Group.fbx"
            };
            for (int i = 0; i < megaKit.Length; i++)
            {
                RequireAsset<GameObject>($"Assets/ThirdParty/Quaternius/StylizedNatureMegaKit/{megaKit[i]}", $"Quaternius Stylized Nature MegaKit {megaKit[i]}", errors);
            }

            RequireAsset<AudioClip>("Assets/ThirdParty/OpenGameArt/Music/SunsetWalk.ogg", "background music", errors);
            RequireAsset<AudioClip>("Assets/ThirdParty/OpenGameArt/Ambience/Forest_Ambience.mp3", "forest ambience", errors);
            RequireAsset<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Footsteps/leaves01.ogg", "footstep SFX", errors);
            RequireAsset<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Chimes/bell_ding1.wav", "memory/light chime", errors);
            RequireAsset<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Cards/contact1.wav", "card contact SFX", errors);
            RequireAsset<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Campfire/fire.wav", "campfire loop", errors);
            RequireAsset<AudioClip>("Assets/ThirdParty/Kenney/InterfaceSounds/click_001.ogg", "UI click", errors);
            ValidateAudioMixer(errors);
            RequireAsset<GameUITheme>("Assets/_Game/Art/UI/GameUITheme.asset", "UI theme", errors);
            RequireAsset<FinalMessageDefinition>("Assets/_Game/Data/Config/FinalMessage.asset", "final message", errors);
        }

        private static void ValidatePlayerPrefab(List<string> errors)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/Characters/PlayerFox.prefab");
            if (prefab == null)
            {
                errors.Add("PlayerFox prefab is missing.");
                return;
            }

            if (prefab.transform.Find("VisualRoot/ToonFox") == null)
            {
                errors.Add("PlayerFox prefab must contain VisualRoot/ToonFox.");
            }

            if (prefab.transform.Find("VisualRoot/OpenGameArtFox") != null)
            {
                errors.Add("OpenGameArtFox must not remain active as the PlayerFox production visual.");
            }

            Transform visualRoot = prefab.transform.Find("VisualRoot");
            Transform foxVisual = prefab.transform.Find("VisualRoot/ToonFox");
            if (visualRoot == null || visualRoot.localPosition != Vector3.zero || visualRoot.localScale != Vector3.one)
            {
                errors.Add("PlayerFox VisualRoot must stay identity so movement root and presentation root remain separated.");
            }

            if (foxVisual == null || foxVisual.localPosition.y < 0.045f || foxVisual.localPosition.y > 0.065f)
            {
                errors.Add("PlayerFox ToonFox visual offset must be calibrated from Toon Fox locomotion renderer bounds.");
            }
            else
            {
                if (Mathf.Abs(Mathf.DeltaAngle(foxVisual.localEulerAngles.y, 0f)) > 0.001f)
                {
                    errors.Add("PlayerFox ToonFox visual yaw must stay at 0 degrees so the model faces the movement root direction.");
                }

                float groundedBottom = foxVisual.localPosition.y + foxVisual.localScale.y * MeasureToonFoxGroundedRendererMinY();
                if (groundedBottom < 0.015f || groundedBottom > 0.04f)
                {
                    errors.Add($"PlayerFox visual paws should sit near ground; measured clearance was {groundedBottom:0.###}.");
                }
            }

            if (prefab.GetComponentInChildren<FoxAnimationDriver>(true) == null)
            {
                errors.Add("PlayerFox prefab is missing FoxAnimationDriver.");
            }

            Animator animator = prefab.GetComponentInChildren<Animator>(true);
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                errors.Add("PlayerFox prefab is missing a bound AnimatorController.");
            }
            else if (animator.applyRootMotion)
            {
                errors.Add("Fox Animator must have root motion disabled.");
            }

            if (prefab.GetComponent<FootstepAudio>() == null)
            {
                errors.Add("PlayerFox prefab is missing FootstepAudio.");
            }

            AudioSource source = prefab.GetComponent<AudioSource>();
            if (source == null || source.outputAudioMixerGroup == null || source.outputAudioMixerGroup.name != "SFX")
            {
                errors.Add("PlayerFox footstep AudioSource must route to the SFX mixer group.");
            }
        }

        private static void ValidateFoxAnimationImport(List<string> errors)
        {
            RequireClipLoop("Fox_Idle", true, errors);
            RequireClipLoop("Fox_Walk_InPlace", true, errors);
            RequireClipLoop("Fox_Run_InPlace", true, errors);
            RequireClipLoop("Fox_Jump_InAir", false, errors);
        }

        private static void ValidateFoxAnimatorController(List<string> errors)
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/_Game/Art/Characters/FoxAnimatorController.controller");
            if (controller == null || controller.layers == null || controller.layers.Length == 0)
            {
                errors.Add("Fox AnimatorController must exist with a Base Layer.");
                return;
            }

            RequireAnimatorParameter(controller, "Speed", AnimatorControllerParameterType.Float, errors);
            RequireAnimatorParameter(controller, "Grounded", AnimatorControllerParameterType.Bool, errors);
            RequireAnimatorParameter(controller, "VerticalVelocity", AnimatorControllerParameterType.Float, errors);
            RequireAnimatorParameter(controller, "IdleSit", AnimatorControllerParameterType.Trigger, errors);
            RequireAnimatorParameter(controller, "IdleBreak", AnimatorControllerParameterType.Trigger, errors);

            AnimatorState locomotion = controller.layers[0].stateMachine.defaultState;
            if (locomotion == null || locomotion.name != "Locomotion" || locomotion.motion is not BlendTree blendTree)
            {
                errors.Add("Fox AnimatorController must use a grounded Locomotion 1D BlendTree.");
                return;
            }

            if (blendTree.blendParameter != "Speed" || blendTree.children.Length != 3)
            {
                errors.Add("Fox Locomotion BlendTree must blend Idle/Walk/Run with the Speed parameter.");
                return;
            }

            RequireBlendChild(blendTree, 0, "Fox_Idle", 0f, 1f, errors);
            RequireBlendChild(blendTree, 1, "Fox_Walk_InPlace", 0.59f, 1.08f, errors);
            RequireBlendChild(blendTree, 2, "Fox_Run_InPlace", 1f, 1.16f, errors);

            if (FindAnimatorState(controller.layers[0].stateMachine, "Jump") == null)
            {
                errors.Add("Fox AnimatorController must use the actual Toon Fox jump clip for jump ascent.");
            }

            if (FindAnimatorState(controller.layers[0].stateMachine, "Fall") == null)
            {
                errors.Add("Fox AnimatorController must use the actual Toon Fox falling clip for falling descent.");
            }
        }

        private static void ValidateForestScene(List<string> errors)
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            RequireSceneObject("Environment_Visuals", errors);
            RequireSceneObject("Sprint 5 Scene Audio", errors);
            RequireSceneObject("Final Message Panel", errors);

            if (Object.FindFirstObjectByType<SceneLoopAudio>(FindObjectsInactive.Include) == null)
            {
                errors.Add("Forest scene is missing SceneLoopAudio.");
            }

            RequireAudioRoute("Music Loop", "Music", errors);
            RequireAudioRoute("Forest Ambience Loop", "Ambience", errors);
            RequireAudioRoute("Campfire Loop", "Ambience", errors);

            if (Object.FindFirstObjectByType<MemoryAudioFeedback>(FindObjectsInactive.Include) == null)
            {
                errors.Add("Forest memories are missing MemoryAudioFeedback.");
            }

            if (Object.FindFirstObjectByType<LightPathAudioFeedback>(FindObjectsInactive.Include) == null)
            {
                errors.Add("Light Path is missing LightPathAudioFeedback.");
            }

            if (Object.FindFirstObjectByType<CardMatchingAudioFeedback>(FindObjectsInactive.Include) == null)
            {
                errors.Add("Card Matching is missing CardMatchingAudioFeedback.");
            }

            RequireBridgeRailCollider("Bridge Left Rail Collider", -1.95f, errors);
            RequireBridgeRailCollider("Bridge Right Rail Collider", 1.95f, errors);

            ValidateCardMatchingPanel(errors);
            ValidateInitialPresentation(errors);

            FinalCampController camp = Object.FindFirstObjectByType<FinalCampController>(FindObjectsInactive.Include);
            if (camp == null)
            {
                errors.Add("Forest scene is missing FinalCampController.");
            }
            else
            {
                FinalSequenceController sequence = camp.GetComponent<FinalSequenceController>();
                if (sequence == null)
                {
                    errors.Add("Final camp is missing FinalSequenceController.");
                }
                else
                {
                    ValidateFinalSequence(sequence, errors);
                }
            }

            ValidateEnvironmentVisuals(errors);
            ValidateSprint55WorldLayout(errors);

            if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>(FindObjectsInactive.Include) == null)
            {
                errors.Add("Forest scene is missing an EventSystem for modal UI focus.");
            }

            foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                int missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(root);
                if (missingCount > 0)
                {
                    errors.Add($"Scene root '{root.name}' has {missingCount} missing script reference(s).");
                }
            }
        }

        private static void ValidateBuildSettings(List<string> errors)
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            if (scenes.Length < 2)
            {
                errors.Add("Build Settings must include Bootstrap and Forest scenes.");
                return;
            }

            if (!scenes[0].enabled || scenes[0].path != SceneIds.BootstrapPath)
            {
                errors.Add("Build Settings scene 0 must be the enabled Bootstrap scene.");
            }

            if (!scenes[1].enabled || scenes[1].path != SceneIds.ForestPath)
            {
                errors.Add("Build Settings scene 1 must be the enabled Forest scene.");
            }
        }

        private static void ValidateDocumentation(List<string> errors)
        {
            if (!File.Exists("ASSET_NOTES.md"))
            {
                errors.Add("ASSET_NOTES.md is missing.");
                return;
            }

            string assetNotes = File.ReadAllText("ASSET_NOTES.md");
            if (!assetNotes.Contains("Asset Name: Bell dings/chimes") || !assetNotes.Contains("Author: PWL"))
            {
                errors.Add("ASSET_NOTES.md must list Bell dings/chimes author as PWL.");
            }

            if (!assetNotes.Contains("Asset Name: Toon Fox") || !assetNotes.Contains("Author: Pxltiger") || !assetNotes.Contains("https://assetstore.unity.com/packages/3d/characters/animals/toon-fox-183005"))
            {
                errors.Add("ASSET_NOTES.md must document the active Pxltiger Toon Fox source and author.");
            }
        }

        private static void ValidateFinalSequence(FinalSequenceController sequence, List<string> errors)
        {
            SerializedObject serialized = new(sequence);
            if (serialized.FindProperty("gameplayCamera")?.objectReferenceValue == null)
            {
                errors.Add("FinalSequenceController must reference the gameplay camera.");
            }

            if (serialized.FindProperty("cameraController")?.objectReferenceValue == null)
            {
                errors.Add("FinalSequenceController must reference the ThirdPersonCameraController.");
            }

            if (serialized.FindProperty("cameraFocus")?.objectReferenceValue == null)
            {
                errors.Add("FinalSequenceController must reference a camera focus transform.");
            }

            if (serialized.FindProperty("finalPanel")?.objectReferenceValue == null)
            {
                errors.Add("FinalSequenceController must reference the final message panel.");
            }
        }

        private static void ValidateCardMatchingPanel(List<string> errors)
        {
            CardMatchingPanelUI panel = Object.FindFirstObjectByType<CardMatchingPanelUI>(FindObjectsInactive.Include);
            if (panel == null)
            {
                errors.Add("Forest scene is missing CardMatchingPanelUI.");
                return;
            }

            SerializedObject serialized = new(panel);
            if (serialized.FindProperty("panel")?.objectReferenceValue == null)
            {
                errors.Add("CardMatchingPanelUI must reference its CanvasGroup.");
            }

            if (serialized.FindProperty("inputActions")?.objectReferenceValue == null)
            {
                errors.Add("CardMatchingPanelUI must reference the InputActionAsset.");
            }

            if (serialized.FindProperty("cameraController")?.objectReferenceValue == null)
            {
                errors.Add("CardMatchingPanelUI must reference the ThirdPersonCameraController.");
            }

            SerializedProperty cardButtons = serialized.FindProperty("cardButtons");
            if (cardButtons == null || !cardButtons.isArray || cardButtons.arraySize == 0)
            {
                errors.Add("CardMatchingPanelUI must reference card buttons.");
            }
            else
            {
                for (int i = 0; i < cardButtons.arraySize; i++)
                {
                    if (cardButtons.GetArrayElementAtIndex(i).objectReferenceValue == null)
                    {
                        errors.Add($"CardMatchingPanelUI cardButtons[{i}] is missing.");
                    }
                }
            }
        }

        private static void ValidateInitialPresentation(List<string> errors)
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                errors.Add("Forest scene is missing an active Main Camera.");
            }
            else
            {
                if (!camera.isActiveAndEnabled)
                {
                    errors.Add("Main Camera must be active and enabled for the initial Game View.");
                }

                if (camera.targetDisplay != 0)
                {
                    errors.Add("Main Camera target display must be Display 1.");
                }

                if (camera.targetTexture != null)
                {
                    errors.Add("Main Camera target texture must be None.");
                }

                if (camera.cullingMask == 0)
                {
                    errors.Add("Main Camera culling mask must render gameplay layers.");
                }

                ThirdPersonCameraController cameraController = camera.GetComponent<ThirdPersonCameraController>();
                if (cameraController == null || !cameraController.isActiveAndEnabled)
                {
                    errors.Add("Main Camera must have an enabled ThirdPersonCameraController.");
                }
                else
                {
                    SerializedObject serializedCamera = new(cameraController);
                    if (serializedCamera.FindProperty("target")?.objectReferenceValue == null)
                    {
                        errors.Add("ThirdPersonCameraController must reference the player Camera Target.");
                    }

                    if (cameraController.IsExternalControlActive)
                    {
                        errors.Add("ThirdPersonCameraController external control must be false at initial gameplay.");
                    }
                }
            }

            GameObject player = GameObject.Find("PlayerFox");
            if (player == null || !player.activeInHierarchy)
            {
                errors.Add("PlayerFox must be active for the initial Game View.");
            }
            else if (player.transform.Find("VisualRoot") == null || !player.transform.Find("VisualRoot").gameObject.activeInHierarchy)
            {
                errors.Add("PlayerFox VisualRoot must be active for the initial Game View.");
            }

            ValidateHiddenPanel<DialoguePanelUI>("Dialogue panel", errors);
            ValidateHiddenPanel<CardMatchingPanelUI>("Card Matching panel", errors);
            ValidateHiddenPanel<FinalMessagePanelUI>("Final message panel", errors);
            ValidateHiddenPanel<MemoryFeedbackUI>("Memory feedback panel", errors);
            ValidateHiddenPanel<LightPathHUD>("Light Path HUD panel", errors);
            ValidateHiddenPanel<QuestHUD>("Quest HUD panel", errors);
        }

        private static void ValidateHiddenPanel<T>(string label, List<string> errors) where T : Component
        {
            T component = Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
            if (component == null)
            {
                errors.Add($"Forest scene is missing {label}.");
                return;
            }

            SerializedObject serialized = new(component);
            CanvasGroup panel = serialized.FindProperty("panel")?.objectReferenceValue as CanvasGroup;
            if (panel == null)
            {
                panel = component.GetComponent<CanvasGroup>();
            }

            if (panel == null)
            {
                errors.Add($"{label} must reference a CanvasGroup.");
                return;
            }

            if (panel.alpha > 0.001f || panel.interactable || panel.blocksRaycasts)
            {
                errors.Add($"{label} must be hidden in the serialized Forest scene default state.");
            }
        }

        private static void ValidateEnvironmentVisuals(List<string> errors)
        {
            GameObject environmentVisuals = GameObject.Find("Environment_Visuals");
            if (environmentVisuals == null)
            {
                return;
            }

            Collider[] visualColliders = environmentVisuals.GetComponentsInChildren<Collider>(true);
            if (visualColliders.Length > 0)
            {
                errors.Add("Environment_Visuals must remain visual-only and contain no colliders.");
            }
        }

        private static void ValidateSprint55WorldLayout(List<string> errors)
        {
            GameObject world = GameObject.Find("World");
            if (world == null)
            {
                errors.Add("Forest scene is missing the Sprint 5.5-B World root.");
                return;
            }

            GameObject terrainObject = GameObject.Find("Sprint55B_PrimaryTerrain");
            Terrain terrain = terrainObject != null ? terrainObject.GetComponent<Terrain>() : null;
            if (terrain == null || terrain.terrainData == null)
            {
                errors.Add("Forest scene is missing the Sprint 5.5-B primary Terrain.");
                return;
            }

            TerrainCollider terrainCollider = terrainObject.GetComponent<TerrainCollider>();
            if (terrainCollider == null || !terrainCollider.enabled || terrainCollider.terrainData != terrain.terrainData)
            {
                errors.Add("Sprint 5.5-B primary Terrain must keep an enabled TerrainCollider bound to the generated TerrainData.");
            }

            Vector3 size = terrain.terrainData.size;
            if (Mathf.Abs(size.x - 512f) > 0.01f || Mathf.Abs(size.z - 512f) > 0.01f)
            {
                errors.Add($"Sprint 5.5-B terrain must be 512x512, but was {size.x:0.#}x{size.z:0.#}.");
            }

            if (size.y < 40f || size.y > 60f)
            {
                errors.Add($"Sprint 5.5-B terrain height range must be roughly 40-60m, but was {size.y:0.#}m.");
            }

            if (terrain.terrainData.heightmapResolution != 257)
            {
                errors.Add("Sprint 5.5-B terrain must use a small-world heightmap resolution of 257.");
            }

            string[] requiredAnchors =
            {
                "LM_SpawnMeadow",
                "LM_NPCGrove",
                "LM_MemoryRoute_A",
                "LM_MemoryRoute_B",
                "LM_Bridge",
                "LM_Lake",
                "LM_LightGrove",
                "LM_HeartGarden",
                "LM_FinalHill"
            };

            Bounds bounds = new(Vector3.zero, Vector3.zero);
            bool hasBounds = false;
            Dictionary<string, GameObject> anchors = new();
            for (int i = 0; i < requiredAnchors.Length; i++)
            {
                GameObject anchor = GameObject.Find(requiredAnchors[i]);
                if (anchor == null)
                {
                    errors.Add($"Forest scene is missing world layout anchor {requiredAnchors[i]}.");
                    continue;
                }

                anchors[requiredAnchors[i]] = anchor;
                if (!hasBounds)
                {
                    bounds = new Bounds(anchor.transform.position, Vector3.zero);
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(anchor.transform.position);
                }
            }

            if (hasBounds)
            {
                if (bounds.size.x < 290f || bounds.size.x > 350f || bounds.size.z < 290f || bounds.size.z > 350f)
                {
                    errors.Add($"Sprint 5.5-B playable landmark core should be about 300-350m wide/deep, but was {bounds.size.x:0.#}x{bounds.size.z:0.#}.");
                }
            }

            if (anchors.TryGetValue("LM_SpawnMeadow", out GameObject spawn)
                && anchors.TryGetValue("LM_FinalHill", out GameObject finalHill)
                && finalHill.transform.position.y - spawn.transform.position.y < 24f)
            {
                errors.Add("Final Hill must be meaningfully higher than Spawn Meadow.");
            }

            if (anchors.TryGetValue("LM_Lake", out GameObject lake)
                && anchors.TryGetValue("LM_FinalHill", out GameObject hill)
                && hill.transform.position.y - lake.transform.position.y < 26f)
            {
                errors.Add("Lake must sit well below Final Hill.");
            }

            if (GameObject.Find("GB_Lake_TempWater") == null || GameObject.Find("GB_Creek_TempWater") == null)
            {
                errors.Add("Sprint 5.5-B needs temporary lake and creek water reference meshes.");
            }

            ValidateStartPlatform(errors);
            ValidateSprint55CEnvironment(errors);

            if (GameObject.Find("Small Bridge") == null || GameObject.Find("Small Bridge").transform.position.x < 5f)
            {
                errors.Add("Small Bridge must be placed at the Sprint 5.5-B creek crossing.");
            }

            string[] arenaWalls =
            {
                "North Boundary Ridge",
                "South Boundary Ridge",
                "East Boundary Ridge",
                "West Boundary Ridge"
            };

            for (int i = 0; i < arenaWalls.Length; i++)
            {
                GameObject wall = FindSceneObjectIncludingInactive(arenaWalls[i]);
                if (wall != null && wall.activeInHierarchy)
                {
                    errors.Add($"{arenaWalls[i]} must not remain active as a visible arena boundary.");
                }
            }
        }

        private static void ValidateStartPlatform(List<string> errors)
        {
            GameObject platform = GameObject.Find("Start Platform");
            if (platform == null)
            {
                errors.Add("Sprint 5.5-B needs a visible, solid Start Platform under PlayerFox.");
                return;
            }

            BoxCollider collider = platform.GetComponent<BoxCollider>();
            if (collider == null || collider.isTrigger)
            {
                errors.Add("Start Platform must have a solid BoxCollider so the fox cannot fall through at spawn.");
            }
        }

        private static void ValidateSprint55CEnvironment(List<string> errors)
        {
            GameObject environment = GameObject.Find("Environment");
            if (environment == null || environment.transform.parent == null || environment.transform.parent.name != "World")
            {
                errors.Add("Sprint 5.5-C needs a World/Environment production dressing root.");
                return;
            }

            int treeCount = CountNamedChildren(environment.transform, "QuaterniusTree_");
            if (treeCount < 150 || treeCount > 360)
            {
                errors.Add($"Sprint 5.5-C should use about 150-360 production trees; found {treeCount}.");
            }

            int treeColliderCount = CountTreeColliders(environment.transform);
            if (treeColliderCount != treeCount)
            {
                errors.Add($"Sprint 5.5-C.1 requires all production trees to have trunk colliders; found {treeColliderCount}/{treeCount}.");
            }

            int variants = CountTreeVariants(environment.transform);
            if (variants < 10)
            {
                errors.Add($"Sprint 5.5-C needs at least 10 meaningful tree variants; found {variants}.");
            }

            if (CountNamedChildren(environment.transform, "QuaterniusRock_") < 80)
            {
                errors.Add("Sprint 5.5-C needs rock clusters supporting shoreline, slopes, and boundaries.");
            }

            int blockingRockColliders = CountBlockingRockColliders(environment.transform);
            if (blockingRockColliders < 35)
            {
                errors.Add($"Sprint 5.5-C.1 requires medium/large production rocks to block movement; found {blockingRockColliders} blocking rock colliders.");
            }

            if (CountNamedChildren(environment.transform, "QuaterniusPlant_") < 50)
            {
                errors.Add("Sprint 5.5-C needs bushes/plants to support trails, shoreline, and landmarks.");
            }

            if (CountNamedChildren(environment.transform, "QuaterniusFlower_") < 40)
            {
                errors.Add("Sprint 5.5-C needs intentional flower clusters, especially around meadow and Heart Garden.");
            }

            Terrain terrain = GameObject.Find("Sprint55B_PrimaryTerrain")?.GetComponent<Terrain>();
            TerrainLayer[] layers = terrain != null && terrain.terrainData != null ? terrain.terrainData.terrainLayers : null;
            string[] expectedLayers =
            {
                "Sprint55C_Grass",
                "Sprint55C_ForestDirt",
                "Sprint55C_PathDryGround",
                "Sprint55C_Rock"
            };

            if (layers == null || layers.Length < expectedLayers.Length)
            {
                errors.Add("Sprint 5.5-C terrain must have Grass, Forest Dirt, Path/Dry Ground, and Rock layers.");
            }
            else
            {
                for (int i = 0; i < expectedLayers.Length; i++)
                {
                    if (layers[i] == null || layers[i].name != expectedLayers[i])
                    {
                        errors.Add($"Sprint 5.5-C terrain layer {i} should be {expectedLayers[i]}.");
                    }
                }
            }

            if (terrain == null || terrain.terrainData == null || terrain.terrainData.detailPrototypes.Length < 3)
            {
                errors.Add("Sprint 5.5-C terrain must use controlled grass detail prototypes.");
            }

            if (GameObject.Find("Sprint55C_Bridge_Walkway") == null)
            {
                errors.Add("Sprint 5.5-C needs the production bridge walkway visual under Small Bridge.");
            }

            GameObject oldForest = FindSceneObjectIncludingInactive("TreeCollectionForest");
            if (oldForest != null)
            {
                errors.Add("TreeCollectionForest must be removed after Sprint 5.5-C so old TreePack prototype visuals no longer stay in the runtime scene.");
            }

            string[] removedPrototypeMarkers =
            {
                "GB_SpawnMeadow_Readability",
                "GB_NPCGrove_Readability",
                "GB_FinalHill_Summit",
                "Safe Clearing Ground",
                "Path To Trees",
                "Future Memory Area Marker",
                "Future Quest Area Marker"
            };

            for (int i = 0; i < removedPrototypeMarkers.Length; i++)
            {
                GameObject marker = FindSceneObjectIncludingInactive(removedPrototypeMarkers[i]);
                if (marker != null)
                {
                    errors.Add($"{removedPrototypeMarkers[i]} must be removed from the production scene.");
                }
            }
        }

        private static int CountNamedChildren(Transform root, string prefix)
        {
            int count = root.name.StartsWith(prefix) ? 1 : 0;
            for (int i = 0; i < root.childCount; i++)
            {
                count += CountNamedChildren(root.GetChild(i), prefix);
            }

            return count;
        }

        private static int CountTreeColliders(Transform root)
        {
            int count = root.name.StartsWith("QuaterniusTree_") && root.GetComponent<Collider>() != null ? 1 : 0;
            for (int i = 0; i < root.childCount; i++)
            {
                count += CountTreeColliders(root.GetChild(i));
            }

            return count;
        }

        private static int CountBlockingRockColliders(Transform root)
        {
            int count = root.name.StartsWith("QuaterniusRock_") && root.GetComponent<BoxCollider>() != null ? 1 : 0;
            for (int i = 0; i < root.childCount; i++)
            {
                count += CountBlockingRockColliders(root.GetChild(i));
            }

            return count;
        }

        private static int CountTreeVariants(Transform root)
        {
            HashSet<string> variants = new();
            CollectTreeVariants(root, variants);
            return variants.Count;
        }

        private static void CollectTreeVariants(Transform root, HashSet<string> variants)
        {
            if (root.name.StartsWith("QuaterniusTree_"))
            {
                string[] parts = root.name.Split('_');
                if (parts.Length >= 3)
                {
                    variants.Add($"{parts[1]}_{parts[2]}");
                }
            }

            for (int i = 0; i < root.childCount; i++)
            {
                CollectTreeVariants(root.GetChild(i), variants);
            }
        }

        private static void ValidateAudioMixer(List<string> errors)
        {
            AudioMixer mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(MixerPath);
            if (mixer == null)
            {
                errors.Add($"Audio mixer is missing at {MixerPath}.");
                return;
            }

            RequireMixerGroup(mixer, "Master", errors);
            RequireMixerGroup(mixer, "Music", errors);
            RequireMixerGroup(mixer, "Ambience", errors);
            RequireMixerGroup(mixer, "SFX", errors);

            string mixerText = File.ReadAllText(MixerPath);
            RequireMixerParameter(mixerText, "MasterVolume", errors);
            RequireMixerParameter(mixerText, "MusicVolume", errors);
            RequireMixerParameter(mixerText, "AmbienceVolume", errors);
            RequireMixerParameter(mixerText, "SFXVolume", errors);
        }

        private static void RequireMixerGroup(AudioMixer mixer, string groupName, List<string> errors)
        {
            if (mixer.FindMatchingGroups(groupName).Length == 0)
            {
                errors.Add($"Audio mixer is missing the {groupName} group.");
            }
        }

        private static void RequireMixerParameter(string mixerText, string parameterName, List<string> errors)
        {
            if (!mixerText.Contains($"name: {parameterName}"))
            {
                errors.Add($"Audio mixer is missing exposed parameter {parameterName}.");
            }
        }

        private static void RequireAsset<T>(string path, string label, List<string> errors) where T : Object
        {
            if (AssetDatabase.LoadAssetAtPath<T>(path) == null)
            {
                errors.Add($"{label} is missing at {path}.");
            }
        }

        private static void RequireClip(List<string> errors, string clipName)
        {
            if (FindClip(clipName) == null)
            {
                errors.Add($"Toon Fox package is missing animation clip '{clipName}'.");
            }
        }

        private static void RequireClipLoop(string clipName, bool expectedLoop, List<string> errors)
        {
            AnimationClip clip = FindClip(clipName);
            if (clip == null)
            {
                errors.Add($"Fox FBX is missing animation clip '{clipName}'.");
                return;
            }

            if (AnimationUtility.GetAnimationClipSettings(clip).loopTime != expectedLoop)
            {
                errors.Add($"Fox clip '{clipName}' loopTime must be {expectedLoop}.");
            }
        }

        private static AnimationClip FindClip(string clipName)
        {
            string path = $"Assets/Fox/Animations/{clipName}.fbx";
            Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] is AnimationClip clip && clip.name == clipName)
                {
                    return clip;
                }
            }

            return null;
        }

        private static void RequireAnimatorParameter(AnimatorController controller, string name, AnimatorControllerParameterType type, List<string> errors)
        {
            for (int i = 0; i < controller.parameters.Length; i++)
            {
                AnimatorControllerParameter parameter = controller.parameters[i];
                if (parameter.name == name && parameter.type == type)
                {
                    return;
                }
            }

            errors.Add($"Fox AnimatorController is missing {type} parameter '{name}'.");
        }

        private static void RequireBlendChild(BlendTree blendTree, int index, string clipName, float threshold, float timeScale, List<string> errors)
        {
            ChildMotion child = blendTree.children[index];
            if (child.motion == null || child.motion.name != clipName)
            {
                errors.Add($"Fox Locomotion BlendTree child {index} must use '{clipName}'.");
            }

            if (Mathf.Abs(child.threshold - threshold) > 0.001f)
            {
                errors.Add($"Fox Locomotion BlendTree child {index} threshold must be {threshold:0.###}.");
            }

            if (Mathf.Abs(child.timeScale - timeScale) > 0.001f)
            {
                errors.Add($"Fox Locomotion BlendTree child {index} playback speed must be {timeScale:0.###}.");
            }
        }

        private static AnimatorState FindAnimatorState(AnimatorStateMachine stateMachine, string stateName)
        {
            foreach (ChildAnimatorState child in stateMachine.states)
            {
                if (child.state != null && child.state.name == stateName)
                {
                    return child.state;
                }
            }

            foreach (ChildAnimatorStateMachine childMachine in stateMachine.stateMachines)
            {
                AnimatorState found = FindAnimatorState(childMachine.stateMachine, stateName);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private static float MeasureToonFoxGroundedRendererMinY()
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(ToonFoxPrefabPath);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(asset);
            try
            {
                float minY = float.PositiveInfinity;
                string[] clipNames = { "Fox_Idle", "Fox_Walk_InPlace", "Fox_Run_InPlace" };
                for (int clipIndex = 0; clipIndex < clipNames.Length; clipIndex++)
                {
                    AnimationClip clip = FindClip(clipNames[clipIndex]);
                    if (clip == null)
                    {
                        continue;
                    }

                    int sampleCount = Mathf.Max(8, Mathf.CeilToInt(clip.length * 30f));
                    for (int sample = 0; sample <= sampleCount; sample++)
                    {
                        clip.SampleAnimation(instance, clip.length * sample / sampleCount);
                        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
                        for (int i = 0; i < renderers.Length; i++)
                        {
                            minY = Mathf.Min(minY, renderers[i].bounds.min.y);
                        }
                    }
                }

                return minY;
            }
            finally
            {
                Object.DestroyImmediate(instance);
            }
        }

        private static void RequireSceneObject(string name, List<string> errors)
        {
            if (GameObject.Find(name) == null)
            {
                errors.Add($"Forest scene is missing {name}.");
            }
        }

        private static void RequireBridgeRailCollider(string name, float expectedX, List<string> errors)
        {
            GameObject rail = GameObject.Find(name);
            if (rail == null)
            {
                errors.Add($"Forest bridge is missing {name}.");
                return;
            }

            if (rail.transform.parent == null || rail.transform.parent.name != "Small Bridge")
            {
                errors.Add($"{name} must stay under Small Bridge.");
            }

            if (Mathf.Abs(rail.transform.localPosition.x - expectedX) > 0.001f || Mathf.Abs(rail.transform.localPosition.y - 0.6f) > 0.001f)
            {
                errors.Add($"{name} has drifted from the calibrated bridge rail position.");
            }

            BoxCollider collider = rail.GetComponent<BoxCollider>();
            if (collider == null || collider.isTrigger || Vector3.Distance(collider.size, new Vector3(0.2f, 1f, 27.2f)) > 0.001f)
            {
                errors.Add($"{name} must be a solid BoxCollider sized for fox body blocking.");
            }
        }

        private static void RequireAudioRoute(string objectName, string groupName, List<string> errors)
        {
            GameObject gameObject = GameObject.Find(objectName);
            AudioSource source = gameObject == null ? null : gameObject.GetComponent<AudioSource>();
            if (source == null || source.outputAudioMixerGroup == null || source.outputAudioMixerGroup.name != groupName)
            {
                errors.Add($"{objectName} must route to the {groupName} mixer group.");
            }
        }

        private static GameObject FindSceneObjectIncludingInactive(string name)
        {
            GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < objects.Length; i++)
            {
                GameObject gameObject = objects[i];
                if (gameObject.name == name && gameObject.scene.IsValid())
                {
                    return gameObject;
                }
            }

            return null;
        }
    }
}
