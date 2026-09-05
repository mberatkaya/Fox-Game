using UnityEngine;

namespace TilkiMacera
{
    public class QuestGiver : MonoBehaviour, IInteractable
    {
        public string speakerName = "Ay Isigi";
        public string collectPrompt = "F - Gorevi dinle";
        public string lightPathPrompt = "F - Isik Yolu";
        public string heartGardenPrompt = "F - Kalp Bahcesi";

        public string InteractionText
        {
            get
            {
                QuestData quest = QuestManager.Instance.ActiveQuest;
                if (quest == null)
                {
                    return "F - Gizli final";
                }

                if (quest.kind == QuestKind.LightPath)
                {
                    return lightPathPrompt;
                }

                if (quest.kind == QuestKind.HeartGarden)
                {
                    return heartGardenPrompt;
                }

                return collectPrompt;
            }
        }

        public void Interact(PlayerInteractor interactor)
        {
            QuestData quest = QuestManager.Instance.ActiveQuest;
            if (quest == null)
            {
                UIController.Instance.ShowFinal();
                return;
            }

            if (quest.kind == QuestKind.LightPath)
            {
                MinigameManager.Instance.StartLightPath();
                return;
            }

            if (quest.kind == QuestKind.HeartGarden)
            {
                MinigameManager.Instance.StartHeartGarden();
                return;
            }

            QuestManager.Instance.ShowActiveQuestIntro();
        }
    }
}
