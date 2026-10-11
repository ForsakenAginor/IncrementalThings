using System;
using UnityEngine;

namespace Source.Scripts.SaveSystem
{
    public class PlayerPrefsSaveLoadService : ISaveLoadService
    {
        private readonly string _playerPrefsKey = nameof(_playerPrefsKey);

        public PlayerPrefsSaveLoadService(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            _playerPrefsKey = key;
        }

        public string GetSavedInfo()
        {
            if (PlayerPrefs.HasKey(_playerPrefsKey) == false)
                return null;

            return PlayerPrefs.GetString(_playerPrefsKey);
        }

        public void SaveInfo(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            PlayerPrefs.SetString(_playerPrefsKey, value);
        }
    }
}