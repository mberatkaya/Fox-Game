using System;
using System.IO;
using NUnit.Framework;
using UnityEditor;
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
            Assert.That(prefab.transform.Find("VisualRoot/QuaterniusFox/FoxModel"), Is.Not.Null);
            Assert.That(animator, Is.Not.Null);
            Assert.That(prefab.GetComponentInChildren<FoxAnimationDriver>(true), Is.Not.Null);
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
    }
}
