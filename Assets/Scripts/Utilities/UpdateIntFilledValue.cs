using UnityEngine;
using UnityEngine.UI;

namespace Cursed.Utilities
{
    public class UpdateIntFilledValue : MonoBehaviour
    {
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
            _currentValue = f;
            _updateValue = true;
        }
    }
}