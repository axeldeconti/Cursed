using UnityEngine;
using Cursed.Character;

namespace Cursed.Combat
{
    [CreateAssetMenu(fileName = "Weapon.asset", menuName = "Attack/Weapon")]
    public class Weapon : AttackDefinition
    {
        [Header("Prefabs")]
        [SerializeField] private Rigidbody _weaponPrefab;

        public void ExecuteAttack(GameObject attacker, GameObject defender)
        {
            if (defender == null)
                return;

            //Check if defender is in range of the attacker
            /*
            if (Vector3.Distance(attacker.transform.position, defender.transform.position) > _range)
                return;
            */

            //Check if defender is in front of the player
            if (!attacker.transform.IsFacingTarget(defender.transform))
                return;

            //At this point the attack will connect
            var attackerStats = attacker.GetComponent<CharacterStats>();
            var defenderStats = defender.GetComponent<CharacterStats>();

            var attack = CreateAttack(attackerStats, defenderStats);

            var attackables = defender.GetComponentsInChildren(typeof(IAttackable));
            foreach (IAttackable a in attackables)
            {
                a.OnAttack(attacker, attack);
            }
        }

        public Rigidbody WeaponPrefab => _weaponPrefab;
    }
}