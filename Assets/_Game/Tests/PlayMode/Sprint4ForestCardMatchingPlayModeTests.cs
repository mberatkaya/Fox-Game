using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TilkiOyunu.Foundation.PlayModeTests
{
    public sealed class Sprint4ForestCardMatchingPlayModeTests
    {
        [TearDown]
        public void TearDown()
        {
            foreach (GameBootstrap bootstrap in Object.FindObjectsByType<GameBootstrap>(FindObjectsSortMode.None))
            {
                Object.DestroyImmediate(bootstrap.gameObject);
            }

            if (GameServices.HasCurrent)
            {
                GameServices.Shutdown();
            }
        }

        [UnityTest]
        public IEnumerator ForestContainsSprint4CardMatchingDependencies()
        {
            yield return LoadBootstrapToForest();

            Assert.That(GameServices.HasCurrent, Is.True);
            Assert.That(Object.FindFirstObjectByType<GuideNpc>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<QuestHUD>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<LightPathController>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<CardMatchingStart>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<CardMatchingPanelUI>(FindObjectsInactive.Include), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<FinalCampController>(), Is.Not.Null);

            CardMatchingController controller = Object.FindFirstObjectByType<CardMatchingController>();
            Assert.That(controller, Is.Not.Null);
            Assert.That(controller.Quest, Is.Not.Null);
            Assert.That(controller.Quest.Id, Is.EqualTo(QuestService.CardMatchingQuestId));
            Assert.That(controller.Quest.PrerequisiteQuestIds, Does.Contain(QuestService.LightPathQuestId));
            Assert.That(controller.CardCount, Is.EqualTo(8));
            Assert.That(controller.PairCount, Is.EqualTo(4));
        }

        [UnityTest]
        public IEnumerator CardMatchingFlowUnlocksFinalCamp()
        {
            yield return LoadBootstrapToForest();

            QuestService quests = GameServices.Current.Quest;
            QuestDefinition memoryQuest = quests.FindQuest(QuestService.CollectMemoriesQuestId);
            QuestDefinition lightPathQuest = quests.FindQuest(QuestService.LightPathQuestId);
            QuestDefinition cardQuest = quests.FindQuest(QuestService.CardMatchingQuestId);

            Assert.That(memoryQuest, Is.Not.Null);
            Assert.That(lightPathQuest, Is.Not.Null);
            Assert.That(cardQuest, Is.Not.Null);

            CompleteQuestChainThroughLightPath(quests, memoryQuest, lightPathQuest);
            Assert.That(quests.IsFinalCampUnlocked, Is.False);

            Assert.That(quests.StartQuest(cardQuest), Is.True);
            CardMatchingController controller = Object.FindFirstObjectByType<CardMatchingController>();
            Assert.That(controller.StartRun(), Is.True);
            RevealAllPairs(controller);

            Assert.That(quests.GetQuestStatus(cardQuest), Is.EqualTo(QuestStatus.ReadyToTurnIn));
            Assert.That(quests.TurnInQuest(cardQuest), Is.True);
            Assert.That(quests.GetQuestStatus(cardQuest), Is.EqualTo(QuestStatus.Completed));
            Assert.That(quests.IsFinalCampUnlocked, Is.True);

            FinalCampController camp = Object.FindFirstObjectByType<FinalCampController>();
            yield return null;
            Assert.That(camp.IsUnlocked, Is.True);
        }

        private static IEnumerator LoadBootstrapToForest()
        {
            new SaveService().DeleteSave();
            GameServices.Shutdown();
            yield return SceneManager.LoadSceneAsync(SceneIds.Bootstrap, LoadSceneMode.Single);

            float timeoutAt = Time.realtimeSinceStartup + 5f;
            while (SceneManager.GetActiveScene().name != SceneIds.Forest && Time.realtimeSinceStartup < timeoutAt)
            {
                yield return null;
            }

            yield return null;
        }

        private static void CompleteQuestChainThroughLightPath(QuestService service, QuestDefinition memoryQuest, QuestDefinition lightPathQuest)
        {
            if (service.GetQuestStatus(memoryQuest) == QuestStatus.NotStarted)
            {
                service.StartQuest(memoryQuest);
            }

            while (service.GetQuestState(memoryQuest).CurrentAmount < memoryQuest.RequiredAmount)
            {
                service.AddProgress(memoryQuest, 1);
            }

            if (service.GetQuestStatus(memoryQuest) == QuestStatus.ReadyToTurnIn)
            {
                service.TurnInQuest(memoryQuest);
            }

            if (service.GetQuestStatus(lightPathQuest) == QuestStatus.NotStarted)
            {
                service.StartQuest(lightPathQuest);
            }

            if (service.GetQuestStatus(lightPathQuest) == QuestStatus.Active)
            {
                service.RecordLightPathCompleted(lightPathQuest);
            }

            if (service.GetQuestStatus(lightPathQuest) == QuestStatus.ReadyToTurnIn)
            {
                service.TurnInQuest(lightPathQuest);
            }
        }

        private static void RevealAllPairs(CardMatchingController controller)
        {
            Dictionary<string, List<int>> pairSlots = new();
            for (int i = 0; i < controller.CardCount; i++)
            {
                string pairId = controller.GetCard(i).PairId;
                if (!pairSlots.TryGetValue(pairId, out List<int> slots))
                {
                    slots = new List<int>();
                    pairSlots[pairId] = slots;
                }

                slots.Add(i);
            }

            foreach (List<int> slots in pairSlots.Values)
            {
                Assert.That(slots, Has.Count.EqualTo(2));
                Assert.That(controller.TryReveal(slots[0]), Is.True);
                Assert.That(controller.TryReveal(slots[1]), Is.True);
            }
        }
    }
}
