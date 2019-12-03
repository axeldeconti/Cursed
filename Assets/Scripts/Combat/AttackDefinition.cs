using UnityEngine;
using Cursed.Character;
using Cursed.Item;

namespace Cursed.Combat
{
    [CreateAssetMenu(fileName = "New Attack", menuName = "Attack/BaseAttack")]
    public class AttackDefinition : PickUp_SO
    {
        [Header("Data")]
        public float cooldown = 1f;

        [Header("Damage")]
        public DamageType_SO damageType = null;

        [Header("Critic")]
        public float criticalMultiplier = 1.5f;
        public float criticalChance = 0.1f;

        public Attack CreateAttack(CharacterStats attackerStats, CharacterStats defenserStats)
        {
            float coreDamage = 0f;
            coreDamage += damageType.GetDamages();

            coreDamage += attackerStats.GetStatModifier(damageType.Modifier);

            bool isCritical = Random.value < criticalChance;
            if (isCritical)
                coreDamage *= criticalMultiplier;

            return new Attack((int)coreDamage, isCritical, damageType.Effect);
        }
    }
}