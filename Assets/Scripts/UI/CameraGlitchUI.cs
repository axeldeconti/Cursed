using UnityEngine;

namespace Cursed.UI
{
    public class CameraGlitchUI : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Enter()
        {
            _animator.SetBool("Enter", true);
            _animator.SetBool("Exit", false);
        }

        public void Exit()
        {
            _animator.SetBool("Enter", false);
            _animator.SetBool("Exit", true);
        }
    }
}
