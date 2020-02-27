using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

namespace Cursed.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private TextMeshProUGUI _text = null;

        private Button _button = null;

        private void Start()
        {
            _button = GetComponent<Button>();

            _button.onClick.AddListener(() => StartCoroutine(Press()));
        }

        private IEnumerator Press()
        {
            SetTextColor(_button.colors.pressedColor);
            yield return new WaitForSeconds(.1f);
            SetTextColor(_button.colors.selectedColor);
        }

        private void SetTextColor(Color color)
        {
            if (_text)
                _text.color = color;
        }

        public void OnSelect(BaseEventData eventData)
        {
            SetTextColor(_button.colors.selectedColor);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            StopAllCoroutines();
            SetTextColor(_button.colors.normalColor);
        }
    }
}