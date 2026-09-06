using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TilkiOyunu.Foundation.PlayModeTests
{
    public sealed class Sprint2ForestQuestPlayModeTests
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
        public IEnumerator ForestContainsSprint2QuestDependencies()
        {
            yield return LoadBootstrapToForest();

            Assert.That(GameServices.HasCurrent, Is.True);
            Assert.That(GameServices.Current.Quest, Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<GuideNpc>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<DialoguePanelUI>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<QuestHUD>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<MemoryFeedbackUI>(), Is.Not.Null);

            MemoryCollectible[] collectibles = Object.FindObjectsByType<MemoryCollectible>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Assert.That(collectibles, Has.Length.EqualTo(5));

            HashSet<string> ids = new();
            foreach (MemoryCollectible collectible in collectibles)
            {
                Assert.That(collectible.Memory, Is.Not.Null);
                Assert.That(collectible.Quest, Is.Not.Null);
                Assert.That(ids.Add(collectible.Memory.Id), Is.True);
            }
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
