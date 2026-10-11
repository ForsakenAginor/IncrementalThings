using Source.Scripts.DI.Services.Global;
using UnityEngine;
using Zenject;

namespace Source.Scripts.DI.Installers
{
    public class MainMenuInstaller : MonoInstaller
    {
        [SerializeField] private UIAudioPlayer _uiAudioPlayerPrefab;
        [SerializeField] private MusicPlayer _musicPlayerPrefab;

        private ZenjectInstantiateWrapper _instantiateWrapper;

        public override void InstallBindings()
        {
            BindInstantiateWrapper();
            BindCoroutineRunner();
            BindAudio();
        }

        private void BindCoroutineRunner()
        {
            ZenjectCoroutineRunner runner = new(this);

            Container
                .Bind<ICoroutineRunner>()
                .To<ZenjectCoroutineRunner>()
                .FromInstance(runner)
                .AsSingle()
                .NonLazy();
        }

        private void BindInstantiateWrapper()
        {
            _instantiateWrapper = new ZenjectInstantiateWrapper(Container);

            Container.Bind<IZenjectInstantiateWrapper>()
                .To<ZenjectInstantiateWrapper>()
                .FromInstance(_instantiateWrapper)
                .AsSingle()
                .NonLazy();
        }

        private void BindAudio()
        {
            UIAudioPlayer uIAudioPlayer = Container.InstantiatePrefabForComponent<UIAudioPlayer>(_uiAudioPlayerPrefab);
            MusicPlayer musicPlayer = Container.InstantiatePrefabForComponent<MusicPlayer>(_musicPlayerPrefab);

            Container
                .Bind<IUIAudioPlayer>()
                .To<UIAudioPlayer>()
                .FromInstance(uIAudioPlayer)
                .AsSingle()
                .NonLazy();

            Container
                .Bind<IMusicPlayer>()
                .To<MusicPlayer>()
                .FromInstance(musicPlayer)
                .AsSingle()
                .NonLazy();
        }
    }
}