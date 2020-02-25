using UnityEngine;
using Cursed.Combat;

namespace Cursed.Traps
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Trap : MonoBehaviour
    {
        [SerializeField] private AttackDefinition _attack = null;

        /// <summary>
        /// Inflict damages to all attackable that enter in the trap
        /// </summary>
        protected virtual void InflinctDamage(Component[] attackables)
        {
            foreach (IAttackable a in attackables)
            {
                a.OnAttack(gameObject, _attack.CreateAttack()); ;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Component[] attackables = collision.GetComponentsInChildren(typeof(IAttackable));

            InflinctDamage(attackables);
        }
    }
}