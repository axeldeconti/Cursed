using UnityEngine;
using System.Collections;
using Cursed.Managers;

namespace Cursed.UI
{
    public class MainMenu : MonoBehaviour
    {
        private GameManager _gameManager = null;

        [SerializeField] private GameObject _mainMenu = null;
        [SerializeField] private GameObject _options = null;
        [SerializeField] private GameObject _credits = null;
        [SerializeField] private GameObject _tuto = null;
        [SerializeField] private Animator _mainMenuAnimator;
        [SerializeField] private Animator _creditsAnimator;
        [SerializeField] private Animator _optionsAnimator;
        [SerializeField] private Animator _tutoAnimator;

        [SerializeField] private string Level_Tuto;
        [SerializeField] private string Level_Intro;

        [SerializeField] private GameObject _mainCameraMenu;
        [SerializeField] private GameObject _virtualCameraMenu;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _mainMenu.SetActive(true);
            _options.SetActive(false);
            _credits.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                if(_options.activeSelf)
                    OptionsToHome();
                if (_credits.activeSelf)
                    CreditsToHome();
            }
        }

        public void Play()
        {
            _mainMenuAnimator.SetTrigger("Close");
            StartCoroutine(WaitForActive(_mainMenu, false, _mainMenuAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitForActive(_tuto, true, _mainMenuAnimator.GetCurrentAnimatorClipInfo(0).Length));
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
    }
}