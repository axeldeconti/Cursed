using Cursed.Character;
using System.Collections;
using UnityEngine;

public class CameraTargetManager : MonoBehaviour
{
    public float _noiseDuration = 2f;
    public float _timeBeforeSwitchTarget = 7f;

    [SerializeField] private VoidEvent _onEnemyDeath;

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
    }

    private void ChangeTarget()
    {
        _enemyChosen = _enemyList[Random.Range(0, _enemyList.Length)];
        _onChangeTarget?.Invoke();
        StartCoroutine(TimerForSwitchTarget());
    }

    IEnumerator TimerForSwitchTarget()
    {
        yield return new WaitForSeconds(_noiseDuration);
        yield return new WaitForSeconds(_timeBeforeSwitchTarget);
        ChangeTarget();
    }
}
