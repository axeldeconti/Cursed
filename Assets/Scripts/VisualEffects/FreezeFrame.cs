using System.Collections;
using UnityEngine;

namespace Cursed.VisualEffect
{
    public class FreezeFrame : MonoBehaviour
    {
        private bool _isFrozen = false;
        private bool _pendingFreeze = false;
        private float _timeLerp = 1f;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                Freeze();
            }           
            
            if (_pendingFreeze && !_isFrozen)
            {
                StartCoroutine(DoFreeze(0.2f));
            }
        }

        public void Freeze ()
        {
            _pendingFreeze = true;
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