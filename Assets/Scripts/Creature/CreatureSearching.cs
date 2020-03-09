using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureSearching : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private FloatReference _onWallRadius;
        [SerializeField] private FloatReference _inAirRadius;
        [SerializeField] private FloatReference _timeBeforeChasingOnWall;
        private float _currentTimerBeforeChasingOnWall = 0f;
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
            if (_creatureManager.CurrentState == CreatureState.Moving)
            {
                RaycastHit2D[] obj = Physics2D.CircleCastAll(_originRayInAir.position, _inAirRadius.Value, new Vector2(0f, 0f));
                float distance = Mathf.Infinity;
                Transform enemyTransform = null;
                foreach (RaycastHit2D hit in obj)
                {
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        if (EnemyInRange(hit.point, hit.collider.transform, _originRayInAir.position, _inAirRadius.Value, true))
                        {
                            if (Vector2.Distance(hit.point, _originRayInAir.position) < distance)
                            {
                                distance = Vector2.Distance(hit.point, _originRayInAir.position);
                                enemyTransform = hit.collider.transform;
                            }
                        }
                    }
                }
                if (enemyTransform != null)
                {
                    _enemyHit = enemyTransform;
                    _creatureManager.CurrentState = CreatureState.Chasing;
                }
            }
            else if(_creatureManager.CurrentState == CreatureState.OnComeBack)
            {
                _enemyHit = null;
            }
            #endregion

            #region ON WALL
            if (_creatureManager.CurrentState == CreatureState.OnWall && _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AC_GoToWall")
            {
                RaycastHit2D[] obj = Physics2D.CircleCastAll(_originRayOnWall.position, _onWallRadius.Value, new Vector2(0f, 0f));
                float distance = Mathf.Infinity;
                Transform enemyTransform = null;
                foreach (RaycastHit2D hit in obj)
                {
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        if (EnemyInRange(hit.point, hit.collider.transform, _originRayOnWall.position,_onWallRadius.Value, true))
                        {
                            if (Vector2.Distance(hit.point, _originRayOnWall.position) < distance)
                            {
                                distance = Vector2.Distance(hit.point, _originRayOnWall.position);
                                enemyTransform = hit.collider.transform;
                            }
                        }
                    }
                }
                if(enemyTransform != null)
                {
                    _currentTimerBeforeChasingOnWall += Time.deltaTime;
                    if (_currentTimerBeforeChasingOnWall >= _timeBeforeChasingOnWall.Value)
                    {
                        _currentTimerBeforeChasingOnWall = 0f;
                        _enemyHit = enemyTransform;
                        _creatureManager.CurrentState = CreatureState.Chasing;
                    }
                }
            }
            else if (_creatureManager.CurrentState == CreatureState.OnComeBack)
            {
                _enemyHit = null;
            }
            #endregion
        }

        private bool EnemyInRange(Vector2 hit, Transform ennemy, Vector2 origin, float range, bool raycastOn)
        {
            if (ennemy && Vector2.Distance(hit, origin) < range)
            {
                if (raycastOn)
                {
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
                Gizmos.DrawWireSphere(_originRayInAir.position, _inAirRadius.Value);
            if (_originRayOnWall != null && _creatureManager.CurrentState == CreatureState.OnWall)
                Gizmos.DrawWireSphere(_originRayOnWall.position, _onWallRadius.Value);
        } 


        #region GETTERS
        public Transform Enemy => _enemyHit;

        #endregion
    }
}
