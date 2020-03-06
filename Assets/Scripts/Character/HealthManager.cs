using UnityEngine;
using Cursed.Combat;
using System.Collections;

namespace Cursed.Character
{
    public class HealthManager : MonoBehaviour, IAttackable
    {
        [SerializeField] private IntReference _maxHealth;
        [SerializeField] private FloatReference _invincibleTime;

        private CharacterStats _stats = null;
        private int _currentHealth = 0;
        private bool _isInvincible = false;
        private float _timeInvincibleLeft = 0f;

        public IntEvent onHealthUpdate;
        public IntEvent onMaxHealthUpdate;
        public VoidEvent onDeath;

        private SFXHandler _sfxHandler = null;

        #region Initalizer

        private void Start()
        {
            //Set to an eventual base number
            _stats = GetComponent<CharacterStats>();
            if (_stats != null)
                _maxHealth.Value = _stats.BaseStats.MaxHealth;

            _sfxHandler = GetComponent<SFXHandler>();

            UpdateCurrentHealth(_maxHealth);

            _isInvincible = false;
            _timeInvincibleLeft = 0f;
        }

        #endregion

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
                _stats.ModifyStat(Stat.Health, -10);

            if (Input.GetKeyDown(KeyCode.G))
                _stats.ModifyStat(Stat.Health, 10);
 
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
            if (_isInvincible)
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

                Debug.Log(gameObject.name + " got attacked by " + attacker.name + " and did " + attack.Damage + " damages");

                //Play sound, vfx and animation
                //AkSoundEngine.PostEvent("Play_Damage_Enemy", gameObject);
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

                _sfxHandler.LowHealth();
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

        #endregion

        #region Death

        private void Die()
        {
            Debug.Log(gameObject.name + " is dead :(");
            //AkSoundEngine.PostEvent("Play_Death_Enemy", gameObject);
            onDeath?.Raise();
        }

        #endregion

        #region Getters

        public float CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;

        #endregion
    }
}