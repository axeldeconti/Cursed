using Cursed.Combat;
using Cursed.Creature;
using UnityEngine;

namespace Cursed.Character
{
    public class EnemyHealth : HealthManager
    {
        [SerializeField] private IntReference _minCreatureAmountHealth;

        public override void OnAttack(GameObject attacker, Attack attack)
        {
            if (attacker.tag.Equals("Creature"))
            {
                if (_currentHealth > _minCreatureAmountHealth)
                    UpdateCurrentHealth(_currentHealth - attack.Damage);
                else
                {
                    attacker.GetComponent<CreatureManager>().CurrentState = CreatureState.OnComeBack;

                    // LAUNCH SFX & VFX
                }
            }
            else
            {
                base.OnAttack(attacker, attack);
            }
        }
    }
}

