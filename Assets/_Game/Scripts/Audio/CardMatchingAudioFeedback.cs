using UnityEngine;

namespace TilkiOyunu.Foundation
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class CardMatchingAudioFeedback : MonoBehaviour
    {
        [SerializeField] private CardMatchingController controller;
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip openClip;
        [SerializeField] private AudioClip revealClip;
        [SerializeField] private AudioClip matchClip;
        [SerializeField] private AudioClip completeClip;
        [SerializeField, Range(0f, 1f)] private float volume = 0.42f;

        private int previousMatchedPairs;

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
                controller = GetComponent<CardMatchingController>();
            }

            if (controller == null)
            {
                return;
            }

            controller.Started += HandleStarted;
            controller.CardChanged += HandleCardChanged;
            controller.ProgressChanged += HandleProgressChanged;
            controller.Completed += HandleCompleted;
        }

        private void OnDisable()
        {
            if (controller == null)
            {
                return;
            }

            controller.Started -= HandleStarted;
            controller.CardChanged -= HandleCardChanged;
            controller.ProgressChanged -= HandleProgressChanged;
            controller.Completed -= HandleCompleted;
        }

        private void HandleStarted()
        {
            previousMatchedPairs = 0;
            Play(openClip, 0.9f);
        }

        private void HandleCardChanged(int index, CardMatchingSlotView view)
        {
            if (view.State == CardMatchingCardState.Revealed)
            {
                Play(revealClip, 1f);
            }
        }

        private void HandleProgressChanged(int matched, int total)
        {
            if (matched > previousMatchedPairs)
            {
                Play(matchClip, 1.04f);
            }

            previousMatchedPairs = matched;
        }

        private void HandleCompleted()
        {
            Play(completeClip, 1f);
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
