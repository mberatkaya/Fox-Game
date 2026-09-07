using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

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

        [UnityTest]
        public IEnumerator FinalSequenceOwnsCameraAndReleasesGameplayControl()
        {
            yield return LoadBootstrapToForest();

            CompleteQuestChain(GameServices.Current.Quest);

            FinalSequenceController final = Object.FindFirstObjectByType<FinalSequenceController>(FindObjectsInactive.Include);
            Assert.That(final, Is.Not.Null);
            ThirdPersonCameraController cameraController = final.CameraController != null
                ? final.CameraController
                : Object.FindFirstObjectByType<ThirdPersonCameraController>(FindObjectsInactive.Include);
            GameplayInputLock inputLock = Object.FindFirstObjectByType<GameplayInputLock>(FindObjectsInactive.Include);
            Camera gameplayCamera = final.GameplayCamera != null ? final.GameplayCamera : cameraController.GetComponent<Camera>();
            Assert.That(cameraController, Is.Not.Null);
            Assert.That(inputLock, Is.Not.Null);
            Assert.That(gameplayCamera, Is.Not.Null);
            Assert.That(final.TryGetPresentationCameraPose(out Vector3 targetPosition, out Quaternion targetRotation), Is.True);

            yield return null;
            gameplayCamera.transform.SetPositionAndRotation(
                targetPosition + new Vector3(2.5f, 0.75f, 2.5f),
                targetRotation * Quaternion.Euler(0f, 22f, 0f));
            cameraController.ResumeFromCurrentTransform();
            Vector3 startPosition = gameplayCamera.transform.position;

            Assert.That(final.Begin(), Is.True);
            yield return WaitUntilCameraMoves(gameplayCamera.transform, startPosition, 0.05f, 2f);

            Assert.That(final.HasCameraControl, Is.True);
            Assert.That(cameraController.IsExternalControlActive, Is.True);
            Assert.That(inputLock.IsLocked, Is.True);
            Assert.That(Vector3.Distance(startPosition, gameplayCamera.transform.position), Is.GreaterThan(0.05f));

            Vector3 presentationPosition = gameplayCamera.transform.position;
            yield return WaitFrames(4);
            Assert.That(cameraController.IsExternalControlActive, Is.True);
            Assert.That(Vector3.Distance(presentationPosition, gameplayCamera.transform.position), Is.LessThan(1.5f));

            final.CompleteAndClose();
            yield return null;

            Assert.That(final.HasCameraControl, Is.False);
            Assert.That(cameraController.IsExternalControlActive, Is.False);
            Assert.That(inputLock.IsLocked, Is.False);
        }

        [UnityTest]
        public IEnumerator FinalSequenceDisableReleasesCameraAndInput()
        {
            yield return LoadBootstrapToForest();

            CompleteQuestChain(GameServices.Current.Quest);

            FinalSequenceController final = Object.FindFirstObjectByType<FinalSequenceController>(FindObjectsInactive.Include);
            ThirdPersonCameraController cameraController = Object.FindFirstObjectByType<ThirdPersonCameraController>(FindObjectsInactive.Include);
            GameplayInputLock inputLock = Object.FindFirstObjectByType<GameplayInputLock>(FindObjectsInactive.Include);
            Assert.That(final.Begin(), Is.True);
            yield return null;

            Assert.That(cameraController.IsExternalControlActive, Is.True);
            Assert.That(inputLock.IsLocked, Is.True);

            final.enabled = false;
            yield return null;

            Assert.That(cameraController.IsExternalControlActive, Is.False);
            Assert.That(inputLock.IsLocked, Is.False);
        }

        [UnityTest]
        public IEnumerator CardMatchingPanelUnlocksCursorFocusesUiAndRestoresInput()
        {
            yield return LoadBootstrapToForest();
            PrepareCardQuestActive(GameServices.Current.Quest);

            CardMatchingController controller = Object.FindFirstObjectByType<CardMatchingController>(FindObjectsInactive.Include);
            CardMatchingPanelUI panel = Object.FindFirstObjectByType<CardMatchingPanelUI>(FindObjectsInactive.Include);
            GameplayInputLock inputLock = Object.FindFirstObjectByType<GameplayInputLock>(FindObjectsInactive.Include);
            ThirdPersonCameraController cameraController = Object.FindFirstObjectByType<ThirdPersonCameraController>(FindObjectsInactive.Include);
            Assert.That(controller, Is.Not.Null);
            Assert.That(panel, Is.Not.Null);
            Assert.That(EventSystem.current, Is.Not.Null);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            CursorLockMode previousLockState = Cursor.lockState;
            bool previousVisible = Cursor.visible;

            Assert.That(controller.Open(), Is.True);
            yield return null;

            Assert.That(panel.IsOpen, Is.True);
            Assert.That(Cursor.visible, Is.True);
            Assert.That(Cursor.lockState, Is.EqualTo(CursorLockMode.None));
            Assert.That(inputLock.IsLocked, Is.True);
            Assert.That(cameraController.IsLookInputLocked, Is.True);
            AssertSelectedUiIsInteractable();

            panel.Close();
            yield return null;

            Assert.That(panel.IsOpen, Is.False);
            Assert.That(Cursor.lockState, Is.EqualTo(previousLockState));
            Assert.That(Cursor.visible, Is.EqualTo(previousVisible));
            Assert.That(inputLock.IsLocked, Is.False);
            Assert.That(cameraController.IsLookInputLocked, Is.False);

            panel.Close();
            yield return null;
            Assert.That(inputLock.IsLocked, Is.False);
        }

        [UnityTest]
        public IEnumerator CardMatchingFocusRecoversWhenSelectedCardIsMatched()
        {
            yield return LoadBootstrapToForest();
            PrepareCardQuestActive(GameServices.Current.Quest);

            CardMatchingController controller = Object.FindFirstObjectByType<CardMatchingController>(FindObjectsInactive.Include);
            Assert.That(controller.Open(), Is.True);
            yield return null;

            (int first, int second) = FindMatchingPair(controller);
            CardMatchingCardButton firstButton = FindCardButton(first);
            CardMatchingCardButton secondButton = FindCardButton(second);
            Assert.That(firstButton, Is.Not.Null);
            Assert.That(secondButton, Is.Not.Null);

            EventSystem.current.SetSelectedGameObject(firstButton.SelectionObject);
            Assert.That(controller.TryReveal(first), Is.True);
            Assert.That(controller.TryReveal(second), Is.True);
            yield return null;

            Assert.That(firstButton.IsInteractable, Is.False);
            Assert.That(secondButton.IsInteractable, Is.False);
            Assert.That(EventSystem.current.currentSelectedGameObject, Is.Not.EqualTo(firstButton.SelectionObject));
            Assert.That(EventSystem.current.currentSelectedGameObject, Is.Not.EqualTo(secondButton.SelectionObject));
            AssertSelectedUiIsInteractable();
        }

        [UnityTest]
        public IEnumerator FinalCompletionPersistsAfterReload()
        {
            yield return LoadBootstrapToForest();
            CompleteQuestChain(GameServices.Current.Quest);

            FinalSequenceController final = Object.FindFirstObjectByType<FinalSequenceController>(FindObjectsInactive.Include);
            Assert.That(final.Begin(), Is.True);
            yield return WaitFrames(100);
            final.CompleteAndClose();
            yield return null;

            Assert.That(GameServices.Current.Quest.SaveData.finalCompleted, Is.True);
            Assert.That(GameServices.Current.Quest.SaveData.gameState, Is.EqualTo(GameState.Completed));

            yield return SceneManager.LoadSceneAsync(SceneIds.Bootstrap, LoadSceneMode.Single);
            float timeoutAt = Time.realtimeSinceStartup + 5f;
            while (SceneManager.GetActiveScene().name != SceneIds.Forest && Time.realtimeSinceStartup < timeoutAt)
            {
                yield return null;
            }

            Assert.That(GameServices.Current.Quest.SaveData.finalCompleted, Is.True);
            Assert.That(GameServices.Current.Quest.SaveData.gameState, Is.EqualTo(GameState.Completed));
        }

        [UnityTest]
        public IEnumerator EnvironmentVisualLayerIsColliderFree()
        {
            yield return LoadBootstrapToForest();

            GameObject environmentVisuals = GameObject.Find("Environment_Visuals");
            Assert.That(environmentVisuals, Is.Not.Null);
            Assert.That(environmentVisuals.GetComponentsInChildren<Collider>(true), Is.Empty);
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

        private static void PrepareCardQuestActive(QuestService service)
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
        }

        private static IEnumerator WaitFrames(int frameCount)
        {
            for (int i = 0; i < frameCount; i++)
            {
                yield return null;
            }
        }

        private static IEnumerator WaitUntilCameraMoves(Transform cameraTransform, Vector3 startPosition, float minimumDistance, float timeoutSeconds)
        {
            float timeoutAt = Time.realtimeSinceStartup + timeoutSeconds;
            while (Vector3.Distance(startPosition, cameraTransform.position) < minimumDistance && Time.realtimeSinceStartup < timeoutAt)
            {
                yield return null;
            }
        }

        private static (int first, int second) FindMatchingPair(CardMatchingController controller)
        {
            Dictionary<string, int> firstByPair = new();
            for (int i = 0; i < controller.CardCount; i++)
            {
                CardMatchingSlotView view = controller.GetCard(i);
                if (firstByPair.TryGetValue(view.PairId, out int first))
                {
                    return (first, i);
                }

                firstByPair[view.PairId] = i;
            }

            Assert.Fail("No matching card pair found.");
            return (-1, -1);
        }

        private static CardMatchingCardButton FindCardButton(int index)
        {
            foreach (CardMatchingCardButton button in Object.FindObjectsByType<CardMatchingCardButton>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (button.Index == index)
                {
                    return button;
                }
            }

            return null;
        }

        private static void AssertSelectedUiIsInteractable()
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            Assert.That(selected, Is.Not.Null);
            Assert.That(selected.activeInHierarchy, Is.True);
            Selectable selectable = selected.GetComponent<Selectable>();
            Assert.That(selectable, Is.Not.Null);
            Assert.That(selectable.IsInteractable(), Is.True);
        }
    }
}
