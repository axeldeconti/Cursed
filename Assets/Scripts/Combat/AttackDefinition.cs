using UnityEngine;
using Cursed.Character;

namespace Cursed.Combat
{
    [CreateAssetMenu(fileName = "New Attack", menuName = "Attack/BaseAttack")]
    public class AttackDefinition : ScriptableObject
    {
        [Header("Data")]
        [SerializeField] private float _cooldown;
        [SerializeField] protected float _range;

        [Header("Damages")]
        [SerializeField] private float _minDamage;
        [SerializeField] private float _maxDamage;

        [Header("Crit")]
        [SerializeField] private float _criticalMultiplier;
        [SerializeField] private float _criticalChance;

        public Attack CreateAttack(CharacterStats wielderStats, CharacterStats defenderStats)
        {
            //Base damages
            float coreDamage = wielderStats.baseStats.BaseDamage;
            //Add the damages of the attack
            coreDamage += Random.Range(_minDamage, _maxDamage);

            bool isCritical = Random.value < _criticalChance;
            if (isCritical)
                coreDamage *= _criticalMultiplier;

            return new Attack((int)coreDamage, isCritical);
        }
    }
}