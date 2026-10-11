using UnityEngine;

namespace Source.Scripts.DI.Services.Global
{
    public interface IUIAudioPlayer
    {
        public void PlayButtonClick();
        public void PlayButtonHighlight();
    }

    public class UIAudioPlayer : MonoBehaviour, IUIAudioPlayer
    {
        [SerializeField] private AudioSource _buttonClick;
        [SerializeField] private AudioSource _buttonHighlight;

        public void PlayButtonClick() => _buttonClick.Play();

        public void PlayButtonHighlight() => _buttonHighlight.Play();
    }
}