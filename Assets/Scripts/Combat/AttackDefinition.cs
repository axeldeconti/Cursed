using UnityEngine;
using Cursed.Character;
using Cursed.Item;
using Cursed.Creature;
using Cursed.Utilities;

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

        [Header("Vfx")]
        [SerializeField] private GameObject[] _vfxTouchImpact;
        [SerializeField] private GameObject _vfxCombo3;

        [Header("Vibration")]
        [SerializeField] private VibrationData_SO _vibrationData;

        public Attack CreateAttack()
        {
            float coreDamage = 0f;
            coreDamage += _damageType.GetDamages();

            bool isCritical = Random.value < _criticalChance;
            if (isCritical)
                coreDamage *= _criticalMultiplier;

            return new Attack((int)coreDamage, isCritical, _damageType.Effect);
        }

        public Attack CreateAttack(CharacterStats attackerStats)
        {
            float coreDamage = 0f;
            coreDamage += _damageType.GetDamages();

            coreDamage += attackerStats.GetStatModifier(_damageType.Modifier);

            bool isCritical = Random.value < _criticalChance;
            if (isCritical)
                coreDamage *= _criticalMultiplier;

            return new Attack((int)coreDamage, isCritical, _damageType.Effect);
        }

        public Attack CreateAttack(CreatureStats creatureStats, CharacterStats defenserStats)
        {
            float coreDamage = 0f;
            coreDamage += _damageType.GetDamages();

            return new Attack((int)coreDamage, false, null);
        }

        public float Cooldown { get => _cooldown; set => _cooldown.Value = value; }
        public DamageType_SO DamageType { get => _damageType; set => _damageType = value; }
        public float CriticalMultiplier { get => _criticalMultiplier; set => _criticalMultiplier.Value = value; }
        public float CriticalChance { get => _criticalChance; set => _criticalChance.Value = value; }
        public GameObject[] VfxTouchImpact { get => _vfxTouchImpact; set => _vfxTouchImpact = value; }
        public GameObject VfxCombo3 { get => _vfxCombo3; set => _vfxCombo3 = value; }
        public VibrationData_SO Vibration {get => _vibrationData; set => _vibrationData = value; }
    }
}