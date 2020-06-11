using UnityEngine;

namespace Cursed.UI
{
    public class FadeBlackScreenManager : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void FadeIn()
        {
            _animator.SetBool("Loaded", false);
            _animator.SetBool("FadeIn", true);
        }

        public void FadeOut()
        {
            _animator.SetBool("Loaded", false);
            _animator.SetBool("FadeOut", true);
        }

        public void ResetToIdle()
        {
            _animator.SetBool("FadeIn", false);
            _animator.SetBool("FadeOut", false);
            _animator.SetBool("Loaded", true);
        }
    }
}
