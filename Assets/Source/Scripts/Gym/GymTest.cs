using System;
using Cysharp.Threading.Tasks;
using R3;
using Source.Scripts.DI.Services.Global;
using Source.Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GymTest : MonoBehaviour
{
    [SerializeField] private MusicPlayer _musicPlayer;
    [SerializeField] private Button _takeDamageButton;
    [SerializeField] private Button _healButton;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private GameObject _objetct;
    [SerializeField] private TMP_Text _timerText;
    
    private Health _health;
    private ISceneCancellationTokenProvider _cancellationTokenProvider;

    [Inject]
    public void Construct(ISceneCancellationTokenProvider cancellationTokenProvider)
    {
        _cancellationTokenProvider = cancellationTokenProvider;
    }

    private void Awake()
    {
        _health = new Health(100);

        _takeDamageButton.onClick.AddListener(TakeDamage);
        _healButton.onClick.AddListener(Heal);

        _health.ResourceAmount.Subscribe(
                onNext: value => _healthText.text = value.ToString(),
                onCompleted: (_) => Die())
            .AddTo(_objetct);
        
        Count();
    }

    private void OnDestroy()
    {
        _takeDamageButton.onClick.RemoveListener(TakeDamage);
        _healButton.onClick.RemoveListener(Heal);
    }

    [Sirenix.OdinInspector.Button]
    private void PlayBossMusic()
    {
        _musicPlayer.PlayBossFightMusic();
    }

    private async UniTaskVoid Count()
    {
        int frame = 0;
        var token = _cancellationTokenProvider.Token;
        
        while (true)
        {
            await UniTask.NextFrame(token);
            frame++;
            _timerText.text = frame.ToString();
        }
    }

    private void Die()
    {
        _healthText.text = "Died";
    }

    private void Heal()
    {
        _health.Add(5);
    }

    private void TakeDamage()
    {
        _health.Spent(5);
    }
}

public class Health : Resource
{
    public Health(int amount) : base(amount)
    {
    }

    public Health(int amount, int maximum = Int32.MaxValue) : base(amount, maximum)
    {
    }

    protected override void DoOnResourceOver()
    {
        CallResourceOver();
    }
}