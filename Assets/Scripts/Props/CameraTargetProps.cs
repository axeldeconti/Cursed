using UnityEngine;

public class CameraTargetProps : MonoBehaviour
{
    [SerializeField] private bool targetIsEnemy;
    private Cinemachine.CinemachineVirtualCamera _cinemachineVCComponent;

    private GameObject[] _enemyList;
    private GameObject _enemyChosen;
    private GameObject _player;

    private void Awake()
    {
        _cinemachineVCComponent = this.GetComponent<Cinemachine.CinemachineVirtualCamera>();
        _enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (targetIsEnemy)
        {
            _enemyChosen = _enemyList[Random.Range(0, _enemyList.Length)].gameObject;
            _cinemachineVCComponent.LookAt = _enemyChosen.transform;
            _cinemachineVCComponent.Follow = _enemyChosen.transform;
        }
        else
        {
            _cinemachineVCComponent.LookAt = _player.transform;
            _cinemachineVCComponent.Follow = _player.transform;
        }
    }
}
