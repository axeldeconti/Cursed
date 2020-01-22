using UnityEngine;
using UnityEngine.UI;
using Cursed.Character;

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

        [SerializeField] private float _shrinkSpeed = 1f;

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
                    _damageBar.fillAmount -= _shrinkSpeed * Time.deltaTime; 
                }
            }
        }

        public void LaunchShrink()
        {
            _damageBar.fillAmount = _healthBar.fillAmount;
            _damagedHealthShrinkTimer = _damagedHealthShrinkMaxTimer;
            _shrinkSpeed = (_damageBar.fillAmount - (float)(GameObject.FindGameObjectWithTag("Player").GetComponent<HealthManager>().CurrentHealth / 100)) * 5;
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

