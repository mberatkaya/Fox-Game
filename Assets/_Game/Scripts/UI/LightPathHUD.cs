using TMPro;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class LightPathHUD : MonoBehaviour
    {
        [SerializeField] private LightPathController controller;
        [SerializeField] private CanvasGroup panel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text feedbackText;
        [SerializeField, Min(0.5f)] private float feedbackSeconds = 2.5f;

        private float hideFeedbackAt;
        private int currentProgress;
        private int totalNodes;

        private void Awake()
        {
            SetVisible(false);
            SetFeedback(string.Empty);
        }

        private void OnEnable()
        {
            if (controller == null)
            {
                return;
            }

            controller.Started += HandleStarted;
            controller.ProgressChanged += HandleProgressChanged;
            controller.TimeChanged += HandleTimeChanged;
            controller.Failed += HandleFailed;
            controller.Completed += HandleCompleted;
        }

        private void OnDisable()
        {
            if (controller == null)
            {
                return;
            }

            controller.Started -= HandleStarted;
            controller.ProgressChanged -= HandleProgressChanged;
            controller.TimeChanged -= HandleTimeChanged;
            controller.Failed -= HandleFailed;
            controller.Completed -= HandleCompleted;
        }

        private void Update()
        {
            if (feedbackText != null && !string.IsNullOrWhiteSpace(feedbackText.text) && Time.unscaledTime >= hideFeedbackAt)
            {
                SetFeedback(string.Empty);
                if (controller == null || controller.State != LightPathRunState.Running)
                {
                    SetVisible(false);
                }
            }
        }

        private void HandleStarted()
        {
            SetVisible(true);
            SetFeedback(string.Empty);
            if (titleText != null)
            {
                titleText.text = "Işık Yolu";
            }
        }

        private void HandleProgressChanged(int current, int total)
        {
            currentProgress = current;
            totalNodes = total;
            if (progressText != null)
            {
                progressText.text = $"Işık: {currentProgress} / {totalNodes}";
            }
        }

        private void HandleTimeChanged(int seconds)
        {
            if (timerText != null)
            {
                timerText.text = $"Kalan süre: {seconds}";
            }
        }

        private void HandleFailed(string reason)
        {
            SetVisible(true);
            SetFeedback(string.IsNullOrWhiteSpace(reason) ? "Yol kayboldu. Tekrar dene." : reason);
        }

        private void HandleCompleted()
        {
            SetVisible(true);
            HandleProgressChanged(totalNodes, totalNodes);
            if (timerText != null)
            {
                timerText.text = "Kalan süre: 0";
            }

            SetFeedback("Yol tamamlandı.");
        }

        private void SetFeedback(string message)
        {
            if (feedbackText != null)
            {
                feedbackText.text = message;
            }

            hideFeedbackAt = Time.unscaledTime + feedbackSeconds;
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
