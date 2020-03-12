using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cursed.Combat;
using Cursed.Character;

namespace Cursed.Combat
{
    public class DestructibleBox : MonoBehaviour, IAttackable
    {
        public void OnAttack(GameObject attacker, Attack attack)
        {
            if(attacker.GetComponent<CharacterMovement>().IsDiveKicking)
                Destroy(this.gameObject);
        }
    }
}
