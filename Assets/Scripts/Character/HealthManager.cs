using UnityEngine;
using Cursed.Combat;
using System.Collections;

namespace Cursed.Character
{
    public class HealthManager : MonoBehaviour, IAttackable
    {
        [SerializeField] private IntReference _maxHealth;

        [SerializeField] private int _currentHealth = 0;
        private CharacterStats _stats = null;

        public IntEvent onHealthUpdate;
        public IntEvent onMaxHealthUpdate;
        public VoidEvent onDeath;

        #region Initalizer

        private void Start()
        {
            //Set to an eventual base number
            _stats = GetComponent<CharacterStats>();
            if (_stats != null)
                _maxHealth.Value = _stats.BaseStats.MaxHealth;

            UpdateCurrentHealth(_maxHealth);
        }

        #endregion


        #region Modifiers

        public void OnAttack(GameObject attacker, Attack attack)
        {
            //Update health
            UpdateCurrentHealth(_currentHealth - attack.Damage);

            //Apply the effect of the attack
            if (attack.Effect != null && _stats != null)
                attack.Effect.Invoke(_stats);

            //Play sound, vfx and animation
            //Do something if critical
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

            if (_currentHealth < 0)
                _currentHealth = 0;

            if (_currentHealth >= MaxHealth)
                _currentHealth = MaxHealth;

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

            UpdateCurrentHealth(_currentHealth + amount);
        }

        public void ApplyDot(float damagePerSecond, float duration)
        {
            StartCoroutine(ApplyDotCoroutine(damagePerSecond, duration));
        }

        private IEnumerator ApplyDotCoroutine(float damagePerSecond, float duration)
        {
            //Aplly dot
            float timeLeft = duration;

            while(timeLeft > 0)
            {
                UpdateCurrentHealth((int)((float)_currentHealth - damagePerSecond));
                yield return new WaitForSeconds(1f);
            }

            //End dot
        }

        #endregion

        #region Death

        private void Die()
        {
            Debug.Log(gameObject.name + " is dead :(");
            onDeath?.Raise();
        }

        #endregion

        #region Getters

        public float CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;

        #endregion
    }
}