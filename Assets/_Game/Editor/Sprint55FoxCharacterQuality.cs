using System;
using System.Collections.Generic;
using System.Globalization;
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
        private const string FoxFbxPath = "Assets/ThirdParty/Quaternius/UltimateAnimatedAnimals/Fox/Fox.fbx";
        private const string PlayerPrefabPath = "Assets/_Game/Prefabs/Characters/PlayerFox.prefab";
        private const string AnimatorPath = "Assets/_Game/Art/Characters/FoxAnimatorController.controller";
        private const string MixerPath = "Assets/_Game/Audio/Mixers/TilkiAudioMixer.mixer";

        private const float FoxVisualScale = 0.62f;
        private const float PawGroundClearance = 0.025f;
        private const float WalkThreshold = 0.59f;
        private const float RunThreshold = 1f;
        private const float WalkPlaybackSpeed = 1.35f;
        private const float RunPlaybackSpeed = 1.18f;
        private const float SprintSpeed = 6.1f;
        private const float AnimatorDampSeconds = 0.1f;
        private const float CharacterHeight = 1.52f;
        private const float CharacterRadius = 0.34f;
        private const float CharacterCenterY = 0.76f;
        private const float CameraTargetY = 0.9f;
        private const float FootstepVolume = 0.24f;
        private const float FootstepPitchVariation = 0.035f;
        private const float FootstepMinSpeed = 0.35f;

        private const string IdleClipName = "AnimalArmature|Idle";
        private const string WalkClipName = "AnimalArmature|Walk";
        private const string RunClipName = "AnimalArmature|Gallop";
        private const string AirClipName = "AnimalArmature|Gallop_Jump";

        [MenuItem("Tilki Oyunu/Sprint 5.5/Report Fox Character Data")]
        public static void ReportFoxCharacterData()
        {
            Debug.Log(BuildCharacterReport());
        }

        [MenuItem("Tilki Oyunu/Sprint 5.5/Apply Fox Character Quality")]
        public static void ApplyFoxCharacterQuality()
        {
            ConfigureFoxImporterLoopSettings();
            AnimatorController controller = EnsureFoxAnimatorController();
            ApplyPlayerPrefabQuality(controller);
            ApplyForestCameraQuality();
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
            BoundsReport bounds = MeasureSampledFbxBounds(LoadFoxClips());
            return CalculateVisualLocalY(bounds.MinY, FoxVisualScale, PawGroundClearance);
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
            AnimationClip air = FindClip(AirClipName) ?? RequireClip("AnimalArmature|Jump_ToIdle");

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

            Transform foxVisual = visualRoot.Find("QuaterniusFox");
            if (foxVisual == null)
            {
                foxVisual = new GameObject("QuaterniusFox").transform;
                foxVisual.SetParent(visualRoot, false);
            }

            EnsureFoxModelChild(foxVisual);
            foxVisual.localPosition = new Vector3(0f, CalculatePreferredVisualLocalY(), 0f);
            foxVisual.localRotation = Quaternion.identity;
            foxVisual.localScale = Vector3.one * FoxVisualScale;

            Animator animator = foxVisual.GetComponent<Animator>();
            if (animator == null)
            {
                animator = foxVisual.gameObject.AddComponent<Animator>();
            }

            animator.runtimeAnimatorController = animatorController;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            FoxAnimationDriver animationDriver = EnsureComponent<FoxAnimationDriver>(foxVisual.gameObject);
            SetObject(animationDriver, "controller", controller);
            SetObject(animationDriver, "animator", animator);
            SetFloat(animationDriver, "runSpeed", SprintSpeed);
            SetFloat(animationDriver, "dampSeconds", AnimatorDampSeconds);

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
            ModelImporter importer = AssetImporter.GetAtPath(FoxFbxPath) as ModelImporter;
            report.AppendLine("SPRINT55_FOX_REPORT_BEGIN");
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
                report.AppendLine($"Importer clipAnimations: {importer.clipAnimations.Length}");
                report.AppendLine($"Importer avatarSetup: {importer.avatarSetup}");
                report.AppendLine($"Importer motionNodeName: {importer.motionNodeName}");
            }

            List<AnimationClip> clips = LoadFoxClips();
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
                        $"rootCurves={clip.hasRootCurves}",
                        $"genericRoot={clip.hasGenericRootTransform}",
                        $"motionCurves={clip.hasMotionCurves}"));
            }

            BoundsReport rawBounds = MeasureSampledFbxBounds(clips);
            report.AppendLine($"Raw sampled minY: {Format(rawBounds.MinY)}");
            report.AppendLine($"Raw sampled maxY: {Format(rawBounds.MaxY)}");
            report.AppendLine($"Raw sampled height: {Format(rawBounds.MaxY - rawBounds.MinY)}");
            report.AppendLine($"Scale: {Format(FoxVisualScale)}");
            report.AppendLine($"Calculated visual localY: {Format(CalculateVisualLocalY(rawBounds.MinY, FoxVisualScale, PawGroundClearance))}");
            report.AppendLine($"Target paw clearance: {Format(PawGroundClearance)}");
            report.AppendLine("Per-clip sampled bounds:");
            foreach (KeyValuePair<string, BoundsReport> entry in rawBounds.PerClip)
            {
                BoundsReport bounds = entry.Value;
                report.AppendLine($"{entry.Key} | minY={Format(bounds.MinY)} | maxY={Format(bounds.MaxY)} | height={Format(bounds.MaxY - bounds.MinY)}");
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (prefab != null)
            {
                CharacterController characterController = prefab.GetComponent<CharacterController>();
                Transform visualRoot = prefab.transform.Find("VisualRoot");
                Transform foxVisual = prefab.transform.Find("VisualRoot/QuaterniusFox");
                Transform cameraTarget = prefab.transform.Find("Camera Target");
                report.AppendLine($"Prefab VisualRoot local: {FormatTransform(visualRoot)}");
                report.AppendLine($"Prefab QuaterniusFox local: {FormatTransform(foxVisual)}");
                report.AppendLine($"Prefab CameraTarget local: {FormatTransform(cameraTarget)}");
                if (characterController != null)
                {
                    report.AppendLine($"CharacterController height={Format(characterController.height)} radius={Format(characterController.radius)} center={Format(characterController.center)}");
                }
            }

            report.AppendLine("SPRINT55_FOX_REPORT_END");
            return report.ToString();
        }

        private static void ConfigureFoxImporterLoopSettings()
        {
            ModelImporter importer = AssetImporter.GetAtPath(FoxFbxPath) as ModelImporter;
            if (importer == null)
            {
                throw new InvalidOperationException($"Missing fox FBX importer at {FoxFbxPath}.");
            }

            ModelImporterClipAnimation[] clips = importer.defaultClipAnimations;
            for (int i = 0; i < clips.Length; i++)
            {
                bool loops = IsLoopingClip(clips[i].name);
                clips[i].loopTime = loops;
                clips[i].loopPose = loops;
            }

            importer.clipAnimations = clips;
            importer.SaveAndReimport();
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

        private static List<AnimationClip> LoadFoxClips()
        {
            List<AnimationClip> clips = new();
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(FoxFbxPath);
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] is AnimationClip clip && !clip.name.StartsWith("__preview__", StringComparison.OrdinalIgnoreCase))
                {
                    clips.Add(clip);
                }
            }

            clips.Sort((left, right) => string.Compare(left.name, right.name, StringComparison.OrdinalIgnoreCase));
            return clips;
        }

        private static AnimationClip RequireClip(string clipName)
        {
            AnimationClip clip = FindClip(clipName);
            if (clip == null)
            {
                throw new InvalidOperationException($"Missing required fox animation clip '{clipName}'.");
            }

            return clip;
        }

        private static AnimationClip FindClip(string clipName)
        {
            List<AnimationClip> clips = LoadFoxClips();
            for (int i = 0; i < clips.Count; i++)
            {
                if (string.Equals(clips[i].name, clipName, StringComparison.Ordinal))
                {
                    return clips[i];
                }
            }

            return null;
        }

        private static BoundsReport MeasureSampledFbxBounds(IReadOnlyList<AnimationClip> clips)
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(FoxFbxPath);
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
            return string.Equals(clipName, IdleClipName, StringComparison.Ordinal)
                || string.Equals(clipName, "AnimalArmature|Idle_2", StringComparison.Ordinal)
                || string.Equals(clipName, "AnimalArmature|Idle_2_HeadLow", StringComparison.Ordinal)
                || string.Equals(clipName, "AnimalArmature|Eating", StringComparison.Ordinal)
                || string.Equals(clipName, WalkClipName, StringComparison.Ordinal)
                || string.Equals(clipName, RunClipName, StringComparison.Ordinal);
        }

        private static void EnsureFoxModelChild(Transform foxVisual)
        {
            if (foxVisual.Find("FoxModel") != null)
            {
                return;
            }

            GameObject foxAsset = AssetDatabase.LoadAssetAtPath<GameObject>(FoxFbxPath);
            if (foxAsset == null)
            {
                return;
            }

            GameObject model = (GameObject)PrefabUtility.InstantiatePrefab(foxAsset, foxVisual);
            model.name = "FoxModel";
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
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
