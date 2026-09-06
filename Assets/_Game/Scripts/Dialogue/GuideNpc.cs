using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class GuideNpc : MonoBehaviour, IInteractable
    {
        [SerializeField] private QuestDefinition collectMemoriesQuest;
        [SerializeField] private QuestDefinition lightPathQuest;
        [SerializeField] private DialogueDefinition introDialogue;
        [SerializeField] private DialogueDefinition activeDialogue;
        [SerializeField] private DialogueDefinition readyToTurnInDialogue;
        [SerializeField] private DialogueDefinition completedDialogue;
        [SerializeField] private DialogueDefinition lightPathIntroDialogue;
        [SerializeField] private DialogueDefinition lightPathActiveDialogue;
        [SerializeField] private DialogueDefinition lightPathTurnInDialogue;
        [SerializeField] private DialogueDefinition lightPathCompletedDialogue;
        [SerializeField] private DialoguePanelUI dialogueUI;

        public string InteractionLabel => GetInteractionLabel();

        public bool CanInteract(InteractionContext context)
        {
            return GameServices.HasCurrent
                && GameServices.Current.Quest != null
                && collectMemoriesQuest != null
                && dialogueUI != null
                && !dialogueUI.IsOpen;
        }

        public void Interact(InteractionContext context)
        {
            if (!CanInteract(context))
            {
                return;
            }

            QuestStatus status = GameServices.Current.Quest.GetQuestStatus(collectMemoriesQuest);
            if (status != QuestStatus.Completed || lightPathQuest == null)
            {
                ShowCollectMemoriesDialogue(status);
                return;
            }

            ShowLightPathDialogue(GameServices.Current.Quest.GetQuestStatus(lightPathQuest));
        }

        private string GetInteractionLabel()
        {
            if (!GameServices.HasCurrent || collectMemoriesQuest == null)
            {
                return "Konuş";
            }

            QuestService quests = GameServices.Current.Quest;
            if (quests.GetQuestStatus(collectMemoriesQuest) == QuestStatus.ReadyToTurnIn)
            {
                return "Görevi teslim et";
            }

            return lightPathQuest != null && quests.GetQuestStatus(lightPathQuest) == QuestStatus.ReadyToTurnIn
                ? "Görevi teslim et"
                : "Konuş";
        }

        private void ShowCollectMemoriesDialogue(QuestStatus status)
        {
            switch (status)
            {
                case QuestStatus.NotStarted:
                    dialogueUI.Show(introDialogue, () => GameServices.Current.Quest.StartQuest(collectMemoriesQuest));
                    break;
                case QuestStatus.Active:
                    dialogueUI.Show(activeDialogue);
                    break;
                case QuestStatus.ReadyToTurnIn:
                    dialogueUI.Show(readyToTurnInDialogue, () => GameServices.Current.Quest.TurnInQuest(collectMemoriesQuest));
                    break;
                case QuestStatus.Completed:
                    dialogueUI.Show(completedDialogue);
                    break;
            }
        }

        private void ShowLightPathDialogue(QuestStatus status)
        {
            switch (status)
            {
                case QuestStatus.NotStarted:
                    dialogueUI.Show(lightPathIntroDialogue, () => GameServices.Current.Quest.StartQuest(lightPathQuest));
                    break;
                case QuestStatus.Active:
                    dialogueUI.Show(lightPathActiveDialogue);
                    break;
                case QuestStatus.ReadyToTurnIn:
                    dialogueUI.Show(lightPathTurnInDialogue, () => GameServices.Current.Quest.TurnInQuest(lightPathQuest));
                    break;
                case QuestStatus.Completed:
                    dialogueUI.Show(lightPathCompletedDialogue != null ? lightPathCompletedDialogue : completedDialogue);
                    break;
            }
        }
    }
}
