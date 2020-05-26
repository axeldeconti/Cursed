using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cursed.Props
{
    public class AndroidCuveBehaviour : MonoBehaviour
    {
        [SerializeField] private RuntimeAnimatorController _normalCuve;
        [SerializeField] private RuntimeAnimatorController _brokenCuve;
        [SerializeField] private VoidEvent _cuveBroken;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Attack_1"))
                CheckScene();
        }

        public void CheckScene()
        {
            if (SceneManager.GetActiveScene().name == "Main")
            {
                UpdateAnimator(_normalCuve);
            }
            else if (SceneManager.GetActiveScene().name == "Tuto" || SceneManager.GetActiveScene().name == "Intro")
            {
                UpdateAnimator(_brokenCuve);
                _cuveBroken?.Raise();
            }
        }

        private void UpdateAnimator(RuntimeAnimatorController newAnimator)
        {
            _animator.runtimeAnimatorController = newAnimator;
        }
    }
}
