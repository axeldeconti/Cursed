using Cursed.Character;
using UnityEngine;

namespace Cursed.Combat
{
    [CreateAssetMenu(fileName = "New Dot", menuName = "Attack/Types/Dot Damage")]
    public class DotDamage : DamageType_SO
    {
        [SerializeField] private FloatReference _damagePerSecond;
        [SerializeField] private FloatReference _duration;

        private void OnEnable() => _modifier = Stat.DotDamage;

        public override void Effect(CharacterStats statsToAffect)
        {
            statsToAffect.HealthManager.ApplyDot(_damagePerSecond, _duration);
        }

        public override int GetDamages()
        {
            return 0;
        }

        public float DamagePerSecond { get => _damagePerSecond; set => _damagePerSecond.Value = value; }
        public float Duration { get => _duration; set => _duration.Value = value; }
    }
}