using UnityEngine;

namespace TilkiOyunu.Foundation
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class MemoryAudioFeedback : MonoBehaviour
    {
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip pickupClip;
        [SerializeField, Range(0f, 1f)] private float volume = 0.58f;

        private void Awake()
        {
            if (source == null)
            {
                source = GetComponent<AudioSource>();
            }

            if (source != null)
            {
                source.playOnAwake = false;
                source.spatialBlend = 0.35f;
            }
        }

        public void PlayPickup()
        {
            if (source != null && pickupClip != null)
            {
                source.PlayOneShot(pickupClip, volume);
            }
        }
    }
}
