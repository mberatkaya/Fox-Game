using TMPro;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class QuestHUD : MonoBehaviour
    {
        [SerializeField] private QuestDefinition quest;
        [SerializeField] private QuestDefinition[] quests;
        [SerializeField] private CanvasGroup panel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text objectiveText;
        private LightPathController lightPath;
        private float nextRefresh;

        private void Start() { lightPath = FindFirstObjectByType<LightPathController>(); Refresh(); }
        private void Update()
        {
            if (Time.unscaledTime >= nextRefresh)
            {
                nextRefresh = Time.unscaledTime + .2f;
                Refresh();
            }
        }

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
            Refresh();
        }

        private void Refresh()
        {
            if (!GameServices.HasCurrent)
            {
                SetVisible(false);
                return;
            }

            if (!TryGetVisibleQuestState(out QuestState state))
            {
                var service = GameServices.Current.Quest;
                SetVisible(!service.SaveData.finalCompleted);
                if (titleText != null) titleText.text = service.IsFinalCampUnlocked ? "Son Bir Anı" : "Ormanın Hatırası";
                if (objectiveText != null) objectiveText.text = service.IsFinalCampUnlocked
                    ? "Final Kampı’na git. M · Harita" : "Rehber ile konuş. M · Harita";
                return;
            }

            Refresh(state);
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
                objectiveText.text = FormatObjective(state);
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

        private bool TryGetVisibleQuestState(out QuestState visibleState)
        {
            QuestDefinition[] visibleQuests = GetQuestList();
            QuestService questService = GameServices.Current.Quest;

            for (int i = 0; i < visibleQuests.Length; i++)
            {
                QuestDefinition candidate = visibleQuests[i];
                if (candidate == null)
                {
                    continue;
                }

                QuestState state = questService.GetQuestState(candidate);
                if (state.Status == QuestStatus.ReadyToTurnIn)
                {
                    visibleState = state;
                    return true;
                }
            }

            for (int i = 0; i < visibleQuests.Length; i++)
            {
                QuestDefinition candidate = visibleQuests[i];
                if (candidate == null)
                {
                    continue;
                }

                QuestState state = questService.GetQuestState(candidate);
                if (state.Status == QuestStatus.Active)
                {
                    visibleState = state;
                    return true;
                }
            }

            visibleState = default;
            return false;
        }

        private QuestDefinition[] GetQuestList()
        {
            if (quests != null && quests.Length > 0)
            {
                return quests;
            }

            return quest != null ? new[] { quest } : System.Array.Empty<QuestDefinition>();
        }

        private string FormatObjective(QuestState state)
        {
            if (state.Status == QuestStatus.ReadyToTurnIn)
            {
                return "NPC’ye geri dön. M · Harita";
            }

            QuestDefinition definition = GameServices.Current.Quest.FindQuest(state.QuestId);
            return definition != null
                ? FormatTypedObjective(definition, state)
                : $"İlerleme: {state.CurrentAmount} / {state.RequiredAmount}";
        }

        private string FormatTypedObjective(QuestDefinition definition, QuestState state)
        {
            return definition.ObjectiveType switch
            {
                QuestObjectiveType.CompleteLightPath => lightPath != null && lightPath.State == LightPathRunState.Running
                    ? $"Sıradaki ışığı takip et: {lightPath.CurrentIndex}/{lightPath.NodeCount}"
                    : "Işık Korusu’na git. Hazır olduğunda F ile başlat.",
                QuestObjectiveType.CompleteCardMatch => $"Kalp Bahçesi’ne git. Eşleşmeler: {state.CurrentAmount}/{state.RequiredAmount}",
                _ => $"Waffle Malzemeleri: {state.CurrentAmount}/{state.RequiredAmount} · M Harita"
            };
        }
    }
}
