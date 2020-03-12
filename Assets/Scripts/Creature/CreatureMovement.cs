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
                // ON WALL
                if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AC_GoFromWall")
                {
                    RotateToTarget(_playerPosition.GetChild(0), false);
                    MoveToTarget(_playerPosition.GetChild(0), _creatureStats.CurrentMoveSpeedChaseAndComeBack);
                    _joystick.Direction = Vector2.zero;
                }
                /*else
                {
                    RotateToDirection(_collision.WallDirection);
                    _rb.velocity = Vector2.zero;
                    _rb.angularVelocity = 0f;
                }*/

                // ON ENEMY
               /* if(_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AC_GoFromCharacter")
                {
                    RotateToTarget(_playerPosition.GetChild(0), false);
                    MoveToTarget(_playerPosition.GetChild(0), _creatureStats.CurrentMoveSpeedChaseAndComeBack);
                    _joystick.Direction = Vector2.zero;
                }
                else
                {
                    RotateToTarget(_playerPosition.GetChild(0), false);
                    MoveToTargetPosition(_creatureSearching.Enemy.position + new Vector3(0f, 2.5f, 0f), 150f);
                }*/

                // CHECK IF CREATURE IS ON PLAYER
                if (this.transform.position == _playerPosition.GetChild(0).position)
                    _collision.CollideWithCharacter(CreatureState.OnCharacter, _playerPosition);
            }
            #endregion

            #region MOVING
            if (_creatureManager.CurrentState == CreatureState.Moving)
            {
                if (_joystick.Direction != Vector3.zero)
                {
                    //RotateToDirection(_joystick.Direction);
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
                    //RotateToDirection(_direction);
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
            }
            #endregion

            #region ON WALL
            if(_creatureManager.CurrentState == CreatureState.OnWall)
            {
                StuckOnWall();
            }

            #endregion

            #region CHASING
            if (_creatureManager.CurrentState == CreatureState.Chasing)
            {
                MoveToTarget(_creatureSearching.Enemy.GetChild(0), _creatureStats.CurrentMoveSpeedChaseAndComeBack);
                RotateToTarget(_creatureSearching.Enemy.GetChild(0), false);

                /*if (this.transform.position == _creatureSearching.Enemy.GetChild(0).position)
                    _collision.CollideWithCharacter(CreatureState.OnEnemy, _creatureSearching.Enemy);*/
            }
            #endregion

            #region ON CHARACTER
            if (_creatureManager.CurrentState == CreatureState.OnCharacter)
            {
                MoveToTargetPosition(_playerPosition.position + new Vector3(0f, 2.5f, 0f), 150f);
                _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            else
                _rb.constraints = RigidbodyConstraints2D.None;

            #endregion

            #region ON ENEMY
            if(_creatureManager.CurrentState == CreatureState.OnEnemy)
            {
                if (_creatureSearching.Enemy != null)
                {
                    MoveToTargetPosition(_creatureSearching.Enemy.position + new Vector3(0f, 2.5f, 0f), 150f);
                    _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
                else
                    _creatureManager.CurrentState = CreatureState.OnComeBack;
            }
            else
                _rb.constraints = RigidbodyConstraints2D.None;
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

        public void MoveToTargetPosition(Vector3 target, float speed)
        {
            _rb.velocity = new Vector2(0f, 0f);
            transform.position = Vector3.MoveTowards(this.transform.position, target, speed * Time.deltaTime);
        }

        public void StuckOnWall()
        {
            RotateToDirection(-_collision.WallDirection);
            _rb.velocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _creatureManager.CurrentState = CreatureState.OnWall;
        }
        #endregion

        #region ROTATE FUNCTIONS
        private void RotateToDirection(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Euler(rotation.eulerAngles);
        }

        private void RotateToDirection(int direction)
        {
            if (direction == 1)
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            else if (direction == -1)
                transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }

        private void RotateToTarget(Transform target, bool lerp)
        {
            Vector2 direction = target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            if (lerp)
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 20f * Time.deltaTime);
            else
                transform.rotation = Quaternion.Euler(rotation.eulerAngles);
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
