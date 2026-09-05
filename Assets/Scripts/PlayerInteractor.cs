using UnityEngine;

namespace TilkiMacera
{
    public class PlayerInteractor : MonoBehaviour
    {
        public float range = 2.4f;
        public KeyCode interactKey = KeyCode.F;

        private readonly Collider[] hits = new Collider[12];
        private IInteractable focused;

        private void Update()
        {
            FindFocusedInteractable();

            if (focused != null && Input.GetKeyDown(interactKey))
            {
                focused.Interact(this);
            }
        }

        private void FindFocusedInteractable()
        {
            IInteractable best = null;
            float bestDistance = float.MaxValue;
            int count = Physics.OverlapSphereNonAlloc(transform.position, range, hits);

            for (int i = 0; i < count; i++)
            {
                IInteractable interactable = hits[i].GetComponentInParent<IInteractable>();
                if (interactable == null)
                {
                    continue;
                }

                float distance = Vector3.Distance(transform.position, hits[i].transform.position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = interactable;
                }
            }

            focused = best;
            UIController.Instance.SetInteractionPrompt(focused == null ? string.Empty : focused.InteractionText);
        }
    }
}
