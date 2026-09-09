using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

namespace TilkiOyunu.Foundation.Editor
{
    public static class Sprint55FoxCharacterQuality
    {
        public const string ToonFoxPackageRoot = "Assets/Fox";
        public const string ToonFoxSourcePrefabPath = "Assets/Fox/Prefabs/Fox.prefab";
        public const string ToonFoxModelPath = "Assets/Fox/FBXs/Fox.fbx";
        public const string ToonFoxWrapperPrefabPath = "Assets/_Game/Prefabs/Characters/ToonFoxVisual.prefab";
        public const string PlayerPrefabPath = "Assets/_Game/Prefabs/Characters/PlayerFox.prefab";
        public const string AnimatorPath = "Assets/_Game/Art/Characters/FoxAnimatorController.controller";
        public const string MaterialPath = "Assets/_Game/Art/Characters/ToonFox_URP.mat";
        public const string BaseTexturePath = "Assets/Fox/Textures/T_Fox_BC.png";
        public const string NormalTexturePath = "Assets/Fox/Textures/T_Fox_Normal.png";
        public const string OcclusionTexturePath = "Assets/Fox/Textures/T_Fox_AO.png";

        private const string MixerPath = "Assets/_Game/Audio/Mixers/TilkiAudioMixer.mixer";
        private const string ToonFoxVisualName = "ToonFox";

        private const float ToonFoxVisualScale = 0.62f;
        private const float PawGroundClearance = 0.025f;
        private const float VisualYawDegrees = 0f;
        private const float WalkThreshold = 0.59f;
        private const float RunThreshold = 1f;
        private const float WalkPlaybackSpeed = 1.08f;
        private const float RunPlaybackSpeed = 1.16f;
        private const float SprintSpeed = 6.1f;
        private const float AnimatorDampSeconds = 0.1f;
        private const float CharacterHeight = 1.12f;
        private const float CharacterRadius = 0.34f;
        private const float CharacterCenterY = 0.56f;
        private const float CameraTargetY = 0.9f;
        private const float FootstepVolume = 0.24f;
        private const float FootstepPitchVariation = 0.035f;
        private const float FootstepMinSpeed = 0.35f;

        private const string IdleClipName = "Fox_Idle";
        private const string WalkClipName = "Fox_Walk_InPlace";
        private const string RunClipName = "Fox_Run_InPlace";
        private const string AirClipName = "Fox_Jump_InAir";

        [MenuItem("Tilki Oyunu/Sprint 5.5/Report Fox Character Data")]
        public static void ReportFoxCharacterData()
        {
            Debug.Log(BuildCharacterReport());
        }

        [MenuItem("Tilki Oyunu/Sprint 5.5/Apply Fox Character Quality")]
        public static void ApplyFoxCharacterQuality()
        {
            RequireToonFoxPackage();
            ConfigureFoxTextures();
            ConfigureFoxImporterLoopSettings();
            AnimatorController controller = EnsureFoxAnimatorController();
            EnsureFoxMaterial();
            EnsureToonFoxWrapperPrefab(controller);
            ApplyPlayerPrefabQuality(controller);
            ApplyForestCameraQuality();
            ApplyForestCollisionQuality();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void ApplyFoxCharacterQualityFromCommandLine()
        {
            ApplyFoxCharacterQuality();
            Debug.Log(BuildCharacterReport());
            EditorApplication.Exit(0);
        }

        public static void ReportFoxCharacterDataFromCommandLine()
        {
            Debug.Log(BuildCharacterReport());
            EditorApplication.Exit(0);
        }

        public static float CalculatePreferredVisualLocalY()
        {
            BoundsReport bounds = MeasureSampledPrefabBounds(LoadGroundAlignmentClips());
            return CalculateVisualLocalY(bounds.MinY, ToonFoxVisualScale, PawGroundClearance);
        }

        public static AnimatorController EnsureFoxAnimatorController()
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(AnimatorPath);
            if (controller == null || controller.layers == null || controller.layers.Length == 0)
            {
                if (controller != null)
                {
                    AssetDatabase.DeleteAsset(AnimatorPath);
                }

                controller = AnimatorController.CreateAnimatorControllerAtPath(AnimatorPath);
            }

            RemoveBlendTreeSubAssets();
            controller.parameters = Array.Empty<AnimatorControllerParameter>();
            AddAnimatorParameter(controller, "Speed", AnimatorControllerParameterType.Float);
            AddAnimatorParameter(controller, "Grounded", AnimatorControllerParameterType.Bool);
            AddAnimatorParameter(controller, "VerticalVelocity", AnimatorControllerParameterType.Float);

            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            ClearStateMachine(stateMachine);

            AnimationClip idle = RequireClip(IdleClipName);
            AnimationClip walk = RequireClip(WalkClipName);
            AnimationClip run = RequireClip(RunClipName);
            AnimationClip air = FindClip(AirClipName) ?? RequireClip("Fox_Jump");

            AnimatorState locomotionState = stateMachine.AddState("Locomotion", new Vector3(240f, 120f, 0f));
            BlendTree locomotion = new()
            {
                name = "Fox Locomotion",
                blendType = BlendTreeType.Simple1D,
                blendParameter = "Speed",
                useAutomaticThresholds = false
            };
            AssetDatabase.AddObjectToAsset(locomotion, controller);
            locomotionState.motion = locomotion;
            locomotionState.writeDefaultValues = true;
            locomotion.AddChild(idle, 0f);
            locomotion.AddChild(walk, WalkThreshold);
            locomotion.AddChild(run, RunThreshold);

            ChildMotion[] children = locomotion.children;
            children[0].timeScale = 1f;
            children[1].timeScale = WalkPlaybackSpeed;
            children[2].timeScale = RunPlaybackSpeed;
            locomotion.children = children;

            AnimatorState airState = stateMachine.AddState("Air", new Vector3(520f, 120f, 0f));
            airState.motion = air;
            airState.speed = 1f;
            airState.writeDefaultValues = true;

            stateMachine.defaultState = locomotionState;
            AddGroundedTransition(locomotionState, airState, false, 0.08f);
            AddGroundedTransition(airState, locomotionState, true, 0.12f);

            EditorUtility.SetDirty(controller);
            return controller;
        }

        public static void ConfigureLoadedPlayerPrefab(GameObject root, RuntimeAnimatorController animatorController)
        {
            if (root == null)
            {
                return;
            }

            FoxController controller = root.GetComponent<FoxController>();
            Transform visualRoot = root.transform.Find("VisualRoot") ?? root.transform.Find("Visual");
            if (visualRoot == null)
            {
                visualRoot = new GameObject("VisualRoot").transform;
                visualRoot.SetParent(root.transform, false);
            }

            visualRoot.name = "VisualRoot";
            visualRoot.localPosition = Vector3.zero;
            visualRoot.localRotation = Quaternion.identity;
            visualRoot.localScale = Vector3.one;

            RemoveAllVisualChildren(visualRoot);
            GameObject wrapperAsset = AssetDatabase.LoadAssetAtPath<GameObject>(ToonFoxWrapperPrefabPath);
            if (wrapperAsset == null)
            {
                throw new InvalidOperationException($"Missing Toon Fox wrapper prefab at {ToonFoxWrapperPrefabPath}.");
            }

            GameObject toonFox = (GameObject)PrefabUtility.InstantiatePrefab(wrapperAsset, visualRoot);
            toonFox.name = ToonFoxVisualName;
            toonFox.transform.localPosition = new Vector3(0f, CalculatePreferredVisualLocalY(), 0f);
            toonFox.transform.localRotation = Quaternion.Euler(0f, VisualYawDegrees, 0f);
            toonFox.transform.localScale = Vector3.one * ToonFoxVisualScale;
            ConfigureToonFoxInstance(toonFox, controller, animatorController);

            AudioSource source = EnsureComponent<AudioSource>(root);
            source.outputAudioMixerGroup = FindMixerGroup("SFX");
            source.playOnAwake = false;
            source.spatialBlend = 0.7f;

            FootstepAudio footsteps = EnsureComponent<FootstepAudio>(root);
            SetObject(footsteps, "controller", controller);
            SetObject(footsteps, "source", source);
            SetObjectArray(footsteps, "clips", LoadAudioClips("Assets/ThirdParty/OpenGameArt/SFX/Footsteps/leaves01.ogg", "Assets/ThirdParty/OpenGameArt/SFX/Footsteps/leaves02.ogg"));
            SetFloat(footsteps, "walkInterval", CalculateStepInterval(WalkClipName, WalkPlaybackSpeed));
            SetFloat(footsteps, "runInterval", CalculateStepInterval(RunClipName, RunPlaybackSpeed));
            SetFloat(footsteps, "volume", FootstepVolume);
            SetFloat(footsteps, "pitchVariation", FootstepPitchVariation);
            SetFloat(footsteps, "minSpeed", FootstepMinSpeed);

            CharacterController characterController = root.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.height = CharacterHeight;
                characterController.radius = CharacterRadius;
                characterController.center = new Vector3(0f, CharacterCenterY, 0f);
                characterController.skinWidth = 0.045f;
                characterController.stepOffset = 0.22f;
                characterController.slopeLimit = 48f;
            }

            Transform cameraTarget = root.transform.Find("Camera Target");
            if (cameraTarget != null)
            {
                cameraTarget.localPosition = new Vector3(0f, CameraTargetY, 0f);
            }
        }

        public static string BuildCharacterReport()
        {
            StringBuilder report = new();
            ModelImporter importer = AssetImporter.GetAtPath(ToonFoxModelPath) as ModelImporter;
            report.AppendLine("SPRINT55_TOON_FOX_REPORT_BEGIN");
            report.AppendLine($"Package root: {ToonFoxPackageRoot}");
            report.AppendLine($"Source prefab: {ToonFoxSourcePrefabPath}");
            report.AppendLine($"Product ID: {GetAssetOriginProductId(ToonFoxModelPath)}");
            if (importer == null)
            {
                report.AppendLine("Importer: MISSING");
            }
            else
            {
                report.AppendLine($"Importer animationType: {importer.animationType}");
                report.AppendLine($"Importer importAnimation: {importer.importAnimation}");
                report.AppendLine($"Importer globalScale: {Format(importer.globalScale)}");
                report.AppendLine($"Importer useFileScale: {importer.useFileScale}");
                report.AppendLine($"Importer avatarSetup: {importer.avatarSetup}");
                report.AppendLine($"Importer motionNodeName: {importer.motionNodeName}");
            }

            List<AnimationClip> clips = LoadToonFoxClips();
            report.AppendLine($"Clip count: {clips.Count}");
            for (int i = 0; i < clips.Count; i++)
            {
                AnimationClip clip = clips[i];
                AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
                report.AppendLine(
                    string.Join(
                        " | ",
                        "Clip",
                        clip.name,
                        $"duration={Format(clip.length)}",
                        $"frameRate={Format(clip.frameRate)}",
                        $"frames={Format(clip.length * clip.frameRate)}",
                        $"loop={settings.loopTime}",
                        $"used={IsProductionClip(clip.name)}",
                        $"rootCurves={clip.hasRootCurves}",
                        $"genericRoot={clip.hasGenericRootTransform}",
                        $"motionCurves={clip.hasMotionCurves}"));
            }

            BoundsReport rawBounds = MeasureSampledPrefabBounds(clips);
            BoundsReport groundBounds = MeasureSampledPrefabBounds(LoadGroundAlignmentClips());
            report.AppendLine($"Raw sampled minY: {Format(rawBounds.MinY)}");
            report.AppendLine($"Raw sampled maxY: {Format(rawBounds.MaxY)}");
            report.AppendLine($"Raw sampled height: {Format(rawBounds.MaxY - rawBounds.MinY)}");
            report.AppendLine($"Ground alignment sampled minY: {Format(groundBounds.MinY)}");
            report.AppendLine($"Ground alignment clips: {string.Join(", ", GroundAlignmentClipNames)}");
            report.AppendLine($"Scale: {Format(ToonFoxVisualScale)}");
            report.AppendLine($"Rotation Y: {Format(VisualYawDegrees)}");
            report.AppendLine($"Calculated visual localY: {Format(CalculateVisualLocalY(groundBounds.MinY, ToonFoxVisualScale, PawGroundClearance))}");
            report.AppendLine($"Target paw clearance: {Format(PawGroundClearance)}");
            report.AppendLine("Per-clip sampled bounds:");
            foreach (KeyValuePair<string, BoundsReport> entry in rawBounds.PerClip)
            {
                BoundsReport bounds = entry.Value;
                report.AppendLine($"{entry.Key} | minY={Format(bounds.MinY)} | maxY={Format(bounds.MaxY)} | height={Format(bounds.MaxY - bounds.MinY)} | skeletonDelta={Format(CalculateSkeletonDelta(entry.Key))}");
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (prefab != null)
            {
                CharacterController characterController = prefab.GetComponent<CharacterController>();
                Transform visualRoot = prefab.transform.Find("VisualRoot");
                Transform foxVisual = prefab.transform.Find($"VisualRoot/{ToonFoxVisualName}");
                Transform cameraTarget = prefab.transform.Find("Camera Target");
                report.AppendLine($"Prefab VisualRoot local: {FormatTransform(visualRoot)}");
                report.AppendLine($"Prefab {ToonFoxVisualName} local: {FormatTransform(foxVisual)}");
                report.AppendLine($"Prefab CameraTarget local: {FormatTransform(cameraTarget)}");
                if (characterController != null)
                {
                    report.AppendLine($"CharacterController height={Format(characterController.height)} radius={Format(characterController.radius)} center={Format(characterController.center)} skinWidth={Format(characterController.skinWidth)} stepOffset={Format(characterController.stepOffset)} slopeLimit={Format(characterController.slopeLimit)}");
                }
            }

            report.AppendLine("SPRINT55_TOON_FOX_REPORT_END");
            return report.ToString();
        }

        private static void RequireToonFoxPackage()
        {
            RequireAsset<GameObject>(ToonFoxSourcePrefabPath, "Toon Fox source prefab");
            RequireAsset<GameObject>(ToonFoxModelPath, "Toon Fox model");
            RequireAsset<Texture2D>(BaseTexturePath, "Toon Fox base color texture");
            RequireAsset<Texture2D>(NormalTexturePath, "Toon Fox normal texture");
            RequireAsset<Texture2D>(OcclusionTexturePath, "Toon Fox occlusion texture");
            RequireClip(IdleClipName);
            RequireClip(WalkClipName);
            RequireClip(RunClipName);
        }

        private static void ConfigureFoxImporterLoopSettings()
        {
            foreach (string path in Directory.GetFiles("Assets/Fox/Animations", "*.fbx", SearchOption.TopDirectoryOnly))
            {
                string assetPath = path.Replace('\\', '/');
                ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                if (importer == null)
                {
                    continue;
                }

                importer.animationType = ModelImporterAnimationType.Generic;
                importer.avatarSetup = ModelImporterAvatarSetup.CopyFromOther;
                importer.sourceAvatar = AssetDatabase.LoadAssetAtPath<Avatar>(ToonFoxModelPath);
                importer.optimizeGameObjects = false;
                importer.preserveHierarchy = true;

                ModelImporterClipAnimation[] clips = importer.clipAnimations.Length > 0
                    ? importer.clipAnimations
                    : importer.defaultClipAnimations;
                for (int i = 0; i < clips.Length; i++)
                {
                    bool loops = IsLoopingClip(clips[i].name);
                    clips[i].loopTime = loops;
                    clips[i].loopPose = loops;
                    clips[i].keepOriginalPositionY = true;
                    clips[i].keepOriginalPositionXZ = assetPath.Contains("_InPlace");
                }

                importer.clipAnimations = clips;
                importer.SaveAndReimport();
            }
        }

        private static void ConfigureFoxTextures()
        {
            TextureImporter normalImporter = AssetImporter.GetAtPath(NormalTexturePath) as TextureImporter;
            if (normalImporter != null && normalImporter.textureType != TextureImporterType.NormalMap)
            {
                normalImporter.textureType = TextureImporterType.NormalMap;
                normalImporter.SaveAndReimport();
            }
        }

        private static Material EnsureFoxMaterial()
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                material = new Material(shader)
                {
                    name = "ToonFox_URP"
                };
                AssetDatabase.CreateAsset(material, MaterialPath);
            }
            else
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader != null)
                {
                    material.shader = shader;
                }
            }

            Texture2D baseColor = AssetDatabase.LoadAssetAtPath<Texture2D>(BaseTexturePath);
            Texture2D normal = AssetDatabase.LoadAssetAtPath<Texture2D>(NormalTexturePath);
            Texture2D occlusion = AssetDatabase.LoadAssetAtPath<Texture2D>(OcclusionTexturePath);
            if (material.HasProperty("_BaseMap"))
            {
                material.SetTexture("_BaseMap", baseColor);
                material.SetColor("_BaseColor", Color.white);
            }
            else if (material.HasProperty("_MainTex"))
            {
                material.SetTexture("_MainTex", baseColor);
                material.SetColor("_Color", Color.white);
            }

            if (material.HasProperty("_BumpMap"))
            {
                material.SetTexture("_BumpMap", normal);
                material.EnableKeyword("_NORMALMAP");
            }

            if (material.HasProperty("_OcclusionMap"))
            {
                material.SetTexture("_OcclusionMap", occlusion);
                material.SetFloat("_OcclusionStrength", 0.75f);
            }

            if (material.HasProperty("_Metallic"))
            {
                material.SetFloat("_Metallic", 0f);
            }

            if (material.HasProperty("_Smoothness"))
            {
                material.SetFloat("_Smoothness", 0.18f);
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static GameObject EnsureToonFoxWrapperPrefab(RuntimeAnimatorController animatorController)
        {
            GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(ToonFoxSourcePrefabPath);
            if (source == null)
            {
                throw new InvalidOperationException($"Missing Toon Fox source prefab at {ToonFoxSourcePrefabPath}.");
            }

            EnsureDirectory(Path.GetDirectoryName(ToonFoxWrapperPrefabPath));
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(source);
            try
            {
                instance.name = ToonFoxVisualName;
                instance.transform.localPosition = new Vector3(0f, CalculatePreferredVisualLocalY(), 0f);
                instance.transform.localRotation = Quaternion.Euler(0f, VisualYawDegrees, 0f);
                instance.transform.localScale = Vector3.one * ToonFoxVisualScale;
                ConfigureToonFoxInstance(instance, null, animatorController);
                return PrefabUtility.SaveAsPrefabAsset(instance, ToonFoxWrapperPrefabPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }

        private static void ConfigureToonFoxInstance(GameObject toonFox, FoxController controller, RuntimeAnimatorController animatorController)
        {
            Animator animator = toonFox.GetComponent<Animator>();
            if (animator == null)
            {
                animator = toonFox.AddComponent<Animator>();
            }

            animator.runtimeAnimatorController = animatorController;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            FoxAnimationDriver animationDriver = EnsureComponent<FoxAnimationDriver>(toonFox);
            SetObject(animationDriver, "controller", controller);
            SetObject(animationDriver, "animator", animator);
            SetFloat(animationDriver, "runSpeed", SprintSpeed);
            SetFloat(animationDriver, "dampSeconds", AnimatorDampSeconds);

            AssignFoxMaterial(toonFox);
        }

        private static void AssignFoxMaterial(GameObject model)
        {
            Material material = EnsureFoxMaterial();
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].sharedMaterial = material;
            }
        }

        private static void ApplyPlayerPrefabQuality(RuntimeAnimatorController animatorController)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
            try
            {
                ConfigureLoadedPlayerPrefab(root, animatorController);
                PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void ApplyForestCameraQuality()
        {
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            Camera camera = Camera.main ?? UnityEngine.Object.FindFirstObjectByType<Camera>(FindObjectsInactive.Include);
            if (camera != null)
            {
                camera.fieldOfView = 60f;
                ThirdPersonCameraController cameraController = camera.GetComponent<ThirdPersonCameraController>();
                if (cameraController != null)
                {
                    SetVector3(cameraController, "targetOffset", new Vector3(0f, 0.35f, 0f));
                    SetFloat(cameraController, "distance", 5f);
                    SetFloat(cameraController, "minDistance", 1.35f);
                    SetFloat(cameraController, "collisionRadius", 0.2f);

                    ForestGameplayBootstrap bootstrap = UnityEngine.Object.FindFirstObjectByType<ForestGameplayBootstrap>(FindObjectsInactive.Include);
                    if (bootstrap != null)
                    {
                        SetObject(bootstrap, "cameraController", cameraController);
                    }
                }
            }

            EditorSceneManager.SaveScene(scene);
        }

        private static void ApplyForestCollisionQuality()
        {
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            GameObject bridge = GameObject.Find("Small Bridge");
            if (bridge != null)
            {
                EnsureBridgeRailCollider(bridge.transform, "Bridge Left Rail Collider", new Vector3(-1.5f, 0.56f, 0f));
                EnsureBridgeRailCollider(bridge.transform, "Bridge Right Rail Collider", new Vector3(1.5f, 0.56f, 0f));
            }

            EditorSceneManager.SaveScene(scene);
        }

        private static List<AnimationClip> LoadToonFoxClips()
        {
            List<AnimationClip> clips = new();
            foreach (string path in Directory.GetFiles("Assets/Fox/Animations", "*.fbx", SearchOption.TopDirectoryOnly))
            {
                string assetPath = path.Replace('\\', '/');
                UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
                for (int i = 0; i < assets.Length; i++)
                {
                    if (assets[i] is AnimationClip clip && !clip.name.StartsWith("__preview__", StringComparison.OrdinalIgnoreCase))
                    {
                        clips.Add(clip);
                    }
                }
            }

            clips.Sort((left, right) => string.Compare(left.name, right.name, StringComparison.OrdinalIgnoreCase));
            return clips;
        }

        private static readonly string[] GroundAlignmentClipNames =
        {
            IdleClipName,
            WalkClipName,
            RunClipName
        };

        private static List<AnimationClip> LoadGroundAlignmentClips()
        {
            List<AnimationClip> clips = new();
            for (int i = 0; i < GroundAlignmentClipNames.Length; i++)
            {
                clips.Add(RequireClip(GroundAlignmentClipNames[i]));
            }

            return clips;
        }

        private static AnimationClip RequireClip(string clipName)
        {
            AnimationClip clip = FindClip(clipName);
            if (clip == null)
            {
                throw new InvalidOperationException($"Missing required Toon Fox animation clip '{clipName}'.");
            }

            return clip;
        }

        private static AnimationClip FindClip(string clipName)
        {
            List<AnimationClip> clips = LoadToonFoxClips();
            for (int i = 0; i < clips.Count; i++)
            {
                if (clips[i].name == clipName)
                {
                    return clips[i];
                }
            }

            return null;
        }

        private static BoundsReport MeasureSampledPrefabBounds(IReadOnlyList<AnimationClip> clips)
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(ToonFoxSourcePrefabPath);
            if (asset == null)
            {
                return BoundsReport.Empty();
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(asset);
            instance.hideFlags = HideFlags.HideAndDontSave;
            instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            instance.transform.localScale = Vector3.one;

            try
            {
                BoundsReport total = BoundsReport.Empty();
                AccumulateCurrentBounds(instance, ref total);
                for (int clipIndex = 0; clipIndex < clips.Count; clipIndex++)
                {
                    AnimationClip clip = clips[clipIndex];
                    BoundsReport clipBounds = BoundsReport.Empty();
                    int sampleCount = Mathf.Max(8, Mathf.CeilToInt(clip.length * 30f));
                    for (int sample = 0; sample <= sampleCount; sample++)
                    {
                        float time = clip.length * sample / sampleCount;
                        clip.SampleAnimation(instance, time);
                        AccumulateCurrentBounds(instance, ref clipBounds);
                        AccumulateCurrentBounds(instance, ref total);
                    }

                    total.PerClip[clip.name] = clipBounds;
                }

                return total;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }

        private static void AccumulateCurrentBounds(GameObject root, ref BoundsReport report)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null || !renderer.enabled)
                {
                    continue;
                }

                Bounds bounds = renderer.bounds;
                report.MinY = Mathf.Min(report.MinY, bounds.min.y);
                report.MaxY = Mathf.Max(report.MaxY, bounds.max.y);
            }
        }

        private static float CalculateSkeletonDelta(string clipName)
        {
            AnimationClip clip = FindClip(clipName);
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(ToonFoxSourcePrefabPath);
            if (clip == null || asset == null)
            {
                return 0f;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(asset);
            try
            {
                clip.SampleAnimation(instance, 0f);
                Dictionary<string, PoseSample> start = CapturePose(instance.transform);
                clip.SampleAnimation(instance, clip.length * 0.5f);
                Dictionary<string, PoseSample> middle = CapturePose(instance.transform);

                float delta = 0f;
                foreach (KeyValuePair<string, PoseSample> entry in start)
                {
                    if (middle.TryGetValue(entry.Key, out PoseSample value))
                    {
                        delta += Vector3.Distance(entry.Value.Position, value.Position);
                        delta += Quaternion.Angle(entry.Value.Rotation, value.Rotation) / 180f;
                    }
                }

                return delta;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }

        private static Dictionary<string, PoseSample> CapturePose(Transform root)
        {
            Dictionary<string, PoseSample> samples = new();
            Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                samples[GetPath(root, transforms[i])] = new PoseSample(transforms[i].localPosition, transforms[i].localRotation);
            }

            return samples;
        }

        private static string GetPath(Transform root, Transform transform)
        {
            if (transform == root)
            {
                return root.name;
            }

            return GetPath(root, transform.parent) + "/" + transform.name;
        }

        private static float CalculateVisualLocalY(float sampledMinY, float visualScale, float clearance)
        {
            return clearance - sampledMinY * visualScale;
        }

        private static float CalculateStepInterval(string clipName, float playbackSpeed)
        {
            AnimationClip clip = RequireClip(clipName);
            return Mathf.Clamp((clip.length / playbackSpeed) * 0.5f, 0.12f, 0.55f);
        }

        private static bool IsLoopingClip(string clipName)
        {
            string lower = clipName.ToLowerInvariant();
            return lower.Contains("idle") || lower.Contains("walk") || lower.Contains("run");
        }

        private static bool IsProductionClip(string clipName)
        {
            return clipName == IdleClipName || clipName == WalkClipName || clipName == RunClipName || clipName == AirClipName;
        }

        private static void EnsureBridgeRailCollider(Transform parent, string name, Vector3 localPosition)
        {
            Transform child = parent.Find(name);
            if (child == null)
            {
                child = new GameObject(name).transform;
                child.SetParent(parent, false);
            }

            child.localPosition = localPosition;
            child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one;

            BoxCollider collider = child.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = child.gameObject.AddComponent<BoxCollider>();
            }

            collider.isTrigger = false;
            collider.center = Vector3.zero;
            collider.size = new Vector3(0.18f, 0.9f, 4.45f);
        }

        private static void RemoveAllVisualChildren(Transform visualRoot)
        {
            for (int i = visualRoot.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(visualRoot.GetChild(i).gameObject);
            }
        }

        private static T EnsureComponent<T>(GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }

        private static AudioClip[] LoadAudioClips(params string[] paths)
        {
            AudioClip[] clips = new AudioClip[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                clips[i] = AssetDatabase.LoadAssetAtPath<AudioClip>(paths[i]);
            }

            return clips;
        }

        private static AudioMixerGroup FindMixerGroup(string groupName)
        {
            AudioMixer mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(MixerPath);
            if (mixer == null)
            {
                return null;
            }

            AudioMixerGroup[] groups = mixer.FindMatchingGroups(groupName);
            return groups.Length > 0 ? groups[0] : null;
        }

        private static void SetObject(UnityEngine.Object target, string fieldName, UnityEngine.Object value)
        {
            if (target == null)
            {
                return;
            }

            SerializedObject serialized = new(target);
            serialized.FindProperty(fieldName).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetObjectArray(UnityEngine.Object target, string fieldName, UnityEngine.Object[] values)
        {
            SerializedObject serialized = new(target);
            SerializedProperty property = serialized.FindProperty(fieldName);
            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetFloat(UnityEngine.Object target, string fieldName, float value)
        {
            SerializedObject serialized = new(target);
            serialized.FindProperty(fieldName).floatValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetVector3(UnityEngine.Object target, string fieldName, Vector3 value)
        {
            SerializedObject serialized = new(target);
            serialized.FindProperty(fieldName).vector3Value = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AddAnimatorParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
        {
            controller.AddParameter(name, type);
        }

        private static void AddGroundedTransition(AnimatorState from, AnimatorState to, bool grounded, float duration)
        {
            AnimatorStateTransition transition = from.AddTransition(to);
            transition.hasExitTime = false;
            transition.duration = duration;
            transition.hasFixedDuration = true;
            transition.canTransitionToSelf = false;
            transition.AddCondition(grounded ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot, 0f, "Grounded");
        }

        private static void ClearStateMachine(AnimatorStateMachine stateMachine)
        {
            ChildAnimatorState[] states = stateMachine.states;
            for (int i = 0; i < states.Length; i++)
            {
                stateMachine.RemoveState(states[i].state);
            }

            ChildAnimatorStateMachine[] childMachines = stateMachine.stateMachines;
            for (int i = 0; i < childMachines.Length; i++)
            {
                stateMachine.RemoveStateMachine(childMachines[i].stateMachine);
            }

            AnimatorStateTransition[] anyStateTransitions = stateMachine.anyStateTransitions;
            for (int i = 0; i < anyStateTransitions.Length; i++)
            {
                stateMachine.RemoveAnyStateTransition(anyStateTransitions[i]);
            }
        }

        private static void RemoveBlendTreeSubAssets()
        {
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(AnimatorPath);
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] is BlendTree blendTree)
                {
                    UnityEngine.Object.DestroyImmediate(blendTree, true);
                }
            }
        }

        private static void RequireAsset<T>(string path, string label) where T : UnityEngine.Object
        {
            if (AssetDatabase.LoadAssetAtPath<T>(path) == null)
            {
                throw new InvalidOperationException($"{label} is missing at {path}.");
            }
        }

        private static void EnsureDirectory(string directory)
        {
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static string GetAssetOriginProductId(string assetPath)
        {
            string metaPath = assetPath + ".meta";
            if (!File.Exists(metaPath))
            {
                return "UNKNOWN";
            }

            foreach (string line in File.ReadLines(metaPath))
            {
                string trimmed = line.Trim();
                if (trimmed.StartsWith("productId:", StringComparison.Ordinal))
                {
                    return trimmed.Substring("productId:".Length).Trim();
                }
            }

            return "UNKNOWN";
        }

        private static string FormatTransform(Transform transform)
        {
            if (transform == null)
            {
                return "MISSING";
            }

            return $"pos={Format(transform.localPosition)} rot={Format(transform.localEulerAngles)} scale={Format(transform.localScale)}";
        }

        private static string Format(Vector3 value)
        {
            return $"({Format(value.x)}, {Format(value.y)}, {Format(value.z)})";
        }

        private static string Format(float value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private readonly struct PoseSample
        {
            public PoseSample(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
            }

            public Vector3 Position { get; }
            public Quaternion Rotation { get; }
        }

        private struct BoundsReport
        {
            public float MinY;
            public float MaxY;
            public Dictionary<string, BoundsReport> PerClip;

            public static BoundsReport Empty()
            {
                return new BoundsReport
                {
                    MinY = float.PositiveInfinity,
                    MaxY = float.NegativeInfinity,
                    PerClip = new Dictionary<string, BoundsReport>()
                };
            }
        }
    }
}
