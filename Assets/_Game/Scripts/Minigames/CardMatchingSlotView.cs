using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public readonly struct CardMatchingSlotView
    {
        public CardMatchingSlotView(int index, string pairId, string symbol, Color color, CardMatchingCardState state)
        {
            Index = index;
            PairId = pairId;
            Symbol = symbol;
            Color = color;
            State = state;
        }

        public int Index { get; }
        public string PairId { get; }
        public string Symbol { get; }
        public Color Color { get; }
        public CardMatchingCardState State { get; }
    }
}
