using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace TilkiOyunu.Foundation.Tests
{
    public sealed class Sprint4CardMatchingTests
    {
        private string tempDirectory;
        private string savePath;

        [SetUp]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "TilkiOyunuSprint4Tests", Guid.NewGuid().ToString("N"));
            savePath = Path.Combine(tempDirectory, SaveService.DefaultFileName);
            GameServices.Shutdown();
        }

        [TearDown]
        public void TearDown()
        {
            GameServices.Shutdown();
            foreach (CardMatchingController controller in UnityEngine.Object.FindObjectsByType<CardMatchingController>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                UnityEngine.Object.DestroyImmediate(controller.gameObject);
            }

            foreach (FinalCampController controller in UnityEngine.Object.FindObjectsByType<FinalCampController>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                UnityEngine.Object.DestroyImmediate(controller.gameObject);
            }

            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Test]
        public void CardQuestDoesNotStartBeforeLightPathCompleted()
        {
            QuestDefinition cardQuest = CreateCardQuest();
            QuestService service = CreateQuestService();

            bool started = service.StartQuest(cardQuest);

            Assert.That(started, Is.False);
            Assert.That(service.GetQuestStatus(cardQuest), Is.EqualTo(QuestStatus.NotStarted));
        }

        [Test]
        public void CardQuestCanStartAfterLightPathCompleted()
        {
            QuestDefinition memoryQuest = CreateMemoryQuest();
            QuestDefinition lightPathQuest = CreateLightPathQuest();
            QuestDefinition cardQuest = CreateCardQuest();
            QuestService service = CreateQuestService();

            CompleteQuestChainThroughLightPath(service, memoryQuest, lightPathQuest);
            bool started = service.StartQuest(cardQuest);

            Assert.That(started, Is.True);
            Assert.That(service.GetQuestStatus(cardQuest), Is.EqualTo(QuestStatus.Active));
        }

        [Test]
        public void CardDeckContainsExactlyFourPairs()
        {
            QuestDefinition cardQuest = StartCardQuest(out QuestService service);
            CardMatchingController controller = CreateController(cardQuest);

            Assert.That(controller.StartRun(), Is.True);
            Assert.That(controller.CardCount, Is.EqualTo(8));
            Assert.That(controller.PairCount, Is.EqualTo(4));

            Dictionary<string, int> counts = CountPairIds(controller);
            Assert.That(counts, Has.Count.EqualTo(4));
            foreach (int count in counts.Values)
            {
                Assert.That(count, Is.EqualTo(2));
            }

            Assert.That(service.GetQuestStatus(cardQuest), Is.EqualTo(QuestStatus.Active));
        }

        [Test]
        public void SameCardCannotBeSelectedTwice()
        {
            QuestDefinition cardQuest = StartCardQuest(out _);
            CardMatchingController controller = CreateController(cardQuest);
            controller.StartRun();

            Assert.That(controller.TryReveal(0), Is.True);
            Assert.That(controller.TryReveal(0), Is.False);
            Assert.That(controller.GetCard(0).State, Is.EqualTo(CardMatchingCardState.Revealed));
        }

        [Test]
        public void MatchingPairBecomesMatched()
        {
            QuestDefinition cardQuest = StartCardQuest(out QuestService service);
            CardMatchingController controller = CreateController(cardQuest);
            controller.StartRun();

            controller.TryReveal(0);
            controller.TryReveal(1);

            Assert.That(controller.GetCard(0).State, Is.EqualTo(CardMatchingCardState.Matched));
            Assert.That(controller.GetCard(1).State, Is.EqualTo(CardMatchingCardState.Matched));
            Assert.That(service.GetQuestState(cardQuest).CurrentAmount, Is.EqualTo(1));
        }

        [Test]
        public void NonMatchReturnsToHiddenAfterDelayResolution()
        {
            QuestDefinition cardQuest = StartCardQuest(out _);
            CardMatchingController controller = CreateController(cardQuest);
            controller.StartRun();

            controller.TryReveal(0);
            controller.TryReveal(2);

            Assert.That(controller.IsResolvingMismatch, Is.True);
            Assert.That(controller.TryReveal(3), Is.False);

            controller.ResolveMismatchNowForTests();

            Assert.That(controller.GetCard(0).State, Is.EqualTo(CardMatchingCardState.Hidden));
            Assert.That(controller.GetCard(2).State, Is.EqualTo(CardMatchingCardState.Hidden));
            Assert.That(controller.IsResolvingMismatch, Is.False);
        }

        [Test]
        public void MatchedPairDoesNotIncreaseProgressAgain()
        {
            QuestDefinition cardQuest = StartCardQuest(out QuestService service);
            CardMatchingController controller = CreateController(cardQuest);
            controller.StartRun();

            controller.TryReveal(0);
            controller.TryReveal(1);
            Assert.That(service.GetQuestState(cardQuest).CurrentAmount, Is.EqualTo(1));

            Assert.That(controller.TryReveal(0), Is.False);
            Assert.That(controller.TryReveal(1), Is.False);
            Assert.That(service.GetQuestState(cardQuest).CurrentAmount, Is.EqualTo(1));
        }

        [Test]
        public void FourPairsReadyQuestForTurnIn()
        {
            QuestDefinition cardQuest = StartCardQuest(out QuestService service);
            CardMatchingController controller = CreateController(cardQuest);
            controller.StartRun();

            RevealAllPairs(controller);

            Assert.That(service.GetQuestStatus(cardQuest), Is.EqualTo(QuestStatus.ReadyToTurnIn));
            Assert.That(service.GetQuestState(cardQuest).CurrentAmount, Is.EqualTo(4));
            Assert.That(service.SaveData.cardMatchingCompleted, Is.True);
        }

        [Test]
        public void NpcTurnInCompletesCardQuest()
        {
            QuestDefinition cardQuest = StartCardQuest(out QuestService service);
            CardMatchingController controller = CreateController(cardQuest);
            controller.StartRun();
            RevealAllPairs(controller);

            bool turnedIn = service.TurnInQuest(cardQuest);

            Assert.That(turnedIn, Is.True);
            Assert.That(service.GetQuestStatus(cardQuest), Is.EqualTo(QuestStatus.Completed));
        }

        [Test]
        public void FinalCampIsLockedWithOnlyTwoMainQuestsCompleted()
        {
            QuestDefinition memoryQuest = CreateMemoryQuest();
            QuestDefinition lightPathQuest = CreateLightPathQuest();
            QuestService service = CreateQuestService();

            CompleteQuestChainThroughLightPath(service, memoryQuest, lightPathQuest);

            Assert.That(service.IsFinalCampUnlocked, Is.False);
        }

        [Test]
        public void FinalCampUnlocksAfterThreeMainQuestsCompleted()
        {
            QuestDefinition cardQuest = StartCardQuest(out QuestService service);
            CardMatchingController controller = CreateController(cardQuest);
            controller.StartRun();
            RevealAllPairs(controller);
            service.TurnInQuest(cardQuest);

            Assert.That(service.IsFinalCampUnlocked, Is.True);
            Assert.That(service.SaveData.finalUnlocked, Is.True);
        }

        [Test]
        public void SaveLoadThirdQuestStateIsPreserved()
        {
            QuestDefinition cardQuest = StartCardQuest(out QuestService firstService);
            CardMatchingController controller = CreateController(cardQuest);
            controller.StartRun();
            RevealAllPairs(controller);

            QuestService loadedService = new(new SaveService(savePath), null);

            Assert.That(firstService.GetQuestStatus(cardQuest), Is.EqualTo(QuestStatus.ReadyToTurnIn));
            Assert.That(loadedService.GetQuestStatus(cardQuest), Is.EqualTo(QuestStatus.ReadyToTurnIn));
            Assert.That(loadedService.SaveData.cardMatchingCompleted, Is.True);
        }

        [Test]
        public void ReloadFinalUnlockIsCalculatedFromCompletedQuests()
        {
            QuestDefinition cardQuest = StartCardQuest(out QuestService firstService);
            CardMatchingController controller = CreateController(cardQuest);
            controller.StartRun();
            RevealAllPairs(controller);
            firstService.TurnInQuest(cardQuest);

            QuestService loadedService = new(new SaveService(savePath), null);

            Assert.That(loadedService.IsFinalCampUnlocked, Is.True);
            Assert.That(loadedService.SaveData.finalUnlocked, Is.True);
        }

        private QuestDefinition StartCardQuest(out QuestService service)
        {
            QuestDefinition memoryQuest = CreateMemoryQuest();
            QuestDefinition lightPathQuest = CreateLightPathQuest();
            QuestDefinition cardQuest = CreateCardQuest();
            service = CreateQuestService();
            CompleteQuestChainThroughLightPath(service, memoryQuest, lightPathQuest);
            service.StartQuest(cardQuest);
            return cardQuest;
        }

        private QuestService CreateQuestService()
        {
            SaveService saveService = new(savePath);
            QuestService questService = new(saveService, null);
            GameServices.Initialize(saveService, new SceneService(), new AudioService(null), questService);
            return questService;
        }

        private static void CompleteQuestChainThroughLightPath(QuestService service, QuestDefinition memoryQuest, QuestDefinition lightPathQuest)
        {
            service.StartQuest(memoryQuest);
            service.AddProgress(memoryQuest, memoryQuest.RequiredAmount);
            service.TurnInQuest(memoryQuest);
            service.StartQuest(lightPathQuest);
            service.RecordLightPathCompleted(lightPathQuest);
            service.TurnInQuest(lightPathQuest);
        }

        private static CardMatchingController CreateController(QuestDefinition quest)
        {
            GameObject gameObject = new("CardMatchingControllerTest");
            CardMatchingController controller = gameObject.AddComponent<CardMatchingController>();
            SerializedObject serializedObject = new(controller);
            serializedObject.FindProperty("quest").objectReferenceValue = quest;
            serializedObject.FindProperty("mismatchDelaySeconds").floatValue = 0.1f;
            serializedObject.FindProperty("shuffleOnStart").boolValue = false;
            SerializedProperty pairs = serializedObject.FindProperty("pairs");
            pairs.arraySize = 4;
            for (int i = 0; i < 4; i++)
            {
                SerializedProperty pair = pairs.GetArrayElementAtIndex(i);
                pair.FindPropertyRelative("pairId").stringValue = $"pair_{i + 1:00}";
                pair.FindPropertyRelative("symbol").stringValue = $"S{i + 1}";
                pair.FindPropertyRelative("color").colorValue = Color.white;
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return controller;
        }

        private static Dictionary<string, int> CountPairIds(CardMatchingController controller)
        {
            Dictionary<string, int> counts = new();
            for (int i = 0; i < controller.CardCount; i++)
            {
                string pairId = controller.GetCard(i).PairId;
                counts[pairId] = counts.TryGetValue(pairId, out int count) ? count + 1 : 1;
            }

            return counts;
        }

        private static void RevealAllPairs(CardMatchingController controller)
        {
            controller.TryReveal(0);
            controller.TryReveal(1);
            controller.TryReveal(2);
            controller.TryReveal(3);
            controller.TryReveal(4);
            controller.TryReveal(5);
            controller.TryReveal(6);
            controller.TryReveal(7);
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

        private static QuestDefinition CreateCardQuest()
        {
            QuestDefinition quest = ScriptableObject.CreateInstance<QuestDefinition>();
            SerializedObject serializedObject = new(quest);
            serializedObject.FindProperty("id").stringValue = QuestService.CardMatchingQuestId;
            serializedObject.FindProperty("title").stringValue = "Kalp Bahçesi";
            serializedObject.FindProperty("description").stringValue = "Bahçedeki kartların eşlerini bul.";
            serializedObject.FindProperty("objectiveType").enumValueIndex = (int)QuestObjectiveType.CompleteCardMatch;
            serializedObject.FindProperty("targetId").stringValue = "heart_garden";
            serializedObject.FindProperty("requiredAmount").intValue = 4;
            SerializedProperty prerequisites = serializedObject.FindProperty("prerequisiteQuestIds");
            prerequisites.arraySize = 1;
            prerequisites.GetArrayElementAtIndex(0).stringValue = QuestService.LightPathQuestId;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return quest;
        }
    }
}
