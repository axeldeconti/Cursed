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
        private Transform _originRayInAir;
        private Transform _originRayOnWall;

        void Start()
        {
            _creatureManager = GetComponent<CreatureManager>();
            _animator = GetComponent<Animator>();
            _originRayInAir = GetComponentInChildren<Collider2D>().transform.GetChild(0);
            _originRayOnWall = GetComponentInChildren<Collider2D>().transform.GetChild(1);
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
                RaycastHit2D[] obj = Physics2D.CircleCastAll(_originRayInAir.position, _inAirRadius, new Vector2(0f, 0f));
                float distance = Mathf.Infinity;
                Transform enemyTransform = null;
                foreach (RaycastHit2D hit in obj)
                {
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        Debug.Log("Hit enemy");
                        
                        if (EnemyInRange(hit.point, hit.collider.transform, _originRayInAir.position,_inAirRadius, true))
                        {
                            if (Vector2.Distance(hit.point, _originRayInAir.position) < distance)
                            {
                                distance = Vector2.Distance(hit.point, _originRayInAir.position);
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
                RaycastHit2D[] obj = Physics2D.CircleCastAll(_originRayOnWall.position, _onWallRadius, new Vector2(0f, 0f));
                float distance = Mathf.Infinity;
                Transform enemyTransform = null;
                foreach (RaycastHit2D hit in obj)
                {
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        Debug.Log("Hit enemy");
                        if (EnemyInRange(hit.point, hit.collider.transform, _originRayOnWall.position,_onWallRadius, true))
                        {
                            Debug.Log("Enemy in range");
                            if (Vector2.Distance(hit.point, _originRayOnWall.position) < distance)
                            {
                                distance = Vector2.Distance(hit.point, _originRayOnWall.position);
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

        private bool EnemyInRange(Vector2 hit, Transform ennemy, Vector2 origin, float range, bool raycastOn)
        {
            if (ennemy && Vector2.Distance(hit, origin) < range)
            {
                if (raycastOn)
                {
                    Debug.Log(Physics2D.Linecast(origin, hit, _checkLayer).collider.name);
                    if (Physics2D.Linecast(origin, hit, _checkLayer).collider.gameObject.layer == ennemy.gameObject.layer)
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
            if(_originRayInAir != null && _creatureManager.CurrentState == CreatureState.Moving)
                Gizmos.DrawWireSphere(_originRayInAir.position, _inAirRadius);
            if (_originRayOnWall != null && _creatureManager.CurrentState == CreatureState.OnWall)
                Gizmos.DrawWireSphere(_originRayOnWall.position, _onWallRadius);
        } 


        #region GETTERS
        public Transform Enemy => _enemyHit;

        #endregion
    }
}
