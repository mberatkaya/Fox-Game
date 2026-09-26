using UnityEngine;

namespace TilkiOyunu.Foundation
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class LightPathAudioFeedback : MonoBehaviour
    {
        [SerializeField] private LightPathController controller;
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip activateClip;
        [SerializeField] private AudioClip completedClip;
        [SerializeField] private AudioClip failedClip;
        [SerializeField, Range(0f, 1f)] private float volume = 0.45f;

        private void Awake()
        {
            if (source == null)
            {
                source = GetComponent<AudioSource>();
            }

            if (source != null)
            {
                source.playOnAwake = false;
            }
        }

        private void OnEnable()
        {
            if (controller == null)
            {
                controller = GetComponent<LightPathController>();
            }

            if (controller == null)
            {
                return;
            }

            controller.Started += HandleStarted;
            controller.ProgressChanged += HandleProgressChanged;
            controller.Completed += HandleCompleted;
            controller.Failed += HandleFailed;
        }

        private void OnDisable()
        {
            if (controller == null)
            {
                return;
            }

            controller.Started -= HandleStarted;
            controller.ProgressChanged -= HandleProgressChanged;
            controller.Completed -= HandleCompleted;
            controller.Failed -= HandleFailed;
        }

        private void HandleStarted()
        {
            Play(activateClip, 0.8f);
        }

        private void HandleProgressChanged(int current, int total)
        {
            if (current > 0 && current < total)
            {
                Play(activateClip, 1f);
            }
        }

        private void HandleCompleted()
        {
            Play(completedClip, 1.1f);
        }

        private void HandleFailed(string reason)
        {
            Play(failedClip, 0.85f);
        }

        private void Play(AudioClip clip, float pitch)
        {
            if (source == null || clip == null)
            {
                return;
            }

            source.pitch = pitch;
            source.PlayOneShot(clip, volume);
        }
    }
}
