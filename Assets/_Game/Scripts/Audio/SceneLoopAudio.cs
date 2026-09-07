using UnityEngine;

namespace TilkiOyunu.Foundation
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class SceneLoopAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip clip;
        [SerializeField, Range(0f, 1f)] private float volume = 0.35f;

        private void Awake()
        {
            if (source == null)
            {
                source = GetComponent<AudioSource>();
            }

            if (source == null || clip == null)
            {
                return;
            }

            source.clip = clip;
            source.loop = true;
            source.playOnAwake = true;
            source.volume = volume;
            if (!source.isPlaying)
            {
                source.Play();
            }
        }
    }
}
