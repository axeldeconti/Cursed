using System.Collections;
using UnityEngine;

namespace Cursed.VisualEffect
{
    public class SlowMotion : Singleton<SlowMotion>
    {
        private bool _isFrozen = false;
        private bool _pendingFreeze = false;
        private float _currentFreezeTime;
        private bool _lerpFreezing = false;
        private float _targetTime = 0;
        private float _currentSpeed = 0;

        [SerializeField] private float _freezingSpeedIn = 5f;
        [SerializeField] private float _freezingSpeedOut = 5f;


        [Range(0, 1)] public float _slowMotionPower;

        private void Update()
        {
            if(_lerpFreezing)
            {
                float currentTime = Mathf.Lerp(Time.timeScale, _targetTime, _currentSpeed * Time.deltaTime);
                Time.timeScale = currentTime;

                if (Time.timeScale - _targetTime < .01f && Time.timeScale - _targetTime > 0f)
                    _lerpFreezing = false;
            }
        }

        public void Freeze (FloatReference _freezeData)
        {
            _currentFreezeTime = _freezeData.Value;
            _pendingFreeze = true; 
            
            if (_pendingFreeze && !_isFrozen)
            {
                StartCoroutine(DoFreeze(_currentFreezeTime));
            }
        }

        private void LerpFreeze(float target, float speed)
        {
            _lerpFreezing = true;
            _targetTime = target;
            _currentSpeed = speed;
        }

        private IEnumerator DoFreeze (float _duration)
        {
            _isFrozen = true;
            LerpFreeze(_slowMotionPower, _freezingSpeedIn);
            yield return new WaitForSecondsRealtime (_duration);
            _isFrozen = false;
            LerpFreeze(1f, _freezingSpeedOut);
            _pendingFreeze = false;

        }
    }
}