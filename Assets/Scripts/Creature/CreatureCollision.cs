using UnityEngine;
using Cursed.Character;

namespace Cursed.Creature
{
    public class CreatureCollision : MonoBehaviour
    {
        [Header ("Referencies")]
        [SerializeField] private GameObject _creatureOnCharacter;
        [SerializeField] private GameObject _creaturePart;
        private bool _onWall;
        private bool _canCollideWithPlayer, _canCollideWithEnemy;
        private Vector2 _ricochetDirection;
        private CreatureManager _creatureManager;
        private CreatureSearching _creatureSearching;
        private Animator _animator;
        private Transform _wallCollision;
        private Vector2 _wallDirection;

        private void Awake()
        {
            _creatureManager = GetComponentInParent<CreatureManager>();
            _animator = GetComponentInParent<Animator>();
            _creatureSearching = GetComponentInParent<CreatureSearching>();
            _canCollideWithPlayer = _canCollideWithEnemy = true;
        }

        public void CollideWithCharacter(CreatureState type, Transform target)
        {
            _creatureManager.CurrentState = type;
            Instantiate(_creatureOnCharacter, target.position, Quaternion.identity, target);
        }


        #region COLLISIONS & TRIGGERS

        // TRIGGERS
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.transform.GetComponent<CharacterMovement>())
            {
                if (collider.gameObject.CompareTag("Player"))
                {
                    _creatureManager.CurrentState = CreatureState.OnCharacter;
                }
                else if (collider.gameObject.CompareTag("Enemy"))
                {
                    _creatureManager.CurrentState = CreatureState.OnEnemy;
                }

                Instantiate(_creatureOnCharacter, collider.transform.position + new Vector3(0f, 0f, 0f), Quaternion.identity, collider.transform);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
            {
                _onWall = true;
                //_creatureManager.CurrentState = CreatureState.OnWall;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _onWall = false;
        }


        // COLLISIONS
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player") && _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AC_GoFromCharacter")
            { 
                CollideWithCharacter(CreatureState.OnCharacter, collision.transform);
                AkSoundEngine.PostEvent("Play_Creature_Grabbing", gameObject);
            }

            if (collision.gameObject.CompareTag("Enemy") && _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AC_GoFromCharacter")
            {
                if (_creatureManager.CurrentState != CreatureState.OnComeBack)
                {
                    if (_creatureSearching.Enemy == null)
                        _creatureSearching.Enemy = collision.transform;

                    CollideWithCharacter(CreatureState.OnEnemy, collision.transform);
                    AkSoundEngine.PostEvent("Play_Creature_Grabbing", gameObject);
                }
            }

            if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
            {
                if (_creatureManager.CurrentState == CreatureState.Moving)
                {
                    _wallDirection = collision.contacts[0].normal;
                    _wallCollision = collision.transform;
                    _creatureManager.CurrentState = CreatureState.OnWall;
                    AkSoundEngine.PostEvent("Play_Creature_HitWall", gameObject);
                }
                //_ricochetDirection = v;
            }
            /*else
                _onWall = false;*/
        }

        #endregion

        #region GETTERS
        public bool OnWall => _onWall;
        public Vector2 RicochetDirection => _ricochetDirection;
        public Transform WallCollision => _wallCollision;
        public Vector2 WallDirection => _wallDirection;

        #endregion
    }
}