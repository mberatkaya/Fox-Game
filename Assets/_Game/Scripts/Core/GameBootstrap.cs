using UnityEngine;
using UnityEngine.Audio;

namespace TilkiOyunu.Foundation
{
    [DefaultExecutionOrder(-1000)]
    public sealed class GameBootstrap : MonoBehaviour
    {
        // Integration harnesses can enter gameplay directly; production starts at the menu.
        public static bool ShowMainMenuOnStart { get; set; } = true;
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
            gameObject.AddComponent<SettingsRuntime>();
            gameObject.AddComponent<SettingsMenuUI>();
            AppLog.Info(LogCategory.Boot, "Game services initialized.");
        }

        private void Start()
        {
            if (loadForestOnStart && GameServices.HasCurrent && GameServices.Current.Owner == this && GameServices.Current.Scenes.ActiveSceneName == SceneIds.Bootstrap)
            {
                if (ShowMainMenuOnStart) GetComponent<SettingsMenuUI>().ShowMainMenu();
                else GameServices.Current.Scenes.LoadForest();
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
