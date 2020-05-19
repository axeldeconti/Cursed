using UnityEngine;
using Cinemachine;
using System.Collections;

namespace Cursed.Utilities
{
    public class CameraZoomController : MonoBehaviour
    {
        public static CameraZoomController Instance;

        public CinemachineVirtualCamera _camera { get; private set; }
        public float _initialZoom;

        [Space]
        public float _maxZoomCreature;
        public float _maxZoomKill;

        [Space]
        public float _zoomInCreatureSpeed;
        public float _zoomOutCreatureSpeed;
        public float _zoomInKillSpeed;
        public float _zoomOutKillSpeed;

        public bool _updateZoom { get; private set; } = false;
        private float _targetFOV;
        private float _speedFOV;

        private void Awake()
        {
            Instance = this;
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        private void Start()
        {
            _initialZoom = 115f;
        }

        private void Update()
        {
            if(_updateZoom)
            {
                float currentSize = Mathf.Lerp(_camera.m_Lens.FieldOfView, _targetFOV, _speedFOV * Time.deltaTime);
                _camera.m_Lens.FieldOfView = currentSize;

                if (_camera.m_Lens.FieldOfView - _targetFOV < .1f && _camera.m_Lens.FieldOfView - _targetFOV > 0f)
                    _updateZoom = false;
            }
        }

        public void Zoom(float targetFOV, float speedFOV, bool update)
        {
            _updateZoom = update;
            if (!update)
            {
                float currentSize = Mathf.Lerp(_camera.m_Lens.FieldOfView, targetFOV, speedFOV * Time.deltaTime);
                _camera.m_Lens.FieldOfView = currentSize;
            }
            else
            {
                _targetFOV = targetFOV;
                _speedFOV = speedFOV;
            }
        }

        public void CallWaitForZoom(float delay, float targetFOV, float speedFOV, bool update)
        {
            StartCoroutine(WaitForZoom(delay, targetFOV, speedFOV, update));
        }

        private IEnumerator WaitForZoom(float delay, float targetFOV, float speedFOV, bool update)
        {
            yield return new WaitForSecondsRealtime(delay);
            Zoom(targetFOV, speedFOV, update);
        }
    }
}
