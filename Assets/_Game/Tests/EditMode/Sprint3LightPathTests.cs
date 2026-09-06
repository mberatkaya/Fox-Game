using System;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace TilkiOyunu.Foundation.Tests
{
    public sealed class Sprint3LightPathTests
    {
        private string tempDirectory;
        private string savePath;

        [SetUp]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "TilkiOyunuSprint3Tests", Guid.NewGuid().ToString("N"));
            savePath = Path.Combine(tempDirectory, SaveService.DefaultFileName);
            GameServices.Shutdown();
        }

        [TearDown]
        public void TearDown()
        {
            GameServices.Shutdown();
            foreach (LightPathController controller in UnityEngine.Object.FindObjectsByType<LightPathController>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                UnityEngine.Object.DestroyImmediate(controller.gameObject);
            }

            foreach (LightPathNode node in UnityEngine.Object.FindObjectsByType<LightPathNode>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                UnityEngine.Object.DestroyImmediate(node.gameObject);
            }

            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Test]
        public void LightPathDoesNotStartBeforeMemoryQuestCompleted()
        {
            QuestDefinition lightPathQuest = CreateLightPathQuest();
            QuestService service = CreateQuestService();

            bool started = service.StartQuest(lightPathQuest);

            Assert.That(started, Is.False);
            Assert.That(service.GetQuestStatus(lightPathQuest), Is.EqualTo(QuestStatus.NotStarted));
        }

        [Test]
        public void LightPathCanStartAfterMemoryQuestCompleted()
        {
            QuestDefinition memoryQuest = CreateMemoryQuest();
            QuestDefinition lightPathQuest = CreateLightPathQuest();
            QuestService service = CreateQuestService();

            CompleteMemoryQuest(service, memoryQuest);
            bool started = service.StartQuest(lightPathQuest);

            Assert.That(started, Is.True);
            Assert.That(service.GetQuestStatus(lightPathQuest), Is.EqualTo(QuestStatus.Active));
        }

        [Test]
        public void CorrectNodeSequenceAdvancesProgress()
        {
            QuestDefinition memoryQuest = CreateMemoryQuest();
            QuestDefinition lightPathQuest = CreateLightPathQuest();
            QuestService service = CreateQuestService();
            CompleteMemoryQuest(service, memoryQuest);
            service.StartQuest(lightPathQuest);
            LightPathController controller = CreateController(lightPathQuest, out LightPathNode[] nodes);

            controller.StartRun();
            bool advanced = controller.HandleNodeTriggered(nodes[0]);

            Assert.That(advanced, Is.True);
            Assert.That(controller.CurrentIndex, Is.EqualTo(1));
            Assert.That(service.GetQuestState(lightPathQuest).CurrentAmount, Is.EqualTo(0));
        }

        [Test]
        public void WrongNodeResetsRun()
        {
            QuestDefinition memoryQuest = CreateMemoryQuest();
            QuestDefinition lightPathQuest = CreateLightPathQuest();
            QuestService service = CreateQuestService();
            CompleteMemoryQuest(service, memoryQuest);
            service.StartQuest(lightPathQuest);
            LightPathController controller = CreateController(lightPathQuest, out LightPathNode[] nodes);

            controller.StartRun();
            controller.HandleNodeTriggered(nodes[2]);

            Assert.That(controller.State, Is.EqualTo(LightPathRunState.Inactive));
            Assert.That(controller.CurrentIndex, Is.EqualTo(0));
            Assert.That(service.GetQuestStatus(lightPathQuest), Is.EqualTo(QuestStatus.Active));
        }

        [Test]
        public void DuplicateNodeActivationDoesNotDoubleCount()
        {
            QuestDefinition memoryQuest = CreateMemoryQuest();
            QuestDefinition lightPathQuest = CreateLightPathQuest();
            QuestService service = CreateQuestService();
            CompleteMemoryQuest(service, memoryQuest);
            service.StartQuest(lightPathQuest);
            LightPathController controller = CreateController(lightPathQuest, out LightPathNode[] nodes);

            controller.StartRun();
            controller.HandleNodeTriggered(nodes[0]);
            controller.HandleNodeTriggered(nodes[0]);

            Assert.That(controller.State, Is.EqualTo(LightPathRunState.Running));
            Assert.That(controller.CurrentIndex, Is.EqualTo(1));
        }

        [Test]
        public void TimeoutResetsRun()
        {
            QuestDefinition memoryQuest = CreateMemoryQuest();
            QuestDefinition lightPathQuest = CreateLightPathQuest();
            QuestService service = CreateQuestService();
            CompleteMemoryQuest(service, memoryQuest);
            service.StartQuest(lightPathQuest);
            LightPathController controller = CreateController(lightPathQuest, out _);

            controller.StartRun();
            controller.ForceTimeoutForTests();

            Assert.That(controller.State, Is.EqualTo(LightPathRunState.Inactive));
            Assert.That(controller.CurrentIndex, Is.EqualTo(0));
            Assert.That(service.GetQuestStatus(lightPathQuest), Is.EqualTo(QuestStatus.Active));
        }

        [Test]
        public void AllNodesCompleteReadiesQuestForTurnIn()
        {
            QuestDefinition memoryQuest = CreateMemoryQuest();
            QuestDefinition lightPathQuest = CreateLightPathQuest();
            QuestService service = CreateQuestService();
            CompleteMemoryQuest(service, memoryQuest);
            service.StartQuest(lightPathQuest);
            LightPathController controller = CreateController(lightPathQuest, out LightPathNode[] nodes);

            controller.StartRun();
            for (int i = 0; i < nodes.Length; i++)
            {
                controller.HandleNodeTriggered(nodes[i]);
            }

            Assert.That(controller.State, Is.EqualTo(LightPathRunState.Completed));
            Assert.That(service.GetQuestStatus(lightPathQuest), Is.EqualTo(QuestStatus.ReadyToTurnIn));
            Assert.That(service.SaveData.lightPathCompleted, Is.True);
        }

        [Test]
        public void NpcTurnInCompletesLightPathQuest()
        {
            QuestDefinition memoryQuest = CreateMemoryQuest();
            QuestDefinition lightPathQuest = CreateLightPathQuest();
            QuestService service = CreateQuestService();
            CompleteMemoryQuest(service, memoryQuest);
            service.StartQuest(lightPathQuest);
            service.RecordLightPathCompleted(lightPathQuest);

            bool turnedIn = service.TurnInQuest(lightPathQuest);

            Assert.That(turnedIn, Is.True);
            Assert.That(service.GetQuestStatus(lightPathQuest), Is.EqualTo(QuestStatus.Completed));
        }

        [Test]
        public void SaveLoadReadyToTurnInIsPreserved()
        {
            QuestDefinition memoryQuest = CreateMemoryQuest();
            QuestDefinition lightPathQuest = CreateLightPathQuest();
            QuestService firstService = CreateQuestService();
            CompleteMemoryQuest(firstService, memoryQuest);
            firstService.StartQuest(lightPathQuest);
            firstService.RecordLightPathCompleted(lightPathQuest);

            QuestService loadedService = new(new SaveService(savePath), null);

            Assert.That(loadedService.GetQuestStatus(lightPathQuest), Is.EqualTo(QuestStatus.ReadyToTurnIn));
            Assert.That(loadedService.SaveData.lightPathCompleted, Is.True);
        }

        [Test]
        public void SaveLoadCompletedIsPreserved()
        {
            QuestDefinition memoryQuest = CreateMemoryQuest();
            QuestDefinition lightPathQuest = CreateLightPathQuest();
            QuestService firstService = CreateQuestService();
            CompleteMemoryQuest(firstService, memoryQuest);
            firstService.StartQuest(lightPathQuest);
            firstService.RecordLightPathCompleted(lightPathQuest);
            firstService.TurnInQuest(lightPathQuest);

            QuestService loadedService = new(new SaveService(savePath), null);

            Assert.That(loadedService.GetQuestStatus(lightPathQuest), Is.EqualTo(QuestStatus.Completed));
            Assert.That(loadedService.SaveData.lightPathCompleted, Is.True);
        }

        private QuestService CreateQuestService()
        {
            SaveService saveService = new(savePath);
            QuestService questService = new(saveService, null);
            GameServices.Initialize(saveService, new SceneService(), new AudioService(null), questService);
            return questService;
        }

        private static void CompleteMemoryQuest(QuestService service, QuestDefinition memoryQuest)
        {
            service.StartQuest(memoryQuest);
            service.AddProgress(memoryQuest, memoryQuest.RequiredAmount);
            service.TurnInQuest(memoryQuest);
        }

        private static LightPathController CreateController(QuestDefinition quest, out LightPathNode[] nodes)
        {
            GameObject controllerObject = new("LightPathControllerTest");
            LightPathController controller = controllerObject.AddComponent<LightPathController>();
            nodes = new LightPathNode[5];
            for (int i = 0; i < nodes.Length; i++)
            {
                GameObject nodeObject = new($"LightPathNode_{i}");
                nodes[i] = nodeObject.AddComponent<LightPathNode>();
                SetInt(nodes[i], "sequenceIndex", i);
            }

            SerializedObject serializedObject = new(controller);
            serializedObject.FindProperty("quest").objectReferenceValue = quest;
            SerializedProperty nodesProperty = serializedObject.FindProperty("nodes");
            nodesProperty.arraySize = nodes.Length;
            for (int i = 0; i < nodes.Length; i++)
            {
                nodesProperty.GetArrayElementAtIndex(i).objectReferenceValue = nodes[i];
            }

            serializedObject.FindProperty("durationSeconds").floatValue = 10f;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return controller;
        }

        private static QuestDefinition CreateMemoryQuest()
        {
            QuestDefinition quest = ScriptableObject.CreateInstance<QuestDefinition>();
            SerializedObject serializedObject = new(quest);
            serializedObject.FindProperty("id").stringValue = QuestService.CollectMemoriesQuestId;
            serializedObject.FindProperty("title").stringValue = "Dağılmış Anılar";
            serializedObject.FindProperty("description").stringValue = "Ormanda saklanan 5 küçük anıyı bul.";
            serializedObject.FindProperty("objectiveType").enumValueIndex = (int)QuestObjectiveType.CollectMemory;
            serializedObject.FindProperty("targetId").stringValue = "memory";
            serializedObject.FindProperty("requiredAmount").intValue = 5;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return quest;
        }

        private static QuestDefinition CreateLightPathQuest()
        {
            QuestDefinition quest = ScriptableObject.CreateInstance<QuestDefinition>();
            SerializedObject serializedObject = new(quest);
            serializedObject.FindProperty("id").stringValue = QuestService.LightPathQuestId;
            serializedObject.FindProperty("title").stringValue = "Işıkların İzinde";
            serializedObject.FindProperty("description").stringValue = "Ormandaki ışıkları doğru sırayla takip et.";
            serializedObject.FindProperty("objectiveType").enumValueIndex = (int)QuestObjectiveType.CompleteLightPath;
            serializedObject.FindProperty("targetId").stringValue = "light_path";
            serializedObject.FindProperty("requiredAmount").intValue = 5;

            SerializedProperty prerequisites = serializedObject.FindProperty("prerequisiteQuestIds");
            prerequisites.arraySize = 1;
            prerequisites.GetArrayElementAtIndex(0).stringValue = QuestService.CollectMemoriesQuestId;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return quest;
        }

        private static void SetInt(UnityEngine.Object target, string propertyName, int value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).intValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
