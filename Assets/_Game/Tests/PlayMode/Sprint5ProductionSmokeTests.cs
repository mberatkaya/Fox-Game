using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TilkiOyunu.Foundation.PlayModeTests
{
    public sealed class Sprint5ProductionSmokeTests
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
        public IEnumerator BootstrapForestContainsProductionPolish()
        {
            yield return LoadBootstrapToForest();

            GameObject player = GameObject.Find("PlayerFox");
            Assert.That(player, Is.Not.Null);
            Assert.That(player.transform.Find("VisualRoot/QuaterniusFox/FoxModel"), Is.Not.Null);
            Assert.That(player.GetComponentInChildren<FoxAnimationDriver>(true), Is.Not.Null);
            Assert.That(player.GetComponent<FootstepAudio>(), Is.Not.Null);

            Assert.That(GameObject.Find("Environment_Visuals"), Is.Not.Null);
            Assert.That(GameObject.Find("Sprint 5 Scene Audio"), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<SceneLoopAudio>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<FinalSequenceController>(FindObjectsInactive.Include), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<FinalMessagePanelUI>(FindObjectsInactive.Include), Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator CompletedQuestChainAllowsFinalSequenceToOpen()
        {
            yield return LoadBootstrapToForest();

            QuestService quests = GameServices.Current.Quest;
            CompleteQuestChain(quests);

            FinalSequenceController final = Object.FindFirstObjectByType<FinalSequenceController>(FindObjectsInactive.Include);
            Assert.That(final, Is.Not.Null);
            Assert.That(final.CanBegin, Is.True);
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

        private static void CompleteQuestChain(QuestService service)
        {
            QuestDefinition memory = service.FindQuest(QuestService.CollectMemoriesQuestId);
            QuestDefinition light = service.FindQuest(QuestService.LightPathQuestId);
            QuestDefinition card = service.FindQuest(QuestService.CardMatchingQuestId);

            service.StartQuest(memory);
            while (service.GetQuestState(memory).CurrentAmount < memory.RequiredAmount)
            {
                service.AddProgress(memory, 1);
            }

            service.TurnInQuest(memory);
            service.StartQuest(light);
            service.RecordLightPathCompleted(light);
            service.TurnInQuest(light);
            service.StartQuest(card);
            service.RecordCardMatchingCompleted(card);
            service.TurnInQuest(card);
        }
    }
}
