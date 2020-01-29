using System.Collections;
using UnityEngine;

namespace Cursed.VisualEffect
{
    public class FreezeFrame : MonoBehaviour
    {
        [SerializeField] FloatReference _duration;

        private bool _isFrozen = false;
        private float _pendingFreezeDuration = 0f;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                Freeze();
            }

            if (_pendingFreezeDuration > 0f && !_isFrozen)
            {
                StartCoroutine(DoFreeze());
            }
        }

        public void Freeze ()
        {
            _pendingFreezeDuration = _duration;
        }

        private IEnumerator DoFreeze ()
        {
            _isFrozen = true;
            float _original = Time.timeScale;
            Time.timeScale = 0f;
            Debug.Log("Freeze");

            yield return new WaitForSecondsRealtime (_duration);

            Time.timeScale = _original;
            _pendingFreezeDuration = 0f;
            _isFrozen = false;
            Debug.Log("Not Freeze");
        }
    }
}