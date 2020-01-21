using UnityEngine;
using UnityEngine.UI;

namespace Cursed.UI
{
    public class BarShrink : MonoBehaviour
    {
        [SerializeField] private float _barWidth;
        [SerializeField] private Transform _damageBarTemplate;
        [SerializeField] private Image _healthBar;
        [SerializeField] private float _damageHealthShrinkTimer;
        private float beforeDamagedBarFillAmount;

        public IntEvent onHealthUpdate;

        private void Awake()
        {
            _barWidth = GetComponent<RectTransform>().sizeDelta.x;
            beforeDamagedBarFillAmount = _healthBar.fillAmount;
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

