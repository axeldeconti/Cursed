using Cursed.Traps;
using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureCollision : MonoBehaviour
    {
        [Header("Referencies")]
        [SerializeField] private GameObject _creatureOnCharacter;
        [SerializeField] private GameObject _creaturePart;
        [SerializeField] private LayerMask _wallLayer;
        private bool _onWall;
        private bool _canCollideWithPlayer, _canCollideWithEnemy;
        private Vector2 _ricochetDirection;
        private CreatureManager _creatureManager;
        private CreatureSearching _creatureSearching;
        private Animator _animator;
        private Transform _wallCollision;
        private Vector3 _wallNormalPoint;
        private Vector2 _wallPoint;
        private Transform _hitTransform;

        private void Awake()
        {
            _creatureManager = GetComponentInParent<CreatureManager>();
            _animator = GetComponentInParent<Animator>();
            _creatureSearching = GetComponentInParent<CreatureSearching>();
            _canCollideWithPlayer = _canCollideWithEnemy = true;
        }

        private void FixedUpdate()
        {
            UpdateRaycastWall();
        }

        private void UpdateRaycastWall()
        {
            if (_creatureManager.CurrentState != CreatureState.Moving)
                return;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, _creatureManager.LaunchDirection, 10f, _wallLayer);
            if (hit.collider != null && Vector2.Distance(transform.position, hit.point) <= .6f)
            {
                _wallPoint = hit.point;
                _wallNormalPoint = hit.normal;
                CollideWithWall();
                return;
            }
        }

        public void CollideWithObject(CreatureState type, Transform target, bool flip = false)
        {
            _hitTransform = target;
            _creatureManager.CurrentState = type;
            CreatureOnCharacter creatureOnCharacter = Instantiate(_creatureOnCharacter, target.position, Quaternion.identity, target).GetComponent<CreatureOnCharacter>();
            creatureOnCharacter.GetComponent<SpriteRenderer>().flipY = flip;
        }

        private void CollideWithWall()
        {
            _creatureManager.CurrentState = CreatureState.OnWall;
            AkSoundEngine.PostEvent("Play_Creature_HitWall", gameObject);
        }


        #region COLLISIONS & TRIGGERS

        // TRIGGERS
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player") && _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AC_GoFromCharacter")
            {
                if (_creatureManager.CurrentState == CreatureState.OnComeBack || _creatureManager.CurrentState == CreatureState.OnWall)
                {
                    CollideWithObject(CreatureState.OnCharacter, collision.transform);
                    AkSoundEngine.PostEvent("Play_Creature_Grabbing", gameObject);
                }
            }

            if (collision.gameObject.CompareTag("Enemy") && _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AC_GoFromCharacter")
            {
                if (_creatureManager.CurrentState != CreatureState.OnComeBack)
                {
                    if (_creatureSearching.Enemy == null)
                        _creatureSearching.Enemy = collision.transform;

                    CollideWithObject(CreatureState.OnEnemy, collision.transform);
                    AkSoundEngine.PostEvent("Play_Creature_Grabbing", gameObject);
                }
            }
            if (collision.gameObject.GetComponent<DoorSwitch>())
            {
                if (_creatureManager.CurrentState != CreatureState.OnComeBack)
                {
                    collision.gameObject.GetComponent<DoorSwitch>().ToggleDoors();
                    CollideWithObject(CreatureState.OnDoorSwitch, collision.transform);
                }
            }
            if (collision.gameObject.GetComponent<EndLaserBeam>())
            {
                if (_creatureManager.CurrentState != CreatureState.OnComeBack)
                {
                    _hitTransform = collision.transform;
                    collision.gameObject.GetComponent<EndLaserBeam>()._laserBeam.DeActiveLaser();
                    CollideWithObject(CreatureState.OnLaser, collision.transform, true);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (_creatureManager.CurrentState == CreatureState.OnComeBack)
            {
                if (collision.gameObject.GetComponent<DoorSwitch>())
                {
                    collision.gameObject.GetComponent<DoorSwitch>().ToggleDoors();
                }

                if (collision.gameObject.GetComponent<EndLaserBeam>())
                {
                    collision.gameObject.GetComponent<EndLaserBeam>()._laserBeam.ActiveLaser();
                }
            }
        }

        // COLLISIONS
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player") && _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AC_GoFromCharacter")
            {
                CollideWithObject(CreatureState.OnCharacter, collision.transform);
                AkSoundEngine.PostEvent("Play_Creature_Grabbing", gameObject);
            }

            if (collision.gameObject.CompareTag("Enemy") && _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AC_GoFromCharacter")
            {
                if (_creatureManager.CurrentState != CreatureState.OnComeBack)
                {
                    if (_creatureSearching.Enemy == null)
                        _creatureSearching.Enemy = collision.transform;

                    CollideWithObject(CreatureState.OnEnemy, collision.transform);
                    AkSoundEngine.PostEvent("Play_Creature_Grabbing", gameObject);
                }
            }

            if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
            {
                if (_creatureManager.CurrentState == CreatureState.Moving)
                {
                    _wallNormalPoint = collision.contacts[0].normal;
                    _wallCollision = collision.transform;
                    _creatureManager.CurrentState = CreatureState.OnWall;
                    AkSoundEngine.PostEvent("Play_Creature_HitWall", gameObject);
                }
            }
        }

        #endregion

        #region GETTERS
        public bool OnWall => _onWall;
        public Vector2 RicochetDirection => _ricochetDirection;
        public Vector3 WallNormalPoint => _wallNormalPoint;
        public Vector2 WallPoint => _wallPoint;
        public Transform HitTransform => _hitTransform;

        #endregion
    }
}