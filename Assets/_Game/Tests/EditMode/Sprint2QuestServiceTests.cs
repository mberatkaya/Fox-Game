using System;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace TilkiOyunu.Foundation.Tests
{
    public sealed class Sprint2QuestServiceTests
    {
        private string tempDirectory;
        private string savePath;

        [SetUp]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "TilkiOyunuSprint2Tests", Guid.NewGuid().ToString("N"));
            savePath = Path.Combine(tempDirectory, SaveService.DefaultFileName);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Test]
        public void QuestStartsCorrectly()
        {
            QuestDefinition quest = CreateQuest(5);
            QuestService service = CreateService();

            bool started = service.StartQuest(quest);
            QuestState state = service.GetQuestState(quest);

            Assert.That(started, Is.True);
            Assert.That(state.Status, Is.EqualTo(QuestStatus.Active));
            Assert.That(state.CurrentAmount, Is.EqualTo(0));
        }

        [Test]
        public void DuplicateMemoryIdDoesNotIncreaseProgress()
        {
            QuestDefinition quest = CreateQuest(5);
            MemoryDefinition memoryA = CreateMemory("memory_01");
            MemoryDefinition memoryB = CreateMemory("memory_01");
            QuestService service = CreateService();

            service.StartQuest(quest);
            bool firstCollected = service.RecordMemoryCollected(memoryA, quest);
            bool duplicateCollected = service.RecordMemoryCollected(memoryB, quest);

            Assert.That(firstCollected, Is.True);
            Assert.That(duplicateCollected, Is.False);
            Assert.That(service.GetQuestState(quest).CurrentAmount, Is.EqualTo(1));
        }

        [Test]
        public void FiveUniqueMemoriesReadyQuestForTurnIn()
        {
            QuestDefinition quest = CreateQuest(5);
            QuestService service = CreateService();

            service.StartQuest(quest);
            for (int i = 1; i <= 5; i++)
            {
                service.RecordMemoryCollected(CreateMemory($"memory_{i:00}"), quest);
            }

            QuestState state = service.GetQuestState(quest);
            Assert.That(state.CurrentAmount, Is.EqualTo(5));
            Assert.That(state.Status, Is.EqualTo(QuestStatus.ReadyToTurnIn));
        }

        [Test]
        public void TurnInCompletesQuest()
        {
            QuestDefinition quest = CreateQuest(1);
            QuestService service = CreateService();

            service.StartQuest(quest);
            service.RecordMemoryCollected(CreateMemory("memory_01"), quest);
            bool turnedIn = service.TurnInQuest(quest);

            Assert.That(turnedIn, Is.True);
            Assert.That(service.GetQuestState(quest).Status, Is.EqualTo(QuestStatus.Completed));
        }

        [Test]
        public void SaveLoadQuestProgressIsPreserved()
        {
            QuestDefinition quest = CreateQuest(5);
            QuestService firstService = CreateService();
            firstService.StartQuest(quest);
            firstService.RecordMemoryCollected(CreateMemory("memory_01"), quest);
            firstService.RecordMemoryCollected(CreateMemory("memory_02"), quest);

            QuestService loadedService = new(new SaveService(savePath), null);

            QuestState loadedState = loadedService.GetQuestState(quest);
            Assert.That(loadedState.Status, Is.EqualTo(QuestStatus.Active));
            Assert.That(loadedState.CurrentAmount, Is.EqualTo(2));
        }

        [Test]
        public void CollectedMemoryIdsArePreserved()
        {
            QuestDefinition quest = CreateQuest(5);
            QuestService firstService = CreateService();
            firstService.StartQuest(quest);
            firstService.RecordMemoryCollected(CreateMemory("memory_01"), quest);

            QuestService loadedService = new(new SaveService(savePath), null);

            Assert.That(loadedService.HasCollectedMemory("memory_01"), Is.True);
        }

        [Test]
        public void OldSaveMissingFieldsLoadsSafely()
        {
            Directory.CreateDirectory(tempDirectory);
            File.WriteAllText(savePath, "{\"saveVersion\":1}");
            SaveService saveService = new(savePath);

            SaveGameData loaded = saveService.Load();

            Assert.That(loaded.quests, Is.Not.Null);
            Assert.That(loaded.collectedMemoryIds, Is.Not.Null);
        }

        private QuestService CreateService()
        {
            return new QuestService(new SaveService(savePath), null);
        }

        private static QuestDefinition CreateQuest(int requiredAmount)
        {
            QuestDefinition quest = ScriptableObject.CreateInstance<QuestDefinition>();
            SerializedObject serializedObject = new(quest);
            serializedObject.FindProperty("id").stringValue = QuestService.CollectMemoriesQuestId;
            serializedObject.FindProperty("title").stringValue = "Dağılmış Anılar";
            serializedObject.FindProperty("description").stringValue = "Ormanda saklanan 5 küçük anıyı bul.";
            serializedObject.FindProperty("objectiveType").enumValueIndex = (int)QuestObjectiveType.CollectMemory;
            serializedObject.FindProperty("targetId").stringValue = "memory";
            serializedObject.FindProperty("requiredAmount").intValue = requiredAmount;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return quest;
        }

        private static MemoryDefinition CreateMemory(string id)
        {
            MemoryDefinition memory = ScriptableObject.CreateInstance<MemoryDefinition>();
            SerializedObject serializedObject = new(memory);
            serializedObject.FindProperty("id").stringValue = id;
            serializedObject.FindProperty("displayName").stringValue = id;
            serializedObject.FindProperty("shortText").stringValue = "Kısa bir anı.";
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return memory;
        }
    }
}
