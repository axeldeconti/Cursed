using UnityEngine;
using Cinemachine;
using Cursed.Character;

namespace Cursed.Utilities
{
    public class CameraSpeComportement : MonoBehaviour
    {
        public static CameraSpeComportement Instance;

        private CinemachineVirtualCamera _camera;
        [SerializeField] private GameObject _refPlayer;
        [SerializeField] private float _initialLookahead;

        private void Awake()
        {
            Instance = this;
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        private void Update()
        {
            //Camera Center if player stay in place
            CameraCenter();
        }

        public void CameraCenter()
        {
            if (_refPlayer.GetComponent<CharacterMovement>().State == CharacterMovementState.Idle)
                _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_LookaheadTime = 0f;
            else
                _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_LookaheadTime = _initialLookahead;
        }
    }
}
