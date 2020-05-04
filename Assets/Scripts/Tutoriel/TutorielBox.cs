using UnityEngine;
using Cursed.Character;

public enum TutorielType
{
    Move,
    Jump,
    DoubleJump,
    WallRun,
    Dash,
    Attack
}
namespace Cursed.Tutoriel
{

    public class TutorielBox : MonoBehaviour
    {
        [SerializeField] private TutorielType _tutorielType;

        private CharacterMovement _playerMovement;
        private CharacterAttackManager _playerAttacks;

        private bool _alreadyTriggered;
        public event System.Action<TutorielType> SpellUnlock;

        private void UnlockSpell(TutorielType type)
        {
            switch(type)
            {
                case TutorielType.Jump:
                    _playerMovement.JumpUnlock = true;
                    break;

                case TutorielType.DoubleJump:
                    _playerMovement.DoubleJumpUnlock = true;
                    break;

                case TutorielType.WallRun:
                    _playerMovement.WallRunUnlock = true;
                    break;

                case TutorielType.Dash:
                    _playerMovement.DashUnlock = true;
                    break;

                case TutorielType.Attack:
                    _playerAttacks.AttacksUnlock = true;
                    break;

            }

            Debug.Log("Spell Unlock : " + type);
            SpellUnlock?.Invoke(type);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.CompareTag("Player") && !_alreadyTriggered)
            {
                _playerMovement = collision.GetComponent<CharacterMovement>();
                _playerAttacks = collision.GetComponent<CharacterAttackManager>();
                UnlockSpell(_tutorielType);
                _alreadyTriggered = true;
            }
        }

        public TutorielType TypeOfTutoriel => _tutorielType;
        public CharacterMovement PlayerMovement => _playerMovement;
        public CharacterAttackManager PlayerAttacks => _playerAttacks;
    }
}
