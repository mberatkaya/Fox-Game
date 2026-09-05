using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public readonly struct InteractionContext
    {
        public InteractionContext(GameObject actor)
        {
            Actor = actor;
        }

        public GameObject Actor { get; }
    }
}
