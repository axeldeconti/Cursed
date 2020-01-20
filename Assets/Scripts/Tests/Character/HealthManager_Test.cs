using Cursed.Character;
using Cursed.Combat;
using NUnit.Framework;
using System;
using UnityEngine;

namespace Tests
{
    public class HealthManager_Test
    {
        [Test][TestCase(1)][TestCase(10)][TestCase(50)]
        public void HealthManager_AttackReduceLifeCorrectly_LifeLeft(int amount)
        {
            var healthManager = new GameObject().AddComponent<HealthManager>();

            healthManager.UpdateCurrentHealth(100);

            healthManager.OnAttack(null, new Attack(amount, false, null));

            Assert.AreEqual(healthManager.CurrentHealth, 100 - amount);
        }

        [Test][TestCase(-1)][TestCase(-10)][TestCase(-50)]
        public void HealthManager_MaxHealthDontGoBelowZero_NotNegative(int amount)
        {
            var healthManager = new GameObject().AddComponent<HealthManager>();

            healthManager.AddMaxHealth(10);
            healthManager.AddMaxHealth(amount);

            Assert.GreaterOrEqual(healthManager.MaxHealth, 0);
        }
    }
}