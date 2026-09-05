using UnityEngine;

namespace TilkiMacera
{
    public class FinalShrine : MonoBehaviour, IInteractable
    {
        public string InteractionText => QuestManager.Instance.IsAdventureComplete ? "F - Gizli final" : "Final icin 3 gorev gerekli";

        public void Interact(PlayerInteractor interactor)
        {
            if (QuestManager.Instance.IsAdventureComplete)
            {
                UIController.Instance.ShowFinal();
                QuestManager.Instance.MarkFinalSeen();
            }
            else
            {
                UIController.Instance.ShowResult("Burasi son aniyi sakliyor. Once gorevleri tamamla.", 3f);
            }
        }
    }
}
