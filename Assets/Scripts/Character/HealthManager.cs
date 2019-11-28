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

        public Action<int> onHealthUpdate;
        public Action<int> onMaxHealthUpdate;
        public Action onDeath;

        #region Initalizer

        private void Start()
        {
            //Set to an eventual base number
            CharacterStats charStats = GetComponent<CharacterStats>();
            if (charStats != null)
                _maxHealth = charStats.baseStats.MaxHealth;

            UpdateHealth(_maxHealth);
        }

        #endregion

        public void OnAttack(GameObject attacker, Attack attack)
        {
            if (attack.Duration != 0)
                StartCoroutine(ApplyDot(attack));
            else
                UpdateHealth(_currentHealth - attack.Damage);

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

        public IEnumerator ApplyDot(Attack attack)
        {
            //Apply dot
            return null;
        }

        #region Death

        private void Die()
        {
            Debug.Log(gameObject.name + " is dead :(");
        }

        #endregion
    }
}