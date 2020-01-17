using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureSearching : MonoBehaviour
    {
        [Header("Settings")]
        public float Radius = 4f;
        [Header("Referencies")]
        public LayerMask groundLayer;
        private Transform _enemyHit;
        private CreatureManager _creatureManager;

        void Start()
        {
            _creatureManager = GetComponent<CreatureManager>();
        }
        void Update()
        {
            if(_creatureManager.CurrentState == CreatureState.Moving)
            {
                RaycastHit2D[] obj = Physics2D.CircleCastAll(transform.position, Radius, new Vector2(0f,0f));
                foreach (RaycastHit2D hit in obj)
                {
                    if(hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        _enemyHit = hit.collider.transform;
                        if (EnemyInRange(Radius, false))
                        {
                            _creatureManager.CurrentState = CreatureState.Chasing;
                        }
                    }
                }
            }
        }

        

        private bool EnemyInRange(float range, bool raycastOn)
        {
            if (_enemyHit && Vector3.Distance(_enemyHit.position, transform.position) < range)
            {
                if (raycastOn && !Physics2D.Linecast(transform.position, _enemyHit.position, groundLayer))
                {
                    Debug.Log("In Range");
                    return true;
                }
                else if (!raycastOn)
                {
                    return true;
                }
            }
            return false;
        }

        public Transform Enemy 
        {
            get => _enemyHit;
            set => _enemyHit = value;
        }
    }
}
