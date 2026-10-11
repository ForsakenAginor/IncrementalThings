using Source.Scripts.DI.Services.Boot;
using Source.Scripts.SaveSystem;
using Source.Scripts.Utility.SeededRandom;
using UnityEngine;
using Zenject;

namespace Source.Scripts.DI.Installers
{
    public class BootsTrapInstaller : MonoInstaller
    {
        [SerializeField] private SceneChanger _sceneChanger;
        [SerializeField] private ConfigurationProvider _configurationProvider;

        public override void InstallBindings()
        {
            InitSceneChanger();
            BindSaveLoadService();
            BindConfigurations();
            BindSeededRandomSystem();
        }

        private void BindSeededRandomSystem()
        {
            GameRandomInitializer randomInitializer = new GameRandomInitializer();

            Container
                .Bind<IGameRandomInitializer>()
                .To<GameRandomInitializer>()
                .FromInstance(randomInitializer)
                .AsSingle()
                .NonLazy();
        }

        private void BindConfigurations()
        {
            Container
                .Bind<ConfigurationProvider>()
                .To<ConfigurationProvider>()
                .FromInstance(_configurationProvider)
                .AsSingle()
                .NonLazy();
        }

        private void InitSceneChanger()
        {
            Container
                .Bind<ISceneChanger>()
                .To<SceneChanger>()
                .FromComponentInNewPrefab(_sceneChanger)
                .AsSingle();
        }

        private void BindSaveLoadService()
        {
            PlayerPrefsSaveLoadService saveService = new("SavedData");
            DataSerializer<SaveData> serializer = new(saveService);
            SaveDataProvider saveDataProvider = new(serializer);

            Container
                .Bind<SaveDataProvider>()
                .To<SaveDataProvider>()
                .FromInstance(saveDataProvider)
                .AsSingle()
                .NonLazy();
        }
    }
}