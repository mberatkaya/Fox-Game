using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class GuideNpc : MonoBehaviour, IInteractable
    {
        [SerializeField] private QuestDefinition collectMemoriesQuest;
        [SerializeField] private DialogueDefinition introDialogue;
        [SerializeField] private DialogueDefinition activeDialogue;
        [SerializeField] private DialogueDefinition readyToTurnInDialogue;
        [SerializeField] private DialogueDefinition completedDialogue;
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

        private string GetInteractionLabel()
        {
            if (!GameServices.HasCurrent || collectMemoriesQuest == null)
            {
                return "Konuş";
            }

            return GameServices.Current.Quest.GetQuestStatus(collectMemoriesQuest) == QuestStatus.ReadyToTurnIn
                ? "Görevi teslim et"
                : "Konuş";
        }
    }
}
