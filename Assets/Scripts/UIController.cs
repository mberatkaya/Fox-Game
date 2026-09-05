using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TilkiOyunu.Foundation;

namespace TilkiMacera
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance { get; private set; }

        private Text questText;
        private Text promptText;
        private Text dialogueText;
        private Text memoryText;
        private Text resultText;
        private Text minigameStatusText;
        private GameObject dialoguePanel;
        private GameObject memoryPanel;
        private GameObject resultPanel;
        private GameObject finalPanel;
        private GameObject heartGardenPanel;
        private Coroutine resultCoroutine;
        [SerializeField] private FinalMessageDefinition finalMessage;

        private Queue<string> dialogueQueue = new Queue<string>();
        private Action dialogueComplete;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            BuildUI();
        }

        private void Update()
        {
            if (dialoguePanel.activeSelf && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)))
            {
                ShowNextDialogueLine();
            }

            if (memoryPanel.activeSelf && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)))
            {
                memoryPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                SetPlayerMovement(true);
            }

            if (finalPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            {
                finalPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                SetPlayerMovement(true);
            }
        }

        public void RefreshQuestDisplay()
        {
            if (QuestManager.Instance == null)
            {
                return;
            }

            QuestData quest = QuestManager.Instance.ActiveQuest;
            if (quest == null)
            {
                questText.text = "Gizli final acildi\nKamp alanindaki kalp tasina git.";
                return;
            }

            questText.text = $"{quest.title}\n{quest.description}\nIlerleme: {QuestManager.Instance.ActiveQuestProgress}/{quest.requiredCount}";
        }

        public void SetInteractionPrompt(string message)
        {
            promptText.text = message;
            promptText.enabled = !string.IsNullOrWhiteSpace(message);
        }

        public void SetMinigameStatus(string message)
        {
            minigameStatusText.text = message;
            minigameStatusText.enabled = !string.IsNullOrWhiteSpace(message);
        }

        public void ShowDialogue(IEnumerable<string> lines, Action onComplete = null)
        {
            dialogueQueue.Clear();
            if (lines != null)
            {
                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        dialogueQueue.Enqueue(line);
                    }
                }
            }

            dialogueComplete = onComplete;
            if (dialogueQueue.Count == 0)
            {
                dialogueComplete?.Invoke();
                dialogueComplete = null;
                return;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SetPlayerMovement(false);
            dialoguePanel.SetActive(true);
            ShowNextDialogueLine();
        }

        public void ShowMemory(string title, string message)
        {
            memoryText.text = $"{title}\n\n{message}\n\nDevam etmek icin Space";
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SetPlayerMovement(false);
            memoryPanel.SetActive(true);
        }

        public void ShowResult(string message, float seconds)
        {
            if (resultCoroutine != null)
            {
                StopCoroutine(resultCoroutine);
            }

            resultText.text = message;
            resultPanel.SetActive(true);
            resultCoroutine = StartCoroutine(HideResultAfterDelay(seconds));
        }

        public void ShowFinal()
        {
            finalPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SetPlayerMovement(false);
        }

        public void ShowHeartGarden(List<string> pairs, Action<bool> onComplete)
        {
            heartGardenPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            foreach (Transform child in heartGardenPanel.transform)
            {
                Destroy(child.gameObject);
            }

            Text title = CreateText("Baslik", heartGardenPanel.transform, "Kalp Bahcesi", 30, TextAnchor.MiddleCenter);
            title.rectTransform.anchorMin = new Vector2(0.08f, 0.82f);
            title.rectTransform.anchorMax = new Vector2(0.92f, 0.95f);
            title.rectTransform.offsetMin = Vector2.zero;
            title.rectTransform.offsetMax = Vector2.zero;

            Transform grid = CreatePanel("Kartlar", heartGardenPanel.transform, new Color(1f, 1f, 1f, 0f)).transform;
            RectTransform gridRect = grid.GetComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0.16f, 0.16f);
            gridRect.anchorMax = new Vector2(0.84f, 0.78f);
            gridRect.offsetMin = Vector2.zero;
            gridRect.offsetMax = Vector2.zero;

            GridLayoutGroup layout = grid.gameObject.AddComponent<GridLayoutGroup>();
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = 4;
            layout.spacing = new Vector2(10f, 10f);
            layout.cellSize = new Vector2(145f, 82f);

            List<string> cards = new List<string>();
            foreach (string pair in pairs)
            {
                cards.Add(pair);
                cards.Add(pair);
            }

            Shuffle(cards);
            string firstValue = null;
            Button firstButton = null;
            int matchedCards = 0;
            bool locked = false;

            for (int i = 0; i < cards.Count; i++)
            {
                string value = cards[i];
                Button button = CreateButton($"Kart {i + 1}", grid, "?");
                Text buttonText = button.GetComponentInChildren<Text>();

                button.onClick.AddListener(() =>
                {
                    if (locked || buttonText.text != "?")
                    {
                        return;
                    }

                    buttonText.text = value;
                    if (firstValue == null)
                    {
                        firstValue = value;
                        firstButton = button;
                        return;
                    }

                    if (firstValue == value)
                    {
                        button.interactable = false;
                        firstButton.interactable = false;
                        matchedCards += 2;
                        firstValue = null;
                        firstButton = null;

                        if (matchedCards >= cards.Count)
                        {
                            heartGardenPanel.SetActive(false);
                            onComplete?.Invoke(true);
                        }
                    }
                    else
                    {
                        locked = true;
                        StartCoroutine(HideCardsAfterDelay(firstButton.GetComponentInChildren<Text>(), buttonText, () =>
                        {
                            firstValue = null;
                            firstButton = null;
                            locked = false;
                        }));
                    }
                });
            }
        }

        private void ShowNextDialogueLine()
        {
            if (dialogueQueue.Count > 0)
            {
                dialogueText.text = dialogueQueue.Dequeue() + "\n\nDevam etmek icin Space";
                return;
            }

            dialoguePanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SetPlayerMovement(true);
            dialogueComplete?.Invoke();
            dialogueComplete = null;
        }

        private IEnumerator HideCardsAfterDelay(Text first, Text second, Action done)
        {
            yield return new WaitForSeconds(0.7f);
            first.text = "?";
            second.text = "?";
            done?.Invoke();
        }

        private IEnumerator HideResultAfterDelay(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            resultPanel.SetActive(false);
            resultCoroutine = null;
        }

        private static void SetPlayerMovement(bool canMove)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.SetCanMove(canMove);
            }
        }

        private void BuildUI()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                DontDestroyOnLoad(eventSystem);
            }

            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            gameObject.AddComponent<GraphicRaycaster>();

            questText = CreateText("Quest Text", transform, string.Empty, 18, TextAnchor.UpperLeft);
            questText.rectTransform.anchorMin = new Vector2(0.02f, 0.78f);
            questText.rectTransform.anchorMax = new Vector2(0.38f, 0.98f);
            questText.rectTransform.offsetMin = Vector2.zero;
            questText.rectTransform.offsetMax = Vector2.zero;

            promptText = CreateText("Prompt Text", transform, string.Empty, 22, TextAnchor.MiddleCenter);
            promptText.rectTransform.anchorMin = new Vector2(0.35f, 0.08f);
            promptText.rectTransform.anchorMax = new Vector2(0.65f, 0.15f);
            promptText.rectTransform.offsetMin = Vector2.zero;
            promptText.rectTransform.offsetMax = Vector2.zero;
            promptText.enabled = false;

            minigameStatusText = CreateText("Minigame Status", transform, string.Empty, 22, TextAnchor.UpperCenter);
            minigameStatusText.rectTransform.anchorMin = new Vector2(0.34f, 0.88f);
            minigameStatusText.rectTransform.anchorMax = new Vector2(0.66f, 0.96f);
            minigameStatusText.rectTransform.offsetMin = Vector2.zero;
            minigameStatusText.rectTransform.offsetMax = Vector2.zero;
            minigameStatusText.enabled = false;

            dialoguePanel = CreatePanel("Dialogue Panel", transform, new Color(0.08f, 0.07f, 0.08f, 0.88f));
            SetAnchors(dialoguePanel, new Vector2(0.18f, 0.05f), new Vector2(0.82f, 0.25f));
            dialogueText = CreateText("Dialogue Text", dialoguePanel.transform, string.Empty, 20, TextAnchor.MiddleLeft);
            Stretch(dialogueText.rectTransform, 22f);
            dialoguePanel.SetActive(false);

            memoryPanel = CreatePanel("Memory Panel", transform, new Color(1f, 0.94f, 0.78f, 0.94f));
            SetAnchors(memoryPanel, new Vector2(0.26f, 0.24f), new Vector2(0.74f, 0.66f));
            memoryText = CreateText("Memory Text", memoryPanel.transform, string.Empty, 22, TextAnchor.MiddleCenter);
            memoryText.color = new Color(0.18f, 0.12f, 0.08f);
            Stretch(memoryText.rectTransform, 22f);
            memoryPanel.SetActive(false);

            resultPanel = CreatePanel("Result Panel", transform, new Color(0.1f, 0.14f, 0.16f, 0.86f));
            SetAnchors(resultPanel, new Vector2(0.31f, 0.72f), new Vector2(0.69f, 0.84f));
            resultText = CreateText("Result Text", resultPanel.transform, string.Empty, 20, TextAnchor.MiddleCenter);
            Stretch(resultText.rectTransform, 12f);
            resultPanel.SetActive(false);

            finalPanel = CreatePanel("Final Panel", transform, new Color(0.98f, 0.89f, 0.72f, 0.97f));
            SetAnchors(finalPanel, new Vector2(0.18f, 0.18f), new Vector2(0.82f, 0.82f));
            Text finalText = CreateText("Final Text", finalPanel.transform, BuildFinalText(), 28, TextAnchor.MiddleCenter);
            finalText.color = new Color(0.24f, 0.1f, 0.08f);
            Stretch(finalText.rectTransform, 32f);
            finalPanel.SetActive(false);

            heartGardenPanel = CreatePanel("Heart Garden Panel", transform, new Color(0.08f, 0.1f, 0.12f, 0.93f));
            SetAnchors(heartGardenPanel, new Vector2(0.18f, 0.16f), new Vector2(0.82f, 0.84f));
            heartGardenPanel.SetActive(false);
        }

        private string BuildFinalText()
        {
            if (finalMessage == null)
            {
                return "Gizli Final\n\nBu kucuk yolculuk, beraber biriktirilecek guzel anilar icin hazirlandi.\n\nCikmak icin Esc";
            }

            return $"{finalMessage.Title}\n\n{finalMessage.Body}\n\n{finalMessage.Signature}\n\nCikmak icin Esc";
        }

        private static Text CreateText(string name, Transform parent, string text, int fontSize, TextAnchor anchor)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            Text label = textObject.GetComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            label.fontSize = fontSize;
            label.alignment = anchor;
            label.color = Color.white;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            return label;
        }

        private static GameObject CreatePanel(string name, Transform parent, Color color)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            panel.GetComponent<Image>().color = color;
            return panel;
        }

        private static Button CreateButton(string name, Transform parent, string text)
        {
            GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            buttonObject.GetComponent<Image>().color = new Color(0.96f, 0.75f, 0.38f);
            Button button = buttonObject.GetComponent<Button>();
            Text label = CreateText("Text", buttonObject.transform, text, 20, TextAnchor.MiddleCenter);
            label.color = new Color(0.16f, 0.1f, 0.05f);
            Stretch(label.rectTransform, 0f);
            return button;
        }

        private static void SetAnchors(GameObject target, Vector2 min, Vector2 max)
        {
            RectTransform rect = target.GetComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void Stretch(RectTransform rect, float padding)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(padding, padding);
            rect.offsetMax = new Vector2(-padding, -padding);
        }

        private static void Shuffle<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, list.Count);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
    }
}
