using UnityEngine;
using System.Collections;
using Cursed.Managers;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace Cursed.UI
{
    public class MainMenu : MonoBehaviour
    {
        private GameManager _gameManager = null;

        [Header("Menu Objects")]
        [SerializeField] private GameObject _splashScreen = null;
        [SerializeField] private GameObject _mainMenu = null;
        [SerializeField] private GameObject _options = null;
        [SerializeField] private GameObject _credits = null;
        [SerializeField] private GameObject _tuto = null;
        [SerializeField] private GameObject _controls = null;

        [Header("Menu Animators")]
        [SerializeField] private Animator _splashScreenAnimator;
        [SerializeField] private Animator _mainMenuAnimator;
        [SerializeField] private Animator _creditsAnimator;
        [SerializeField] private Animator _optionsAnimator;
        [SerializeField] private Animator _tutoAnimator;
        [SerializeField] private Animator _controlsAnimator;

        [Header("Scenes Name")]
        [SerializeField] private string Level_Tuto;
        [SerializeField] private string Level_Intro;

        [Header("Cameras")]
        [SerializeField] private GameObject _mainCameraMenu;
        [SerializeField] private GameObject _virtualCameraMenu;
        private Animator _cameraAnimator;


        [Header("Post Process")]
        [SerializeField] private Volume _globalVolume = null;
        private DepthOfField _depthOfField;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _cameraAnimator = _virtualCameraMenu.GetComponent<Animator>();
            _splashScreen.SetActive(true);
            _mainMenu.SetActive(false);
            _options.SetActive(false);
            _credits.SetActive(false);
            _controls.SetActive(false);

            // SET BLUR EFFECT 
            if (_globalVolume != null)
            {
                DepthOfField depthOfField;
                if (_globalVolume.profile.TryGet<DepthOfField>(out depthOfField))
                    _depthOfField = depthOfField;
            }
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                if(_options.activeSelf)
                    OptionsToHome();
                if (_credits.activeSelf)
                    CreditsToHome();
                if (_controls.activeSelf)
                    ControlsToOption();
                if (_tuto.activeSelf)
                    TutoToHome();
            }

            if(Input.GetButtonDown("Pause"))
            {
                if (_splashScreen.activeSelf)
                    SkipSplashScreen();
            }
        }

        public void SkipSplashScreen()
        {
            _splashScreenAnimator.SetTrigger("Pressed");
            StartCoroutine(WaitForActive(_mainMenu, true, _splashScreenAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitForActive(_splashScreen, false, _splashScreenAnimator.GetCurrentAnimatorClipInfo(0).Length));
            _depthOfField.mode.value = DepthOfFieldMode.Off;
        }

        public void Play()
        {
            _mainMenuAnimator.SetTrigger("Close");
            StartCoroutine(WaitForActive(_mainMenu, false, _mainMenuAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitForActive(_tuto, true, _mainMenuAnimator.GetCurrentAnimatorClipInfo(0).Length));
        }

        public void Retry()
        {
            StartCoroutine(WaitForActive(_mainMenu, false, .05f));
            StartCoroutine(WaitBeforeLoad(0f, Level_Intro, false));
        }

        public void Tuto()
        {
            _tutoAnimator.SetTrigger("Close");
            StartCoroutine(WaitForActive(_tuto, false, _tutoAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitBeforeLoad(_tutoAnimator.GetCurrentAnimatorClipInfo(0).Length, Level_Tuto, false));
        }

        public void Intro()
        {
            _tutoAnimator.SetTrigger("Close");
            StartCoroutine(WaitForActive(_tuto, false, _tutoAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitBeforeLoad(_tutoAnimator.GetCurrentAnimatorClipInfo(0).Length, Level_Intro, false));
        }

        public void CreditsToHome()
        {
            _creditsAnimator.SetTrigger("Close");
            StartCoroutine(WaitForActive(_mainMenu, true, _creditsAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitForActive(_credits, false, _creditsAnimator.GetCurrentAnimatorClipInfo(0).Length));
        }

        public void OptionsToHome()
        {
            _optionsAnimator.SetTrigger("Close");
            StartCoroutine(WaitForActive(_mainMenu, true, _optionsAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitForActive(_options, false, _optionsAnimator.GetCurrentAnimatorClipInfo(0).Length));
        }

        public void TutoToHome()
        {
            _tutoAnimator.SetTrigger("Close");
            StartCoroutine(WaitForActive(_mainMenu, true, _tutoAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitForActive(_tuto, false, _tutoAnimator.GetCurrentAnimatorClipInfo(0).Length));
        }

        public void Credits()
        {
            _mainMenuAnimator.SetTrigger("Close");
            StartCoroutine(WaitForActive(_mainMenu, false, _mainMenuAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitForActive(_credits, true, _mainMenuAnimator.GetCurrentAnimatorClipInfo(0).Length));
        }

        public void Options()
        {
            _mainMenuAnimator.SetTrigger("Close");
            StartCoroutine(WaitForActive(_mainMenu, false, _mainMenuAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitForActive(_options, true, _mainMenuAnimator.GetCurrentAnimatorClipInfo(0).Length));
        }

        public void OptionsToControls()
        {
            _optionsAnimator.SetTrigger("Close");
            StartCoroutine(WaitForActive(_options, false, _optionsAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitForLaunchAnimation(_cameraAnimator, "Controls", _optionsAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitForActive(_controls, true, _cameraAnimator.GetCurrentAnimatorClipInfo(0).Length));
        }

        public void ControlsToOption()
        {
            _controlsAnimator.SetTrigger("Close");
            StartCoroutine(WaitForActive(_controls, false, _controlsAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitForLaunchAnimation(_cameraAnimator, "Idle", _controlsAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitForActive(_options, true, _cameraAnimator.GetCurrentAnimatorClipInfo(0).Length));
        }

        public void Quit()
        {
            _gameManager.QuitGame();
        }

        IEnumerator WaitBeforeLoad(float delay, string sceneName, bool unloadAll)
        {
            yield return new WaitForSeconds(delay);
            _gameManager.LoadLevel(sceneName, unloadAll);
            Destroy(_mainCameraMenu);
            Destroy(_virtualCameraMenu);
        }

        IEnumerator WaitForActive(GameObject go, bool active, float delay)
        {
            yield return new WaitForSeconds(delay);
            go.SetActive(active);
        }

        IEnumerator WaitForLaunchAnimation(Animator animator, string trigger, float delay)
        {
            yield return new WaitForSeconds(delay);
            animator.SetTrigger(trigger);
        }
    }
}