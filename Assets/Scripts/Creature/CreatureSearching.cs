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
            _rayStartTransform = GetComponentInChildren<Collider2D>().transform;
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
                foreach (RaycastHit2D hit in obj)
                {
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        if (EnemyInRange(hit.collider.transform, _inAirRadius, true))
                        {
                            _creatureManager.CurrentState = CreatureState.Chasing;
                        }
                    }
                }
            }
            #endregion

            #region ON WALL
            if (_creatureManager.CurrentState == CreatureState.OnWall && _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AC_GoToWall")
            {
                RaycastHit2D[] obj = Physics2D.CircleCastAll(_rayStartTransform.position, _onWallRadius, new Vector2(0f, 0f));
                foreach (RaycastHit2D hit in obj)
                {
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        if (EnemyInRange(hit.collider.transform, _onWallRadius, true))
                        {
                            _creatureManager.CurrentState = CreatureState.Chasing;
                        }
                    }
                }
            }
            #endregion
        }

        private bool EnemyInRange(Transform ennemy, float range, bool raycastOn)
        {
            if (ennemy && Vector3.Distance(ennemy.position, _rayStartTransform.position) < range)
            {
                if (raycastOn)
                {
                    if (Physics2D.Linecast(transform.position, ennemy.position, _checkLayer).collider.gameObject.layer == ennemy.gameObject.layer)
                    {
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            if(_rayStartTransform != null && _creatureManager.CurrentState == CreatureState.Moving)
                Gizmos.DrawWireSphere(_rayStartTransform.position, _inAirRadius);
            if (_rayStartTransform != null && _creatureManager.CurrentState == CreatureState.OnWall)
                Gizmos.DrawWireSphere(_rayStartTransform.position, _onWallRadius);
        } 


        #region GETTERS
        public Transform Enemy => _enemyHit;

        #endregion
    }
}
