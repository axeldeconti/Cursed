using UnityEngine;
using Cinemachine;

namespace Cursed.Utilities
{
    public class CameraAnimatorController : MonoBehaviour
    {
        [SerializeField] private Transform _playerTransform;
        private Animator _animator;
        private CinemachineVirtualCamera _camera;
        private bool _goToPlayer;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        private void Update()
        {
            if (_goToPlayer)
                MoveToPlayer();
        }

        private void MoveToPlayer()
        {
            transform.position = Vector2.MoveTowards(transform.position, _playerTransform.position, 5f * Time.deltaTime);
        }

        public void IntroDone()
        {
            _goToPlayer = true;
            _animator.SetTrigger("Intro");
        }

        public void SetTarget()
        {
            _playerTransform.gameObject.SetActive(true);
            _camera.m_Follow = _playerTransform;
            _camera.m_LookAt = _playerTransform;
        }
    }
}
