using Cursed.Character;
using UnityEngine;

namespace Cursed.Combat
{
    [CreateAssetMenu(fileName = "New Fork Damages", menuName = "Attack/Types/Fork Damage")]
    public class ForkDamage : DamageType_SO
    {
        [SerializeField] private IntReference _minDamage;
        [SerializeField] private IntReference _maxDamage;

        private void OnEnable() => _modifier = Stat.FixedDamage;

        public override void Effect(CharacterStats statsToAffect)
        {
            
        }

        public override int GetDamages()
        {
            return Random.Range(_minDamage, _maxDamage + 1);
        }

        public int MinDamage { get => _minDamage; set => _minDamage.Value = value; }
        public int MaxDamage { get => _maxDamage; set => _maxDamage.Value = value; }
    }
}