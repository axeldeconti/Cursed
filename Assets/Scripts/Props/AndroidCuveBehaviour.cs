using UnityEngine;
using UnityEngine.SceneManagement;
using Cursed.Managers;
using System.Collections;

namespace Cursed.Props
{
    public class AndroidCuveBehaviour : MonoBehaviour
    {
        [SerializeField] private RuntimeAnimatorController _normalCuve;
        [SerializeField] private RuntimeAnimatorController _brokenCuve;
        [SerializeField] private RuntimeAnimatorController _brokingCuve;
        [SerializeField] private VoidEvent _cuveBroken;

        private Animator _animator;

        private ControlerManager _controlerManager;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _controlerManager = ControlerManager.Instance;
        }

        private void Update()
        {
            if (_controlerManager._ControlerType == ControlerManager.ControlerType.XBOX || _controlerManager._ControlerType == ControlerManager.ControlerType.None)
            {
                if (Input.GetButtonDown("Attack_1"))
                    CheckScene();
            }
            else if(_controlerManager._ControlerType == ControlerManager.ControlerType.PS4)
            {
                if (Input.GetButtonDown("Attack_1_PS4"))
                    CheckScene();
            }
        }

        public void CheckScene()
        {
            if (SceneManager.GetActiveScene().name == "Main")
            {
                UpdateAnimator(_normalCuve);
            }
            else if (SceneManager.GetActiveScene().name == "Tuto" || SceneManager.GetActiveScene().name == "Intro")
            {
                UpdateAnimator(_brokingCuve);
                StartCoroutine(WaitForLaunchBrokenEffet(.25f));
            }
        }

        private void UpdateAnimator(RuntimeAnimatorController newAnimator)
        {
            _animator.runtimeAnimatorController = newAnimator;
        }

        private IEnumerator WaitForLaunchBrokenEffet(float delay)
        {
            yield return new WaitForSeconds(delay);
            UpdateAnimator(_brokenCuve);
            _cuveBroken?.Raise();
            AkSoundEngine.PostEvent("Play_Cuve_Break", gameObject);
        }
    }
}
