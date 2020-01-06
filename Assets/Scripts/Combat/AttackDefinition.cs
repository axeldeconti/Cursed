﻿using UnityEngine;
using Cursed.Character;
using Cursed.Item;

namespace Cursed.Combat
{
    [CreateAssetMenu(fileName = "New Attack", menuName = "Attack/BaseAttack")]
    public class AttackDefinition : PickUp_SO
    {
        [Header("Data")]
        [SerializeField] private FloatReference _cooldown;

        [Header("Damage")]
        [SerializeField] private DamageType_SO _damageType = null;

        [Header("Critic")]
        [SerializeField] private FloatReference _criticalMultiplier;
        [SerializeField] private FloatReference _criticalChance;

        public Attack CreateAttack(CharacterStats attackerStats, CharacterStats defenserStats)
        {
            float coreDamage = 0f;
            coreDamage += _damageType.GetDamages();

            coreDamage += attackerStats.GetStatModifier(_damageType.Modifier);

            bool isCritical = Random.value < _criticalChance;
            if (isCritical)
                coreDamage *= _criticalMultiplier;

            return new Attack((int)coreDamage, isCritical, _damageType.Effect);
        }

        public float Cooldown { get => _cooldown; set => _cooldown.Value = value; }
        public DamageType_SO DamageType { get => _damageType; set => _damageType = value; }
        public float CriticalMultiplier { get => _criticalMultiplier; set => _criticalMultiplier.Value = value; }
        public float CriticalChance { get => _criticalChance; set => _criticalChance.Value = value; }
    }
}