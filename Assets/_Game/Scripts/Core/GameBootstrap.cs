using UnityEngine;
using UnityEngine.Audio;

namespace TilkiOyunu.Foundation
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private GameContentConfig contentConfig;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private bool persistAcrossScenes = true;

        public GameContentConfig ContentConfig => contentConfig;

        private void Awake()
        {
            if (GameServices.HasCurrent)
            {
                Destroy(gameObject);
                return;
            }

            if (persistAcrossScenes)
            {
                DontDestroyOnLoad(gameObject);
            }

            GameServices.Initialize(new SaveService(), new SceneService(), new AudioService(audioMixer));
            GameServices.Current.Owner = this;
            AppLog.Info(LogCategory.Boot, "Game services initialized.");
        }

        private void OnDestroy()
        {
            if (GameServices.HasCurrent && GameServices.Current.Owner == this)
            {
                GameServices.Shutdown();
            }
        }

    }
}
