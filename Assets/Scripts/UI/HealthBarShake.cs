using UnityEngine;
using Cursed.Character;

namespace Cursed.UI
{
    public class HealthBarShake : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        void Start()
        {
            EnemyHealth enemyHealth = GetComponentInParent<EnemyHealth>();


            if (enemyHealth != null)
                enemyHealth.onEnemyHealthUpdate += (value) => Shake();
        }

        private void Shake()
        {
            Debug.Log("Shake");
            _animator.SetTrigger("Shake");
        }
    }
}
