using UnityEngine;

namespace TilkiOyunu.Foundation
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class FootstepAudio : MonoBehaviour
    {
        [SerializeField] private FoxController controller;
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip[] clips;
        [SerializeField, Min(0.05f)] private float walkInterval = 0.48f;
        [SerializeField, Min(0.05f)] private float runInterval = 0.32f;
        [SerializeField, Range(0f, 1f)] private float volume = 0.28f;
        [SerializeField, Range(0f, 0.2f)] private float pitchVariation = 0.04f;
        [SerializeField, Min(0.01f)] private float minSpeed = 0.2f;

        private float timer;
        private int clipIndex;

        private void Awake()
        {
            if (source == null)
            {
                source = GetComponent<AudioSource>();
            }

            source.playOnAwake = false;
            source.spatialBlend = 0.7f;
        }

        private void Update()
        {
            if (controller == null || source == null || clips == null || clips.Length == 0)
            {
                return;
            }

            Vector3 velocity = controller.Velocity;
            float horizontalSpeed = new Vector2(velocity.x, velocity.z).magnitude;
            if (!controller.IsGrounded || horizontalSpeed < minSpeed)
            {
                timer = 0f;
                return;
            }

            timer -= Time.deltaTime;
            if (timer > 0f)
            {
                return;
            }

            AudioClip clip = clips[clipIndex % clips.Length];
            clipIndex++;
            timer = controller.IsSprinting ? runInterval : walkInterval;
            if (clip == null)
            {
                return;
            }

            source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            source.PlayOneShot(clip, volume);
        }
    }
}
