using System;
using System.Collections.Generic;
using UnityEngine;

namespace TilkiMacera
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        public List<QuestData> quests = new List<QuestData>();
        public event Action OnQuestChanged;

        private readonly HashSet<string> collectedMemoryIds = new HashSet<string>();
        private GameSaveData saveData;

        public QuestData ActiveQuest => IsAdventureComplete ? null : quests[saveData.currentQuestIndex];
        public int ActiveQuestProgress => saveData.currentQuestProgress;
        public bool IsAdventureComplete => quests.Count == 0 || saveData.currentQuestIndex >= quests.Count;
        public bool FinalSeen => saveData.finalSeen;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            saveData = SimpleSaveSystem.Load();
            collectedMemoryIds.Clear();
            foreach (string memoryId in saveData.collectedMemoryIds)
            {
                collectedMemoryIds.Add(memoryId);
            }

            if (saveData.currentQuestIndex < 0)
            {
                saveData.currentQuestIndex = 0;
            }
        }

        private void Start()
        {
            NotifyQuestChanged();
        }

        public bool HasCollectedMemory(string memoryId)
        {
            return collectedMemoryIds.Contains(memoryId);
        }

        public bool TryCollectMemory(string memoryId, string title, string message)
        {
            QuestData quest = ActiveQuest;
            if (quest == null || quest.kind != QuestKind.CollectMemories || collectedMemoryIds.Contains(memoryId))
            {
                return false;
            }

            collectedMemoryIds.Add(memoryId);
            saveData.collectedMemoryIds.Add(memoryId);
            UIController.Instance.ShowMemory(title, message);
            RegisterProgress(QuestKind.CollectMemories, 1);
            return true;
        }

        public void RegisterProgress(QuestKind kind, int amount)
        {
            QuestData quest = ActiveQuest;
            if (quest == null || quest.kind != kind)
            {
                return;
            }

            saveData.currentQuestProgress = Mathf.Clamp(saveData.currentQuestProgress + amount, 0, Mathf.Max(1, quest.requiredCount));
            if (saveData.currentQuestProgress >= quest.requiredCount)
            {
                CompleteActiveQuest();
            }
            else
            {
                SaveAndNotify();
            }
        }

        public void ShowActiveQuestIntro()
        {
            QuestData quest = ActiveQuest;
            if (quest == null)
            {
                UIController.Instance.ShowFinal();
                return;
            }

            UIController.Instance.ShowDialogue(quest.introLines);
        }

        public void MarkFinalSeen()
        {
            saveData.finalSeen = true;
            SaveAndNotify();
        }

        public void ResetAdventure()
        {
            SimpleSaveSystem.Clear();
            saveData = new GameSaveData();
            collectedMemoryIds.Clear();
            SaveAndNotify();
        }

        private void CompleteActiveQuest()
        {
            QuestData completedQuest = ActiveQuest;
            saveData.currentQuestIndex++;
            saveData.currentQuestProgress = 0;
            SimpleSaveSystem.Save(saveData);

            if (completedQuest != null && completedQuest.completionLines != null && completedQuest.completionLines.Length > 0)
            {
                UIController.Instance.ShowDialogue(completedQuest.completionLines, NotifyQuestChanged);
            }
            else
            {
                NotifyQuestChanged();
            }

            if (IsAdventureComplete)
            {
                UIController.Instance.ShowResult("Tum anilar tamamlandi. Gizli final acildi.", 4f);
            }
        }

        private void SaveAndNotify()
        {
            SimpleSaveSystem.Save(saveData);
            NotifyQuestChanged();
        }

        private void NotifyQuestChanged()
        {
            OnQuestChanged?.Invoke();
            UIController.Instance.RefreshQuestDisplay();
        }
    }
}
