using UnityEngine;
using Cursed.Combat;

namespace Cursed.Character
{
    public class AttackCollider : MonoBehaviour
    {
        [SerializeField] private CharacterAttackManager _attackManager;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<IAttackable>() != null)
            {
                _attackManager.ExecuteAttack(collision.gameObject);
            }
        }
    }
}