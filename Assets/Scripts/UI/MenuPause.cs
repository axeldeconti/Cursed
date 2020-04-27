using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Cursed.UI
{
    public class MenuPause : MonoBehaviour
    {
        private GameManager _gameManager = null;

        [Header("Referencies")]
        [SerializeField] private GameObject _pauseMenu = null;
        [SerializeField] private GameObject _controlsMenu = null;
        [SerializeField] private GameObject _inGameMenu = null;

        [Header("Post Process")]
        [SerializeField] private Volume _globalVolume = null;
        private DepthOfField _depthOfField;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _pauseMenu.SetActive(false);
            _controlsMenu.SetActive(false);

            // SET BLUR EFFECT 
            DepthOfField depthOfField;
            if (_globalVolume.profile.TryGet<DepthOfField>(out depthOfField))
                _depthOfField = depthOfField;
        }

        private void Update()
        {
            if (Input.GetButtonDown("Pause"))
                TooglePause();
        }

        public void TooglePause()
        {
            switch (_gameManager.State)
            {
                case GameManager.GameState.InGame:
                    _gameManager.State = GameManager.GameState.Pause;
                    _pauseMenu.SetActive(true);
                    _controlsMenu.SetActive(false);
                    _inGameMenu.SetActive(false);
                    _depthOfField.mode.value = DepthOfFieldMode.Gaussian;
                    ButtonHandler.isFirstSelected = true;
                    break;
                case GameManager.GameState.Pause:
                    _gameManager.State = GameManager.GameState.InGame;
                    _pauseMenu.SetActive(false);
                    _controlsMenu.SetActive(false);
                    _inGameMenu.SetActive(true);
                    _depthOfField.mode.value = DepthOfFieldMode.Off;
                    break;
                case GameManager.GameState.InDevConsole:
                    break;
                case GameManager.GameState.WinLoose:
                    break;
                default:
                    break;
            }
        }

        public void Quit()
        {
            _gameManager.LoadLevel("Main");
        }
    }
}