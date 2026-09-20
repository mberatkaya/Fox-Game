using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public enum MapMarkerType { Player, NPC, QuestObjective, LightPath, CardQuest, FinalCamp }

    /// <summary>Scene target metadata only. QuestService remains the sole progression authority.</summary>
    public sealed class MapMarker : MonoBehaviour
    {
        public MapMarkerType Type { get; private set; }
        public string Label { get; private set; }
        public string PersistentId { get; private set; }
        public int SequenceIndex { get; private set; } = -1;

        public void Configure(MapMarkerType type, string label, string persistentId = "", int sequenceIndex = -1)
        {
            Type = type;
            Label = label;
            PersistentId = persistentId;
            SequenceIndex = sequenceIndex;
        }

        public bool IsVisible(QuestService quests, bool fullMap, LightPathController lightPath)
        {
            if (quests == null) return false;
            switch (Type)
            {
                case MapMarkerType.Player: return true;
                case MapMarkerType.NPC:
                    foreach (string id in QuestService.FinalCampRequiredQuestIds)
                    {
                        var status = quests.GetQuestStatus(id);
                        if (status == QuestStatus.ReadyToTurnIn) return true;
                        if (status == QuestStatus.Active) return false;
                        if (status == QuestStatus.NotStarted) return true;
                    }
                    return false;
                case MapMarkerType.QuestObjective:
                    return quests.GetQuestStatus(QuestService.CollectMemoriesQuestId) == QuestStatus.Active
                        && !quests.HasCollectedMemory(PersistentId);
                case MapMarkerType.LightPath:
                    if (quests.GetQuestStatus(QuestService.LightPathQuestId) != QuestStatus.Active) return false;
                    bool running = lightPath != null && lightPath.State == LightPathRunState.Running;
                    if (SequenceIndex < 0) return !running;
                    return running && SequenceIndex >= lightPath.CurrentIndex
                        && (fullMap || SequenceIndex == lightPath.CurrentIndex);
                case MapMarkerType.CardQuest:
                    return quests.GetQuestStatus(QuestService.CardMatchingQuestId) == QuestStatus.Active;
                case MapMarkerType.FinalCamp:
                    return quests.IsFinalCampUnlocked && !quests.SaveData.finalCompleted;
                default: return false;
            }
        }

        public bool IsHighlighted(QuestService quests, LightPathController lightPath)
        {
            if (Type == MapMarkerType.NPC)
            {
                foreach (string id in QuestService.FinalCampRequiredQuestIds)
                    if (quests.GetQuestStatus(id) == QuestStatus.ReadyToTurnIn) return true;
            }
            return Type == MapMarkerType.LightPath && lightPath != null
                && lightPath.State == LightPathRunState.Running && SequenceIndex == lightPath.CurrentIndex;
        }
    }
}
