using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace TilkiOyunu.Foundation.PlayModeTests
{
    public sealed class Sprint55E2CardRegressionTests
    {
        [UnityTearDown] public IEnumerator Cleanup()
        {
            Time.timeScale = 1;
            foreach (var bootstrap in Object.FindObjectsByType<GameBootstrap>(FindObjectsSortMode.None)) Object.Destroy(bootstrap.gameObject);
            yield return null; GameServices.Shutdown();
        }

        internal static IEnumerator LoadCards()
        {
            new SaveService().DeleteSave();
            yield return SceneManager.LoadSceneAsync(SceneIds.Bootstrap);
            float deadline = Time.realtimeSinceStartup + 20;
            while (Object.FindFirstObjectByType<CardMatchingController>() == null && Time.realtimeSinceStartup < deadline) yield return null;
            yield return null; yield return null;
            var quests = GameServices.Current.Quest;
            foreach (string id in new[] { QuestService.CollectMemoriesQuestId, QuestService.LightPathQuestId })
            {
                var quest = quests.FindQuest(id); quests.StartQuest(quest);
                if (id == QuestService.CollectMemoriesQuestId) quests.AddProgress(quest, quest.RequiredAmount);
                else quests.RecordLightPathCompleted(quest);
                quests.TurnInQuest(quest);
            }
            Assert.That(quests.StartQuest(quests.FindQuest(QuestService.CardMatchingQuestId)), Is.True);
        }

        [UnityTest] public IEnumerator TenRealMismatchTimersKeepProductionButtonsPlayableThenComplete()
        {
            yield return LoadCards();
            var controller = Object.FindFirstObjectByType<CardMatchingController>();
            Assert.That(controller.Open(), Is.True);
            var buttons = Object.FindObjectsByType<CardMatchingCardButton>(FindObjectsSortMode.None).OrderBy(b => b.Index).ToArray();
            int first = 0, wrong = Enumerable.Range(1, 7).First(i => controller.GetCard(i).PairId != controller.GetCard(first).PairId);
            for (int cycle = 0; cycle < 10; cycle++)
            {
                Assert.That(buttons[first].IsInteractable, Is.True, $"Cycle {cycle}: production button unlocked");
                buttons[first].GetComponent<Button>().onClick.Invoke();
                Assert.That(controller.TryReveal(first), Is.False, "same-card double click ignored");
                buttons[wrong].GetComponent<Button>().onClick.Invoke();
                Assert.That(controller.IsResolvingMismatch, Is.True);
                Assert.That(buttons.All(b => !b.IsInteractable), Is.True, "all third clicks blocked during resolve");
                Assert.That(controller.TryReveal(7), Is.False);
                if (cycle == 0) yield return Sprint55E2SettingsPlayModeTests.Capture("08_card_mismatch_before");
                // Proves UI delays work even if another layer pauses scaled time.
                Time.timeScale = 0;
                yield return new WaitForSecondsRealtime(controller.MismatchDelaySeconds + .1f);
                Time.timeScale = 1;
                Assert.That(controller.IsResolvingMismatch, Is.False);
                Assert.That(buttons.All(b => b.IsInteractable), Is.True, "Hidden notification must see unlocked state");
                if (cycle == 0) yield return Sprint55E2SettingsPlayModeTests.Capture("09_card_mismatch_reset");
            }
            var pairs = Enumerable.Range(0, 8).GroupBy(i => controller.GetCard(i).PairId).Select(g => g.ToArray()).ToArray();
            for (int p = 0; p < pairs.Length; p++)
            {
                foreach (int i in pairs[p]) buttons[i].GetComponent<Button>().onClick.Invoke();
                Assert.That(controller.GetCard(pairs[p][0]).State, Is.EqualTo(CardMatchingCardState.Matched));
                Assert.That(buttons[pairs[p][0]].IsInteractable, Is.False);
                if (p == 0)
                {
                    buttons[pairs[1][0]].GetComponent<Button>().onClick.Invoke();
                    buttons[pairs[2][0]].GetComponent<Button>().onClick.Invoke();
                    yield return new WaitForSecondsRealtime(controller.MismatchDelaySeconds + .1f);
                    Assert.That(buttons[pairs[1][0]].IsInteractable, Is.True, "mismatch after match");
                    Assert.That(buttons[pairs[0][0]].IsInteractable, Is.False, "matched stays completed");
                }
            }
            Assert.That(controller.IsComplete, Is.True);
            Assert.That(controller.MatchedPairs, Is.EqualTo(4));
            yield return Sprint55E2SettingsPlayModeTests.Capture("10_card_game_completed");
            Assert.That(GameServices.Current.Quest.GetQuestStatus(controller.Quest), Is.EqualTo(QuestStatus.ReadyToTurnIn));
        }

        [UnityTest] public IEnumerator CloseDuringMismatchAndDisableRestoreOwnershipAndReopen()
        {
            yield return LoadCards();
            var controller = Object.FindFirstObjectByType<CardMatchingController>();
            var panel = Object.FindFirstObjectByType<CardMatchingPanelUI>();
            var inputLock = Object.FindFirstObjectByType<GameplayInputLock>();
            var camera = Object.FindFirstObjectByType<ThirdPersonCameraController>();
            Cursor.lockState = CursorLockMode.None; Cursor.visible = false;
            // A prior owner must remain locked after this panel releases its own lock.
            inputLock.Acquire(); camera.SetLookInputLocked(true);
            for (int iteration = 0; iteration < 2; iteration++)
            {
                Assert.That(controller.Open(), Is.True);
                controller.TryReveal(0);
                controller.TryReveal(Enumerable.Range(1, 7).First(i => controller.GetCard(i).PairId != controller.GetCard(0).PairId));
                if (iteration == 0) panel.Close(); else panel.enabled = false;
                Assert.That(controller.IsResolvingMismatch, Is.False); Assert.That(controller.IsRunning, Is.False);
                Assert.That(panel.HasCursorOverride, Is.False);
                Assert.That(inputLock.IsLocked, Is.True); Assert.That(camera.IsLookInputLocked, Is.True);
                Assert.That(Cursor.visible, Is.False); Assert.That(Cursor.lockState, Is.EqualTo(CursorLockMode.None));
                panel.enabled = true;
                yield return new WaitForSecondsRealtime(controller.MismatchDelaySeconds + .1f);
            }
            inputLock.Release(); camera.SetLookInputLocked(false);
            Assert.That(controller.Open(), Is.True); Assert.That(controller.TryReveal(0), Is.True);
            panel.Close(); Assert.That(inputLock.IsLocked, Is.False); Assert.That(camera.IsLookInputLocked, Is.False);
        }
    }
}
