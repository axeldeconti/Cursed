using System;
using UnityEngine;
using Cursed.Combat;

namespace Cursed.Character
{
    public class HealthManager : MonoBehaviour, IAttackable
    {
        [SerializeField] private int _maxHealth = 100;
        
        private int _currentHealth = 0;

        public Action<int> onHealthUpdate;
        public Action onDeath;

        private void Start()
        {
            UpdateHealth(_maxHealth);
        }

        public void OnAttack(GameObject attacker, Attack attack)
        {
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

        private void Die()
        {
            Debug.Log(gameObject.name + " is dead :(");
        }
    }
}