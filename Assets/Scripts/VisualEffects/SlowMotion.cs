using System.Collections;
using UnityEngine;

namespace Cursed.VisualEffect
{
    public class SlowMotion : Singleton<SlowMotion>
    {
        private bool _isFrozen = false;
        private bool _pendingFreeze = false;
        private float _currentFreezeTime;

        [Range(0, 1)] public float _slowMotionPower;

        public void Freeze (FloatReference _freezeData)
        {
            _currentFreezeTime = _freezeData.Value;
            _pendingFreeze = true; 
            
            if (_pendingFreeze && !_isFrozen)
            {
                StartCoroutine(DoFreeze(_currentFreezeTime));
            }
        }

        private IEnumerator DoFreeze (float _duration)
        {
            _isFrozen = true;
            Time.timeScale = _slowMotionPower;

            yield return new WaitForSecondsRealtime (_duration);

            _isFrozen = false;            
            Time.timeScale = 1;
            _pendingFreeze = false;

        }
    }
}