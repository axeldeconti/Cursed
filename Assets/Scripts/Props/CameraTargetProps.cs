using UnityEngine;
using System.Collections;
using Cursed.Character;
using Cursed.Managers;

public class CameraTargetProps : MonoBehaviour
{
    [SerializeField] private bool _targetIsEnemy;
    private Cinemachine.CinemachineVirtualCamera _cinemachineVCComponent;

    private SwitchCellInfo _switchCellInfo;

    private CameraTargetManager _cameraManager;
    private GameObject _player;
    [SerializeField] private GameObject _screenGO;

    private MeshRenderer _screenRenderer;

    [SerializeField] private Material _noiseMat;
    [SerializeField] private Material _screenMat;


    private void Awake()
    {
        _cameraManager = GameObject.FindObjectOfType<CameraTargetManager>();
        _cinemachineVCComponent = this.GetComponent<Cinemachine.CinemachineVirtualCamera>();
        _switchCellInfo = this.GetComponent<SwitchCellInfo>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _screenRenderer = _screenGO.GetComponent<MeshRenderer>();        

        _cameraManager._onChangeTarget += () => ChangeTarget();
    }

    private void ChangeTarget()
    {
        if (_targetIsEnemy)
        {
            //Faire en sorte que la cam ne choisisse pas un ennemi déjà choisi
            //Faire en sorte (suite au com précédent) que s'il reste un ennemi dans la liste il soit tout de même choisi
            _screenRenderer.material = _noiseMat;
            _cinemachineVCComponent.LookAt = _cameraManager._enemyChosen.transform;
            _cinemachineVCComponent.Follow = _cameraManager._enemyChosen.transform;
            StartCoroutine(TimerForSwitchTarget());
        }
        else
        {
            _cinemachineVCComponent.LookAt = _player.transform;
            _cinemachineVCComponent.Follow = _player.transform;
            StartCoroutine(TimerForSwitchTarget());
        }
    }

    private IEnumerator TimerForSwitchTarget()
    {
        yield return new WaitForSeconds(_cameraManager._noiseDuration);
        _switchCellInfo.UpdateCellInformation(_cameraManager._enemyChosen.GetComponent<EnemyRegister>()._currentCell.cellNumberInfo);
        _screenRenderer.material = _screenMat;
        yield return new WaitForSeconds(_cameraManager._timeBeforeSwitchTarget);
        ChangeTarget();
    }
}
