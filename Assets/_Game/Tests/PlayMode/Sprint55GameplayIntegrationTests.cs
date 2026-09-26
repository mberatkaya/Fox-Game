using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace TilkiOyunu.Foundation.PlayModeTests
{
    public sealed class Sprint55GameplayIntegrationTests
    {
        private static T Find<T>() where T : Object => Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        private static QuestService Quests => GameServices.Current.Quest;
        private static InteractionContext Context => new(Find<FoxController>().gameObject);

        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            foreach (var bootstrap in Object.FindObjectsByType<GameBootstrap>(FindObjectsSortMode.None)) Object.Destroy(bootstrap.gameObject);
            yield return null;
            GameServices.Shutdown();
        }

        private static IEnumerator Reload()
        {
            foreach (var bootstrap in Object.FindObjectsByType<GameBootstrap>(FindObjectsSortMode.None)) Object.Destroy(bootstrap.gameObject);
            yield return null;
            GameServices.Shutdown();
            yield return SceneManager.LoadSceneAsync(SceneIds.Bootstrap);
            float deadline = Time.realtimeSinceStartup + 15;
            while (SceneManager.GetActiveScene().name != SceneIds.Forest && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(SceneIds.Forest));
            yield return null;
            yield return null;
        }

        private static void Talk()
        {
            Find<GuideNpc>().Interact(Context);
            var panel = Find<DialoguePanelUI>();
            Assert.That(panel.IsOpen, Is.True);
            for (int i = 0; i < 30 && panel.IsOpen; i++) panel.Advance();
            Assert.That(panel.IsOpen, Is.False);
        }

        [UnityTest]
        public IEnumerator ActualMemoryIdsMinigamesTurnInsAndReloadsReachFinalCompletion()
        {
            // Suite-level ProductionSaveProtection preserves a pre-existing default save.
            new SaveService().DeleteSave();
            yield return Reload();
            Assert.That(Quests.CollectedMemoryIds, Is.Empty);
            Assert.That(Find<FinalSequenceController>().Begin(), Is.False);
            Assert.That(Find<LightPathController>().CanStartRun, Is.False);
            Assert.That(Find<CardMatchingController>().CanStartRun, Is.False);
            Talk();
            Assert.That(Quests.GetQuestStatus("collect_memories"), Is.EqualTo(QuestStatus.Active));
            var memories = Object.FindObjectsByType<MemoryCollectible>(FindObjectsSortMode.None).OrderBy(m => m.Memory.Id).ToArray();
            for (int i = 0; i < 2; i++)
            {
                memories[i].Interact(Context);
                memories[i].Interact(Context);
                Assert.That(Quests.CollectedMemoryIds.Count, Is.EqualTo(i + 1));
            }
            yield return Reload();
            Assert.That(Quests.CollectedMemoryIds, Is.EquivalentTo(new[] { "memory_01", "memory_02" }));
            memories = Object.FindObjectsByType<MemoryCollectible>(FindObjectsSortMode.None).OrderBy(m => m.Memory.Id).ToArray();
            for (int i = 0; i < memories.Length; i++)
            {
                Assert.That(memories[i].CanInteract(Context), Is.EqualTo(i >= 2), memories[i].Memory.Id);
                if (i < 2) Assert.That(memories[i].GetComponentsInChildren<Renderer>().All(r => !r.enabled), Is.True);
                else memories[i].Interact(Context);
            }
            Assert.That(Quests.CollectedMemoryIds.Count, Is.EqualTo(5));
            Assert.That(Quests.GetQuestStatus("collect_memories"), Is.EqualTo(QuestStatus.ReadyToTurnIn));
            Assert.That(Find<LightPathController>().CanStartRun, Is.False);
            Talk();
            yield return Reload();
            Assert.That(Quests.GetQuestStatus("collect_memories"), Is.EqualTo(QuestStatus.Completed));
            Talk();
            Assert.That(Quests.GetQuestStatus("light_path"), Is.EqualTo(QuestStatus.Active));
            var light = Find<LightPathController>();
            var nodes = Object.FindObjectsByType<LightPathNode>(FindObjectsSortMode.None).OrderBy(n => n.SequenceIndex).ToArray();
            Assert.That(light.StartRun(), Is.True);
            Assert.That(nodes[1].Activate(), Is.False);
            Assert.That(light.State, Is.EqualTo(LightPathRunState.Inactive));
            Assert.That(light.StartRun(), Is.True);
            float timeout = light.RemainingSeconds;
            yield return new WaitForSeconds(timeout + .2f);
            Assert.That(light.State, Is.EqualTo(LightPathRunState.Inactive), "real timer expiry");
            Assert.That(light.StartRun(), Is.True);
            Assert.That(nodes[0].Activate(), Is.True);
            yield return Reload();
            Assert.That(Quests.GetQuestStatus("light_path"), Is.EqualTo(QuestStatus.Active));
            light = Find<LightPathController>();
            Assert.That(light.State, Is.EqualTo(LightPathRunState.Inactive));
            Assert.That(light.CurrentIndex, Is.EqualTo(0));
            Assert.That(light.StartRun(), Is.True);
            nodes = Object.FindObjectsByType<LightPathNode>(FindObjectsSortMode.None).OrderBy(n => n.SequenceIndex).ToArray();
            foreach (var node in nodes) Assert.That(node.Activate(), Is.True);
            Assert.That(Quests.GetQuestStatus("light_path"), Is.EqualTo(QuestStatus.ReadyToTurnIn));
            Assert.That(Find<CardMatchingController>().CanStartRun, Is.False);
            Talk(); Talk();
            Assert.That(Quests.GetQuestStatus("light_path"), Is.EqualTo(QuestStatus.Completed));
            Assert.That(Quests.GetQuestStatus("card_matching"), Is.EqualTo(QuestStatus.Active));
            Find<CardMatchingStart>().Interact(Context);
            var cards = Find<CardMatchingController>();
            var cardPanel = Find<CardMatchingPanelUI>();
            Assert.That(cardPanel.IsOpen, Is.True);
            Assert.That(Find<ThirdPersonCameraController>().IsLookInputLocked, Is.True);
            Assert.That(cards.CardCount, Is.EqualTo(8));
            int mismatch = Enumerable.Range(1, 7).First(i => cards.GetCard(i).PairId != cards.GetCard(0).PairId);
            Assert.That(cards.TryReveal(0), Is.True); Assert.That(cards.TryReveal(mismatch), Is.True);
            yield return new WaitForSeconds(cards.MismatchDelaySeconds + .1f);
            Assert.That(cards.GetCard(0).State, Is.EqualTo(CardMatchingCardState.Hidden));
            Assert.That(cards.GetCard(mismatch).State, Is.EqualTo(CardMatchingCardState.Hidden));
            foreach (var pair in Enumerable.Range(0, 8).GroupBy(i => cards.GetCard(i).PairId).Select(g => g.ToArray()).ToArray())
            {
                Assert.That(cards.TryReveal(pair[0]), Is.True); Assert.That(cards.TryReveal(pair[1]), Is.True);
                Assert.That(cards.GetCard(pair[0]).State, Is.EqualTo(CardMatchingCardState.Matched));
            }
            Assert.That(Quests.GetQuestStatus("card_matching"), Is.EqualTo(QuestStatus.ReadyToTurnIn));
            cardPanel.Close();
            Assert.That(cardPanel.IsOpen, Is.False);
            Assert.That(cardPanel.HasCursorOverride, Is.False);
            Assert.That(Find<ThirdPersonCameraController>().IsLookInputLocked, Is.False);
            Assert.That(Find<FinalSequenceController>().Begin(), Is.False);
            Talk();
            yield return Reload();
            Assert.That(Quests.GetQuestStatus("card_matching"), Is.EqualTo(QuestStatus.Completed));
            Assert.That(Quests.IsFinalCampUnlocked, Is.True);
            Find<FinalCampController>().Interact(Context);
            var final = Find<FinalSequenceController>();
            Assert.That(final.IsRunning, Is.True);
            Assert.That(final.Begin(), Is.False, "no concurrent finale");
            yield return new WaitForSeconds(2);
            Assert.That(final.HasCameraControl, Is.True);
            Assert.That(final.GameplayCamera.isActiveAndEnabled, Is.True);
            final.CompleteAndClose();
            Assert.That(final.HasCameraControl, Is.False);
            Assert.That(new SaveService().Load().finalCompleted, Is.True);
            yield return Reload();
            Assert.That(Quests.SaveData.finalCompleted, Is.True);
            Assert.That(Quests.CollectedMemoryIds.Count, Is.EqualTo(5));
            foreach (string id in QuestService.FinalCampRequiredQuestIds)
                Assert.That(Quests.GetQuestStatus(id), Is.EqualTo(QuestStatus.Completed));
        }
    }
}
