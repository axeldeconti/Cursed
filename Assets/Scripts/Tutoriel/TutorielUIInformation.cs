using UnityEngine;
using Cursed.Creature;

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
        [SerializeField] private GameObject _sonarTuto;
        [SerializeField] private GameObject _creatureDirectionTuto;
        [SerializeField] private GameObject _creatureLaunchTuto;
        [SerializeField] private GameObject _creatureRecallTuto;

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
                        HideTuto();
                        return;
                    }
                    break;

                case TutorielType.Jump:
                    if (_tutorielBox.PlayerMovement.IsJumping)
                    {
                        HideTuto();
                        return;
                    }
                    break;

                case TutorielType.DoubleJump:
                    if (_tutorielBox.PlayerMovement.IsDoubleJumping)
                    {
                        HideTuto();
                        return;
                    }
                    break;

                case TutorielType.WallRun:
                    if (_tutorielBox.PlayerMovement.IsWallRun)
                    {
                        HideTuto();
                        return;
                    }
                    break;

                case TutorielType.Dash:
                    if (_tutorielBox.PlayerMovement.IsDashing)
                    {
                        HideTuto();
                        return;
                    }
                    break;


                case TutorielType.Sonar:
                    if (FindObjectOfType<CreatureInputController>().Sonar)
                    {
                        HideTuto();
                        return;
                    }
                    break;

                case TutorielType.CreatureDirection:
                    if (FindObjectOfType<CreatureJoystickDirection>().Direction != Vector3.zero)
                    {
                        HideTuto();
                        return;
                    }
                    break;

                case TutorielType.CreatureLaunch:
                    if (FindObjectOfType<CreatureInputController>().Down)
                    {
                        HideTuto();
                        return;
                    }
                    break;

                case TutorielType.CreatureRecall:
                    if (FindObjectOfType<CreatureInputController>().Down)
                    {
                        HideTuto();
                        return;
                    }
                    break;
            }
        }

        private void ShowTutorielInformation(TutorielType type)
        {
            switch (type)
            {
                case TutorielType.Move:
                    ShowTuto(_moveTuto);
                    break;

                case TutorielType.Jump:
                    ShowTuto(_jumpTuto);
                    break;

                case TutorielType.DoubleJump:
                    ShowTuto(_doubleJumpTuto);
                    break;

                case TutorielType.WallRun:
                    ShowTuto(_wallRunTuto);
                    break;

                case TutorielType.Dash:
                    ShowTuto(_dashTuto);
                    break;

                case TutorielType.Attack:
                    ShowTuto(_attackTuto);
                    break;

                case TutorielType.CreatureDirection:
                    ShowTuto(_creatureDirectionTuto);
                    break;

                case TutorielType.CreatureLaunch:
                    ShowTuto(_creatureLaunchTuto);
                    break;

                case TutorielType.CreatureRecall:
                    ShowTuto(_creatureRecallTuto);
                    break;

                case TutorielType.Sonar:
                    ShowTuto(_sonarTuto);
                    break;
            }
        }

        private void ShowTuto(GameObject tutoObject)
        {
            _tutoChild = Instantiate(tutoObject, transform.position, Quaternion.identity, transform);
            _tutoChild.GetComponent<Animator>().SetBool("Open", true);
        }

        private void HideTuto()
        {
            Animator animator = _tutoChild.GetComponent<Animator>();
            animator.SetBool("Close", true);
            animator.SetBool("Open", false);
            Destroy(_tutoChild, animator.GetCurrentAnimatorClipInfo(0).Length);
        }
    }

}
