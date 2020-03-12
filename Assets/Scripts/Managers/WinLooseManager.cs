using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLooseManager : MonoBehaviour
{
    [SerializeField] private GameObject _winScreen = null;
    [SerializeField] private GameObject _looseScreen = null;

    private int _enemyCount = 0;

    private void Start()
    {
        _winScreen.SetActive(false);
        _looseScreen.SetActive(false);
    }

    public void AddEnemy() => _enemyCount++;

    public void OnEnemyDeath()
    {
        if(--_enemyCount <= 0)
        {
            GameManager.Instance.State = GameManager.GameState.Pause;
            _winScreen.SetActive(true);
        }
    }

    public void OnPlayerDeath()
    {
        GameManager.Instance.State = GameManager.GameState.Pause;
        _looseScreen.SetActive(true);
    }

    public void Return()
    {
        GameManager.Instance.LoadLevel("Main");
    }
}
