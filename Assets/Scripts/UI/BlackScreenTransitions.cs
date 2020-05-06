using System.Collections;
using UnityEngine;

namespace Cursed.UI
{
    public class BlackScreenTransitions : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();    
        }

        private void Start()
        {
            FadeOut();
        }

        public void FadeIn()
        {
            _animator.SetTrigger("FadeIn");
        }

        public void FadeOut()
        {
            _animator.SetTrigger("FadeOut");
        }
    }
}
