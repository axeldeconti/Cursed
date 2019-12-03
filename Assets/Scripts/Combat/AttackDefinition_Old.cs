using UnityEngine;
using Cursed.Character;
using Cursed.Item;

namespace Cursed.Combat
{
    //[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/BaseAttack")]
    public class AttackDefinition_Old : PickUp_SO
    {
        [Header("Data")]
        public float cooldown;

        [Header("Damage")]
        //public DamageTypeDefinition damageType;
        public float fixedDamage;
        public float minDamage;
        public float maxDamage;
        public float dotDamage;
        public float dotDuration;

        [Header("Critic")]
        public float criticalMultiplier;
        public float criticalChance;

        public Attack CreateAttack(CharacterStats wielderStats)
        {
            float coreDamage = 0f;

            /*
            switch (damageType)
            {
                case DamageTypeDefinition.Fixed:
                    coreDamage = fixedDamage;
                    coreDamage += wielderStats.GetStatModifier(Stat.FixedDamage);
                    break;
                case DamageTypeDefinition.Fork:
                    coreDamage = Random.Range(minDamage, maxDamage);
                    coreDamage += wielderStats.GetStatModifier(Stat.FixedDamage);
                    break;
                case DamageTypeDefinition.Dot:
                    coreDamage = dotDamage;
                    coreDamage += wielderStats.GetStatModifier(Stat.DotDamage);
                    break;
                default:
                    break;
            }
            */

            bool isCritical = Random.value < criticalChance;
            if (isCritical)
                coreDamage *= criticalMultiplier;

            return new Attack((int)coreDamage, isCritical/*, dotDuration */, null);
        }

        public void ResetDamages()
        {
            fixedDamage = 0f;
            minDamage = 0f;
            maxDamage = 0f;
            dotDamage = 0f;
            dotDuration = 0f;
        }
    }
}