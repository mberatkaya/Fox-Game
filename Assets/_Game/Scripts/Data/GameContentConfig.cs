using UnityEngine;

namespace TilkiOyunu.Foundation
{
    [CreateAssetMenu(menuName = "Tilki Oyunu/Data/Game Content Config", fileName = "GameContentConfig")]
    public sealed class GameContentConfig : ScriptableObject
    {
        [SerializeField] private FinalMessageDefinition finalMessage;
        [SerializeField] private QuestDefinition[] quests;
        [SerializeField] private MemoryDefinition[] memories;
        [SerializeField] private DialogueDefinition[] dialogues;

        public FinalMessageDefinition FinalMessage => finalMessage;
        public QuestDefinition[] Quests => quests;
        public MemoryDefinition[] Memories => memories;
        public DialogueDefinition[] Dialogues => dialogues;
    }
}
