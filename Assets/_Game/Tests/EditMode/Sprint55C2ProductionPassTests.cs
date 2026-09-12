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
            Assert.That(npc.transform.Find("NPC_Guide_PhysicalBlocker")?.GetComponent<CapsuleCollider>()?.isTrigger, Is.False);
        }

        [Test]
        public void GuideNpcPrefabHasHumanoidAvatarAndRelaxedIdle()
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
                float bridgeDistance = DistanceToSegment(point, new Vector2(11f, -32f), new Vector2(35f, -4f));
                Assert.That(bridgeDistance, Is.GreaterThanOrEqualTo(8.5f), $"{rock.name} intrudes into the bridge approach corridor.");
                Assert.That(Vector2.Distance(point, new Vector2(22f, -18f)), Is.GreaterThanOrEqualTo(16f), $"{rock.name} intrudes into bridge center clearance.");
            }
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
                if (collider == null)
                {
                    continue;
                }

                blocking++;
                Assert.That(collider.isTrigger, Is.False);
                Vector3 worldColliderSize = Vector3.Scale(collider.size, rock.lossyScale);
                Assert.That(worldColliderSize.x, Is.InRange(0.3f, bounds.size.x + 0.35f));
                Assert.That(worldColliderSize.z, Is.InRange(0.3f, bounds.size.z + 0.35f));
            }

            Assert.That(blocking, Is.GreaterThanOrEqualTo(35));
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

        private static float DistanceToSegment(Vector2 point, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float t = Mathf.Clamp01(Vector2.Dot(point - a, ab) / ab.sqrMagnitude);
            return Vector2.Distance(point, a + ab * t);
        }
    }
}
