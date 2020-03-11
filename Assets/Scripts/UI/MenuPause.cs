using UnityEngine;

namespace Cursed.UI
{
    public class MenuPause : MonoBehaviour
    {
        private GameManager _gameManager = null;

        [SerializeField] private GameObject _pauseMenu = null;
        [SerializeField] private GameObject _controlsMenu = null;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _pauseMenu.SetActive(false);
            _controlsMenu.SetActive(false);
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
                    break;
                case GameManager.GameState.Pause:
                    _gameManager.State = GameManager.GameState.InGame;
                    _pauseMenu.SetActive(false);
                    break;
                case GameManager.GameState.InDevConsole:
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