using UnityEngine;

namespace Cursed.VisualEffect
{
    public class BackgroundGradient : MonoBehaviour
    {
        [SerializeField] private Gradient _colorGradient;
        [SerializeField] private float _changeColorSpeed = 5f;
        private Color _currentColor;
        private SpriteRenderer _spriteRenderer;

        private float _currentValue = 0;
        private float _maxValue = 100;


        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            UpdateGradient();
        }

        private void Update()
        {
            UpdateCurrentColor();
            UpdateGradient();
        }

        private void UpdateCurrentColor()
        {
            _currentValue = Mathf.PingPong(Time.time * _changeColorSpeed, _maxValue);
        }

        private void UpdateGradient()
        {
            _currentColor = _colorGradient.Evaluate(1 - (_currentValue / _maxValue));
            _spriteRenderer.color = _currentColor;
        }

    }
}
