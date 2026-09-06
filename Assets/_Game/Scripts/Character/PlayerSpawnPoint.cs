using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class PlayerSpawnPoint : MonoBehaviour
    {
        [SerializeField] private FoxController player;

        private void Start()
        {
            if (player != null)
            {
                player.TeleportTo(transform);
            }
        }
    }
}
