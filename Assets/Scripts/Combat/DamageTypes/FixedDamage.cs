using Cursed.Character;
using UnityEngine;

namespace Cursed.Combat
{
    [CreateAssetMenu(fileName = "New Fixed Damage", menuName = "Attack/Types/Fixed Damage")]
    public class FixedDamage : DamageType_SO
    {
        [SerializeField] private int _damage = 0;

        private void OnEnable() => _modifier = Stat.FixedDamage;

        public override void Effect(CharacterStats statsToAffect)
        {
            
        }

        public override int GetDamages()
        {
            return _damage;
        }

        public int Damage { get => _damage; set => _damage = value; }
    }
}