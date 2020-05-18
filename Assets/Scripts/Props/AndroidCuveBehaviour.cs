using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cursed.Props
{
    public class AndroidCuveBehaviour : MonoBehaviour
    {
        [SerializeField] private RuntimeAnimatorController _normalCuve;
        [SerializeField] private RuntimeAnimatorController _brokenCuve;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        private void Start()
        {
            CheckScene();
        }

        public void CheckScene()
        {
            Debug.Log("Checking scene");
            if (SceneManager.GetActiveScene().name == "Main")
            {
                UpdateAnimator(_normalCuve);
            }
            else if (SceneManager.GetActiveScene().name == "Tuto" || SceneManager.GetActiveScene().name == "Intro")
                UpdateAnimator(_brokenCuve);
        }

        private void UpdateAnimator(RuntimeAnimatorController newAnimator)
        {
            _animator.runtimeAnimatorController = newAnimator;
        }
    }
}
