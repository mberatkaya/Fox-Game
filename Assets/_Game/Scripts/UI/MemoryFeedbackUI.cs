using TMPro;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class MemoryFeedbackUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup panel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField, Min(0.5f)] private float visibleSeconds = 3f;

        private float hideAt;

        private void Awake()
        {
            SetVisible(false);
        }

        private void Update()
        {
            if (panel != null && panel.alpha > 0f && Time.unscaledTime >= hideAt)
            {
                SetVisible(false);
            }
        }

        public void Show(string title, string body)
        {
            if (titleText != null)
            {
                titleText.text = title;
            }

            if (bodyText != null)
            {
                bodyText.text = body;
            }

            hideAt = Time.unscaledTime + visibleSeconds;
            SetVisible(true);
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
