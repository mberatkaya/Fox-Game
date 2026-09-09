using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TilkiOyunu.Foundation.Tests
{
    public sealed class Sprint5FinalPresentationTests
    {
        private string tempDirectory;
        private string savePath;

        [SetUp]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "TilkiOyunuSprint5Tests", Guid.NewGuid().ToString("N"));
            savePath = Path.Combine(tempDirectory, SaveService.DefaultFileName);
            GameServices.Shutdown();
        }

        [TearDown]
        public void TearDown()
        {
            GameServices.Shutdown();
            foreach (FinalSequenceController controller in UnityEngine.Object.FindObjectsByType<FinalSequenceController>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                UnityEngine.Object.DestroyImmediate(controller.gameObject);
            }

            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Test]
        public void FinalCannotBeginWhileLocked()
        {
            QuestService service = CreateQuestService();
            FinalSequenceController controller = CreateFinalSequenceController();

            Assert.That(service.IsFinalCampUnlocked, Is.False);
            Assert.That(controller.CanBegin, Is.False);
            Assert.That(controller.Begin(), Is.False);
        }

        [Test]
        public void FinalCanBeginWhenUnlocked()
        {
            QuestService service = CreateQuestService();
            CompleteAllMainQuests(service);
            FinalSequenceController controller = CreateFinalSequenceController();

            Assert.That(service.IsFinalCampUnlocked, Is.True);
            Assert.That(controller.CanBegin, Is.True);
        }

        [Test]
        public void FinalCompletionPersists()
        {
            QuestService service = CreateQuestService();
            CompleteAllMainQuests(service);
            FinalSequenceController controller = CreateFinalSequenceController();

            controller.CompleteAndClose();
            QuestService loaded = new(new SaveService(savePath), null);

            Assert.That(loaded.SaveData.finalCompleted, Is.True);
            Assert.That(loaded.SaveData.gameState, Is.EqualTo(GameState.Completed));
        }

        [Test]
        public void AudioServiceAcceptsMissingMixer()
        {
            AudioService service = new(null);

            Assert.DoesNotThrow(() => service.SetMasterVolume(0.5f));
            Assert.DoesNotThrow(() => service.SetMusicVolume(0.25f));
            Assert.DoesNotThrow(() => service.SetAmbienceVolume(0.2f));
            Assert.DoesNotThrow(() => service.SetSfxVolume(0.7f));
        }

        [Test]
        public void FoxAnimatorBindingAssetsExist()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/Characters/PlayerFox.prefab");
            RuntimeAnimatorController animator = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/_Game/Art/Characters/FoxAnimatorController.controller");

            Assert.That(prefab, Is.Not.Null);
            Assert.That(prefab.transform.Find("VisualRoot/ToonFox"), Is.Not.Null);
            Assert.That(prefab.transform.Find("VisualRoot/OpenGameArtFox"), Is.Null);
            Assert.That(animator, Is.Not.Null);
            Assert.That(prefab.GetComponentInChildren<FoxAnimationDriver>(true), Is.Not.Null);
        }

        [Test]
        public void FoxImportedLocomotionClipsLoop()
        {
            AssertClipLoop("Fox_Idle", true);
            AssertClipLoop("Fox_Walk_InPlace", true);
            AssertClipLoop("Fox_Run_InPlace", true);
            AssertClipLoop("Fox_Jump_InAir", false);
        }

        [Test]
        public void FoxImportedLocomotionClipsMoveSkeleton()
        {
            AssertClipMovesSkeleton("Fox_Idle", 0.01f);
            AssertClipMovesSkeleton("Fox_Walk_InPlace", 0.05f);
            AssertClipMovesSkeleton("Fox_Run_InPlace", 0.05f);
        }

        [Test]
        public void FoxAnimatorUsesGroundedLocomotionBlendTree()
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/_Game/Art/Characters/FoxAnimatorController.controller");

            Assert.That(controller, Is.Not.Null);
            Assert.That(HasParameter(controller, "Speed", AnimatorControllerParameterType.Float), Is.True);
            Assert.That(HasParameter(controller, "Grounded", AnimatorControllerParameterType.Bool), Is.True);
            Assert.That(HasParameter(controller, "VerticalVelocity", AnimatorControllerParameterType.Float), Is.True);

            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            AnimatorState locomotion = stateMachine.defaultState;
            Assert.That(locomotion, Is.Not.Null);
            Assert.That(locomotion.name, Is.EqualTo("Locomotion"));
            Assert.That(locomotion.motion, Is.TypeOf<BlendTree>());

            BlendTree blendTree = (BlendTree)locomotion.motion;
            Assert.That(blendTree.blendParameter, Is.EqualTo("Speed"));
            Assert.That(blendTree.children.Length, Is.EqualTo(3));
            Assert.That(blendTree.children[0].motion.name, Is.EqualTo("Fox_Idle"));
            Assert.That(blendTree.children[0].threshold, Is.EqualTo(0f).Within(0.001f));
            Assert.That(blendTree.children[1].motion.name, Is.EqualTo("Fox_Walk_InPlace"));
            Assert.That(blendTree.children[1].threshold, Is.EqualTo(0.59f).Within(0.001f));
            Assert.That(blendTree.children[1].timeScale, Is.EqualTo(1.08f).Within(0.001f));
            Assert.That(blendTree.children[2].motion.name, Is.EqualTo("Fox_Run_InPlace"));
            Assert.That(blendTree.children[2].threshold, Is.EqualTo(1f).Within(0.001f));
            Assert.That(blendTree.children[2].timeScale, Is.EqualTo(1.16f).Within(0.001f));
        }

        [Test]
        public void FoxPrefabKeepsMovementRootSeparateFromGroundedVisual()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/Characters/PlayerFox.prefab");
            Transform visualRoot = prefab.transform.Find("VisualRoot");
            Transform foxVisual = prefab.transform.Find("VisualRoot/ToonFox");
            Animator animator = prefab.GetComponentInChildren<Animator>(true);
            CharacterController characterController = prefab.GetComponent<CharacterController>();

            Assert.That(visualRoot, Is.Not.Null);
            Assert.That(visualRoot.localPosition, Is.EqualTo(Vector3.zero));
            Assert.That(visualRoot.localScale, Is.EqualTo(Vector3.one));
            Assert.That(foxVisual, Is.Not.Null);
            Assert.That(foxVisual.localPosition.y, Is.InRange(0.045f, 0.065f));
            Assert.That(foxVisual.localEulerAngles.y, Is.EqualTo(0f).Within(0.001f));
            Assert.That(foxVisual.localScale.x, Is.EqualTo(0.62f).Within(0.001f));
            Assert.That(animator, Is.Not.Null);
            Assert.That(animator.applyRootMotion, Is.False);
            Assert.That(characterController.height, Is.EqualTo(1.12f).Within(0.001f));
            Assert.That(characterController.radius, Is.EqualTo(0.34f).Within(0.001f));
            Assert.That(characterController.center.y, Is.EqualTo(0.56f).Within(0.001f));

            float groundedBottom = foxVisual.localPosition.y + foxVisual.localScale.y * MeasureToonFoxGroundedRendererMinY();
            Assert.That(groundedBottom, Is.InRange(0.015f, 0.04f));
        }

        [Test]
        public void ForestCameraFramesFoxUpperBody()
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            Camera camera = Camera.main;
            ThirdPersonCameraController controller = camera.GetComponent<ThirdPersonCameraController>();
            SerializedObject serializedCamera = new(controller);

            Assert.That(camera.fieldOfView, Is.EqualTo(60f).Within(0.001f));
            Assert.That(serializedCamera.FindProperty("targetOffset").vector3Value, Is.EqualTo(new Vector3(0f, 0.35f, 0f)));
            Assert.That(serializedCamera.FindProperty("distance").floatValue, Is.EqualTo(5f).Within(0.001f));
            Assert.That(serializedCamera.FindProperty("minDistance").floatValue, Is.EqualTo(1.35f).Within(0.001f));
        }

        [Test]
        public void ForestBridgeHasBodyBlockingRailColliders()
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            AssertBridgeRailCollider("Bridge Left Rail Collider", -1.5f);
            AssertBridgeRailCollider("Bridge Right Rail Collider", 1.5f);
        }

        private QuestService CreateQuestService()
        {
            SaveService saveService = new(savePath);
            QuestService questService = new(saveService, null);
            GameServices.Initialize(saveService, new SceneService(), new AudioService(null), questService);
            return questService;
        }

        private static FinalSequenceController CreateFinalSequenceController()
        {
            GameObject gameObject = new("FinalSequenceControllerTest");
            FinalSequenceController controller = gameObject.AddComponent<FinalSequenceController>();
            SerializedObject serializedObject = new(controller);
            serializedObject.FindProperty("finalMessage").objectReferenceValue = ScriptableObject.CreateInstance<FinalMessageDefinition>();
            serializedObject.FindProperty("finalPanel").objectReferenceValue = gameObject.AddComponent<FinalMessagePanelUI>();
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return controller;
        }

        private static void CompleteAllMainQuests(QuestService service)
        {
            QuestDefinition memory = CreateQuest(QuestService.CollectMemoriesQuestId, QuestObjectiveType.CollectMemory, 5);
            QuestDefinition light = CreateQuest(QuestService.LightPathQuestId, QuestObjectiveType.CompleteLightPath, 5, QuestService.CollectMemoriesQuestId);
            QuestDefinition card = CreateQuest(QuestService.CardMatchingQuestId, QuestObjectiveType.CompleteCardMatch, 4, QuestService.LightPathQuestId);

            service.StartQuest(memory);
            service.AddProgress(memory, 5);
            service.TurnInQuest(memory);
            service.StartQuest(light);
            service.RecordLightPathCompleted(light);
            service.TurnInQuest(light);
            service.StartQuest(card);
            service.RecordCardMatchingCompleted(card);
            service.TurnInQuest(card);
        }

        private static QuestDefinition CreateQuest(string id, QuestObjectiveType objective, int required, string prerequisite = null)
        {
            QuestDefinition quest = ScriptableObject.CreateInstance<QuestDefinition>();
            SerializedObject serializedObject = new(quest);
            serializedObject.FindProperty("id").stringValue = id;
            serializedObject.FindProperty("title").stringValue = id;
            serializedObject.FindProperty("description").stringValue = id;
            serializedObject.FindProperty("objectiveType").enumValueIndex = (int)objective;
            serializedObject.FindProperty("requiredAmount").intValue = required;
            SerializedProperty prerequisites = serializedObject.FindProperty("prerequisiteQuestIds");
            prerequisites.arraySize = string.IsNullOrWhiteSpace(prerequisite) ? 0 : 1;
            if (prerequisites.arraySize == 1)
            {
                prerequisites.GetArrayElementAtIndex(0).stringValue = prerequisite;
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return quest;
        }

        private static void AssertClipLoop(string clipName, bool expectedLoop)
        {
            AnimationClip clip = FindClip(clipName);

            Assert.That(clip, Is.Not.Null);
            Assert.That(AnimationUtility.GetAnimationClipSettings(clip).loopTime, Is.EqualTo(expectedLoop));
        }

        private static AnimationClip FindClip(string clipName)
        {
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath($"Assets/Fox/Animations/{clipName}.fbx");
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] is AnimationClip clip && clip.name == clipName)
                {
                    return clip;
                }
            }

            return null;
        }

        private static void AssertClipMovesSkeleton(string clipName, float minimumDelta)
        {
            AnimationClip clip = FindClip(clipName);
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fox/Prefabs/Fox.prefab");
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

                Assert.That(delta, Is.GreaterThan(minimumDelta), $"{clipName} should visibly change the imported skeleton pose.");
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

        private static void AssertBridgeRailCollider(string name, float expectedX)
        {
            GameObject rail = GameObject.Find(name);
            Assert.That(rail, Is.Not.Null);
            Assert.That(rail.transform.parent.name, Is.EqualTo("Small Bridge"));
            Assert.That(rail.transform.localPosition.x, Is.EqualTo(expectedX).Within(0.001f));
            Assert.That(rail.transform.localPosition.y, Is.EqualTo(0.56f).Within(0.001f));

            BoxCollider collider = rail.GetComponent<BoxCollider>();
            Assert.That(collider, Is.Not.Null);
            Assert.That(collider.isTrigger, Is.False);
            Assert.That(collider.size, Is.EqualTo(new Vector3(0.18f, 0.9f, 4.45f)));
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

        private static bool HasParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
        {
            for (int i = 0; i < controller.parameters.Length; i++)
            {
                AnimatorControllerParameter parameter = controller.parameters[i];
                if (parameter.name == name && parameter.type == type)
                {
                    return true;
                }
            }

            return false;
        }

        private static float MeasureToonFoxGroundedRendererMinY()
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fox/Prefabs/Fox.prefab");
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(asset);
            try
            {
                float minY = float.PositiveInfinity;
                string[] clipNames = { "Fox_Idle", "Fox_Walk_InPlace", "Fox_Run_InPlace" };
                for (int clipIndex = 0; clipIndex < clipNames.Length; clipIndex++)
                {
                    AnimationClip clip = FindClip(clipNames[clipIndex]);
                    Assert.That(clip, Is.Not.Null);
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
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }
    }
}
