using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureSearching : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _radius = 4f;
        [Header("Referencies")]
        [SerializeField] private LayerMask _checkLayer;
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
                RaycastHit2D[] obj = Physics2D.CircleCastAll(transform.position, _radius, new Vector2(0f,0f));
                foreach (RaycastHit2D hit in obj)
                {
                    if(hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        if (EnemyInRange(hit.collider.transform, _radius, true))
                        {
                            _creatureManager.CurrentState = CreatureState.Chasing;
                        }
                    }
                }
            }
        }        

        private bool EnemyInRange(Transform ennemy, float range, bool raycastOn)
        {
            if (ennemy && Vector3.Distance(ennemy.position, transform.position) < range)
            {
                Debug.Log(ennemy.position);
                if (raycastOn)
                {
                    if (Physics2D.Linecast(transform.position, ennemy.position, _checkLayer).collider.gameObject.layer == ennemy.gameObject.layer)
                    {
                        Debug.Log("In Range");
                        _enemyHit = ennemy;
                        return true;
                    }
                    else
                        return false;
                }
                else if (!raycastOn)
                {
                    return true;
                }
            }
            return false;
        }

        public Transform Enemy => _enemyHit;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
