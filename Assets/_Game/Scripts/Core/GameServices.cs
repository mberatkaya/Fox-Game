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
        public QuestService Quest { get; }
        public GameBootstrap Owner { get; set; }

        private GameServices(SaveService save, SceneService scenes, AudioService audio, QuestService quest)
        {
            Save = save ?? throw new ArgumentNullException(nameof(save));
            Scenes = scenes ?? throw new ArgumentNullException(nameof(scenes));
            Audio = audio ?? throw new ArgumentNullException(nameof(audio));
            Quest = quest ?? throw new ArgumentNullException(nameof(quest));
        }

        public static void Initialize(SaveService save, SceneService scenes, AudioService audio, QuestService quest)
        {
            Current = new GameServices(save, scenes, audio, quest);
        }

        public static void Shutdown()
        {
            Current = null;
        }
    }
}
