using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Source.Scripts.AudioLogic.Mixer
{
    public class AudioMixerHandler
    {
        private AudioMixer _audioMixer;

        public AudioMixerHandler(AudioMixer audioMixer)
        {
            _audioMixer = audioMixer != null ? audioMixer : throw new ArgumentNullException(nameof(audioMixer));
        }

        public void ChangeVolume(float volume, MixerGroups group)
        {
            volume = Mathf.Clamp01(volume);
            float deciBellFactor = 20f;
            float decibell = Mathf.Log10(volume) * deciBellFactor;
            _audioMixer.SetFloat(group.ToString(), decibell);
        }
    }
}