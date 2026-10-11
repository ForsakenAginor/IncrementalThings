using System.Collections.Generic;
using Source.Scripts.AudioLogic;
using Source.Scripts.DI.Services.Boot;
using Source.Scripts.General;
using Source.Scripts.Localization;
using Source.Scripts.SaveSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Source.Scripts.EntryPoint
{
    public class MainMenuRoot : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private AudioSaveLoadService _soundInitializer;

        [Header("Localization")]
        [SerializeField] private Button _toEnglish;
        [SerializeField] private Button _toRussian;
        private LanguageChanger _languageChanger;

        [Header("Other")]
        private ISceneChanger _sceneChanger;
        private SaveDataProvider _saveDataProvider;
        private List<IDataSaveLoadService> _saveLoadServices = new();

        [Inject]
        public void Construct(ISceneChanger sceneChanger, SaveDataProvider saveDataProvider)
        {
            _sceneChanger = sceneChanger;
            _saveDataProvider = saveDataProvider;

            _saveLoadServices.Add(_soundInitializer);
        }

        private void Start()
        {
            _languageChanger = new(_saveDataProvider.PlayerSavedData);

            LoadData();

            _toEnglish.onClick.AddListener(ChangeLanguageToEnglish);
            _toRussian.onClick.AddListener(ChangeLanguageToRussian);
            _playButton.onClick.AddListener(OnPlayButtonClick);

            _sceneChanger.FadeOut();
            Time.timeScale = 1f;
        }

        private void OnDestroy()
        {
            _toEnglish.onClick.RemoveListener(ChangeLanguageToEnglish);
            _toRussian.onClick.RemoveListener(ChangeLanguageToRussian);

            _playButton.onClick.RemoveListener(OnPlayButtonClick);
        }

        private void LoadData()
        {
            foreach (var service in _saveLoadServices)
            {
                service.Init(_saveDataProvider.PlayerSavedData);
            }

            foreach (var service in _saveLoadServices)
            {
                service.Load();
            }
        }

        private void OnPlayButtonClick()
        {
            foreach (var service in _saveLoadServices)
            {
                service.Save();
            }

            _saveDataProvider.Save();
            _sceneChanger.LoadScene(Scenes.Game.ToString());
        }

        private void ChangeLanguageToRussian()
        {
            _languageChanger.SetLanguage(LanguageType.Russian);
        }

        private void ChangeLanguageToEnglish()
        {
            _languageChanger.SetLanguage(LanguageType.English);
        }
    }
}