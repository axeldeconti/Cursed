using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cursed.Tutoriel
{
    public class TutorielUIInformation : MonoBehaviour
    {
        [Header("Tuto objects")]
        [SerializeField] private GameObject _moveTuto;
        [SerializeField] private GameObject _jumpTuto;
        [SerializeField] private GameObject _doubleJumpTuto;
        [SerializeField] private GameObject _wallRunTuto;
        [SerializeField] private GameObject _dashTuto;
        [SerializeField] private GameObject _attackTuto;

        private GameObject _tutoChild;
        private TutorielBox _tutorielBox;
        private TutorielType _tutorielType;

        private void Awake()
        {
            _tutorielBox = GetComponentInParent<TutorielBox>();
            _tutorielType = _tutorielBox.TypeOfTutoriel;
        }

        private void Start()
        {
            _tutorielBox.SpellUnlock += ShowTutorielInformation;
        }

        private void Update()
        {
            UpdateTutoInformation();
        }

        private void UpdateTutoInformation()
        {
            if (_tutorielBox.PlayerMovement == null || _tutorielBox.PlayerAttacks == null)
                return;

            switch (_tutorielType)
            {
                case TutorielType.Move:
                    if (_tutorielBox.PlayerMovement.XSpeed != 0)
                    {
                        Destroy(_tutoChild);
                        return;
                    }
                    break;

                case TutorielType.Jump:
                    if (_tutorielBox.PlayerMovement.IsJumping)
                    {
                        Destroy(_tutoChild);
                        return;
                    }
                    break;

                case TutorielType.DoubleJump:
                    if (_tutorielBox.PlayerMovement.IsDoubleJumping)
                    {
                        Destroy(_tutoChild);
                        return;
                    }
                    break;

                case TutorielType.WallRun:
                    if (_tutorielBox.PlayerMovement.IsWallRun)
                    {
                        Destroy(_tutoChild);
                        return;
                    }
                    break;

                case TutorielType.Dash:
                    if (_tutorielBox.PlayerMovement.IsDashing)
                    {
                        Destroy(_tutoChild);
                        return;
                    }
                    break;

                case TutorielType.Attack:
                    if (_tutorielBox.PlayerAttacks.IsAttacking)
                    {
                        Destroy(_tutoChild);
                        return;
                    }
                    break;
            }
        }

        private void ShowTutorielInformation(TutorielType type)
        {
            switch(type)
            {
                case TutorielType.Move:
                    _tutoChild = Instantiate(_moveTuto, transform.position, Quaternion.identity, transform);
                    break;

                case TutorielType.Jump:
                    _tutoChild = Instantiate(_jumpTuto, transform.position, Quaternion.identity, transform);
                    break;

                case TutorielType.DoubleJump:
                    _tutoChild = Instantiate(_doubleJumpTuto, transform.position, Quaternion.identity, transform);
                    break;

                case TutorielType.WallRun:
                    _tutoChild = Instantiate(_wallRunTuto, transform.position, Quaternion.identity, transform);
                    break;

                case TutorielType.Dash:
                    _tutoChild = Instantiate(_dashTuto, transform.position, Quaternion.identity, transform);
                    break;

                case TutorielType.Attack:
                    _tutoChild = Instantiate(_attackTuto, transform.position, Quaternion.identity, transform);
                    break;

            }
        }
    }

}
