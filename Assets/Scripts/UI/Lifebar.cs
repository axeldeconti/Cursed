using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Cursed.UI
{
    public class Lifebar : MonoBehaviour
    {
        [SerializeField] private MoovingMask _full = null;
        [SerializeField] private MoovingMask _middle = null;
        [SerializeField] private MoovingMask _regain = null;
        [SerializeField] private RectTransform _slider = null;
        [SerializeField] private IntReference _maxValue = null;
        [SerializeField] private FloatReference _delayToShrink = null;
        [SerializeField] private FloatReference _delayToRegain = null;
        [SerializeField] private FloatReference _shinkingSpeed = null;
        [SerializeField] private FloatReference _regainingSpeed = null;

        private float _deltaPosition = 0.001f;
        private float _sliderMaxPosX = 0;
        private float _targetPercent = 1;
        private bool _isShrinking = false;
        private bool _isRegaining = false;
        private float _currentShrinkingSpeed = 0;
        private float _currentRegainingSpeed = 0;

        private void Start()
        {
            _sliderMaxPosX = _slider.anchoredPosition.x;
        }

        private void Update()
        {
            if (_isShrinking)
            {
                //Is shrinking
                if (Mathf.Abs(_targetPercent - _middle.Pos) <= _deltaPosition || _middle.Pos < _targetPercent - _deltaPosition)
                {
                    _isShrinking = false;
                    _middle.SetPos(_targetPercent);
                }
                else
                {
                    _middle.SetPos(_middle.Pos - _currentShrinkingSpeed * Time.deltaTime);
                }
            }
            
            if (_isRegaining)
            {
                //Is Regaining
                if (Mathf.Abs(_targetPercent - _full.Pos) <= _deltaPosition || _full.Pos > _targetPercent + _deltaPosition)
                {
                    _isRegaining = false;
                    _full.SetPos(_targetPercent);
                }
                else
                {
                    _full.SetPos(_full.Pos + _currentRegainingSpeed * Time.deltaTime);
                }
            }
        }

        public void UpdateCurrentValue(int value)
        {
            float percent = (float)value / _maxValue;

            if (percent < _targetPercent)
            {
                //Loosing life
                if (!_isShrinking)
                    StartCoroutine(StartCountdownToShrink());

                _targetPercent = percent;
                if(!_isRegaining)
                    _full.SetPos(percent);
                _regain.SetPos(percent);
                SetSliderPos(percent);
                _currentShrinkingSpeed = _shinkingSpeed * (1 + (_middle.Pos - percent) * 60);
            }
            else if (percent > _targetPercent)
            {
                //Regain life
                if(!_isRegaining)
                    StartCoroutine(StartCountdownToRegain());

                _targetPercent = percent;
                _regain.SetPos(percent);
                if(_middle.Pos < _targetPercent)
                    _middle.SetPos(percent);
                SetSliderPos(percent);
                _currentRegainingSpeed = _regainingSpeed * (1 + (percent - _full.Pos) * 60);
            }
        }

        private IEnumerator StartCountdownToShrink()
        {
            yield return new WaitForSeconds(_delayToShrink);
            _isShrinking = true;
        }

        private IEnumerator StartCountdownToRegain()
        {
            yield return new WaitForSeconds(_delayToRegain);
            _isRegaining = true;
        }

        private void SetSliderPos(float pos)
        {
            _slider.anchoredPosition = new Vector2(pos * _sliderMaxPosX, _slider.anchoredPosition.y);
        }
    }
}