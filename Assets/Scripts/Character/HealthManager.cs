using System;
using UnityEngine;
using Cursed.Combat;
using System.Collections;

namespace Cursed.Character
{
    public class HealthManager : MonoBehaviour, IAttackable
    {
        [SerializeField] private int _maxHealth = 100;
        
        private int _currentHealth = 0;
        private CharacterStats _stats = null;

        public Action<int> onHealthUpdate;
        public Action<int> onMaxHealthUpdate;
        public Action onDeath;

        #region Initalizer

        private void Start()
        {
            //Set to an eventual base number
            _stats = GetComponent<CharacterStats>();
            if (_stats != null)
                _maxHealth = _stats.BaseStats.MaxHealth;

            UpdateHealth(_maxHealth);
        }

        #endregion

        public void OnAttack(GameObject attacker, Attack attack)
        {
            /*
            if (attack.Duration != 0)
                StartCoroutine(ApplyDot(attack));
            else
                UpdateHealth(_currentHealth - attack.Damage);

            */

            //Update health
            UpdateHealth(_currentHealth - attack.Damage);

            //Apply the effect of the attack
            if (attack.Effect != null && _stats != null)
                attack.Effect.Invoke(_stats);

            //Play sound, vfx and animation
            //Do something if critical
        }

        private void UpdateHealth(int health)
        {
            //Check if dead or not
            if(health <= 0)
            {
                Die();
            }
            else
            {
                _currentHealth = health;

                if (onHealthUpdate != null)
                    onHealthUpdate.Invoke(_currentHealth);
            }
        }

        public void AddMaxHealth(int amount)
        {
            _maxHealth += amount;

            if (onMaxHealthUpdate != null)
                onMaxHealthUpdate.Invoke(_maxHealth);

            UpdateHealth(_currentHealth + amount);
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
                UpdateHealth((int)((float)_currentHealth - damagePerSecond));
                yield return new WaitForSeconds(1f);
            }

            //End dot
        }

        #region Death

        private void Die()
        {
            Debug.Log(gameObject.name + " is dead :(");
        }

        #endregion
    }
}