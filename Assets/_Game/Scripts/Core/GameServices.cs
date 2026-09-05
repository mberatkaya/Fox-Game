using System;

namespace TilkiOyunu.Foundation
{
    public sealed class GameServices
    {
        public static GameServices Current { get; private set; }
        public static bool HasCurrent => Current != null;

        public SaveService Save { get; }
        public SceneService Scenes { get; }
        public AudioService Audio { get; }
        public GameBootstrap Owner { get; set; }

        private GameServices(SaveService save, SceneService scenes, AudioService audio)
        {
            Save = save ?? throw new ArgumentNullException(nameof(save));
            Scenes = scenes ?? throw new ArgumentNullException(nameof(scenes));
            Audio = audio ?? throw new ArgumentNullException(nameof(audio));
        }

        public static void Initialize(SaveService save, SceneService scenes, AudioService audio)
        {
            Current = new GameServices(save, scenes, audio);
        }

        public static void Shutdown()
        {
            Current = null;
        }
    }
}
