using System;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class LightPathController : MonoBehaviour
    {
        [SerializeField] private QuestDefinition quest;
        [SerializeField] private LightPathNode[] nodes;
        [SerializeField, Min(5f)] private float durationSeconds = 40f;

        private int currentIndex;
        private float remainingSeconds;
        private LightPathRunState state;

        public event Action Started;
        public event Action<int, int> ProgressChanged;
        public event Action<int> TimeChanged;
        public event Action<string> Failed;
        public event Action Completed;

        public QuestDefinition Quest => quest;
        public LightPathRunState State => state;
        public int CurrentIndex => currentIndex;
        public int NodeCount => nodes != null ? nodes.Length : 0;
        public float RemainingSeconds => remainingSeconds;

        public bool CanStartRun
        {
            get
            {
                if (state != LightPathRunState.Inactive || !GameServices.HasCurrent || quest == null)
                {
                    return false;
                }

                return GameServices.Current.Quest.GetQuestStatus(quest) == QuestStatus.Active;
            }
        }

        private void Awake()
        {
            InitializeNodes();
            RefreshFromQuest();
        }

        private void OnEnable()
        {
            if (GameServices.HasCurrent)
            {
                GameServices.Current.Quest.QuestProgressChanged += HandleQuestChanged;
                GameServices.Current.Quest.QuestReadyToTurnIn += HandleQuestChanged;
                GameServices.Current.Quest.QuestCompleted += HandleQuestChanged;
            }

            RefreshFromQuest();
        }

        private void OnDisable()
        {
            if (GameServices.HasCurrent)
            {
                GameServices.Current.Quest.QuestProgressChanged -= HandleQuestChanged;
                GameServices.Current.Quest.QuestReadyToTurnIn -= HandleQuestChanged;
                GameServices.Current.Quest.QuestCompleted -= HandleQuestChanged;
            }
        }

        private void Update()
        {
            if (state != LightPathRunState.Running)
            {
                return;
            }

            remainingSeconds -= Time.deltaTime;
            TimeChanged?.Invoke(Mathf.Max(0, Mathf.CeilToInt(remainingSeconds)));
            if (remainingSeconds <= 0f)
            {
                FailRun("Yol kayboldu. Tekrar dene.");
            }
        }

        public bool StartRun()
        {
            if (!CanStartRun || nodes == null || nodes.Length == 0)
            {
                return false;
            }

            currentIndex = 0;
            remainingSeconds = durationSeconds;
            state = LightPathRunState.Running;
            ResetNodeVisuals();
            SetAvailableNode(currentIndex);
            Started?.Invoke();
            ProgressChanged?.Invoke(currentIndex, nodes.Length);
            TimeChanged?.Invoke(Mathf.CeilToInt(remainingSeconds));
            return true;
        }

        public bool HandleNodeTriggered(LightPathNode node)
        {
            if (state != LightPathRunState.Running || node == null)
            {
                return false;
            }

            if (node.SequenceIndex < currentIndex)
            {
                return false;
            }

            if (node.SequenceIndex != currentIndex)
            {
                FailRun("Yol kayboldu. Tekrar dene.");
                return false;
            }

            node.SetVisualState(LightPathVisualState.Completed);
            currentIndex++;
            ProgressChanged?.Invoke(currentIndex, nodes.Length);

            if (currentIndex >= nodes.Length)
            {
                CompleteRun();
                return true;
            }

            SetAvailableNode(currentIndex);
            return true;
        }

        public void ResetRun()
        {
            currentIndex = 0;
            remainingSeconds = 0f;
            state = LightPathRunState.Inactive;
            ResetNodeVisuals();
        }

        public void ForceTimeoutForTests()
        {
            if (state == LightPathRunState.Running)
            {
                FailRun("Yol kayboldu. Tekrar dene.");
            }
        }

        private void CompleteRun()
        {
            if (state == LightPathRunState.Completed)
            {
                return;
            }

            remainingSeconds = 0f;
            state = LightPathRunState.Completed;
            if (GameServices.HasCurrent && quest != null)
            {
                GameServices.Current.Quest.RecordLightPathCompleted(quest);
            }

            Completed?.Invoke();
        }

        private void FailRun(string reason)
        {
            ResetRun();
            Failed?.Invoke(reason);
        }

        private void InitializeNodes()
        {
            if (nodes == null)
            {
                return;
            }

            Array.Sort(nodes, (left, right) =>
            {
                if (left == null && right == null)
                {
                    return 0;
                }

                if (left == null)
                {
                    return 1;
                }

                if (right == null)
                {
                    return -1;
                }

                return left.SequenceIndex.CompareTo(right.SequenceIndex);
            });

            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i]?.Configure(this);
            }
        }

        private void ResetNodeVisuals()
        {
            if (nodes == null)
            {
                return;
            }

            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i]?.SetVisualState(LightPathVisualState.Inactive);
            }
        }

        private void SetAvailableNode(int sequenceIndex)
        {
            if (nodes == null)
            {
                return;
            }

            for (int i = 0; i < nodes.Length; i++)
            {
                LightPathNode node = nodes[i];
                if (node != null && node.SequenceIndex == sequenceIndex)
                {
                    node.SetVisualState(LightPathVisualState.Available);
                    return;
                }
            }
        }

        private void RefreshFromQuest()
        {
            if (!GameServices.HasCurrent || quest == null)
            {
                ResetRun();
                return;
            }

            QuestStatus questStatus = GameServices.Current.Quest.GetQuestStatus(quest);
            if (questStatus == QuestStatus.ReadyToTurnIn || questStatus == QuestStatus.Completed)
            {
                currentIndex = NodeCount;
                remainingSeconds = 0f;
                state = LightPathRunState.Completed;
                if (nodes != null)
                {
                    for (int i = 0; i < nodes.Length; i++)
                    {
                        nodes[i]?.SetVisualState(LightPathVisualState.Completed);
                    }
                }

                return;
            }

            ResetRun();
        }

        private void HandleQuestChanged(QuestState questState)
        {
            if (quest != null && questState.QuestId == quest.Id)
            {
                RefreshFromQuest();
            }
        }
    }
}
