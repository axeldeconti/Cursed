using UnityEngine;
using Cursed.Character;

public enum TutorielType
{
    Move,
    Jump,
    DoubleJump,
    WallRun,
    Dash,
    Attack1,
    Attack2,
    Sonar,
    CreatureDirection,
    CreatureLaunch,
    CreatureRecall,
    InteractiveDoor

}
namespace Cursed.Tutoriel
{

    public class TutorielBox : MonoBehaviour
    {
        [SerializeField] private TutorielType _tutorielType;

        private CharacterMovement _playerMovement;
        private CharacterAttackManager _playerAttacks;
        private VfxHandler _vfx;

        [HideInInspector] public bool _alreadyTriggered;
        public event System.Action<TutorielType> SpellUnlock;

        private void UnlockSpell(TutorielType type)
        {
            switch(type)
            {
                case TutorielType.Jump:
                    if(_playerMovement.JumpUnlock == false)
                    {
                        _playerMovement.JumpUnlock = true;
                        _vfx.UnlockComp(_playerMovement.gameObject.transform.position);
                    }                   
                    break;

                case TutorielType.DoubleJump:
                    if (_playerMovement.DoubleJumpUnlock == false)
                    {
                        _playerMovement.DoubleJumpUnlock = true;
                        _vfx.UnlockComp(_playerMovement.gameObject.transform.position);
                    }
                    break;

                case TutorielType.WallRun:
                    if (_playerMovement.WallRunUnlock == false)
                    {
                        _playerMovement.WallRunUnlock = true;
                        _vfx.UnlockComp(_playerMovement.gameObject.transform.position);
                    }
                    break;

                case TutorielType.Dash:
                    if (_playerMovement.DashUnlock == false)
                    {
                        _playerMovement.DashUnlock = true;
                        _vfx.UnlockComp(_playerMovement.gameObject.transform.position);
                    }
                    break;

                case TutorielType.Attack1:
                    if (_playerAttacks.AttacksUnlock == false)
                    {
                        _playerAttacks.AttacksUnlock = true;
                        _vfx.UnlockComp(_playerAttacks.gameObject.transform.position);
                    }
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
                _vfx = collision.GetComponent<VfxHandler>();

                UnlockSpell(_tutorielType);
                _alreadyTriggered = true;
            }
        }

        public TutorielType TypeOfTutoriel => _tutorielType;
        public CharacterMovement PlayerMovement => _playerMovement;
        public CharacterAttackManager PlayerAttacks => _playerAttacks;
    }
}
