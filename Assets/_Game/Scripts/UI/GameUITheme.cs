using UnityEngine;

namespace TilkiOyunu.Foundation
{
    [CreateAssetMenu(menuName = "Tilki Oyunu/UI/Game UI Theme", fileName = "GameUITheme")]
    public sealed class GameUITheme : ScriptableObject
    {
        [SerializeField] private Color forestDark = new(0.08f, 0.16f, 0.12f, 0.88f);
        [SerializeField] private Color forestMid = new(0.28f, 0.46f, 0.27f, 1f);
        [SerializeField] private Color wood = new(0.42f, 0.25f, 0.13f, 1f);
        [SerializeField] private Color foxAccent = new(0.95f, 0.43f, 0.16f, 1f);
        [SerializeField] private Color cream = new(1f, 0.94f, 0.78f, 1f);
        [SerializeField] private Color goldLight = new(1f, 0.75f, 0.34f, 1f);

        public Color ForestDark => forestDark;
        public Color ForestMid => forestMid;
        public Color Wood => wood;
        public Color FoxAccent => foxAccent;
        public Color Cream => cream;
        public Color GoldLight => goldLight;
    }
}
