using Cursed.Character;
using UnityEngine;

namespace Cursed.Combat
{
    [CreateAssetMenu(fileName = "New Fork Damages", menuName = "Attack/Types/Fork Damage")]
    public class ForkDamage : DamageType_SO
    {
        [SerializeField] private int _minDamage = 0;
        [SerializeField] private int _maxDamage = 0;

        private void OnEnable() => _modifier = Stat.FixedDamage;

        public override void Effect(CharacterStats statsToAffect)
        {
            
        }

        public override int GetDamages()
        {
            return Random.Range(_minDamage, _maxDamage);
        }

        public int MinDamage { get => _minDamage; set => _minDamage = value; }
        public int MaxDamage { get => _maxDamage; set => _maxDamage = value; }
    }
}