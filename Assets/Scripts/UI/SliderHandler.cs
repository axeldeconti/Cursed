using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Cursed.UI
{
    [RequireComponent(typeof(Slider))]
    public class SliderHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public static bool isFirstSelected = true;

        [SerializeField] private TextMeshProUGUI _text = null;
        [SerializeField] private Image _fillImg = null;
        [Space]
        private Slider _slider;

        private void Start()
        {
            _slider = GetComponent<Slider>();
        }

        private void SetColor(Color color)
        {
            if (_text)
                _text.color = color;
            if (_fillImg)
                _fillImg.color = color;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SetColor(_slider.colors.normalColor);
        }

        public void OnSelect(BaseEventData eventData)
        {
            SetColor(_slider.colors.selectedColor);

            if (isFirstSelected)
                isFirstSelected = false;
            else
                AkSoundEngine.PostEvent("Play_Button_Selected", gameObject);
        }
    }
}
