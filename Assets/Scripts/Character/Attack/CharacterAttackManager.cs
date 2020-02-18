using UnityEngine;
using Cursed.Combat;

namespace Cursed.Character
{
    [RequireComponent(typeof(AnimationHandler))]
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(CharacterStats))]
    [RequireComponent(typeof(CollisionHandler))]
    [RequireComponent(typeof(WeaponInventory))]
    [RequireComponent(typeof(IInputController))]
    public class CharacterAttackManager : MonoBehaviour
    {
        private AnimationHandler _anim = null;
        private CharacterMovement _move = null;
        private CharacterStats _stats = null;
        private CollisionHandler _coll = null;
        private WeaponInventory _weaponInv = null;
        private IInputController _input = null;

        [SerializeField] private AttackDefinition _divekickAttack = null;

        private bool _isAttacking = false;
        private bool _isDiveKicking = false;
        private int _weaponNb = 0;

        private void Awake()
        {
            _anim = GetComponentInChildren<AnimationHandler>();
            _move = GetComponent<CharacterMovement>();
            _stats = GetComponent<CharacterStats>();
            _coll = GetComponent<CollisionHandler>();
            _weaponInv = GetComponent<WeaponInventory>();
            _input = GetComponent<IInputController>();

            _coll.OnGrounded += () => _isDiveKicking = false;
        }

        private void Start()
        {
            _isAttacking = false;
        }

        private void Update()
        {
            if (_input.Attack_1 || _input.Attack_2)
                UpdateAttack(_input.Attack_1 ? 1 : 2);
        }

        /// <summary>
        /// Handle wich attack should be launch
        /// </summary>
        private void UpdateAttack(int attackNb)
        {
            if (_coll.OnGround)
            {
                if(Mathf.Abs(_move.XSpeed) > 4)
                {
                    //Run attack
                    RunAttack();
                }
                else
                {
                    //Attack normal
                    NormalAttack(attackNb);
                }
            }
            else
            {
                //Dive Kicku
                DiveKick();
            }
        }

        /// <summary>
        /// Handle normal attacks
        /// </summary>
        private void NormalAttack(int attackNb)
        {
            if (_isAttacking)
            {
                //Combo
            }
            else
            {
                _isAttacking = true;

                _weaponNb = attackNb;
                Weapon weapon = _weaponInv.GetWeapon(_weaponNb);

                _anim.LaunchAttack(weapon.WeaponType.GetHashCode());
            }
        }

        /// <summary>
        /// Handle run attacks
        /// </summary>
        private void RunAttack()
        {
            Debug.Log("Run attack");
            _isAttacking = true;
        }

        /// <summary>
        /// Handle the dive kick
        /// </summary>
        private void DiveKick()
        {
            Debug.Log("Dive Kicku");
            _isAttacking = true;
            _isDiveKicking = true;
        }

        /// <summary>
        /// Attack with dive kick
        /// </summary>
        /// <param name="defender">Character beeing attacked</param>
        public void DiveKickAttack(GameObject defender)
        {
            Attack attack = _divekickAttack.CreateAttack(_stats);
            var attackables = defender.GetComponentsInChildren(typeof(IAttackable));
            foreach (IAttackable a in attackables)
            {
                a.OnAttack(gameObject, attack);
            }

            EndAttack();
        }

        /// <summary>
        /// reset IsAttacking
        /// </summary>
        public void EndAttack()
        {
            _isAttacking = false;
            _isDiveKicking = false;
            _weaponNb = 0;
        }

        /// <summary>
        /// Attack the defender with the current weapon
        /// </summary>
        /// <param name="defender">Character beeing attacked</param>
        public void ExecuteAttack(GameObject defender)
        {
            _weaponInv.GetWeapon(_weaponNb).ExecuteAttack(gameObject, defender);
        }

        public bool IsAttacking => _isAttacking;
        public bool IsDiveKicking => _isDiveKicking;
    }
}