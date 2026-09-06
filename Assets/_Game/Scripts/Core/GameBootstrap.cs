using UnityEngine;
using UnityEngine.Audio;

namespace TilkiOyunu.Foundation
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private GameContentConfig contentConfig;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private bool persistAcrossScenes = true;
        [SerializeField] private bool loadForestOnStart = true;

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

            SaveService saveService = new();
            GameServices.Initialize(saveService, new SceneService(), new AudioService(audioMixer), new QuestService(saveService, contentConfig));
            GameServices.Current.Owner = this;
            AppLog.Info(LogCategory.Boot, "Game services initialized.");
        }

        private void Start()
        {
            if (loadForestOnStart && GameServices.HasCurrent && GameServices.Current.Owner == this && GameServices.Current.Scenes.ActiveSceneName == SceneIds.Bootstrap)
            {
                GameServices.Current.Scenes.LoadForest();
            }
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
