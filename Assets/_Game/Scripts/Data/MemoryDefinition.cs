using UnityEngine;

namespace TilkiOyunu.Foundation
{
    [CreateAssetMenu(menuName = "Tilki Oyunu/Data/Memory Definition", fileName = "MemoryDefinition")]
    public sealed class MemoryDefinition : ScriptableObject
    {
        [SerializeField] private string id = "memory_id";
        [SerializeField] private string displayName = "Ani";
        [SerializeField, TextArea(2, 4)] private string shortText = "Kucuk ve sicak bir ani.";
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject worldPrefab;

        public string Id => id;
        public string DisplayName => displayName;
        public string ShortText => shortText;
        public Sprite Icon => icon;
        public GameObject WorldPrefab => worldPrefab;
    }
}
