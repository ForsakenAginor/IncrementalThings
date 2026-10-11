using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Source.Scripts.Utility.UI
{
    public class ButtonTextEnlarger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private TMP_Text _text;
        private Button _button;

        private Vector3 _originalScale = Vector3.one;
        private Vector3 _enlargeScale = Vector3.one * 1.05f;

        private void Awake()
        {
            _text = GetComponentInChildren<TMP_Text>();
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnClick);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Проверяем, что кнопка существует и активна (interactable = true)
            if (_button != null && !_button.interactable)
                return;

            _text.transform.localScale = _enlargeScale;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _text.transform.localScale = _originalScale;
        }

        private void OnClick()
        {
            _text.transform.localScale = _originalScale;
        }
    }
}