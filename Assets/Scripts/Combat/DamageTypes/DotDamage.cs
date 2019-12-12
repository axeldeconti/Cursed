using System.Collections;
using System.Collections.Generic;
using Cursed.Character;
using UnityEngine;

namespace Cursed.Combat
{
    [CreateAssetMenu(fileName = "New Dot", menuName = "Attack/Types/Dot Damage")]
    public class DotDamage : DamageType_SO
    {
        [SerializeField] private float _damagePerSecond = 0f;
        [SerializeField] private float _duration = 0f;

        private void OnEnable() => _modifier = Stat.DotDamage;

        public override void Effect(CharacterStats statsToAffect)
        {
            statsToAffect.HealthManager.ApplyDot(_damagePerSecond, _duration);
        }

        public override int GetDamages()
        {
            return 0;
        }

        public float DamagePerSecond { get => _damagePerSecond; set => _damagePerSecond = value; }
        public float Duration { get => _duration; set => _duration = value; }
    }
}