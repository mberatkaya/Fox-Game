using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TilkiOyunu.Foundation.PlayModeTests
{
    public sealed class Sprint3ForestLightPathPlayModeTests
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
        public IEnumerator ForestContainsSprint3LightPathDependencies()
        {
            yield return LoadBootstrapToForest();

            Assert.That(GameServices.HasCurrent, Is.True);
            Assert.That(Object.FindFirstObjectByType<GuideNpc>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<QuestHUD>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<LightPathHUD>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<LightPathStart>(), Is.Not.Null);

            LightPathController controller = Object.FindFirstObjectByType<LightPathController>();
            Assert.That(controller, Is.Not.Null);
            Assert.That(controller.Quest, Is.Not.Null);
            Assert.That(controller.Quest.Id, Is.EqualTo(QuestService.LightPathQuestId));
            Assert.That(controller.Quest.PrerequisiteQuestIds, Does.Contain(QuestService.CollectMemoriesQuestId));
            Assert.That(controller.NodeCount, Is.EqualTo(5));

            LightPathNode[] nodes = Object.FindObjectsByType<LightPathNode>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Assert.That(nodes, Has.Length.EqualTo(5));

            HashSet<int> indices = new();
            foreach (LightPathNode node in nodes)
            {
                Assert.That(indices.Add(node.SequenceIndex), Is.True);
            }

            for (int i = 0; i < 5; i++)
            {
                Assert.That(indices, Does.Contain(i));
            }

            MemoryCollectible[] memories = Object.FindObjectsByType<MemoryCollectible>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Assert.That(memories, Has.Length.EqualTo(5));
        }

        private static IEnumerator LoadBootstrapToForest()
        {
            yield return SceneManager.LoadSceneAsync(SceneIds.Bootstrap, LoadSceneMode.Single);

            float timeoutAt = Time.realtimeSinceStartup + 5f;
            while (SceneManager.GetActiveScene().name != SceneIds.Forest && Time.realtimeSinceStartup < timeoutAt)
            {
                yield return null;
            }

            yield return null;
        }
    }
}
