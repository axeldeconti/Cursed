using UnityEngine;

namespace Cursed.Combat
{
    public interface IAttackable
    {
        void OnAttack(GameObject attacker, Attack attack);
    }
}