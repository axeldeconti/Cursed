using Cursed.Combat;
using Cursed.VisualEffect;
using System.Collections;
using UnityEngine;

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
        private GameManager _gameManager = null;
        private VfxHandler _vfx = null;

        [SerializeField] private AttackDefinition _divekickAttack = null;
        [SerializeField] private FloatReference _timeAfterCombo = null;

        private bool _isAttacking = false;
        private bool _isDiveKicking = false;
        private int _weaponNb = 0;
        private int _combo = 0;
        private bool _canCombo = false;

        [Space]
        [Header("Stats Camera Shake")]
        [SerializeField] private ShakeData _shakeDivekick = null;
        [SerializeField] private ShakeDataEvent _onCamShake = null;

        [Header("Unlocks")]
        [SerializeField] private bool _attacksUnlock = true;

        private void Awake()
        {
            _anim = GetComponentInChildren<AnimationHandler>();
            _move = GetComponent<CharacterMovement>();
            _stats = GetComponent<CharacterStats>();
            _coll = GetComponent<CollisionHandler>();
            _weaponInv = GetComponent<WeaponInventory>();
            _input = GetComponent<IInputController>();
            _vfx = GetComponent<VfxHandler>();

            _coll.OnGrounded += () => _isDiveKicking = false;
        }

        private void Start()
        {
            _isAttacking = false;
            _isDiveKicking = false;
            _isAttacking = false;
            _combo = 0;
            _canCombo = false;
            _gameManager = GameManager.Instance;
        }

        private void Update()
        {
            if (_gameManager.State != GameManager.GameState.InGame)
                return;

            if (_input.Attack_1 || _input.Attack_2)
                UpdateAttack(_input.Attack_1 ? 1 : 2);

        }

        /// <summary>
        /// Handle wich attack should be launch
        /// </summary>
        private void UpdateAttack(int attackNb)
        {
            if (!_attacksUnlock)
                return;

            if (_coll.OnGround)
            {
                if (_move.IsDashing)
                {
                    bool forced = false;
                    _move.UpdateForceToContinu(ref forced);

                    if (!forced)
                    {
                        //Dash attack
                        NormalAttack(attackNb);
                    }
                }
                else
                {
                    if (Mathf.Abs(_move.XSpeed) > 4)
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
            }
            else
            {
                if (!_move.IsWallRun && !_move.WallSlide)
                {
                    //Dive Kicku
                    DiveKick();
                }
            }
        }

        /// <summary>
        /// Handle normal attacks
        /// </summary>
        private void NormalAttack(int attackNb)
        {
            _weaponNb = attackNb;
            Weapon weapon = _weaponInv.GetWeapon(_weaponNb);

            if (_isAttacking)
            {
                if (_canCombo)
                {
                    //Combo
                    _anim.TiggerComboAttack(weapon.WeaponType.GetHashCode(), ++_combo);
                    _canCombo = false;
                }
            }
            else
            {
                if (_canCombo)
                {
                    //Combo after the end of the animation
                    _isAttacking = true;
                    _canCombo = false;
                    _anim.LaunchAttack(weapon.WeaponType.GetHashCode(), ++_combo);
                }
                else
                {
                    //1st attack
                    _isAttacking = true;

                    _anim.LaunchAttack(weapon.WeaponType.GetHashCode(), ++_combo);
                }
            }
            //Vibration
            if (Combo != 3)
                ControllerVibration.Instance.StartVibration(weapon.ClassicVibration);
            else
                ControllerVibration.Instance.StartVibration(weapon.Combo3Vibration);
        }

        /// <summary>
        /// Handle run attacks
        /// </summary>
        private void RunAttack()
        {
            _isAttacking = true;
            _anim.LaunchAttack(0, ++_combo);
        }

        /// <summary>
        /// Handle the dive kick
        /// </summary>
        private void DiveKick()
        {
            _isAttacking = true;
            _isDiveKicking = true;
            _anim.LaunchAttack(0, ++_combo);
            _onCamShake?.Raise(_shakeDivekick);
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
        /// Reset IsAttacking
        /// </summary>
        public void EndAttack()
        {
            _isAttacking = false;
            _isDiveKicking = false;
            _weaponNb = 0;
            _combo = 0;
            _canCombo = false;
        }

        /// <summary>
        /// Called by attacks 1 and 2 of combo to allow to combo a bit of time after then end of the animation
        /// </summary>
        public void EndComboAttack()
        {
            _isAttacking = false;
            _isDiveKicking = false;
            _weaponNb = 0;

            StartCoroutine(AllowCombo());
        }

        private IEnumerator AllowCombo()
        {
            yield return new WaitForSeconds(_timeAfterCombo);

            if (!_isAttacking)
            {
                _combo = 0;
                _canCombo = false;
            }
        }

        public void CanCombo()
        {
            _canCombo = true;
        }

        /// <summary>
        /// Attack the defender with the current weapon
        /// </summary>
        /// <param name="defender">Character beeing attacked</param>
        public void ExecuteAttack(GameObject defender)
        {
            _weaponInv.GetWeapon(_weaponNb).ExecuteAttack(gameObject, defender);
        }

        public GameObject[] GetVfxTouchImpact()
        {
            if (!IsAttacking)
                return null;

            if (_isDiveKicking)
                return _divekickAttack.VfxTouchImpact;

            return CurrentWeapon.VfxTouchImpact;
        }

        public GameObject GetVfxCombo3()
        {
            if (!IsAttacking)
                return null;

            if (_isDiveKicking)
                return null;

            return CurrentWeapon.VfxCombo3;
        }

        #region GETTERS & SETTERS

        public bool IsAttacking => _isAttacking;
        public bool IsDiveKicking => _isDiveKicking;
        public Weapon CurrentWeapon => _weaponInv.GetWeapon(_weaponNb);
        public int Combo => _combo;
        public bool AttacksUnlock
        {
            get => _attacksUnlock;
            set => _attacksUnlock = value;
        }

        #endregion
    }
}