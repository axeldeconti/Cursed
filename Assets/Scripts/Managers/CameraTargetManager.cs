using Cursed.Character;
using System.Collections;
using UnityEngine;

public class CameraTargetManager : MonoBehaviour
{
    public float _noiseDuration = 2f;
    public float _timeBeforeSwitchTarget = 7f;

    private EnemyRegister[] _enemyList;
    public EnemyRegister _enemyChosen { get; private set; }
    public event System.Action _onChangeTarget;

    private void Awake()
    {
        GetEnemyList();
    }

    private void Start()
    {
        ChangeTarget();
    }

    public void GetEnemyList()
    {
       _enemyList = FindObjectsOfType<EnemyRegister>();
        Debug.Log("Enemy list update");
    }

    private void ChangeTarget()
    {
        _enemyChosen = _enemyList[Random.Range(0, _enemyList.Length)];
        CheckEnemyChosen();

        _onChangeTarget?.Invoke();
        StartCoroutine(TimerForSwitchTarget());
    }

    private void CheckEnemyChosen()
    {
        while (_enemyChosen == null)
            _enemyChosen = _enemyList[Random.Range(0, _enemyList.Length)];
    }

    IEnumerator TimerForSwitchTarget()
    {
        yield return new WaitForSeconds(_noiseDuration);
        yield return new WaitForSeconds(_timeBeforeSwitchTarget);
        ChangeTarget();
    }
}
