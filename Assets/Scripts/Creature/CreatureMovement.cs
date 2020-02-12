using UnityEngine;
using Cursed.Utilities;

namespace Cursed.Creature
{
    public class CreatureMovement : MonoBehaviour
    {
        private Transform _playerPosition;

        private CreatureStats _creatureStats;
        private Rigidbody2D _rb;
        private CreatureManager _creatureManager;
        private CreatureSearching _creatureSearching;
        private CreatureJoystickDirection _joystick;
        private CreatureCollision _collision;
        private Animator _animator;
        private int _direction;
        private float _impulseTimer;
        private bool _alreadyImpulse;

        #region INIT
        private void Start()
        {

            _playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
            _creatureStats = GetComponent<CreatureStats>();
            _rb = GetComponent<Rigidbody2D>();
            _creatureManager = GetComponent<CreatureManager>();
            _creatureSearching = GetComponent<CreatureSearching>();
            _joystick = GetComponent<CreatureJoystickDirection>();
            _collision = GetComponentInChildren<CreatureCollision>();
            _animator = GetComponent<Animator>();
        }
        #endregion

        private void Update()
        {
            #region COME BACK
            if (_creatureManager.CurrentState == CreatureState.OnComeBack)
            {
                MoveToTarget(_playerPosition.GetChild(0), _creatureStats.CurrentMoveSpeedChaseAndComeBack);
                RotateToTarget(_playerPosition.GetChild(0));

                _joystick.Direction = Vector2.zero;
            }
            #endregion

            #region MOVING
            if (_creatureManager.CurrentState == CreatureState.Moving)
            {
                if (_joystick.Direction != Vector3.zero)
                {
                    RotateToDirection(_joystick.Direction);
                    if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AC_GoFromCharacter")
                    {
                        MoveToDirection(_joystick.Direction);
                    }
                    else
                    {
                        RotateToDirection(_joystick.Direction);
                        MoveToTarget(_playerPosition.GetChild(0), 150f);
                    }
                }
                else
                {
                    RotateToDirection(_direction);
                    if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AC_GoFromCharacter")
                    {
                        //_rb.AddForce(Vector2.left * _direction);
                        MoveToDirection(_direction);
                    }
                    else
                    {
                        RotateToDirection(_direction);
                        MoveToTarget(_playerPosition.GetChild(0), 150f);
                    }
                }
                if(_collision.OnWall)
                {
                    RotateToDirection(-_collision.WallDirection);
                    _rb.velocity = Vector2.zero;
                    _rb.angularVelocity = 0f;
                    _creatureManager.CurrentState = CreatureState.OnWall;
                }
            }
            #endregion

            #region CHASING
            if (_creatureManager.CurrentState == CreatureState.Chasing)
            {
                MoveToTarget(_creatureSearching.Enemy.GetChild(0), _creatureStats.CurrentMoveSpeedChaseAndComeBack);
                RotateToTarget(_creatureSearching.Enemy.GetChild(0));
            }
            #endregion

            #region ON CHARACTER
            if (_creatureManager.CurrentState == CreatureState.OnCharacter)
                MoveToTarget(_playerPosition.GetChild(0), 150f);
            #endregion
        }

        #region MOVE FUNCTIONS
        public void MoveToDirection(Vector2 direction)
        {
            _rb.velocity = direction * _creatureStats.CurrentMoveSpeedInAir;
            RotateToDirection(direction);
        }

        public void MoveToDirection(int direction)
        {
            _rb.velocity = new Vector2(direction * _creatureStats.CurrentMoveSpeedInAir, _rb.velocity.y);
            RotateToDirection(direction);
        }

        public void MoveToTarget(Transform target, float speed)
        {
            _rb.velocity = new Vector2(0f,0f);
            transform.position = Vector3.MoveTowards(this.transform.position, target.position, speed * Time.deltaTime);
        }
        #endregion

        #region ROTATE FUNCTIONS
        private void RotateToDirection(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void RotateToDirection(int direction)
        {
            if (direction == 1)
                transform.rotation = Quaternion.AngleAxis(0f, Vector3.forward);
            else if (direction == -1)   
                transform.rotation = Quaternion.AngleAxis(180f, Vector3.forward);
        }

        private void RotateToTarget(Transform target)
        {
            Vector2 direction = target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 20f * Time.deltaTime);
        }

        public void RotateToAngle(float angle)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 20f * Time.deltaTime);
        }
        #endregion

        #region GETTERS & SETTERS
        public int Direction 
        {
            get => _direction;
            set => _direction = value;
        }

        #endregion
    }
}
