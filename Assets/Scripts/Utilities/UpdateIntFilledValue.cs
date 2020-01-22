using UnityEngine;
using UnityEngine.UI;
using Cursed.Character;

namespace Cursed.UI
{
    public class UpdateIntFilledValue : MonoBehaviour
    {
        [SerializeField] private bool _lerpValues = true;

        private Image _fillImage = null;

        [SerializeField] private IntReference _maxValue;
        [SerializeField] private float _lerpSpeed = 5f;
        private bool _updateValue;
        private float _currentValue;


        private void Awake()
        {
            _fillImage = GetComponent<Image>();
        }

        private void Update()
        {
            if(_updateValue)
            {
                _fillImage.fillAmount = Mathf.Lerp(_fillImage.fillAmount, _currentValue, _lerpSpeed * Time.deltaTime);
            }

        }

        public void UpdateValue(int value)
        {
            float f = (float)value / (float)_maxValue;

            if (!_lerpValues)
                _fillImage.fillAmount = f;

            else
            {
                _currentValue = f;
                _updateValue = true;
            }
        }
    }
}