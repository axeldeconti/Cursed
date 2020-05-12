using System.Collections;
using UnityEngine;
using Cinemachine;

namespace Cursed.Utilities
{
    public class CameraZoomController : MonoBehaviour
    {
        public static CameraZoomController Instance;

        private CinemachineVirtualCamera _camera;
        public float _initialZoom;

        [Space]
        public float _maxZoomCreature;
        public float _maxZoomKill;

        [Space]
        public float _zoomInCreatureSpeed;
        public float _zoomOutCreatureSpeed;
        public float _zoomInKillSpeed;
        public float _zoomOutKillSpeed;

        private void Awake()
        {
            Instance = this;
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        private void Start()
        {
            _initialZoom = _camera.m_Lens.FieldOfView;
        }

        public void Zoom(float targetFOV, float speedFOV)
        {
            float currentSize = Mathf.Lerp(_camera.m_Lens.FieldOfView, targetFOV, speedFOV * Time.deltaTime);
            _camera.m_Lens.FieldOfView = currentSize;
        }
    }
}
