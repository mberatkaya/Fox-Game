using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TilkiOyunu.Foundation
{
    public sealed class CardMatchingPanelUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup panel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private TMP_Text successText;
        [SerializeField] private CardMatchingCardButton[] cardButtons;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameplayInputLock inputLock;
        [SerializeField] private InputActionAsset inputActions;

        private CardMatchingController controller;
        private InputActionMap playerMap;
        private InputAction cancelAction;
        private bool isOpen;
        private bool hasInputLock;

        public bool IsOpen => isOpen;

        private void Awake()
        {
            HideImmediate();
        }

        private void OnEnable()
        {
            ResolveInputActions();
            playerMap?.Enable();
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

            ReleaseInputLock();
            Unsubscribe();
            playerMap?.Disable();
        }

        private void Update()
        {
            if (isOpen && WasCancelPressed())
            {
                Close();
            }
        }

        public void Show(CardMatchingController owner)
        {
            Unsubscribe();
            controller = owner;
            Subscribe();

            isOpen = true;
            AcquireInputLock();
            SetVisible(true);
            RefreshAll();
        }

        public void Close()
        {
            if (!isOpen)
            {
                return;
            }

            bool completed = controller != null && controller.IsComplete;
            isOpen = false;
            SetVisible(false);
            ReleaseInputLock();
            if (!completed)
            {
                controller?.CancelRun();
            }
        }

        private void HandleStarted()
        {
            RefreshAll();
        }

        private void HandleCardChanged(int index, CardMatchingSlotView view)
        {
            if (cardButtons == null || index < 0 || index >= cardButtons.Length || cardButtons[index] == null)
            {
                return;
            }

            cardButtons[index].Refresh(view, controller != null && controller.CanReveal(index));
            RefreshInteractivity();
        }

        private void HandleProgressChanged(int matched, int total)
        {
            if (progressText != null)
            {
                progressText.text = $"Eşleşmeler: {matched} / {total}";
            }
        }

        private void HandleCompleted()
        {
            if (successText != null)
            {
                successText.text = "Tüm eşleri buldun.";
                successText.enabled = true;
            }

            RefreshInteractivity();
        }

        private void RefreshAll()
        {
            if (titleText != null)
            {
                titleText.text = "Kalp Bahçesi";
            }

            if (successText != null)
            {
                successText.text = string.Empty;
                successText.enabled = false;
            }

            int buttonCount = cardButtons != null ? cardButtons.Length : 0;
            for (int i = 0; i < buttonCount; i++)
            {
                CardMatchingCardButton cardButton = cardButtons[i];
                if (cardButton == null)
                {
                    continue;
                }

                cardButton.Configure(controller, i);
                cardButton.Refresh(controller != null ? controller.GetCard(i) : default, controller != null && controller.CanReveal(i));
            }

            if (controller != null)
            {
                HandleProgressChanged(controller.MatchedPairs, controller.PairCount);
            }
        }

        private void RefreshInteractivity()
        {
            if (cardButtons == null || controller == null)
            {
                return;
            }

            for (int i = 0; i < cardButtons.Length; i++)
            {
                if (cardButtons[i] != null)
                {
                    cardButtons[i].Refresh(controller.GetCard(i), controller.CanReveal(i));
                }
            }
        }

        private void Subscribe()
        {
            if (controller == null)
            {
                return;
            }

            controller.Started += HandleStarted;
            controller.CardChanged += HandleCardChanged;
            controller.ProgressChanged += HandleProgressChanged;
            controller.Completed += HandleCompleted;
        }

        private void Unsubscribe()
        {
            if (controller == null)
            {
                return;
            }

            controller.Started -= HandleStarted;
            controller.CardChanged -= HandleCardChanged;
            controller.ProgressChanged -= HandleProgressChanged;
            controller.Completed -= HandleCompleted;
            controller = null;
        }

        private void HideImmediate()
        {
            isOpen = false;
            SetVisible(false);
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

        private void AcquireInputLock()
        {
            if (inputLock != null && !hasInputLock)
            {
                inputLock.Acquire();
                hasInputLock = true;
            }
        }

        private void ReleaseInputLock()
        {
            if (inputLock != null && hasInputLock)
            {
                inputLock.Release();
                hasInputLock = false;
            }
        }

        private void ResolveInputActions()
        {
            if (inputActions == null)
            {
                return;
            }

            playerMap = inputActions.FindActionMap(InputActionIds.MapPlayer, false);
            cancelAction = playerMap?.FindAction(InputActionIds.Pause, false);
        }

        private bool WasCancelPressed()
        {
            if (cancelAction != null)
            {
                return cancelAction.WasPressedThisFrame();
            }

            return (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                || (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame);
        }
    }
}
