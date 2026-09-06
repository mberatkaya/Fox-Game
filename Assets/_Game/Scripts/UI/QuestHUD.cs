using TMPro;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class QuestHUD : MonoBehaviour
    {
        [SerializeField] private QuestDefinition quest;
        [SerializeField] private CanvasGroup panel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text objectiveText;

        private void OnEnable()
        {
            Subscribe();
            Refresh();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (!GameServices.HasCurrent)
            {
                return;
            }

            QuestService quests = GameServices.Current.Quest;
            quests.QuestStarted += HandleQuestChanged;
            quests.QuestProgressChanged += HandleQuestChanged;
            quests.QuestReadyToTurnIn += HandleQuestChanged;
            quests.QuestCompleted += HandleQuestChanged;
        }

        private void Unsubscribe()
        {
            if (!GameServices.HasCurrent)
            {
                return;
            }

            QuestService quests = GameServices.Current.Quest;
            quests.QuestStarted -= HandleQuestChanged;
            quests.QuestProgressChanged -= HandleQuestChanged;
            quests.QuestReadyToTurnIn -= HandleQuestChanged;
            quests.QuestCompleted -= HandleQuestChanged;
        }

        private void HandleQuestChanged(QuestState state)
        {
            if (quest != null && state.QuestId == quest.Id)
            {
                Refresh(state);
            }
        }

        private void Refresh()
        {
            if (!GameServices.HasCurrent || quest == null)
            {
                SetVisible(false);
                return;
            }

            Refresh(GameServices.Current.Quest.GetQuestState(quest));
        }

        private void Refresh(QuestState state)
        {
            bool visible = state.Status == QuestStatus.Active || state.Status == QuestStatus.ReadyToTurnIn;
            SetVisible(visible);
            if (!visible)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = state.Title;
            }

            if (objectiveText != null)
            {
                objectiveText.text = state.Status == QuestStatus.ReadyToTurnIn
                    ? "NPC'ye geri dön"
                    : $"Anıları bul: {state.CurrentAmount} / {state.RequiredAmount}";
            }
        }

        private void SetVisible(bool visible)
        {
            if (panel == null)
            {
                return;
            }

            panel.alpha = visible ? 1f : 0f;
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }
    }
}
