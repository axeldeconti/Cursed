using Cursed.Managers;
using Cursed.Utilities;
using Cursed.VisualEffect;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cursed.Props
{
    public class AndroidCuveBehaviour : MonoBehaviour
    {
        [SerializeField] private RuntimeAnimatorController _normalCuve;
        [SerializeField] private RuntimeAnimatorController _brokenCuve;
        [SerializeField] private RuntimeAnimatorController _brokingCuve;
        [SerializeField] private VoidEvent _cuveBroken;

        [Header("Stats Vibration")]
        [SerializeField] private VibrationEvent _onContrVibration = null;
        [SerializeField] private VibrationData_SO _cuveBrokenVibration = null;

        [Header("Stats Camera Shake")]
        [SerializeField] private ShakeData _shakeCuveBroken = null;
        [SerializeField] private ShakeDataEvent _onCamShake = null;

        private Animator _animator;

        private ControlerManager _controlerManager;
        private bool _alreadyBroken = false;

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
            else if (_controlerManager._ControlerType == ControlerManager.ControlerType.PS4)
            {
                if (Input.GetButtonDown("Attack_1_PS4"))
                    CheckScene();
            }
        }

        public void CheckScene()
        {
            if (_alreadyBroken)
                return;

            if (SceneManager.GetActiveScene().name == "Main")
            {
                UpdateAnimator(_normalCuve);
                _alreadyBroken = false;
            }
            else if (SceneManager.GetActiveScene().name == "Tuto" || SceneManager.GetActiveScene().name == "Intro")
            {
                _alreadyBroken = true;
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

            // VIBRATIONS & CAM SHAKE
            _onContrVibration?.Raise(_cuveBrokenVibration);
            _onCamShake?.Raise(_shakeCuveBroken);

            _cuveBroken?.Raise();
            AkSoundEngine.PostEvent("Play_Cuve_Break", gameObject);
        }
    }
}
