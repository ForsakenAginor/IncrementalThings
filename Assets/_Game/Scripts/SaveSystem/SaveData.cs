using System;

namespace Source.Scripts.SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public SaveData()
        {
            SelectedLanguage = "English";
        }

        public ScreenSettings ScreenSettings { get; set; }

        public AudioSettings AudioSettings { get; set; }

        public string SelectedLanguage { get; set; }
    }

    [Serializable]
    public class ScreenSettings
    {
        public int Index { get; set; } = 4;

        public bool IsFullScreen { get; set; } = true;
    }

    [Serializable]
    public class AudioSettings
    {
        public float MasterVolume { get; set; }

        public float EffectsVolume { get; set; }

        public float MusicVolume { get; set; }
    }
}