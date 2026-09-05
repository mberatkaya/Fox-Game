using UnityEngine;

namespace TilkiOyunu.Foundation
{
    [CreateAssetMenu(menuName = "Tilki Oyunu/Data/Final Message", fileName = "FinalMessageDefinition")]
    public sealed class FinalMessageDefinition : ScriptableObject
    {
        [SerializeField] private string title = "Final Mesaji";
        [SerializeField, TextArea(4, 8)] private string body = "Bu kucuk yolculuk, beraber biriktirilecek guzel anilar icin hazirlandi.";
        [SerializeField] private string signature = "Tilki";

        public string Title => title;
        public string Body => body;
        public string Signature => signature;
    }
}
