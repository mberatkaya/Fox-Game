using UnityEngine;
using UnityEngine.Audio;

namespace TilkiOyunu.Foundation
{
    public sealed class AudioService
    {
        private const string MasterVolumeParameter = "MasterVolume";
        private const string MusicVolumeParameter = "MusicVolume";
        private const string SfxVolumeParameter = "SFXVolume";

        private readonly AudioMixer mixer;

        public AudioService(AudioMixer mixer)
        {
            this.mixer = mixer;
        }

        public void SetMasterVolume(float normalizedVolume)
        {
            SetVolume(MasterVolumeParameter, normalizedVolume);
        }

        public void SetMusicVolume(float normalizedVolume)
        {
            SetVolume(MusicVolumeParameter, normalizedVolume);
        }

        public void SetSfxVolume(float normalizedVolume)
        {
            SetVolume(SfxVolumeParameter, normalizedVolume);
        }

        private void SetVolume(string parameterName, float normalizedVolume)
        {
            if (mixer == null)
            {
                AppLog.Info(LogCategory.Audio, $"No AudioMixer assigned for {parameterName}.");
                return;
            }

            float clamped = Mathf.Clamp(normalizedVolume, 0.0001f, 1f);
            mixer.SetFloat(parameterName, Mathf.Log10(clamped) * 20f);
        }
    }
}
