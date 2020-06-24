using Cinemachine;
using Cursed.Managers;
using UnityEngine;
using System.Collections;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] private CinemachineBrain _cameraBrain;
    [SerializeField] private float _resetDelay = 2f;
    private CinemachineVirtualCamera _camera;

    private void Awake()
    {
        _camera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        EndTeleportersManager.Instance._onTeleportSpawn += ChangeTarget;
    }

    private void ChangeTarget(Transform target)
    {
        _camera.m_Follow = target;
        _camera.m_LookAt = target;
        _camera.m_Priority = 100;
        StartCoroutine(WaitForResetPriority(_cameraBrain.m_DefaultBlend.m_Time + _resetDelay));
    }

    private IEnumerator WaitForResetPriority(float delay)
    {
        yield return new WaitForSeconds(delay);
        _camera.m_Priority = 0;
    }
}
