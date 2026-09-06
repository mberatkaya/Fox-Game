using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class CardMatchingController : MonoBehaviour
    {
        [SerializeField] private QuestDefinition quest;
        [SerializeField] private CardMatchingPanelUI panel;
        [SerializeField] private CardMatchingPairDefinition[] pairs;
        [SerializeField, Range(0.1f, 2f)] private float mismatchDelaySeconds = 0.85f;
        [SerializeField] private bool shuffleOnStart = true;
        [SerializeField] private int deterministicSeed = -1;

        private readonly List<CardRuntimeSlot> cards = new();
        private int firstSelection = -1;
        private int secondSelection = -1;
        private int matchedPairs;
        private bool isRunning;
        private bool isResolvingMismatch;
        private bool completionNotified;
        private Coroutine mismatchCoroutine;

        public event Action Started;
        public event Action<int, CardMatchingSlotView> CardChanged;
        public event Action<int, int> ProgressChanged;
        public event Action Completed;

        public QuestDefinition Quest => quest;
        public int CardCount => pairs != null ? pairs.Length * 2 : 0;
        public int PairCount => pairs != null ? pairs.Length : 0;
        public int MatchedPairs => matchedPairs;
        public bool IsRunning => isRunning;
        public bool IsResolvingMismatch => isResolvingMismatch;
        public bool IsComplete => completionNotified;
        public float MismatchDelaySeconds => mismatchDelaySeconds;

        public bool CanStartRun
        {
            get
            {
                if (!GameServices.HasCurrent || quest == null || pairs == null || pairs.Length == 0)
                {
                    return false;
                }

                return GameServices.Current.Quest.GetQuestStatus(quest) == QuestStatus.Active;
            }
        }

        private void OnDisable()
        {
            CancelRun();
        }

        public bool Open()
        {
            if (!StartRun())
            {
                return false;
            }

            panel?.Show(this);
            return true;
        }

        public bool StartRun()
        {
            if (!CanStartRun)
            {
                return false;
            }

            StopMismatchCoroutine();
            cards.Clear();
            for (int i = 0; i < pairs.Length; i++)
            {
                CardMatchingPairDefinition pair = pairs[i];
                if (pair == null || string.IsNullOrWhiteSpace(pair.PairId))
                {
                    continue;
                }

                cards.Add(new CardRuntimeSlot(pair));
                cards.Add(new CardRuntimeSlot(pair));
            }

            if (cards.Count == 0 || cards.Count % 2 != 0)
            {
                cards.Clear();
                return false;
            }

            if (shuffleOnStart)
            {
                ShuffleCards();
            }

            firstSelection = -1;
            secondSelection = -1;
            matchedPairs = 0;
            isRunning = true;
            isResolvingMismatch = false;
            completionNotified = false;
            Started?.Invoke();
            NotifyAllCardsChanged();
            ProgressChanged?.Invoke(matchedPairs, pairs.Length);
            return true;
        }

        public bool TryReveal(int index)
        {
            if (!CanReveal(index))
            {
                return false;
            }

            cards[index].State = CardMatchingCardState.Revealed;
            CardChanged?.Invoke(index, GetCard(index));

            if (firstSelection < 0)
            {
                firstSelection = index;
                return true;
            }

            secondSelection = index;
            ResolveSelection();
            return true;
        }

        public void CancelRun()
        {
            StopMismatchCoroutine();
            cards.Clear();
            firstSelection = -1;
            secondSelection = -1;
            matchedPairs = 0;
            isRunning = false;
            isResolvingMismatch = false;
            completionNotified = false;
        }

        public CardMatchingSlotView GetCard(int index)
        {
            if (index < 0 || index >= cards.Count)
            {
                return new CardMatchingSlotView(index, string.Empty, string.Empty, Color.white, CardMatchingCardState.Hidden);
            }

            CardRuntimeSlot card = cards[index];
            CardMatchingPairDefinition pair = card.Pair;
            return new CardMatchingSlotView(index, pair.PairId, pair.Symbol, pair.Color, card.State);
        }

        public bool CanReveal(int index)
        {
            if (!isRunning || isResolvingMismatch || index < 0 || index >= cards.Count)
            {
                return false;
            }

            if (index == firstSelection || index == secondSelection)
            {
                return false;
            }

            return cards[index].State == CardMatchingCardState.Hidden;
        }

        public void ResolveMismatchNowForTests()
        {
            if (isResolvingMismatch && firstSelection >= 0 && secondSelection >= 0)
            {
                HideMismatchedSelection();
            }
        }

        private void ResolveSelection()
        {
            if (firstSelection < 0 || secondSelection < 0)
            {
                return;
            }

            if (cards[firstSelection].Pair.PairId == cards[secondSelection].Pair.PairId)
            {
                cards[firstSelection].State = CardMatchingCardState.Matched;
                cards[secondSelection].State = CardMatchingCardState.Matched;
                CardChanged?.Invoke(firstSelection, GetCard(firstSelection));
                CardChanged?.Invoke(secondSelection, GetCard(secondSelection));
                firstSelection = -1;
                secondSelection = -1;
                matchedPairs++;
                NotifyPairMatched();
                ProgressChanged?.Invoke(matchedPairs, pairs.Length);

                if (matchedPairs >= pairs.Length)
                {
                    CompleteRun();
                }

                return;
            }

            isResolvingMismatch = true;
            mismatchCoroutine = StartCoroutine(HideMismatchAfterDelay());
        }

        private IEnumerator HideMismatchAfterDelay()
        {
            yield return new WaitForSeconds(mismatchDelaySeconds);
            HideMismatchedSelection();
        }

        private void HideMismatchedSelection()
        {
            StopMismatchCoroutine();

            if (firstSelection >= 0 && firstSelection < cards.Count && cards[firstSelection].State == CardMatchingCardState.Revealed)
            {
                cards[firstSelection].State = CardMatchingCardState.Hidden;
                CardChanged?.Invoke(firstSelection, GetCard(firstSelection));
            }

            if (secondSelection >= 0 && secondSelection < cards.Count && cards[secondSelection].State == CardMatchingCardState.Revealed)
            {
                cards[secondSelection].State = CardMatchingCardState.Hidden;
                CardChanged?.Invoke(secondSelection, GetCard(secondSelection));
            }

            firstSelection = -1;
            secondSelection = -1;
            isResolvingMismatch = false;
        }

        private void CompleteRun()
        {
            if (completionNotified)
            {
                return;
            }

            isRunning = false;
            isResolvingMismatch = false;
            completionNotified = true;
            if (GameServices.HasCurrent && quest != null)
            {
                GameServices.Current.Quest.RecordCardMatchingCompleted(quest);
            }

            Completed?.Invoke();
        }

        private void NotifyPairMatched()
        {
            if (GameServices.HasCurrent && quest != null)
            {
                GameServices.Current.Quest.RecordCardMatchingPairMatched(quest);
            }
        }

        private void NotifyAllCardsChanged()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                CardChanged?.Invoke(i, GetCard(i));
            }
        }

        private void ShuffleCards()
        {
            System.Random random = deterministicSeed >= 0 ? new System.Random(deterministicSeed) : new System.Random();
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int swapIndex = random.Next(i + 1);
                (cards[i], cards[swapIndex]) = (cards[swapIndex], cards[i]);
            }
        }

        private void StopMismatchCoroutine()
        {
            if (mismatchCoroutine != null)
            {
                StopCoroutine(mismatchCoroutine);
                mismatchCoroutine = null;
            }
        }

        [Serializable]
        private sealed class CardRuntimeSlot
        {
            public CardRuntimeSlot(CardMatchingPairDefinition pair)
            {
                Pair = pair;
                State = CardMatchingCardState.Hidden;
            }

            public CardMatchingPairDefinition Pair { get; }
            public CardMatchingCardState State { get; set; }
        }
    }
}
