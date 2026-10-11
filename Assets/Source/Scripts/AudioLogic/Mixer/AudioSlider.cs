using System;
using UnityEngine.UI;

namespace Source.Scripts.AudioLogic.Mixer
{
    public class AudioSlider
    {
        private Slider _slider;
        private AudioMixerHandler _mixerHandler;
        private MixerGroups _group;

        public AudioSlider(Slider slider, AudioMixerHandler mixerHandler, MixerGroups group)
        {
            _slider = slider != null ? slider : throw new ArgumentNullException(nameof(slider));
            _mixerHandler = mixerHandler != null ? mixerHandler : throw new ArgumentNullException(nameof(mixerHandler));
            _group = group;
            ChangeVolume(slider.value);

            _slider.onValueChanged.AddListener(ChangeVolume);
        }

        ~AudioSlider()
        {
            _slider.onValueChanged.RemoveListener(ChangeVolume);
        }

        private void ChangeVolume(float value)
        {
            _mixerHandler.ChangeVolume(value, _group);
        }
    }
}