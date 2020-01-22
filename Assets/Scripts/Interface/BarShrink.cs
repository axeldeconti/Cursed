using UnityEngine;
using UnityEngine.UI;

namespace Cursed.UI
{
    public class BarShrink : MonoBehaviour
    {
        private float _damagedHealthShrinkMaxTimer = .5f;

        private float _barWidth;
        [SerializeField] private Transform _damageBarTemplate;
        [SerializeField] private Image _damageBar;
        [SerializeField] private Image _healthBar;
        private float beforeDamagedBarFillAmount;
        private float _damagedHealthShrinkTimer;

        private void Awake()
        {
            _barWidth = GetComponent<RectTransform>().sizeDelta.x;
            beforeDamagedBarFillAmount = _healthBar.fillAmount;
            _damageBar.fillAmount = _healthBar.fillAmount;
        }

        private void Update()
        {
            _damagedHealthShrinkTimer -= Time.deltaTime;
            if(_damagedHealthShrinkTimer < 0)
            {
                if(_healthBar.fillAmount < _damageBar.fillAmount)
                {
                    float shrinkSpeed = 1f;
                    _damageBar.fillAmount -= shrinkSpeed * Time.deltaTime; 
                }
            }
        }

        public void LaunchShrink()
        {
            _damageBar.fillAmount = _healthBar.fillAmount;
            _damagedHealthShrinkTimer = _damagedHealthShrinkMaxTimer;
        }

        public void LaunchCut()
        {
            Transform damageBar = Instantiate(_damageBarTemplate, transform);
            damageBar.gameObject.SetActive(true);
            damageBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(_healthBar.fillAmount * _barWidth, damageBar.GetComponent<RectTransform>().anchoredPosition.y);
            damageBar.GetComponent<Image>().fillAmount = beforeDamagedBarFillAmount - _healthBar.fillAmount;
            damageBar.gameObject.AddComponent<CutBarTemplate>();
            beforeDamagedBarFillAmount = LastDamagedBarFillAmount();
        }

        private float LastDamagedBarFillAmount()
        {
            return _healthBar.fillAmount;
        }
    }
}

