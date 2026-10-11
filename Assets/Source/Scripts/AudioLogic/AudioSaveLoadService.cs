using Source.Scripts.AudioLogic.Mixer;
using Source.Scripts.SaveSystem;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Source.Scripts.AudioLogic
{
    public class AudioSaveLoadService : MonoBehaviour, IDataSaveLoadService
    {
        private const float DefaultMasterVolume = 0.8f;
        private const float DefaultEffectsVolume = 0.5f;
        private const float DefaultMusicVolume = 0.4f;

        [SerializeField] private AudioMixer _audioMixer;

        [Header("AudioSliders")]
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _effectsVolumeSlider;

        private SaveData _saveData;
        private bool _isInited;

        public bool IsLoaded => throw new System.NotImplementedException();

        public bool IsInited => _isInited;

        public void Init(SaveData saveData, IDataSaveLoadService[] dependentSystems = null)
        {
            _saveData = saveData;

            AudioMixerHandler mixerHandler = new(_audioMixer);
            _ = new AudioSlider(_masterVolumeSlider, mixerHandler, MixerGroups.MasterVolume);
            _ = new AudioSlider(_musicVolumeSlider, mixerHandler, MixerGroups.MusicVolume);
            _ = new AudioSlider(_effectsVolumeSlider, mixerHandler, MixerGroups.EffectsVolume);
            _isInited = true;
        }

        public void Load()
        {
            if (_saveData.AudioSettings != null)
            {
                _masterVolumeSlider.value = _saveData.AudioSettings.MasterVolume;
                _effectsVolumeSlider.value = _saveData.AudioSettings.EffectsVolume;
                _musicVolumeSlider.value = _saveData.AudioSettings.MusicVolume;
            }
            else
            {
                _masterVolumeSlider.value = DefaultMasterVolume;
                _effectsVolumeSlider.value = DefaultEffectsVolume;
                _musicVolumeSlider.value = DefaultMusicVolume;
            }
        }

        public void Save()
        {
            if (_saveData.AudioSettings == null)
            {
                _saveData.AudioSettings = new();
            }

            _saveData.AudioSettings.MasterVolume = _masterVolumeSlider.value;
            _saveData.AudioSettings.EffectsVolume = _effectsVolumeSlider.value;
            _saveData.AudioSettings.MusicVolume = _musicVolumeSlider.value;
        }
    }
}