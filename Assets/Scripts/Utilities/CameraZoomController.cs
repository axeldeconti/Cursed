using UnityEngine;
using Cinemachine;

namespace Cursed.Utilities
{
    public class CameraZoomController : MonoBehaviour
    {
        public static CameraZoomController Instance;

        private CinemachineVirtualCamera _camera;
        private float _initialZoom;
        [SerializeField] private float _maxZoom = 22f;
        [SerializeField] private float _zoomInSpeed = 1f;
        [SerializeField] private float _zoomOutSpeed = .5f;

        private void Awake()
        {
            Instance = this;
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        private void Start()
        {
            _initialZoom = _camera.m_Lens.OrthographicSize;
        }

        public void Zoom(bool reverse)
        {
            if (!reverse)
            {
                float currentSize = Mathf.Lerp(_camera.m_Lens.OrthographicSize, _maxZoom, _zoomOutSpeed * Time.deltaTime);
                _camera.m_Lens.OrthographicSize = currentSize;
            }
            else
            {
                float currentSize = Mathf.Lerp(_camera.m_Lens.OrthographicSize, _initialZoom, _zoomInSpeed * Time.deltaTime);
                _camera.m_Lens.OrthographicSize = currentSize;
            }
        }
    }
}
