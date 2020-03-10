using UnityEngine;
using Cursed.Combat;
using System.Collections;

namespace Cursed.Character
{
    public class HealthManager : MonoBehaviour, IAttackable
    {
        private CharacterMovement _move = null;

        [SerializeField] private IntReference _maxHealth;
        [SerializeField] private FloatReference _invincibleTime;

        private CharacterStats _stats = null;
        private int _currentHealth = 0;
        private bool _isInvincible = false;
        private float _timeInvincibleLeft = 0f;

        public IntEvent onHealthUpdate;
        public IntEvent onMaxHealthUpdate;
        public VoidEvent onDeath;

        private VfxHandler _vfx = null;
        private SFXHandler _sfx = null;

        #region Initalizer

        private void Awake()
        {
            _move = GetComponent<CharacterMovement>();
        }

        private void Start()
        {
            _vfx = GetComponent<VfxHandler>();
            _sfx = GetComponent<SFXHandler>();

            //Set to an eventual base number
            _stats = GetComponent<CharacterStats>();
            if (_stats != null)
                _maxHealth.Value = _stats.BaseStats.MaxHealth;

            UpdateCurrentHealth(_maxHealth);

            _isInvincible = false;
            _timeInvincibleLeft = 0f;
        }

        #endregion

        private void Update()
        { 
            if(_timeInvincibleLeft > 0f)
            {
                _timeInvincibleLeft -= Time.deltaTime;
            }
            else if (_isInvincible)
            {
                //End invincibility
                _isInvincible = false;
            }
        }


        #region Modifiers

        public void OnAttack(GameObject attacker, Attack attack)
        {
            Debug.Log("Invincibility = " + (_isInvincible && IsInvicibleMovement()));
            if (_isInvincible && IsInvicibleMovement())
            {
                //Do something if invincible
            }
            else
            {
                //Update health
                UpdateCurrentHealth(_currentHealth - attack.Damage);

                //Apply the effect of the attack
                if (attack.Effect != null && _stats != null)
                    attack.Effect.Invoke(_stats);

                //Become invincible
                StartInvincibility(_invincibleTime);

                if(attacker != null)
                    Debug.Log(gameObject.name + " got attacked by " + attacker.name + " and did " + attack.Damage + " damages");

                //Play sound, vfx and animation
                if (gameObject.tag.Equals("Player"))
                    _sfx.LowHealth();

                CharacterAttackManager atkMgr = attacker.GetComponent<CharacterAttackManager>();
                if (!attacker.tag.Equals("Creature") && !attacker.tag.Equals("Traps"))
                {
                    if (atkMgr)
                    {
                        if(atkMgr.CurrentWeapon.WeaponType == WeaponType.Sword)
                            _vfx.TouchImpactSwordVfx(gameObject.transform.position);
                        if (atkMgr.CurrentWeapon.WeaponType == WeaponType.Axe)
                            _vfx.TouchImpactAxeVfx(gameObject.transform.position);

                        _sfx.EnemyDamageSFX();
                    }                   
                }

                if (!attacker.tag.Equals("Creature") && gameObject.tag.Equals("Player"))
                {
                    _sfx.PlayerDamageSFX();
                    _vfx.FlashScreenDmgPlayer();
                }

                //Do something if critical
            }
        }

        public void UpdateCurrentHealth(int health)
        {
            //Check if dead or not
            if (health <= 0)
            {
                Die();
            }
            else
            {
                _currentHealth = health;

                if (onHealthUpdate != null)
                    onHealthUpdate.Raise(_currentHealth);
            }
        }

        public void AddCurrentHealth(int amount)
        {
            _currentHealth += amount;

            _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);

            if (onHealthUpdate != null)
                onHealthUpdate.Raise(_currentHealth);
        }

        public void AddMaxHealth(int amount)
        {
            _maxHealth.Value += amount;

            if (_maxHealth < 0)
                _maxHealth.Value = 0;

            if (onMaxHealthUpdate != null)
                onMaxHealthUpdate.Raise(_maxHealth);

            if (amount >= 0)
                AddCurrentHealth(amount);
            else
                AddCurrentHealth(0);
        }

        public void ApplyDot(float damagePerSecond, float duration)
        {
            StartCoroutine(ApplyDotCoroutine(damagePerSecond, duration));
        }

        private IEnumerator ApplyDotCoroutine(float damagePerSecond, float duration)
        {
            //Apply dot
            float timeLeft = duration;

            while(timeLeft > 0)
            {
                UpdateCurrentHealth((int)((float)_currentHealth - damagePerSecond));
                yield return new WaitForSeconds(1f);
            }

            //End dot
        }

        #endregion

        #region Invincibility

        public void StartInvincibility(float time)
        {
            if (time > _timeInvincibleLeft)
                _timeInvincibleLeft = time;

            _isInvincible = true;
        }

        private bool IsInvicibleMovement()
        {
            if (_move)
                return _move.IsInvincible;

            return false;
        }

        #endregion

        #region Death

        private void Die()
        {
            Debug.Log(gameObject.name + " is dead :(");
            onDeath?.Raise();

            if (gameObject.tag.Equals("Player"))
            {
                _sfx.PlayerDeathSFX();
            }
            if (gameObject.tag.Equals("Enemy"))
            {
                _sfx.EnemyDeathSFX();
            }
        }

        #endregion

        #region Getters

        public float CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public bool IsInvincible { get => _isInvincible; set => _isInvincible = value; }

        #endregion
    }
}