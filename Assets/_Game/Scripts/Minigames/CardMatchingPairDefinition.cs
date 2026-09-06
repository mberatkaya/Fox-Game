using System;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    [Serializable]
    public sealed class CardMatchingPairDefinition
    {
        [SerializeField] private string pairId = "pair_01";
        [SerializeField] private string symbol = "*";
        [SerializeField] private Color color = new(0.95f, 0.62f, 0.52f);

        public string PairId => pairId;
        public string Symbol => symbol;
        public Color Color => color;
    }
}
