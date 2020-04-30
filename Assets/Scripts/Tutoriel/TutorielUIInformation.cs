using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cursed.Tutoriel
{
    public class TutorielUIInformation : MonoBehaviour
    {
        [SerializeField] private TMP_Text _tutorielText;
        private TutorielBox _tutorielBox;
        private TutorielType _tutorielType;

        private void Awake()
        {
            _tutorielBox = GetComponentInParent<TutorielBox>();
            _tutorielType = _tutorielBox.TypeOfTutoriel;
        }

        private void Start()
        {
            _tutorielText.text = "";

            _tutorielBox.SpellUnlock += ShowTutorielInformation;
        }

        private void Update()
        {
            UpdateTextInformations();
        }

        private void UpdateTextInformations()
        {
            if (_tutorielBox.PlayerMovement == null || _tutorielBox.PlayerAttacks == null)
                return;

            switch (_tutorielType)
            {
                case TutorielType.Move:
                    if (_tutorielBox.PlayerMovement.XSpeed != 0)
                    {
                        _tutorielText.text = "";
                        return;
                    }
                    break;

                case TutorielType.Jump:
                    if (_tutorielBox.PlayerMovement.IsJumping)
                    {
                        _tutorielText.text = "";
                        return;
                    }
                    break;

                case TutorielType.DoubleJump:
                    if (_tutorielBox.PlayerMovement.IsDoubleJumping)
                    {
                        _tutorielText.text = "";
                        return;
                    }
                    break;

                case TutorielType.WallRun:
                    if (_tutorielBox.PlayerMovement.IsWallRun)
                    {
                        _tutorielText.text = "";
                        return;
                    }
                    break;

                case TutorielType.Dash:
                    if (_tutorielBox.PlayerMovement.IsDashing)
                    {
                        _tutorielText.text = "";
                        return;
                    }
                    break;

                case TutorielType.Attack:
                    if (_tutorielBox.PlayerAttacks.IsAttacking)
                    {
                        _tutorielText.text = "";
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
                    _tutorielText.text = "Use Left Joystick to move";
                    break;

                case TutorielType.Jump:
                    _tutorielText.text = "Press A to jump";
                    break;

                case TutorielType.DoubleJump:
                    _tutorielText.text = "Press A in the air to double jump";
                    break;

                case TutorielType.WallRun:
                    _tutorielText.text = "Press Right Trigger to wall run close to a wall";
                    break;

                case TutorielType.Dash:
                    _tutorielText.text = "Press Right Trigger to dash on the ground and to dodge lasers";
                    break;

                case TutorielType.Attack:
                    _tutorielText.text = "Press X to attack with the first weapon";
                    break;

            }
        }
    }

}
