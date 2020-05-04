using UnityEngine;
using Cursed.Combat;

public class Rope2DAttackable : MonoBehaviour, IAttackable
{
    public void OnAttack(GameObject attacker, Attack attack)
    {
        Destroy(gameObject);
    }
}
