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

        public override void Effect(CharacterStats statsToAffect)
        {
            statsToAffect.HealthManager.ApplyDot(_damagePerSecond, _duration);
        }

        public override int GetDamages()
        {
            return 0;
        }
    }
}