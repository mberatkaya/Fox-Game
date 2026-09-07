using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TilkiOyunu.Foundation
{
    [RequireComponent(typeof(Button))]
    public sealed class CardMatchingCardButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Color hiddenColor = new(0.22f, 0.25f, 0.28f);
        [SerializeField] private Color revealedColor = new(0.95f, 0.86f, 0.72f);
        [SerializeField] private Color matchedColor = new(1f, 0.72f, 0.36f);
        [SerializeField] private float matchedScale = 1.05f;

        private CardMatchingController controller;
        private int index = -1;
        private Vector3 baseScale;

        public int Index => index;
        public GameObject SelectionObject => button != null ? button.gameObject : gameObject;
        public bool IsInteractable => button != null && button.IsInteractable();

        private void Awake()
        {
            CacheReferences();
            baseScale = transform.localScale;
        }

        private void OnEnable()
        {
            CacheReferences();
            button.onClick.AddListener(HandleClicked);
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClicked);
            }
        }

        public void Configure(CardMatchingController owner, int slotIndex)
        {
            controller = owner;
            index = slotIndex;
        }

        public void Refresh(CardMatchingSlotView view, bool canReveal)
        {
            CacheReferences();

            if (label != null)
            {
                label.text = view.State == CardMatchingCardState.Hidden ? "?" : view.Symbol;
                label.color = view.State == CardMatchingCardState.Hidden ? Color.white : Color.black;
            }

            if (background != null)
            {
                background.color = view.State switch
                {
                    CardMatchingCardState.Matched => matchedColor,
                    CardMatchingCardState.Revealed => view.Color,
                    _ => hiddenColor
                };
            }

            transform.localScale = view.State == CardMatchingCardState.Matched ? baseScale * matchedScale : baseScale;

            if (button != null)
            {
                button.interactable = canReveal;
            }
        }

        private void HandleClicked()
        {
            controller?.TryReveal(index);
        }

        private void CacheReferences()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (background == null)
            {
                background = GetComponent<Image>();
            }

            if (label == null)
            {
                label = GetComponentInChildren<TMP_Text>();
            }
        }
    }
}
