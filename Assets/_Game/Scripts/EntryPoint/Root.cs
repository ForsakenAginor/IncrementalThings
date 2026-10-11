using System.Collections.Generic;
using Source.Scripts.AudioLogic;
using Source.Scripts.DI.Services.Boot;
using Source.Scripts.DI.Services.Game.FSE;
using Source.Scripts.General;
using Source.Scripts.SaveSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Source.Scripts.EntryPoint
{
    public class Root : MonoBehaviour
    {
        [Header("Other")]
        [SerializeField] private AudioSaveLoadService _soundInitializer;
        [SerializeField] private Button _closeButton;
        private ISceneChanger _sceneChanger;
        private SaveDataProvider _saveDataProvider;
        private List<IDataSaveLoadService> _saveLoadServices = new();
        private HealthVignetteEffect _healthVignette;
        private NoiceVignetteEffect _noiceVignette;

        [Inject]
        public void Construct(ISceneChanger sceneChanger, SaveDataProvider saveDataProvider, HealthVignetteEffect healthVignette, NoiceVignetteEffect noiceVignette)
        {
            _sceneChanger = sceneChanger;
            _saveDataProvider = saveDataProvider;
            _healthVignette = healthVignette;
            _noiceVignette = noiceVignette;

            _healthVignette.Enable();
            _noiceVignette.Enable();
            _saveLoadServices.Add(_soundInitializer);
        }

        private void Start()
        {
            LoadData();

            _closeButton.onClick.AddListener(OnCloseButtonClick);
            _sceneChanger.FadeOut();
            Time.timeScale = 1f;
        }

        private void OnDestroy()
        {
            _healthVignette.Disable();
            _noiceVignette.Disable();
            _closeButton.onClick.RemoveListener(OnCloseButtonClick);
        }

        private void SaveData()
        {
            foreach (var service in _saveLoadServices)
            {
                service.Save();
            }

            _saveDataProvider.Save();
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

        private void OnCloseButtonClick()
        {
            SaveData();
            _sceneChanger.LoadScene(Scenes.Menu.ToString());
        }
    }
}