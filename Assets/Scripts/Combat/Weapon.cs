using UnityEngine;
using Cursed.Character;

namespace Cursed.Combat
{
    [CreateAssetMenu(fileName = "Weapon.asset", menuName = "Attack/Weapon")]
    public class Weapon : AttackDefinition
    {
        [Header("Prefabs")]
        [SerializeField] private Rigidbody _weaponPrefab;

        [Header("Type")]
        [SerializeField] private WeaponType _weaponType;

        public void ExecuteAttack(GameObject attacker, GameObject defender)
        {
            if (defender == null)
                return;

            var attackerStats = attacker.GetComponent<CharacterStats>();

            var attack = CreateAttack(attackerStats);

            var attackables = defender.GetComponentsInChildren(typeof(IAttackable));
            foreach (IAttackable a in attackables)
            {
                a.OnAttack(attacker, attack);
            }
        }

        public Rigidbody WeaponPrefab => _weaponPrefab;
        public WeaponType WeaponType { get => _weaponType; set => _weaponType = value; }
    }
}