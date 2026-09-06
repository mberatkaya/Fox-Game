using System;
using System.Collections.Generic;

namespace TilkiOyunu.Foundation
{
    public sealed class QuestService
    {
        public const string CollectMemoriesQuestId = "collect_memories";
        public const string LightPathQuestId = "light_path";
        public const string CardMatchingQuestId = "card_matching";

        public static readonly string[] FinalCampRequiredQuestIds =
        {
            CollectMemoriesQuestId,
            LightPathQuestId,
            CardMatchingQuestId
        };

        private readonly SaveService saveService;
        private SaveGameData saveData;
        private GameContentConfig contentConfig;
        private bool finalCampUnlocked;

        public QuestService(SaveService saveService, GameContentConfig contentConfig)
        {
            this.saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
            this.contentConfig = contentConfig;
            saveData = saveService.Load();
            SaveService.Normalize(saveData);
            finalCampUnlocked = AreFinalCampRequirementsMet();
            saveData.finalUnlocked = finalCampUnlocked;
        }

        public event Action<QuestState> QuestStarted;
        public event Action<QuestState> QuestProgressChanged;
        public event Action<QuestState> QuestReadyToTurnIn;
        public event Action<QuestState> QuestCompleted;
        public event Action<bool> FinalCampUnlockChanged;

        public SaveGameData SaveData => saveData;
        public IReadOnlyList<string> CollectedMemoryIds => saveData.collectedMemoryIds;
        public bool IsFinalCampUnlocked => finalCampUnlocked;

        public void ConfigureContent(GameContentConfig config)
        {
            contentConfig = config;
        }

        public QuestDefinition FindQuest(string questId)
        {
            if (contentConfig == null || contentConfig.Quests == null || string.IsNullOrWhiteSpace(questId))
            {
                return null;
            }

            foreach (QuestDefinition quest in contentConfig.Quests)
            {
                if (quest != null && quest.Id == questId)
                {
                    return quest;
                }
            }

            return null;
        }

        public QuestState GetQuestState(QuestDefinition quest)
        {
            if (quest == null)
            {
                return new QuestState(string.Empty, string.Empty, string.Empty, QuestStatus.NotStarted, 0, 0);
            }

            QuestProgress progress = FindQuestProgress(quest.Id);
            QuestStatus status = progress != null ? progress.status : QuestStatus.NotStarted;
            int currentAmount = progress != null ? progress.currentAmount : 0;
            return new QuestState(quest.Id, quest.Title, quest.Description, status, currentAmount, quest.RequiredAmount);
        }

        public QuestStatus GetQuestStatus(QuestDefinition quest)
        {
            return GetQuestState(quest).Status;
        }

        public QuestStatus GetQuestStatus(string questId)
        {
            QuestProgress progress = FindQuestProgress(questId);
            return progress != null ? progress.status : QuestStatus.NotStarted;
        }

        public bool IsQuestCompleted(string questId)
        {
            return GetQuestStatus(questId) == QuestStatus.Completed;
        }

        public bool ArePrerequisitesMet(QuestDefinition quest)
        {
            if (quest == null)
            {
                return false;
            }

            if (quest.PrerequisiteQuestIds == null)
            {
                return true;
            }

            for (int i = 0; i < quest.PrerequisiteQuestIds.Count; i++)
            {
                string prerequisiteId = quest.PrerequisiteQuestIds[i];
                if (!string.IsNullOrWhiteSpace(prerequisiteId) && !IsQuestCompleted(prerequisiteId))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanStartQuest(QuestDefinition quest)
        {
            if (quest == null || !ArePrerequisitesMet(quest))
            {
                return false;
            }

            QuestProgress progress = FindQuestProgress(quest.Id);
            return progress == null || progress.status == QuestStatus.NotStarted;
        }

        public bool HasCollectedMemory(MemoryDefinition memory)
        {
            return memory != null && HasCollectedMemory(memory.Id);
        }

        public bool HasCollectedMemory(string memoryId)
        {
            return !string.IsNullOrWhiteSpace(memoryId) && saveData.collectedMemoryIds.Contains(memoryId);
        }

        public bool StartQuest(QuestDefinition quest)
        {
            if (!CanStartQuest(quest))
            {
                return false;
            }

            QuestProgress progress = GetOrCreateQuestProgress(quest.Id);
            if (progress.status != QuestStatus.NotStarted)
            {
                return false;
            }

            progress.currentAmount = 0;
            progress.status = QuestStatus.Active;
            progress.completed = false;
            Persist();
            QuestStarted?.Invoke(GetQuestState(quest));
            QuestProgressChanged?.Invoke(GetQuestState(quest));
            return true;
        }

        public bool RecordLightPathCompleted(QuestDefinition quest)
        {
            if (quest == null || quest.ObjectiveType != QuestObjectiveType.CompleteLightPath)
            {
                return false;
            }

            QuestProgress progress = GetOrCreateQuestProgress(quest.Id);
            if (progress.status != QuestStatus.Active)
            {
                Persist();
                return false;
            }

            progress.currentAmount = Math.Max(progress.currentAmount, quest.RequiredAmount);
            progress.status = QuestStatus.ReadyToTurnIn;
            saveData.lightPathCompleted = true;

            QuestState state = GetQuestState(quest);
            QuestProgressChanged?.Invoke(state);
            QuestReadyToTurnIn?.Invoke(state);
            Persist();
            return true;
        }

        public bool RecordCardMatchingPairMatched(QuestDefinition quest)
        {
            if (quest == null || quest.ObjectiveType != QuestObjectiveType.CompleteCardMatch)
            {
                return false;
            }

            QuestProgress progress = GetOrCreateQuestProgress(quest.Id);
            if (progress.status != QuestStatus.Active)
            {
                Persist();
                return false;
            }

            int previousAmount = progress.currentAmount;
            progress.currentAmount = Math.Min(quest.RequiredAmount, progress.currentAmount + 1);
            if (progress.currentAmount != previousAmount)
            {
                QuestProgressChanged?.Invoke(GetQuestState(quest));
            }

            if (progress.currentAmount >= quest.RequiredAmount)
            {
                progress.status = QuestStatus.ReadyToTurnIn;
                saveData.cardMatchingCompleted = true;
                QuestReadyToTurnIn?.Invoke(GetQuestState(quest));
            }

            Persist();
            return progress.currentAmount != previousAmount;
        }

        public bool RecordCardMatchingCompleted(QuestDefinition quest)
        {
            if (quest == null || quest.ObjectiveType != QuestObjectiveType.CompleteCardMatch)
            {
                return false;
            }

            QuestProgress progress = GetOrCreateQuestProgress(quest.Id);
            if (progress.status != QuestStatus.Active && progress.status != QuestStatus.ReadyToTurnIn)
            {
                Persist();
                return false;
            }

            if (progress.status == QuestStatus.ReadyToTurnIn && progress.currentAmount >= quest.RequiredAmount)
            {
                saveData.cardMatchingCompleted = true;
                Persist();
                return false;
            }

            progress.currentAmount = Math.Max(progress.currentAmount, quest.RequiredAmount);
            progress.status = QuestStatus.ReadyToTurnIn;
            saveData.cardMatchingCompleted = true;

            QuestState state = GetQuestState(quest);
            QuestProgressChanged?.Invoke(state);
            QuestReadyToTurnIn?.Invoke(state);
            Persist();
            return true;
        }

        public bool RecordMemoryCollected(MemoryDefinition memory, QuestDefinition quest = null)
        {
            if (memory == null || string.IsNullOrWhiteSpace(memory.Id))
            {
                return false;
            }

            if (saveData.collectedMemoryIds.Contains(memory.Id))
            {
                return false;
            }

            saveData.collectedMemoryIds.Add(memory.Id);
            QuestDefinition resolvedQuest = quest != null ? quest : FindQuest(CollectMemoriesQuestId);
            if (resolvedQuest != null && resolvedQuest.ObjectiveType == QuestObjectiveType.CollectMemory)
            {
                AddProgress(resolvedQuest, 1);
            }
            else
            {
                Persist();
            }

            return true;
        }

        public bool AddProgress(QuestDefinition quest, int amount)
        {
            if (quest == null || amount <= 0)
            {
                return false;
            }

            QuestProgress progress = GetOrCreateQuestProgress(quest.Id);
            if (progress.status != QuestStatus.Active)
            {
                Persist();
                return false;
            }

            int previousAmount = progress.currentAmount;
            progress.currentAmount = Math.Min(quest.RequiredAmount, progress.currentAmount + amount);
            if (progress.currentAmount != previousAmount)
            {
                QuestProgressChanged?.Invoke(GetQuestState(quest));
            }

            if (progress.currentAmount >= quest.RequiredAmount)
            {
                progress.status = QuestStatus.ReadyToTurnIn;
                QuestReadyToTurnIn?.Invoke(GetQuestState(quest));
            }

            Persist();
            return progress.currentAmount != previousAmount;
        }

        public bool TurnInQuest(QuestDefinition quest)
        {
            if (quest == null)
            {
                return false;
            }

            QuestProgress progress = GetOrCreateQuestProgress(quest.Id);
            if (progress.status != QuestStatus.ReadyToTurnIn)
            {
                return false;
            }

            progress.status = QuestStatus.Completed;
            progress.completed = true;
            if (quest.Id == LightPathQuestId)
            {
                saveData.lightPathCompleted = true;
            }
            else if (quest.Id == CardMatchingQuestId)
            {
                saveData.cardMatchingCompleted = true;
            }

            Persist();
            QuestCompleted?.Invoke(GetQuestState(quest));
            RefreshFinalCampUnlockState();
            return true;
        }

        public bool AreFinalCampRequirementsMet()
        {
            for (int i = 0; i < FinalCampRequiredQuestIds.Length; i++)
            {
                if (!IsQuestCompleted(FinalCampRequiredQuestIds[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public void RefreshFinalCampUnlockState()
        {
            bool unlocked = AreFinalCampRequirementsMet();
            if (saveData.finalUnlocked != unlocked)
            {
                saveData.finalUnlocked = unlocked;
                Persist();
            }

            if (finalCampUnlocked == unlocked)
            {
                return;
            }

            finalCampUnlocked = unlocked;
            FinalCampUnlockChanged?.Invoke(finalCampUnlocked);
        }

        private QuestProgress GetOrCreateQuestProgress(string questId)
        {
            QuestProgress progress = FindQuestProgress(questId);
            if (progress != null)
            {
                return progress;
            }

            progress = new QuestProgress
            {
                questId = questId,
                currentAmount = 0,
                status = QuestStatus.NotStarted,
                completed = false
            };
            saveData.quests.Add(progress);
            return progress;
        }

        private QuestProgress FindQuestProgress(string questId)
        {
            if (string.IsNullOrWhiteSpace(questId))
            {
                return null;
            }

            for (int i = 0; i < saveData.quests.Count; i++)
            {
                QuestProgress progress = saveData.quests[i];
                if (progress != null && progress.questId == questId)
                {
                    return progress;
                }
            }

            return null;
        }

        private void Persist()
        {
            SaveService.Normalize(saveData);
            saveService.Save(saveData);
        }
    }
}
