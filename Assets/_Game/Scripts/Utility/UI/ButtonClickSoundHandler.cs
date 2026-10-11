using Source.Scripts.DI.Services.Global;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Source.Scripts.Utility.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickSoundHandler : MonoBehaviour, IPointerEnterHandler
    {
        private Button _button;
        private IUIAudioPlayer _audioPlayer;

        [Inject]
        public void Construct(IUIAudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClick);            
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            _audioPlayer?.PlayButtonClick();
        }

        public void OnPointerEnter(PointerEventData _)
        {
            _audioPlayer?.PlayButtonHighlight();
        }
    }
}