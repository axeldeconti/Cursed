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
        [Range(0, 1)] public float _initialLookahead;
        [Range(0, 1)] public float _attackLookahead;

        private void Awake()
        {
            Instance = this;
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        private void Update()
        {
            //Camera center if character didn't move and attack
            if (_refPlayer.GetComponent<CharacterMovement>().State == CharacterMovementState.Idle && _refPlayer.GetComponent<CharacterAttackManager>().IsAttacking == false)
                _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_LookaheadTime = 0f;
            //Camera move if character attack
            else if (_refPlayer.GetComponent<CharacterAttackManager>().IsAttacking == true && _refPlayer.GetComponent<CharacterMovement>().XSpeed == 0)
                _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_LookaheadTime = _attackLookahead;
            //Camera initial lookahead if character move
            else
                _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_LookaheadTime = _initialLookahead;
        }
    }
}
