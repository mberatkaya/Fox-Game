using UnityEngine;

namespace TilkiMacera
{
    public enum QuestKind
    {
        CollectMemories,
        LightPath,
        HeartGarden
    }

    [CreateAssetMenu(menuName = "Tilki Macera/Quest Data", fileName = "QuestData")]
    public class QuestData : ScriptableObject
    {
        public string questId = "quest";
        public string title = "Yeni Gorev";
        [TextArea(2, 4)] public string description = "Gorev aciklamasi";
        public QuestKind kind;
        public int requiredCount = 1;
        [TextArea(2, 4)] public string[] introLines;
        [TextArea(2, 4)] public string[] completionLines;
    }
}
