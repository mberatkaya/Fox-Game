using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TilkiOyunu.Foundation.Tests
{
    public sealed class Sprint55C2ProductionPassTests
    {
        [Test]
        public void UniversalBaseCharactersProductionSubsetIsOrganized()
        {
            Assert.That(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ThirdParty/Quaternius/UniversalBaseCharacters/BaseCharacters/Superhero_Female_FullBody.fbx"), Is.Not.Null);
            Assert.That(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ThirdParty/Quaternius/UniversalBaseCharacters/Hairstyles/Hair_Buns.fbx"), Is.Not.Null);
            Assert.That(File.Exists("Assets/ThirdParty/Quaternius/UniversalBaseCharacters/License_Standard.txt"), Is.True);
            Assert.That(Directory.Exists("Assets/ThirdParty/Quaternius/UniversalBaseCharacters/Base Characters"), Is.False);
            Assert.That(Directory.Exists("Assets/ThirdParty/Quaternius/UniversalBaseCharacters/Godot - UE"), Is.False);
        }

        [Test]
        public void GuideNpcUsesProductionVisualAndKeepsGameplayObject()
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            GameObject npc = GameObject.Find("NPC_Guide");
            Assert.That(npc, Is.Not.Null);
            Assert.That(npc.GetComponent<GuideNpc>(), Is.Not.Null);

            CapsuleCollider interaction = npc.GetComponent<CapsuleCollider>();
            Assert.That(interaction, Is.Not.Null);
            Assert.That(interaction.isTrigger, Is.True);

            AssertPlaceholderHidden(npc.transform.Find("Guide Body"));
            AssertPlaceholderHidden(npc.transform.Find("Guide Head"));
            Assert.That(npc.transform.Find("VisualRoot/NPC_Guide_Visual"), Is.Not.Null);
            CapsuleCollider blocker = npc.transform.Find("NPC_Guide_PhysicalBlocker")?.GetComponent<CapsuleCollider>();
            Assert.That(blocker, Is.Not.Null);
            Assert.That(blocker.isTrigger, Is.False);
            Assert.That(blocker.height, Is.GreaterThanOrEqualTo(3f));
            Assert.That(blocker.radius, Is.GreaterThanOrEqualTo(0.6f));
        }

        [Test]
        public void GuideNpcPrefabHasHumanoidAvatarRelaxedIdleAndReadableForestSilhouette()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/NPC/NPC_Guide_Visual.prefab");
            Assert.That(prefab, Is.Not.Null);

            Animator animator = prefab.GetComponent<Animator>();
            Assert.That(animator, Is.Not.Null);
            Assert.That(animator.applyRootMotion, Is.False);
            Assert.That(animator.avatar, Is.Not.Null);
            Assert.That(animator.avatar.isValid, Is.True);
            Assert.That(animator.avatar.isHuman, Is.True);

            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/_Game/Animations/NPC/GuideNpcAnimator.controller");
            Assert.That(controller, Is.Not.Null);
            Assert.That(controller.layers[0].stateMachine.defaultState.motion?.name, Is.EqualTo("GuideNpc_RelaxedIdle"));

            AnimationClip idle = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_Game/Animations/NPC/GuideNpc_RelaxedIdle.anim");
            Assert.That(idle, Is.Not.Null);
            Assert.That(idle.length, Is.GreaterThan(2.5f));
            Assert.That(prefab.transform.Find("AnimatedRoot/GuideStaff"), Is.Not.Null);
            Assert.That(prefab.transform.Find("AnimatedRoot/GuideBeacon"), Is.Not.Null);
            Assert.That(prefab.transform.Find("AnimatedRoot/GuideVisibilityRing"), Is.Not.Null);
            Assert.That(prefab.transform.Find("AnimatedRoot/GuideShoulderFern_Left"), Is.Not.Null);
            Transform guideBody = prefab.transform.Find("AnimatedRoot/GuideCharacterRig/QuaterniusGuideBody");
            Assert.That(guideBody, Is.Not.Null);

            GameObject foxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/Characters/PlayerFox.prefab");
            Assert.That(foxPrefab, Is.Not.Null);
            Transform foxVisual = foxPrefab.transform.Find("VisualRoot/ToonFox");
            Assert.That(foxVisual, Is.Not.Null);

            Bounds guideBodyBounds = CalculateRendererBounds(guideBody.gameObject);
            Bounds foxBounds = CalculateRendererBounds(foxVisual.gameObject);
            Assert.That(guideBodyBounds.size.y, Is.InRange(2.45f, 3.25f), $"Guide body should read larger than the fox in gameplay, measured {guideBodyBounds.size.y:0.00}m.");
            Assert.That(guideBodyBounds.size.y, Is.GreaterThan(foxBounds.size.y * 1.65f), $"Guide body {guideBodyBounds.size.y:0.00}m should be unmistakably taller than fox {foxBounds.size.y:0.00}m.");
            Assert.That(CalculateRendererBounds(prefab).size.y, Is.GreaterThanOrEqualTo(3.2f));
        }

        [Test]
        public void BridgeCorridorIsClearOfGeneratedRocks()
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            GameObject environment = GameObject.Find("Environment");
            Assert.That(environment, Is.Not.Null);

            foreach (Transform rock in FindNamed(environment.transform, "QuaterniusRock_"))
            {
                Vector2 point = new(rock.position.x, rock.position.z);
                float bridgeDistance = DistanceToSegment(point, new Vector2(13.4f, -11.5f), new Vector2(35.8f, -28.3f));
                Assert.That(bridgeDistance, Is.GreaterThanOrEqualTo(7.5f), $"{rock.name} intrudes into the bridge approach corridor.");
                Assert.That(Vector2.Distance(point, new Vector2(24.6f, -19.9f)), Is.GreaterThanOrEqualTo(16f), $"{rock.name} intrudes into bridge center clearance.");
            }
        }

        [Test]
        public void BridgeSpansCreekPerpendicularBetweenBothBanks()
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            GameObject bridge = GameObject.Find("Small Bridge");
            GameObject walkway = GameObject.Find("Sprint55C_Bridge_Walkway");
            Assert.That(bridge, Is.Not.Null);
            Assert.That(walkway, Is.Not.Null);

            Vector3 bridgeForward = bridge.transform.forward;
            Vector2 bridgeDirection = new(bridgeForward.x, bridgeForward.z);
            Vector2 creekDirection = new Vector2(21f, 28f).normalized;
            Assert.That(Mathf.Abs(Vector2.Dot(bridgeDirection.normalized, creekDirection)), Is.LessThan(0.08f));

            Assert.That(walkway.transform.localScale.x, Is.EqualTo(4.4f).Within(0.01f));
            Assert.That(walkway.transform.localScale.z, Is.EqualTo(28f).Within(0.01f));
            Assert.That(walkway.GetComponent<BoxCollider>()?.isTrigger, Is.False);

            Terrain terrain = UnityEngine.Object.FindFirstObjectByType<Terrain>();
            Assert.That(terrain, Is.Not.Null);
            float terrainY = terrain.transform.position.y + terrain.SampleHeight(bridge.transform.position);
            Assert.That(bridge.transform.position.y - terrainY, Is.GreaterThanOrEqualTo(1.35f));

            GameObject creekWater = GameObject.Find("GB_Creek_TempWater");
            Assert.That(creekWater, Is.Not.Null);
            Bounds walkwayBounds = CalculateRendererBounds(walkway);
            Bounds waterBounds = CalculateRendererBounds(creekWater);
            Assert.That(walkwayBounds.min.y, Is.GreaterThan(waterBounds.max.y + 0.08f));

            Vector2 center = new(bridge.transform.position.x, bridge.transform.position.z);
            Vector2 bankA = center - bridgeDirection.normalized * 14f;
            Vector2 bankB = center + bridgeDirection.normalized * 14f;
            Assert.That(DistanceToSegment(center, new Vector2(17f, -30f), new Vector2(38f, -2f)), Is.LessThan(1.5f));
            Assert.That(DistanceToSegment(bankA, new Vector2(17f, -30f), new Vector2(38f, -2f)), Is.GreaterThan(10f));
            Assert.That(DistanceToSegment(bankB, new Vector2(17f, -30f), new Vector2(38f, -2f)), Is.GreaterThan(10f));
        }

        [Test]
        public void ProductionRockScalesStayBelievableAndBlockingRocksHaveFitColliders()
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            GameObject environment = GameObject.Find("Environment");
            Assert.That(environment, Is.Not.Null);

            int blocking = 0;
            foreach (Transform rock in FindNamed(environment.transform, "QuaterniusRock_"))
            {
                Bounds bounds = CalculateRendererBounds(rock.gameObject);
                float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
                Assert.That(maxDimension, Is.LessThanOrEqualTo(3.1f), $"{rock.name} is too large: {maxDimension:0.00}m.");
                BoxCollider collider = rock.GetComponent<BoxCollider>();
                Assert.That(collider, Is.Not.Null, $"{rock.name} should block the player.");
                blocking++;
                Assert.That(collider.isTrigger, Is.False);
                Vector3 worldColliderSize = Vector3.Scale(collider.size, rock.lossyScale);
                Assert.That(worldColliderSize.x, Is.InRange(0.2f, bounds.size.x + 0.35f));
                Assert.That(worldColliderSize.z, Is.InRange(0.2f, bounds.size.z + 0.35f));
            }

            Assert.That(blocking, Is.GreaterThanOrEqualTo(100));
        }

        [Test]
        public void OldHandmadePrototypeMapDressingIsRemovedFromRuntimeScene()
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);

            string[] removedObjects =
            {
                "TreeCollectionForest",
                "Safe Clearing Ground",
                "Path To Trees",
                "Future Memory Area Marker",
                "Future Quest Area Marker",
                "GB_SpawnMeadow_Readability",
                "GB_NPCGrove_Readability",
                "GB_FinalHill_Summit"
            };

            foreach (string objectName in removedObjects)
            {
                Assert.That(FindSceneObjectIncludingInactive(objectName), Is.Null, $"{objectName} should be removed from the production runtime scene.");
            }

            Assert.That(FindSceneObjectsByPrefix("Traversal Tree "), Is.Empty, "Sprint 1 handmade cylinder/sphere traversal trees should not remain in the runtime scene.");
        }

        [Test]
        public void TreeTrunkCollidersCoverBodyWithoutCanopy()
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            GameObject environment = GameObject.Find("Environment");
            Assert.That(environment, Is.Not.Null);

            foreach (Transform tree in FindNamed(environment.transform, "QuaterniusTree_"))
            {
                CapsuleCollider collider = tree.GetComponent<CapsuleCollider>();
                Assert.That(collider, Is.Not.Null, tree.name);
                float worldRadius = collider.radius * tree.lossyScale.x;
                float worldHeight = collider.height * tree.lossyScale.y;
                Assert.That(worldRadius, Is.InRange(0.18f, 0.44f), tree.name);
                Assert.That(worldHeight, Is.InRange(2.25f, 4.5f), tree.name);
            }
        }

        private static void AssertPlaceholderHidden(Transform placeholder)
        {
            Assert.That(placeholder, Is.Not.Null);
            foreach (Renderer renderer in placeholder.GetComponentsInChildren<Renderer>(true))
            {
                Assert.That(renderer.enabled, Is.False, $"{placeholder.name} renderer should be disabled.");
            }
        }

        private static Transform[] FindNamed(Transform root, string prefix)
        {
            return root.GetComponentsInChildren<Transform>(true)
                .Where(transform => transform.name.StartsWith(prefix, StringComparison.Ordinal))
                .ToArray();
        }

        private static Bounds CalculateRendererBounds(GameObject root)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            Assert.That(renderers.Length, Is.GreaterThan(0), root.name);
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds;
        }

        private static GameObject FindSceneObjectIncludingInactive(string name)
        {
            return UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(transform => transform.gameObject.scene.IsValid())
                .FirstOrDefault(transform => transform.name == name)
                ?.gameObject;
        }

        private static GameObject[] FindSceneObjectsByPrefix(string prefix)
        {
            return UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(transform => transform.gameObject.scene.IsValid() && transform.name.StartsWith(prefix, StringComparison.Ordinal))
                .Select(transform => transform.gameObject)
                .ToArray();
        }

        private static float DistanceToSegment(Vector2 point, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float t = Mathf.Clamp01(Vector2.Dot(point - a, ab) / ab.sqrMagnitude);
            return Vector2.Distance(point, a + ab * t);
        }
    }
}
