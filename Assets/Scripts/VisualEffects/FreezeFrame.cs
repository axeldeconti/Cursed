using System.Collections;
using UnityEngine;

namespace Cursed.VisualEffect
{
    public class FreezeFrame : Singleton<FreezeFrame>
    {
        private bool _isFrozen = false;
        private bool _pendingFreeze = false;
        private float _currentFreezeTime;

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
            Time.timeScale = 0;
            Debug.Log("Freeze");

            yield return new WaitForSecondsRealtime (_duration);

            _isFrozen = false;            
            Time.timeScale = 1;
            _pendingFreeze = false;
            Debug.Log("Not Freeze");
        }
    }
}