using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class ForestGameplayBootstrap : MonoBehaviour
    {
        [SerializeField] private PlayerSpawnPoint spawnPoint;
        [SerializeField] private FoxController player;

        private void Start()
        {
            if (spawnPoint != null && player != null)
            {
                player.TeleportTo(spawnPoint.transform);
            }
        }
    }
}
