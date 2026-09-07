using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class MemoryCollectible : MonoBehaviour, IInteractable
    {
        [SerializeField] private MemoryDefinition memory;
        [SerializeField] private QuestDefinition quest;
        [SerializeField] private MemoryFeedbackUI feedbackUI;
        [SerializeField] private MemoryAudioFeedback audioFeedback;
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Collider[] colliders;
        [SerializeField] private float rotationDegreesPerSecond = 45f;
        [SerializeField] private float bobHeight = 0.12f;
        [SerializeField] private float bobSpeed = 2.2f;

        private Vector3 startLocalPosition;
        private bool collected;

        public MemoryDefinition Memory => memory;
        public QuestDefinition Quest => quest;
        public string InteractionLabel => memory != null ? memory.DisplayName : "Anıyı al";

        private void Awake()
        {
            startLocalPosition = transform.localPosition;
            CacheSceneReferences();
            RefreshCollectedState();
        }

        private void OnEnable()
        {
            RefreshCollectedState();
        }

        private void Update()
        {
            if (collected)
            {
                return;
            }

            transform.Rotate(Vector3.up, rotationDegreesPerSecond * Time.deltaTime, Space.World);
            transform.localPosition = startLocalPosition + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        }

        public bool CanInteract(InteractionContext context)
        {
            if (!GameServices.HasCurrent || memory == null || quest == null || collected)
            {
                return false;
            }

            QuestService quests = GameServices.Current.Quest;
            return !quests.HasCollectedMemory(memory) && quests.GetQuestStatus(quest) == QuestStatus.Active;
        }

        public void Interact(InteractionContext context)
        {
            if (!CanInteract(context))
            {
                return;
            }

            if (GameServices.Current.Quest.RecordMemoryCollected(memory, quest))
            {
                feedbackUI?.Show("Anı bulundu", memory.ShortText);
                audioFeedback?.PlayPickup();
                SetCollected(true);
            }
        }

        public void RefreshCollectedState()
        {
            bool isCollected = GameServices.HasCurrent && memory != null && GameServices.Current.Quest.HasCollectedMemory(memory);
            SetCollected(isCollected);
        }

        private void SetCollected(bool isCollected)
        {
            collected = isCollected;
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].enabled = !isCollected;
                }
            }

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null)
                {
                    colliders[i].enabled = !isCollected;
                }
            }
        }

        private void CacheSceneReferences()
        {
            if (renderers == null || renderers.Length == 0)
            {
                renderers = GetComponentsInChildren<Renderer>();
            }

            if (colliders == null || colliders.Length == 0)
            {
                colliders = GetComponentsInChildren<Collider>();
            }
        }
    }
}
