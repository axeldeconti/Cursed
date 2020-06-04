using UnityEngine;
using Cursed.Props;
using UnityEngine.SceneManagement;

namespace Cursed.Managers
{
    public class WinLooseManager : MonoBehaviour
    {
        private GameManager _gameManager = null;

        [SerializeField] private GameObject _winScreen = null;
        [SerializeField] private GameObject _looseScreen = null;
        [SerializeField] private VoidEvent _retryEvent = null;

        private int _enemyCount = 0;
        private bool _retryLaunch = false;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _winScreen.SetActive(false);
            _looseScreen.SetActive(false);
        }

        public void AddEnemy() => _enemyCount++;

        public void OnEnemyDeath()
        {
            if (_gameManager.CurrentLevelName == "Tuto" || _gameManager.CurrentLevelName == "Intro")
                return;

            if (--_enemyCount <= 0)
            {
                GameManager.Instance.State = GameManager.GameState.WinLoose;
                _winScreen.SetActive(true);
            }
        }

        public void OnPlayerDeath()
        {
            GameManager.Instance.State = GameManager.GameState.WinLoose;
            _looseScreen.SetActive(true);
            UpdateEnemyCountLose();
        }

        private void UpdateEnemyCountLose()
        {
            foreach (EnemyCountText text in GetComponentsInChildren<EnemyCountText>())
                text.UpdateText();
        }

        public void Return()
        {
            if (_gameManager.CurrentLevelName == "Tuto" || _gameManager.CurrentLevelName == "Intro")
                _gameManager.UnloadLevel("Main");

            GameManager.Instance.LoadLevel("Main", true);
        }

        public void Retry()
        {
            if (_gameManager.CurrentLevelName == "Tuto" || _gameManager.CurrentLevelName == "Intro")
                _gameManager.UnloadLevel("Main");

            GameManager.Instance.LoadLevel("Main", true);
            _retryLaunch = true;
        }

        public void RetryEventPush()
        {
            if (_retryLaunch)
                _retryEvent?.Raise();
        }
    }
}