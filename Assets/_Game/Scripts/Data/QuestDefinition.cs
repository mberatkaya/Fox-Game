using System.Collections.Generic;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public enum QuestObjectiveType
    {
        CollectMemory,
        CompleteLightPath,
        CompleteCardMatch,
        ReachFinalCamp
    }

    [CreateAssetMenu(menuName = "Tilki Oyunu/Data/Quest Definition", fileName = "QuestDefinition")]
    public sealed class QuestDefinition : ScriptableObject
    {
        [SerializeField] private string id = "quest_id";
        [SerializeField] private string title = "Yeni Gorev";
        [SerializeField, TextArea(2, 4)] private string description = "Gorev aciklamasi.";
        [SerializeField] private QuestObjectiveType objectiveType;
        [SerializeField] private string targetId;
        [SerializeField, Min(1)] private int requiredAmount = 1;
        [SerializeField] private List<string> prerequisiteQuestIds = new List<string>();

        public string Id => id;
        public string Title => title;
        public string Description => description;
        public QuestObjectiveType ObjectiveType => objectiveType;
        public string TargetId => targetId;
        public int RequiredAmount => requiredAmount;
        public IReadOnlyList<string> PrerequisiteQuestIds => prerequisiteQuestIds;
    }
}
