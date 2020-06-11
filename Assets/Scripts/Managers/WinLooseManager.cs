using UnityEngine;
using System.Collections;
using Cursed.Props;
using Cursed.VisualEffect;
using UnityEngine.SceneManagement;

namespace Cursed.Managers
{
    public class WinLooseManager : MonoBehaviour
    {
        private GameManager _gameManager = null;

        [Header("Objects")]
        [SerializeField] private GameObject _winScreen = null;
        [SerializeField] private GameObject _looseScreen = null;
        [SerializeField] private GameObject _blackScreen = null;

        [Header("Events")]
        [SerializeField] private VoidEvent _retryEvent = null;
        [SerializeField] private VoidEvent _allEnemiesKilled = null;

        [Header("Settings")]
        [SerializeField] private FloatReference _slowMotionDuration;

        private int _enemyCount = 0;
        private bool _retryLaunch = false;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _winScreen.SetActive(false);
            _looseScreen.SetActive(false);
            _blackScreen.SetActive(false);
        }

        public void AddEnemy() => _enemyCount++;

        public void OnEnemyDeath()
        {
            if (_gameManager.CurrentLevelName == "Tuto" || _gameManager.CurrentLevelName == "Intro")
                return;

            if (--_enemyCount <= 0)
                Win();
        }

        private void Win()
        {
            _allEnemiesKilled?.Raise();
            Debug.Log("Win !");
            /*GameManager.Instance.State = GameManager.GameState.WinLoose;

            if (_slowMotionDuration != null)
            {
                Debug.Log("Freeze");
                SlowMotion.Instance.Freeze(_slowMotionDuration);
            }

            StartCoroutine(WaitForActive(2f, _winScreen, true));*/
        }

        public void OnPlayerDeath() => Loose();

        private void Loose()
        {
            GameManager.Instance.State = GameManager.GameState.WinLoose;

            StartCoroutine(WaitForActive(.5f, _blackScreen, true));
            StartCoroutine(WaitForActive(1.5f, _looseScreen, true));
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

        private IEnumerator WaitForActive(float delay, GameObject go, bool active)
        {
            yield return new WaitForSeconds(delay);
            go.SetActive(active);
        }
    }
}