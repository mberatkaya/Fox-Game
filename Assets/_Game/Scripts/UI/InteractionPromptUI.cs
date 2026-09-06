using TMPro;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text promptText;

        private void Awake()
        {
            Clear();
        }

        public void Show(string bindingLabel, string actionLabel)
        {
            if (promptText == null)
            {
                return;
            }

            promptText.text = $"{bindingLabel} - {actionLabel}";
            promptText.enabled = true;
        }

        public void Clear()
        {
            if (promptText == null)
            {
                return;
            }

            promptText.text = string.Empty;
            promptText.enabled = false;
        }
    }
}
