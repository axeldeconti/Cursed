using UnityEngine;
using UnityEngine.UI;
using Cursed.Character;

namespace Cursed.UI
{
    public class UpdateIntFilledValue : MonoBehaviour
    {
        [SerializeField] private bool _lerpValues = true;

        private Image _fillImage = null;

        [SerializeField] private float _lerpSpeed = 5f;
        private bool _updateValue;
        private float _currentValue;

       [SerializeField] private UpdateMaxBar _updateMaxBar;

        private void Start()
        {
            _fillImage = GetComponent<Image>();
            _currentValue = _fillImage.fillAmount;

            EnemyHealth enemyHealth = GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.onEnemyHealthUpdate += UpdateValue;
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
            //_fillImage.fillAmount = _currentValue;
            float f = (float)value / (float)_updateMaxBar.LastMaxValue;

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