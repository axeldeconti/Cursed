using UnityEngine;
using UnityEngine.UI;

namespace Cursed.Utilities
{
    public class UpdateIntFilledValue : MonoBehaviour
    {
        private Image _fillImage = null;

        [SerializeField] private IntReference _maxValue;

        private void Awake()
        {
            _fillImage = GetComponent<Image>();
        }

        public void UpdateValue(int value)
        {
            float f = (float)value / (float)_maxValue;

            _fillImage.fillAmount = f;
        }
    }
}