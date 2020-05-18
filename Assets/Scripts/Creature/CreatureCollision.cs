using Cursed.Traps;
using Cursed.Props;
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
        private Transform _hitTransform = null;
        private bool _alreadyExitFromLaser;
        private bool _alreadyExitFromDoorSwitch;
        private CreatureVfxHandler _creatureVfx;

        private void Awake()
        {
            _creatureManager = GetComponentInParent<CreatureManager>();
            _animator = GetComponentInParent<Animator>();
            _creatureSearching = GetComponentInParent<CreatureSearching>();
            _canCollideWithPlayer = _canCollideWithEnemy = true;
            _creatureVfx = GetComponentInParent<CreatureVfxHandler>();
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
            creatureOnCharacter.transform.localEulerAngles = Vector3.zero;
            creatureOnCharacter.GetComponent<SpriteRenderer>().flipY = flip;
        }

        private void CollideWithWall()
        {
            _creatureManager.CurrentState = CreatureState.OnWall;
            AkSoundEngine.PostEvent("Play_Creature_HitWall", gameObject);
        }

        public void CheckCreatureOnObject()
        {
            if (_hitTransform != null)
            {
                if (_hitTransform.gameObject.GetComponent<DoorSwitch>() != null && !_alreadyExitFromDoorSwitch)
                {
                    _hitTransform.gameObject.GetComponent<DoorSwitch>().ToggleDoors();
                    _alreadyExitFromDoorSwitch = true;
                }

                if (_hitTransform.gameObject.GetComponent<EndLaserBeam>() != null && !_alreadyExitFromLaser)
                {
                    _hitTransform.gameObject.GetComponent<EndLaserBeam>()._laserBeam.ActiveLaser();
                    _hitTransform.gameObject.GetComponent<Collider2D>().enabled = true;
                    _alreadyExitFromLaser = true;
                }
            }
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
                    CollideWithObject(CreatureState.OnDoorSwitch, collision.transform, true);
                    _alreadyExitFromDoorSwitch = false;
                }
            }
            if (collision.gameObject.GetComponent<EndLaserBeam>())
            {
                if (_creatureManager.CurrentState != CreatureState.OnComeBack)
                {
                    if (_creatureManager.CurrentState != CreatureState.OnLaser)
                    {
                        collision.gameObject.GetComponent<EndLaserBeam>()._laserBeam.DeActiveLaser();
                        collision.gameObject.GetComponent<Collider2D>().enabled = false;
                        CollideWithObject(CreatureState.OnLaser, collision.transform, true);
                        _alreadyExitFromLaser = false;
                    }
                }
            }
            if(collision.gameObject.GetComponent<CameraRotation>())
            {
                // LAUNCH SFX & VFX EFFECTS
                _creatureVfx.DestructionCamera(collision.gameObject.transform.position);

                Destroy(collision.gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            /*if (_creatureManager.CurrentState == CreatureState.OnComeBack && _hitTransform != null)
            {
                if (_hitTransform.gameObject.GetComponent<DoorSwitch>() != null && !_alreadyExitFromDoorSwitch)
                {
                    _hitTransform.gameObject.GetComponent<DoorSwitch>().ToggleDoors();
                    _alreadyExitFromDoorSwitch = true;
                }

                if (_hitTransform.gameObject.GetComponent<EndLaserBeam>() != null && !_alreadyExitFromLaser)
                {
                    _hitTransform.gameObject.GetComponent<EndLaserBeam>()._laserBeam.ActiveLaser();
                    _alreadyExitFromLaser = true;
                }
            }*/
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