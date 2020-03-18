using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;

namespace Cursed.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private TextMeshProUGUI _text = null;
        [Space]
        [SerializeField] private UnityEvent _onPress;

        private bool _pressed = false;
        private float _pressedTimer = 0;

        private Button _button = null;

        private void Start()
        {
            _button = GetComponent<Button>();

            _button.onClick.AddListener(() => Press());
        }

        private void Press()
        {
            _pressed = true;
            _pressedTimer = .1f;
            AkSoundEngine.PostEvent("Play_Button_Pressed", gameObject);
        }

        private void Update()
        {
            if (!_pressed)
                return;

            float deltaTime = Time.timeScale == 0 ? 1 / (float)GameSettings.FRAME_RATE : Time.deltaTime;
            _pressedTimer -= deltaTime;

            if (_pressedTimer <= 0)
            {
                _onPress.Invoke();
                _pressed = false;
                SetTextColor(_button.colors.normalColor);
            }
        }

        private void SetTextColor(Color color)
        {
            if (_text)
                _text.color = color;
        }

        public void OnSelect(BaseEventData eventData)
        {
            SetTextColor(_button.colors.selectedColor);
            AkSoundEngine.PostEvent("Play_Button_Selected", gameObject);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SetTextColor(_button.colors.normalColor);
        }
    }
}