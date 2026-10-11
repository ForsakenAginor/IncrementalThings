using Source.Scripts.DI.Services.Game;
using Source.Scripts.DI.Services.Game.FSE;
using Source.Scripts.DI.Services.Global;
using Source.Scripts.Utility.Pools;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Source.Scripts.DI.Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private UIAudioPlayer _uiAudioPlayerPrefab;
        [SerializeField] private MusicPlayer _musicPlayerPrefab;
        [SerializeField] private AudioPlayer _audioPlayerPrefab;

        [Header("Vignettes")]
        [SerializeField] private ScriptableRendererFeature _healthVignetteEffect;
        [SerializeField] private Material _healthVignetteMaterial;
        [SerializeField] private ScriptableRendererFeature _noiceVignetteEffect;
        [SerializeField] private Material _noiceVignetteMaterial;

        private ZenjectInstantiateWrapper _instantiateWrapper;

        public override void InstallBindings()
        {
            BindInstantiateWrapper();
            BindPoolFactory();
            BindCoroutineRunner();
            BindTaskFactory();
            BindAudio();
            BindTimeIncrement();
            BindHealthVignetteEffect();
            BindNoiceVignetteEffect();
        }

        private void BindTaskFactory()
        {
            TaskFactory factory = Container.InstantiateComponent<TaskFactory>(gameObject);

            Container
                .Bind<ISceneCancellationTokenProvider>()
                .To<TaskFactory>()
                .FromInstance(factory)
                .AsSingle();
            
            Container
                .Bind<ITaskFactory>()
                .To<TaskFactory>()
                .FromInstance(factory)
                .AsSingle();
        }
        
        private void BindNoiceVignetteEffect()
        {
            NoiceVignetteEffect effect = new NoiceVignetteEffect(_noiceVignetteEffect, _noiceVignetteMaterial);

            Container
                .Bind<NoiceVignetteEffect>()
                .To<NoiceVignetteEffect>()
                .FromInstance(effect)
                .AsSingle()
                .NonLazy();

            Container
                .Bind<INoiceVignetteEffect>()
                .To<NoiceVignetteEffect>()
                .FromInstance(effect)
                .AsCached();
        }

        private void BindHealthVignetteEffect()
        {
            HealthVignetteEffect effect = new HealthVignetteEffect(_healthVignetteEffect, _healthVignetteMaterial);

            Container
                .Bind<HealthVignetteEffect>()
                .To<HealthVignetteEffect>()
                .FromInstance(effect)
                .AsSingle()
                .NonLazy();

            Container
                .Bind<IHealthDamageEffect>()
                .To<HealthVignetteEffect>()
                .FromInstance(effect)
                .AsCached();
        }

        private void BindPoolFactory()
        {
            PoolableFactory poolableFactory = new (Container);
            PoolFactory factory = new(poolableFactory, transform);

            Container
                .Bind<IPoolFactory>()
                .To<PoolFactory>()
                .FromInstance(factory)
                .AsSingle()
                .NonLazy();
        }

        private void BindTimeIncrement()
        {
            GameTimeService timeService = gameObject.AddComponent<GameTimeService>();

            Container
                .Bind<IGameTimeService>()
                .To<GameTimeService>()
                .FromInstance(timeService)
                .AsSingle();
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
            AudioPlayer audioPlayer = Container.InstantiatePrefabForComponent<AudioPlayer>(_audioPlayerPrefab);

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

            Container
                .Bind<AudioPlayer>()
                .To<AudioPlayer>()
                .FromInstance(audioPlayer)
                .AsSingle()
                .NonLazy();
        }
    }
}