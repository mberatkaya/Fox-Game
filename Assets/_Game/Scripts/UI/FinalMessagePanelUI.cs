using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TilkiOyunu.Foundation
{
    public sealed class FinalMessagePanelUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup panel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private TMP_Text signatureText;
        [SerializeField] private Button closeButton;

        private Action onClose;
        private bool open;

        private void Awake()
        {
            Hide();
        }

        private void OnEnable()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Close);
            }
        }

        private void OnDisable()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Close);
            }
        }

        private void Update()
        {
            if (!open)
            {
                return;
            }

            if ((Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame))
                || (Gamepad.current != null && (Gamepad.current.buttonSouth.wasPressedThisFrame || Gamepad.current.startButton.wasPressedThisFrame)))
            {
                Close();
            }
        }

        public void Show(FinalMessageDefinition message, Action closed)
        {
            if (message == null)
            {
                closed?.Invoke();
                return;
            }

            onClose = closed;
            open = true;
            if (titleText != null)
            {
                titleText.text = message.Title;
            }

            if (bodyText != null)
            {
                bodyText.text = message.Body;
            }

            if (signatureText != null)
            {
                signatureText.text = message.Signature;
            }

            SetVisible(true);
        }

        public void Hide()
        {
            open = false;
            SetVisible(false);
        }

        private void Close()
        {
            if (!open)
            {
                return;
            }

            Hide();
            Action closed = onClose;
            onClose = null;
            closed?.Invoke();
        }

        private void SetVisible(bool visible)
        {
            if (panel == null)
            {
                return;
            }

            panel.alpha = visible ? 1f : 0f;
            panel.interactable = visible;
            panel.blocksRaycasts = visible;
        }
    }
}
