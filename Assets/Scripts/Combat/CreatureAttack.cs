using Cursed.Character;
using Cursed.Creature;
using UnityEngine;

namespace Cursed.Combat
{
    [CreateAssetMenu(fileName = "CreatureAttack", menuName = "Attack/Creature")]
    public class CreatureAttack : AttackDefinition
    {
        [Header ("Creature")]
        [SerializeField] private FloatReference _timeBetweenAttack;

        public Attack InflictDamage(GameObject attacker, GameObject defender)
        {
            IAttackable a = defender.GetComponent<IAttackable>();

            Attack attack = CreateAttack(attacker.GetComponent<CreatureStats>(), defender.GetComponent<CharacterStats>());

            a.OnAttack(attacker, attack);

            return attack;
        }

        public float TimeBetweenAttack => _timeBetweenAttack;
    }
}