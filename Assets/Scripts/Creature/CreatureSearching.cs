using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureSearching : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _onWallRadius = 30f;
        [SerializeField] private float _inAirRadius = 5f;
        [Header("Referencies")]
        [SerializeField] private LayerMask _checkLayer;
        private Transform _enemyHit;
        private CreatureManager _creatureManager;
        private Animator _animator;
        private Transform _rayStartTransform;

        void Start()
        {
            _creatureManager = GetComponent<CreatureManager>();
            _animator = GetComponent<Animator>();
            _rayStartTransform = GetComponentInChildren<Collider2D>().transform.GetChild(0);
        }

        void Update()
        {
            Searching();
        }

        private void Searching()
        {
            #region IN AIR
            if (_creatureManager.CurrentState == CreatureState.Moving && _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AC_GoFromCharacter")
            {
                RaycastHit2D[] obj = Physics2D.CircleCastAll(_rayStartTransform.position, _inAirRadius, new Vector2(0f, 0f));
                float distance = Mathf.Infinity;
                Transform enemyTransform = null;
                foreach (RaycastHit2D hit in obj)
                {
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        Debug.Log("Hit enemy");
                        
                        if (EnemyInRange(hit.point, hit.collider.transform, _inAirRadius, true))
                        {
                            if (Vector2.Distance(hit.point, _rayStartTransform.position) < distance)
                            {
                                distance = Vector2.Distance(hit.point, _rayStartTransform.position);
                                enemyTransform = hit.collider.transform;
                            }
                        }
                    }
                }
                if(enemyTransform != null)
                {
                    _enemyHit = enemyTransform;
                    _creatureManager.CurrentState = CreatureState.Chasing;
                }
            }
            #endregion

            #region ON WALL
            if (_creatureManager.CurrentState == CreatureState.OnWall && _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AC_GoToWall")
            {
                RaycastHit2D[] obj = Physics2D.CircleCastAll(transform.position, _onWallRadius, new Vector2(0f, 0f));
                float distance = Mathf.Infinity;
                Transform enemyTransform = null;
                foreach (RaycastHit2D hit in obj)
                {
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        Debug.Log("Hit enemy");
                        if (EnemyInRange(hit.point, hit.collider.transform, _onWallRadius, true))
                        {
                            Debug.Log("Enemy in range");
                            if (Vector2.Distance(hit.point, _rayStartTransform.position) < distance)
                            {
                                distance = Vector2.Distance(hit.point, _rayStartTransform.position);
                                Debug.Log("Distance : " + distance);
                                enemyTransform = hit.collider.transform;
                            }
                        }
                    }
                }
                if(enemyTransform != null)
                {
                    _enemyHit = enemyTransform;
                    _creatureManager.CurrentState = CreatureState.Chasing;
                }
            }
            #endregion
        }

        private bool EnemyInRange(Vector2 hit, Transform ennemy, float range, bool raycastOn)
        {
            if (ennemy && Vector2.Distance(hit, _rayStartTransform.position) < range)
            {
                if (raycastOn)
                {
                    if (Physics2D.Linecast(_rayStartTransform.position, hit, _checkLayer).collider.gameObject.layer == ennemy.gameObject.layer)
                        return true;
                    else
                        return false;
                }
                else if (!raycastOn)
                    return true;
            }
            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            if(_rayStartTransform != null && _creatureManager.CurrentState == CreatureState.Moving)
                Gizmos.DrawWireSphere(_rayStartTransform.position, _inAirRadius);
            if (_rayStartTransform != null && _creatureManager.CurrentState == CreatureState.OnWall)
                Gizmos.DrawWireSphere(transform.position, _onWallRadius);
        } 


        #region GETTERS
        public Transform Enemy => _enemyHit;

        #endregion
    }
}
