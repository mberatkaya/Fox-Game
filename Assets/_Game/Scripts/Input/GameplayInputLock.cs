using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class GameplayInputLock : MonoBehaviour
    {
        [SerializeField] private FoxController player;
        [SerializeField] private PlayerInteractor interactor;

        private int lockCount;

        public bool IsLocked => lockCount > 0;

        public void Acquire()
        {
            lockCount++;
            Apply();
        }

        public void Release()
        {
            lockCount = Mathf.Max(0, lockCount - 1);
            Apply();
        }

        private void Apply()
        {
            bool locked = IsLocked;
            player?.SetInputLocked(locked);
            interactor?.SetInputLocked(locked);
        }
    }
}
