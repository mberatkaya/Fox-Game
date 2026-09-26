using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TilkiOyunu.Foundation.Tests
{
    public sealed class Sprint55C1CorrectivePassTests
    {
        [Test]
        public void ProductionTreesAllHaveSimpleTrunkColliders()
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            GameObject environment = GameObject.Find("Environment");
            Assert.That(environment, Is.Not.Null);

            int treeCount = CountNamedChildren(environment.transform, "QuaterniusTree_");
            int colliderCount = CountTreeColliders(environment.transform);

            Assert.That(treeCount, Is.InRange(150, 360));
            Assert.That(colliderCount, Is.EqualTo(treeCount));
        }

        [Test]
        public void SolidRocksAndBridgeHaveBlockingColliders()
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            GameObject environment = GameObject.Find("Environment");
            Assert.That(environment, Is.Not.Null);
            int rockCount = CountNamedChildren(environment.transform, "QuaterniusRock_");
            Assert.That(rockCount, Is.GreaterThanOrEqualTo(100));
            Assert.That(CountBlockingRockColliders(environment.transform), Is.EqualTo(rockCount));

            GameObject walkway = GameObject.Find("Sprint55C_Bridge_Walkway");
            GameObject leftRail = GameObject.Find("Bridge Left Rail Collider");
            GameObject rightRail = GameObject.Find("Bridge Right Rail Collider");

            Assert.That(walkway, Is.Not.Null);
            Assert.That(walkway.GetComponent<BoxCollider>(), Is.Not.Null);
            Assert.That(walkway.GetComponent<BoxCollider>().isTrigger, Is.False);
            Assert.That(leftRail, Is.Not.Null);
            Assert.That(rightRail, Is.Not.Null);
            Assert.That(leftRail.GetComponent<BoxCollider>().isTrigger, Is.False);
            Assert.That(rightRail.GetComponent<BoxCollider>().isTrigger, Is.False);
        }

        [Test]
        public void FoxAnimatorKeepsRootMotionOffAndReferencesExtendedStates()
        {
            GameObject player = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/Characters/PlayerFox.prefab");
            Assert.That(player, Is.Not.Null);

            Animator animator = player.GetComponentInChildren<Animator>(true);
            Assert.That(animator, Is.Not.Null);
            Assert.That(animator.applyRootMotion, Is.False);
            Assert.That(animator.runtimeAnimatorController, Is.Not.Null);

            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/_Game/Art/Characters/FoxAnimatorController.controller");
            Assert.That(controller, Is.Not.Null);
            RequireParameter(controller, "Speed", AnimatorControllerParameterType.Float);
            RequireParameter(controller, "Grounded", AnimatorControllerParameterType.Bool);
            RequireParameter(controller, "VerticalVelocity", AnimatorControllerParameterType.Float);
            RequireParameter(controller, "IdleSit", AnimatorControllerParameterType.Trigger);
            RequireParameter(controller, "IdleBreak", AnimatorControllerParameterType.Trigger);
            Assert.That(FindState(controller.layers[0].stateMachine, "Jump"), Is.Not.Null);
            Assert.That(FindState(controller.layers[0].stateMachine, "Fall"), Is.Not.Null);
        }

        [Test]
        public void TerrainCorrectionKeepsGroundMatte()
        {
            Material terrain = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Game/Art/Environment/Sprint55C/Sprint55C_Terrain_URP.mat");
            Assert.That(terrain, Is.Not.Null);
            if (terrain.HasProperty("_Smoothness"))
            {
                Assert.That(terrain.GetFloat("_Smoothness"), Is.LessThanOrEqualTo(0.05f));
            }

            if (terrain.HasProperty("_Metallic"))
            {
                Assert.That(terrain.GetFloat("_Metallic"), Is.EqualTo(0f).Within(0.001f));
            }
        }

        private static void RequireParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
        {
            foreach (AnimatorControllerParameter parameter in controller.parameters)
            {
                if (parameter.name == name && parameter.type == type)
                {
                    return;
                }
            }

            Assert.Fail($"Missing Animator parameter {name}.");
        }

        private static AnimatorState FindState(AnimatorStateMachine stateMachine, string name)
        {
            foreach (ChildAnimatorState child in stateMachine.states)
            {
                if (child.state != null && child.state.name == name)
                {
                    return child.state;
                }
            }

            foreach (ChildAnimatorStateMachine childMachine in stateMachine.stateMachines)
            {
                AnimatorState found = FindState(childMachine.stateMachine, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private static int CountNamedChildren(Transform root, string prefix)
        {
            int count = root.name.StartsWith(prefix) ? 1 : 0;
            for (int i = 0; i < root.childCount; i++)
            {
                count += CountNamedChildren(root.GetChild(i), prefix);
            }

            return count;
        }

        private static int CountTreeColliders(Transform root)
        {
            int count = root.name.StartsWith("QuaterniusTree_") && root.GetComponent<Collider>() != null ? 1 : 0;
            for (int i = 0; i < root.childCount; i++)
            {
                count += CountTreeColliders(root.GetChild(i));
            }

            return count;
        }

        private static int CountBlockingRockColliders(Transform root)
        {
            int count = root.name.StartsWith("QuaterniusRock_") && root.GetComponent<BoxCollider>() != null ? 1 : 0;
            for (int i = 0; i < root.childCount; i++)
            {
                count += CountBlockingRockColliders(root.GetChild(i));
            }

            return count;
        }
    }
}
