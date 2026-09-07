using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class ForestGameplayBootstrap : MonoBehaviour
    {
        [SerializeField] private PlayerSpawnPoint spawnPoint;
        [SerializeField] private FoxController player;
        [SerializeField] private ThirdPersonCameraController cameraController;

        private void Start()
        {
            if (spawnPoint != null && player != null)
            {
                player.TeleportTo(spawnPoint.transform);
                ResolveCameraController();
                cameraController?.SnapToTarget();
            }
        }

        private void ResolveCameraController()
        {
            if (cameraController == null)
            {
                cameraController = Object.FindFirstObjectByType<ThirdPersonCameraController>(FindObjectsInactive.Include);
            }
        }
    }
}
