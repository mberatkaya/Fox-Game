using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
            Assert.That(player.transform.Find("VisualRoot/ToonFox"), Is.Not.Null);
            Assert.That(player.transform.Find("VisualRoot/OpenGameArtFox"), Is.Null);
            Assert.That(player.GetComponentInChildren<FoxAnimationDriver>(true), Is.Not.Null);
            Assert.That(player.GetComponent<FootstepAudio>(), Is.Not.Null);

            Assert.That(GameObject.Find("Environment_Visuals"), Is.Not.Null);
            Assert.That(GameObject.Find("World"), Is.Not.Null);
            Assert.That(GameObject.Find("Sprint55B_PrimaryTerrain"), Is.Not.Null);
            Assert.That(GameObject.Find("Start Platform"), Is.Not.Null);
            Assert.That(GameObject.Find("Environment"), Is.Not.Null);
            Assert.That(GameObject.Find("Sprint55C_Bridge_Walkway"), Is.Not.Null);
            Assert.That(GameObject.Find("LM_FinalHill"), Is.Not.Null);
            Assert.That(GameObject.Find("Sprint 5 Scene Audio"), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<SceneLoopAudio>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<FinalSequenceController>(FindObjectsInactive.Include), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<FinalMessagePanelUI>(FindObjectsInactive.Include), Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator InitialGameplayPresentationStateIsRenderable()
        {
            yield return LoadBootstrapToForest();
            yield return WaitFrames(3);

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(SceneIds.Forest));

            Camera camera = Camera.main;
            Assert.That(camera, Is.Not.Null);
            Assert.That(camera.isActiveAndEnabled, Is.True);
            Assert.That(camera.targetDisplay, Is.EqualTo(0));
            Assert.That(camera.targetTexture, Is.Null);
            Assert.That(camera.cullingMask, Is.Not.EqualTo(0));
            Assert.That(IsFinite(camera.transform.position), Is.True);
            Assert.That(IsFinite(camera.transform.rotation), Is.True);

            ThirdPersonCameraController cameraController = camera.GetComponent<ThirdPersonCameraController>();
            Assert.That(cameraController, Is.Not.Null);
            Assert.That(cameraController.isActiveAndEnabled, Is.True);
            Assert.That(cameraController.Target, Is.Not.Null);
            Assert.That(cameraController.IsExternalControlActive, Is.False);
            Assert.That(Vector3.Distance(camera.transform.position, cameraController.Target.position), Is.GreaterThan(2.5f));
            Assert.That(Vector3.Dot(camera.transform.forward, (cameraController.Target.position - camera.transform.position).normalized), Is.GreaterThan(0.45f));

            GameObject player = GameObject.Find("PlayerFox");
            Assert.That(player, Is.Not.Null);
            Assert.That(player.activeInHierarchy, Is.True);
            Assert.That(player.transform.Find("VisualRoot"), Is.Not.Null);
            Assert.That(player.transform.Find("VisualRoot").gameObject.activeInHierarchy, Is.True);

            Terrain terrain = Object.FindFirstObjectByType<Terrain>();
            Assert.That(terrain, Is.Not.Null);
            TerrainCollider terrainCollider = terrain.GetComponent<TerrainCollider>();
            Assert.That(terrainCollider, Is.Not.Null);
            Assert.That(terrainCollider.enabled, Is.True);
            Assert.That(terrainCollider.terrainData, Is.EqualTo(terrain.terrainData));
            Assert.That(player.transform.position.y, Is.GreaterThanOrEqualTo(terrain.SampleHeight(player.transform.position) - 0.01f));

            GameObject startPlatform = GameObject.Find("Start Platform");
            Assert.That(startPlatform, Is.Not.Null);
            Assert.That(startPlatform.GetComponent<BoxCollider>(), Is.Not.Null);

            GameObject world = GameObject.Find("World");
            GameObject environment = GameObject.Find("Environment");
            Assert.That(world, Is.Not.Null);
            Assert.That(environment, Is.Not.Null);
            Assert.That(HasRenderableInView(camera, player.transform.Find("VisualRoot")), Is.True);
            Assert.That(HasRenderableInView(camera, environment.transform), Is.True);

            AssertHiddenCanvasGroup(Object.FindFirstObjectByType<DialoguePanelUI>(FindObjectsInactive.Include));
            AssertHiddenCanvasGroup(Object.FindFirstObjectByType<CardMatchingPanelUI>(FindObjectsInactive.Include));
            AssertHiddenCanvasGroup(Object.FindFirstObjectByType<FinalMessagePanelUI>(FindObjectsInactive.Include));
            AssertHiddenCanvasGroup(Object.FindFirstObjectByType<MemoryFeedbackUI>(FindObjectsInactive.Include));
            AssertHiddenCanvasGroup(Object.FindFirstObjectByType<LightPathHUD>(FindObjectsInactive.Include));
            var questHud = Object.FindFirstObjectByType<QuestHUD>(FindObjectsInactive.Include);
            Assert.That(questHud.GetComponent<CanvasGroup>().alpha, Is.EqualTo(1f), "Fresh-save navigation must show the Guide destination.");
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
        public IEnumerator GuideStaysAboveTerrainAndTallerThanFoxDuringIdle()
        {
            yield return LoadBootstrapToForest();
            GameObject npc = GameObject.Find("NPC_Guide");
            Transform visual = npc.transform.Find("VisualRoot/NPC_Guide_Visual");
            Transform body = visual.Find("AnimatedRoot/GuideCharacterRig/QuaterniusGuideBody");
            Transform hair = visual.Find("AnimatedRoot/GuideCharacterRig/QuaterniusGuideHair");
            GameObject player = GameObject.Find("PlayerFox");
            Terrain terrain = Object.FindFirstObjectByType<Terrain>();
            Animator animator = visual.GetComponent<Animator>();
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            player.GetComponent<FoxController>().enabled = false;
            CharacterController capsule = player.GetComponent<CharacterController>();
            capsule.enabled = false;
            Vector3 besideGuide = npc.transform.position - npc.transform.right * 1.8f;
            besideGuide.y = terrain.transform.position.y + terrain.SampleHeight(besideGuide) + 0.04f;
            player.transform.SetPositionAndRotation(besideGuide, npc.transform.rotation);
            capsule.enabled = true;
            Bounds fox = SkinnedBounds(player.transform.Find("VisualRoot/ToonFox"));
            float lowestFeet = float.PositiveInfinity;
            float highestFeet = float.NegativeInfinity;

            for (int sample = 0; sample < 8; sample++)
            {
                animator.Update(0.5f);
                yield return null;
                Bounds human = SkinnedBounds(body);
                lowestFeet = Mathf.Min(lowestFeet, human.min.y);
                highestFeet = Mathf.Max(highestFeet, human.min.y);
                Bounds hairstyle = SkinnedBounds(hair);
                float ground = terrain.transform.position.y + terrain.SampleHeight(npc.transform.position);
                Assert.That(human.min.y - ground, Is.InRange(-0.02f, 0.12f), "Animated feet must stay on the terrain.");
                Assert.That(human.size.y, Is.GreaterThan(fox.size.y * 3.1f), "Compare visible meshes, not imported culling bounds or props.");
                Assert.That(hairstyle.max.y - human.max.y, Is.InRange(-0.1f, 0.25f), "Hair must remain attached to the head.");
                AssertGuideAccessoryFit(visual);
            }
            Assert.That(highestFeet - lowestFeet, Is.GreaterThan(0.01f), "Idle must keep moving without sinking the body.");
            Debug.Log($"WORLD_SCALE guideHeight={SkinnedBounds(body).size.y:F3} foxHeight={fox.size.y:F3}");
            CaptureWorldScaleReview("NPC Scale");
            CaptureWorldScaleReview("NPC Outfit");
        }

        private static void AssertGuideAccessoryFit(Transform visual)
        {
            Transform frame = visual.Find("AnimatedRoot");
            var bones = new Dictionary<string, Transform>();
            foreach (Transform child in visual.GetComponentsInChildren<Transform>()) bones[child.name] = child;
            Transform staff = bones["GuideStaff"];
            Assert.That(staff.parent, Is.EqualTo(bones["hand_r"]));
            Assert.That(Vector3.Distance(staff.position, bones["GuideStaffGrip"].position), Is.LessThan(0.01f));
            foreach (string finger in new[] { "index", "middle", "pinky" })
            {
                Vector3 tip = bones[$"{finger}_04_leaf_r"].position;
                float radialDistance = Vector3.ProjectOnPlane(tip - staff.position, staff.up).magnitude;
                Assert.That(radialDistance, Is.LessThan(0.15f), $"{finger} must curl around the staff instead of pointing away.");
            }
            Assert.That(frame.InverseTransformPoint(bones["hand_r"].position).x, Is.GreaterThan(0.55f), "Right arm must remain outside the torso.");
            Assert.That(frame.InverseTransformPoint(bones["hand_l"].position).x, Is.LessThan(-0.5f), "Left arm must remain outside the torso.");
            Assert.That(Vector3.Distance(bones["GuideShoulderFern_Left"].position, bones["upperarm_l"].position), Is.LessThan(0.1f));
            Assert.That(Vector3.Distance(bones["GuideShoulderFern_Right"].position, bones["upperarm_r"].position), Is.LessThan(0.1f));
        }

        [UnityTest]
        public IEnumerator FoxCanCrossRaisedBridgeInBothDirections()
        {
            yield return LoadBootstrapToForest();
            Transform bridge = GameObject.Find("Small Bridge").transform;
            GameObject player = GameObject.Find("PlayerFox");
            FoxController controller = player.GetComponent<FoxController>();
            controller.enabled = false;
            CharacterController capsule = player.GetComponent<CharacterController>();
            Terrain terrain = Object.FindFirstObjectByType<Terrain>();
            Vector3 forward = Vector3.ProjectOnPlane(bridge.forward, Vector3.up).normalized;

            foreach (float direction in new[] { 1f, -1f })
            {
                capsule.enabled = false;
                Vector3 start = bridge.TransformPoint(new Vector3(0f, 0f, -direction * 18f));
                start.y = terrain.transform.position.y + terrain.SampleHeight(start) + 0.12f;
                player.transform.position = start;
                capsule.enabled = true;
                Physics.SyncTransforms();
                for (int step = 0; step < 450; step++)
                {
                    Vector3 motion = forward * (direction * 0.09f) + Vector3.down * 0.1f;
                    capsule.Move(InvokeResolveObstacleMotion(controller, motion));
                    float progress = bridge.InverseTransformPoint(player.transform.position).z;
                    if (Mathf.Abs(progress) < 12f)
                    {
                        float localHeight = bridge.InverseTransformPoint(player.transform.position).y;
                        Assert.That(localHeight, Is.InRange(0.2f, 0.8f), "Fox must remain supported by the deck.");
                    }
                    if (step % 20 == 0) yield return null;
                }
                float end = bridge.InverseTransformPoint(player.transform.position).z * direction;
                Assert.That(end, Is.GreaterThan(17f), $"Fox stalled at {player.transform.position} travelling {direction}.");
            }
            CaptureWorldScaleReview("Bridge Banks");
        }

        private static Bounds SkinnedBounds(Transform root)
        {
            Bounds bounds = default;
            bool hasVertex = false;
            Mesh mesh = new Mesh();
            try
            {
                foreach (SkinnedMeshRenderer renderer in root.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    renderer.BakeMesh(mesh, true);
                    foreach (Vector3 vertex in mesh.vertices)
                    {
                        Vector3 world = renderer.transform.TransformPoint(vertex);
                        if (!hasVertex) { bounds = new Bounds(world, Vector3.zero); hasVertex = true; }
                        else bounds.Encapsulate(world);
                    }
                }
                Assert.That(hasVertex, Is.True, root.name);
                return bounds;
            }
            finally { Object.DestroyImmediate(mesh); }
        }

        private static void CaptureWorldScaleReview(string view)
        {
#if UNITY_EDITOR
            if (System.Environment.GetEnvironmentVariable("TILKI_CAPTURE_WORLD_SCALE") == "1")
                Assert.That(UnityEditor.EditorApplication.ExecuteMenuItem($"Tilki Oyunu/Sprint 5.5/Review/{view}"), Is.True);
#endif
        }

        [UnityTest]
        public IEnumerator EnvironmentVisualLayerIsColliderFree()
        {
            yield return LoadBootstrapToForest();

            GameObject environmentVisuals = GameObject.Find("Environment_Visuals");
            Assert.That(environmentVisuals, Is.Not.Null);
            Assert.That(environmentVisuals.GetComponentsInChildren<Collider>(true), Is.Empty);
        }

        [UnityTest]
        public IEnumerator RepeatedJumpPressesDuringTakeoffDoNotStackImpulse()
        {
            FoxController controller = CreateControllerForJumpTest();

            SetGroundedForTest(controller, true);
            SetPrivateField(controller, "verticalVelocity", -2f);
            Assert.That(InvokeTryStartJump(controller, true), Is.True);
            float firstJumpVelocity = controller.Velocity.y;
            Assert.That(firstJumpVelocity, Is.GreaterThan(0.5f));

            SetGroundedForTest(controller, true);
            SetPrivateField(controller, "verticalVelocity", firstJumpVelocity - 0.1f);
            Assert.That(InvokeTryStartJump(controller, true), Is.False);
            Assert.That(controller.Velocity.y, Is.EqualTo(firstJumpVelocity - 0.1f).Within(0.001f));

            SetGroundedForTest(controller, true);
            SetPrivateField(controller, "verticalVelocity", 0f);
            Assert.That(InvokeTryStartJump(controller, true), Is.False);
            Assert.That(controller.Velocity.y, Is.EqualTo(0f).Within(0.001f));

            InvokeRefreshJumpAvailabilityAfterMove(controller, CollisionFlags.Below);
            SetGroundedForTest(controller, true);
            Assert.That(InvokeTryStartJump(controller, true), Is.True);
            Assert.That(controller.Velocity.y, Is.GreaterThan(0.5f));

            yield return null;
            Object.DestroyImmediate(controller.gameObject);
        }

        [UnityTest]
        public IEnumerator FoxObstacleSweepStopsBeforeSolidObjects()
        {
            FoxController controller = CreateControllerForJumpTest();
            GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.name = "Fox Movement Blocker Test";
            obstacle.transform.position = new Vector3(0f, 0.55f, 0.9f);
            obstacle.transform.localScale = new Vector3(1f, 1.1f, 0.35f);
            Physics.SyncTransforms();

            Vector3 resolvedMotion = InvokeResolveObstacleMotion(controller, Vector3.forward * 2f);

            Assert.That(resolvedMotion.z, Is.LessThan(0.55f));
            Assert.That(resolvedMotion.z, Is.GreaterThanOrEqualTo(0f));

            yield return null;
            Object.DestroyImmediate(obstacle);
            Object.DestroyImmediate(controller.gameObject);
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

        private static void AssertHiddenCanvasGroup(Component component)
        {
            Assert.That(component, Is.Not.Null);
            CanvasGroup panel = component.GetComponent<CanvasGroup>();
            Assert.That(panel, Is.Not.Null);
            Assert.That(panel.alpha, Is.EqualTo(0f));
            Assert.That(panel.interactable, Is.False);
            Assert.That(panel.blocksRaycasts, Is.False);
        }

        private static FoxController CreateControllerForJumpTest()
        {
            GameObject player = new("Jump Test Fox");
            player.SetActive(false);
            player.transform.position = Vector3.zero;
            CharacterController characterController = player.AddComponent<CharacterController>();
            characterController.height = 1.1f;
            characterController.radius = 0.36f;
            characterController.center = new Vector3(0f, 0.55f, 0f);

            Transform groundProbe = new GameObject("Ground Probe").transform;
            groundProbe.SetParent(player.transform, false);
            groundProbe.localPosition = new Vector3(0f, 0.08f, 0f);

            FoxController controller = player.AddComponent<FoxController>();
            SetPrivateField(controller, "groundProbe", groundProbe);
            player.SetActive(true);
            return controller;
        }

        private static bool InvokeTryStartJump(FoxController controller, bool pressedThisFrame)
        {
            MethodInfo method = typeof(FoxController).GetMethod("TryStartJump", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            return (bool)method.Invoke(controller, new object[] { pressedThisFrame });
        }

        private static void InvokeRefreshJumpAvailabilityAfterMove(FoxController controller, CollisionFlags collisionFlags)
        {
            MethodInfo method = typeof(FoxController).GetMethod("RefreshJumpAvailabilityAfterMove", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(controller, new object[] { collisionFlags });
        }

        private static Vector3 InvokeResolveObstacleMotion(FoxController controller, Vector3 motion)
        {
            MethodInfo method = typeof(FoxController).GetMethod("ResolveObstacleMotion", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            return (Vector3)method.Invoke(controller, new object[] { motion });
        }

        private static void SetGroundedForTest(FoxController controller, bool isGrounded)
        {
            typeof(FoxController)
                .GetField("<IsGrounded>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(controller, isGrounded);
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(target, value);
        }

        private static bool HasRenderableInView(Camera camera, Transform root)
        {
            if (camera == null || root == null)
            {
                return false;
            }

            Plane[] frustum = GeometryUtility.CalculateFrustumPlanes(camera);
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer.enabled && renderer.gameObject.activeInHierarchy && GeometryUtility.TestPlanesAABB(frustum, renderer.bounds))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsFinite(Vector3 vector)
        {
            return IsFinite(vector.x)
                && IsFinite(vector.y)
                && IsFinite(vector.z);
        }

        private static bool IsFinite(Quaternion quaternion)
        {
            return IsFinite(quaternion.x)
                && IsFinite(quaternion.y)
                && IsFinite(quaternion.z)
                && IsFinite(quaternion.w);
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
