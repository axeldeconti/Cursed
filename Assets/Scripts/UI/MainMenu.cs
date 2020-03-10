using UnityEngine;

namespace Cursed.UI
{
    public class MainMenu : MonoBehaviour
    {
        private GameManager _gameManager = null;

        [SerializeField] private GameObject _mainMenu = null;

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        public void Play()
        {
            _gameManager.LoadLevel("Scene_Proto_Game");
        }

        public void Quit()
        {
            _gameManager.QuitGame();
        }
    }
}