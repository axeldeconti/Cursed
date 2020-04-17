using UnityEngine;
using System.Collections;

public class CameraTargetProps : MonoBehaviour
{
    [SerializeField] private bool _targetIsEnemy;
    private Cinemachine.CinemachineVirtualCamera _cinemachineVCComponent;

    private SwitchCellInfo _switchCellInfo;

    private GameObject[] _enemyList;
    private GameObject _enemyChosen;
    private GameObject _player;
    [SerializeField] private GameObject _screenGO;

    private MeshRenderer _screenRenderer;

    [SerializeField] private Material _noiseMat;
    [SerializeField] private Material _screenMat;

    [SerializeField] private float _timeBeforeSwitchTarget;
    [SerializeField] private float _noiseDuration;

    private void Awake()
    {
        _cinemachineVCComponent = this.GetComponent<Cinemachine.CinemachineVirtualCamera>();
        _switchCellInfo = this.GetComponent<SwitchCellInfo>();
        _enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        _player = GameObject.FindGameObjectWithTag("Player");
        _screenRenderer = _screenGO.GetComponent<MeshRenderer>();        
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeTarget();
    }

    private void ChangeTarget()
    {
        if (_targetIsEnemy)
        {
            //Faire en sorte que la cam ne choisisse pas un ennemi déjà choisi
            //Faire en sorte (suite au com précédent) que s'il reste un ennemi dans la liste il soit tout de même choisi
            _screenRenderer.material = _noiseMat;
            _enemyChosen = _enemyList[Random.Range(0, _enemyList.Length)].gameObject;
            _cinemachineVCComponent.LookAt = _enemyChosen.transform;
            _cinemachineVCComponent.Follow = _enemyChosen.transform;
            StartCoroutine(TimerForSwitchTarget());
        }
        else
        {
            _cinemachineVCComponent.LookAt = _player.transform;
            _cinemachineVCComponent.Follow = _player.transform;
            StartCoroutine(TimerForSwitchTarget());
        }
    }

    IEnumerator TimerForSwitchTarget()
    {
        yield return new WaitForSeconds(_noiseDuration);
        _switchCellInfo.UpdateCellInformation();
        _screenRenderer.material = _screenMat;
        yield return new WaitForSeconds(_timeBeforeSwitchTarget);
        ChangeTarget();
    }
}
