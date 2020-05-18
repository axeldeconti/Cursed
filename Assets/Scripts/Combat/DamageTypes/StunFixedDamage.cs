using Cursed.Character;
using UnityEngine;
using System;
using System.Collections;

namespace Cursed.Combat
{
    [CreateAssetMenu(fileName = "New Stun", menuName = "Attack/Types/Stun Fixed Damage")]
    public class StunFixedDamage : ForkDamage
    {
        [SerializeField] private FloatReference _stunTime;

        public override void Effect(CharacterStats statsToAffect)
        {
            Debug.Log("Stun : " + statsToAffect.HealthManager.name);
            statsToAffect.HealthManager.GetComponent<CharacterMovement>().CallDisableMovement(_stunTime);
        }
    }
}
