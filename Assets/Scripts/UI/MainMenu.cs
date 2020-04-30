using UnityEngine;
using System.Collections;

namespace Cursed.UI
{
    public class MainMenu : MonoBehaviour
    {
        private GameManager _gameManager = null;

        [SerializeField] private GameObject _mainMenu = null;
        [SerializeField] private GameObject _controls = null;
        [SerializeField] private GameObject _credits = null;
        [SerializeField] private Animator _mainMenuAnimator;
        [SerializeField] private Animator _creditsAnimator;
        [SerializeField] private Animator _controlsAnimator;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _mainMenu.SetActive(true);
            _controls.SetActive(false);
            _credits.SetActive(false);
        }

        public void Play()
        {
            _mainMenuAnimator.SetTrigger("Close");
            StartCoroutine(WaitForActive(_mainMenu, false, _mainMenuAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitBeforeLoad(_mainMenuAnimator.GetCurrentAnimatorClipInfo(0).Length));
        }

        public void CreditsToHome()
        {
            _creditsAnimator.SetTrigger("Close");
            StartCoroutine(WaitForActive(_mainMenu, true, _creditsAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitForActive(_credits, false, _creditsAnimator.GetCurrentAnimatorClipInfo(0).Length));
        }

        public void ControlsToHome()
        {
            _controlsAnimator.SetTrigger("Close");
            StartCoroutine(WaitForActive(_mainMenu, true, _controlsAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitForActive(_controls, false, _controlsAnimator.GetCurrentAnimatorClipInfo(0).Length));
        }

        public void Credits()
        {
            _mainMenuAnimator.SetTrigger("Close");
            StartCoroutine(WaitForActive(_mainMenu, false, _mainMenuAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitForActive(_credits, true, _mainMenuAnimator.GetCurrentAnimatorClipInfo(0).Length));
        }

        public void Controls()
        {
            _mainMenuAnimator.SetTrigger("Close");
            StartCoroutine(WaitForActive(_mainMenu, false, _mainMenuAnimator.GetCurrentAnimatorClipInfo(0).Length));
            StartCoroutine(WaitForActive(_controls, true, _mainMenuAnimator.GetCurrentAnimatorClipInfo(0).Length));
        }

        public void Quit()
        {
            _gameManager.QuitGame();
        }

        IEnumerator WaitBeforeLoad(float delay)
        {
            yield return new WaitForSeconds(delay);
            _gameManager.LoadLevel("Scene_Proto_Game");
        }

        IEnumerator WaitForActive(GameObject go, bool active, float delay)
        {
            yield return new WaitForSeconds(delay);
            go.SetActive(active);
        }
    }
}