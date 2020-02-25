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
        private Vector2 _ricochetDirection;
        private CreatureManager _creatureManager;
        private Transform _wallCollision;
        private Vector2 _wallDirection;

        void Start()
        {
            _creatureManager = GetComponentInParent<CreatureManager>();
        }

        private void Update()
        {
            /*Ray2D ray = new Ray2D(transform.position, transform.right);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 5f);

            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
                Debug.DrawLine(ray.origin, hit.point);
                Vector2 v = Vector2.Reflect(ray.direction, hit.normal);
                float rot = 90 - Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
                Debug.DrawRay(hit.point, v, Color.red);
                transform.eulerAngles = new Vector3(0, 0, rot);
            }*/
        }

        #region COLLISIONS

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

            /*if(collider.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
            {
                _onWall = true;
                //GetComponentInParent<Rigidbody2D>().velocity *= -1f;
            }*/
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.GetComponent<CharacterMovement>())
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    _creatureManager.CurrentState = CreatureState.OnCharacter;
                }
                else if (collision.gameObject.CompareTag("Enemy"))
                {
                    _creatureManager.CurrentState = CreatureState.OnEnemy;
                }

                Instantiate(_creatureOnCharacter, collision.transform.position, Quaternion.identity, collision.transform);
            }

            if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
            {
                _wallDirection = collision.contacts[0].normal;
                _wallCollision = collision.transform;
                _onWall = true;

                //_ricochetDirection = v;
            }
            else
                _onWall = false;
        }
/*
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
            {
                _onWall = true;
                //_creatureManager.CurrentState = CreatureState.OnWall;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            _onWall = false;
        }*/

        #endregion

        #region GETTERS
        public bool OnWall => _onWall;
        public Vector2 RicochetDirection => _ricochetDirection;
        public Transform WallCollision => _wallCollision;
        public Vector2 WallDirection => _wallDirection;

        #endregion
    }
}