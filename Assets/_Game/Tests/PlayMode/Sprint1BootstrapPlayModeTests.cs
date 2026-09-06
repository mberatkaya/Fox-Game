using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TilkiOyunu.Foundation.PlayModeTests
{
    public sealed class Sprint1BootstrapPlayModeTests
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
        public IEnumerator BootstrapLoadsForestGameplayPrototype()
        {
            yield return LoadBootstrapToForest();

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(SceneIds.Forest));
            Assert.That(GameServices.HasCurrent, Is.True);
            Assert.That(Object.FindFirstObjectByType<FoxController>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<CharacterController>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<ThirdPersonCameraController>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<PlayerInteractor>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<InteractionPromptUI>(), Is.Not.Null);
            Assert.That(Object.FindFirstObjectByType<TestInteractable>(), Is.Not.Null);
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
