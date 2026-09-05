using System.Collections.Generic;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    [CreateAssetMenu(menuName = "Tilki Oyunu/Data/Dialogue Definition", fileName = "DialogueDefinition")]
    public sealed class DialogueDefinition : ScriptableObject
    {
        [SerializeField] private string id = "dialogue_id";
        [SerializeField] private string speaker = "Anlatici";
        [SerializeField, TextArea(2, 4)] private List<string> lines = new List<string>();

        public string Id => id;
        public string Speaker => speaker;
        public IReadOnlyList<string> Lines => lines;
    }
}
