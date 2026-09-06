using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TilkiOyunu.Foundation
{
    public sealed class DialoguePanelUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup panel;
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private TMP_Text lineText;
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private GameplayInputLock inputLock;

        private DialogueDefinition dialogue;
        private Action onComplete;
        private int lineIndex;
        private bool isOpen;

        public bool IsOpen => isOpen;

        private void Awake()
        {
            HideImmediate();
        }

        private void Update()
        {
            if (!isOpen)
            {
                return;
            }

            if (WasContinuePressed())
            {
                Advance();
            }
        }

        public void Show(DialogueDefinition definition, Action completed = null)
        {
            if (definition == null || definition.Lines == null || definition.Lines.Count == 0)
            {
                completed?.Invoke();
                return;
            }

            dialogue = definition;
            onComplete = completed;
            lineIndex = -1;
            isOpen = true;
            inputLock?.Acquire();
            SetPanelVisible(true);
            Advance();
        }

        public void Advance()
        {
            if (!isOpen || dialogue == null)
            {
                return;
            }

            lineIndex++;
            if (lineIndex >= dialogue.Lines.Count)
            {
                Close();
                return;
            }

            if (speakerText != null)
            {
                speakerText.text = dialogue.Speaker;
            }

            if (lineText != null)
            {
                lineText.text = dialogue.Lines[lineIndex];
            }

            if (hintText != null)
            {
                hintText.text = "Devam";
            }
        }

        public void Close()
        {
            if (!isOpen)
            {
                return;
            }

            isOpen = false;
            dialogue = null;
            SetPanelVisible(false);
            inputLock?.Release();
            Action completed = onComplete;
            onComplete = null;
            completed?.Invoke();
        }

        private void HideImmediate()
        {
            isOpen = false;
            dialogue = null;
            SetPanelVisible(false);
        }

        private void SetPanelVisible(bool visible)
        {
            if (panel == null)
            {
                return;
            }

            panel.alpha = visible ? 1f : 0f;
            panel.interactable = visible;
            panel.blocksRaycasts = visible;
        }

        private static bool WasContinuePressed()
        {
            return (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.eKey.wasPressedThisFrame))
                || (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                || (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame);
        }
    }
}
