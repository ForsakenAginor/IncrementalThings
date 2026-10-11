using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Source.Scripts.DI.Services.Global
{
    public interface IMusicPlayer
    {
        public void Pause();

        public void PlayBossFightMusic();

        public void Resume();
    }

    public class MusicPlayer : MonoBehaviour, IMusicPlayer
    {
        private readonly Queue<AudioClip> _clipQueue = new Queue<AudioClip>();

        [SerializeField] private AudioClip _bossFightClip;
        [SerializeField] private AudioClip[] _clips;
        [SerializeField] private AudioSource _source;
        
        private CancellationTokenSource _musicLoopCts;
        private bool _allClipsPreloaded = false;
        private float _volume;

        private void Awake()
        {
            PreloadAllClipsAsync();
        }

        private void Start()
        {
            StartMusicAfterPreloadAsync();
        }

        public void Pause()
        {
            _volume = _source.volume;
            _source.volume = 0;
        }

        public void Resume()
        {
            _source.volume = _volume;
        }
        
        public void PlayBossFightMusic()
        {
            if (_bossFightClip == null)
            {
                Debug.Log("BossFight clip not assigned");
                return;
            }
            
            _musicLoopCts?.Cancel();
            _musicLoopCts?.Dispose();
            _musicLoopCts = null;
            
            _source.Stop();
            _source.clip = _bossFightClip;
            _source.loop = true;
            _source.Play();
        }
        
        private async UniTaskVoid StartMusicAfterPreloadAsync()
        {
            await UniTask.WaitUntil(() => _allClipsPreloaded, cancellationToken: destroyCancellationToken);
            _musicLoopCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            PlayLoopMusicAsync(_musicLoopCts.Token);
        }
        
        private async UniTaskVoid PlayLoopMusicAsync(CancellationToken token)
        {
            while (true)
            {
                _source.clip = _clipQueue.Dequeue();
                _source.Play();
                _clipQueue.Enqueue(_source.clip);
                await UniTask.WaitWhile(() => _source.isPlaying, cancellationToken: token);
            }
        }
        
        private async UniTaskVoid PreloadAllClipsAsync()
        {
            foreach (var clip in _clips)
            {
                clip.LoadAudioData();
                while (clip.loadState != AudioDataLoadState.Loaded)
                    await UniTask.NextFrame(destroyCancellationToken);
            }

            if (_bossFightClip != null)
            {
                _bossFightClip.LoadAudioData();
                
                while (_bossFightClip.loadState != AudioDataLoadState.Loaded)
                    await UniTask.NextFrame(destroyCancellationToken);
            }

            foreach (var clip in _clips)
                _clipQueue.Enqueue(clip);

            _allClipsPreloaded = true;
        }
    }
}