using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureMovement : MonoBehaviour
    {
        private Transform playerPosition;

        private CreatureStats _creatureStats;
        private Rigidbody2D _rb;
        private CreatureManager _creatureManager;
        private CreatureSearching _creatureSearching;
        private CreatureJoystickDirection _joystick;
        private Animator _animator;
        private int _direction;

        private void Start()
        {
            //Init referencies
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
            _creatureStats = GetComponent<CreatureStats>();
            _rb = GetComponent<Rigidbody2D>();
            _creatureManager = GetComponent<CreatureManager>();
            _creatureSearching = GetComponent<CreatureSearching>();
            _joystick = GetComponent<CreatureJoystickDirection>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if(_creatureManager.CurrentState == CreatureState.OnComeBack) 
                MoveToTarget(playerPosition.GetChild(0), _creatureStats.CurrentMoveSpeedChaseAndComeBack);

            if (_creatureManager.CurrentState == CreatureState.Moving)
                if (_joystick.Direction != Vector2.zero)
                {
                    RotateToDirection(_joystick.Direction);
                    if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AC_GoFromCharacter")
                        MoveToDirection(_joystick.Direction);
                }
                else
                {
                    RotateToDirection(_direction);
                    if(_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AC_GoFromCharacter")
                        MoveToDirection(_direction);
                }

            if(_creatureManager.CurrentState == CreatureState.Chasing)
                MoveToTarget(_creatureSearching.Enemy.GetChild(0), _creatureStats.CurrentMoveSpeedChaseAndComeBack);

            if (_creatureManager.CurrentState == CreatureState.OnCharacter)
                MoveToTarget(playerPosition.GetChild(0), 50f);

        }

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
            RotateToTarget(target);
        }

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

        // GETTERS & SETTERS
        public int Direction 
        {
            get => _direction;
            set => _direction = value;
        }
    }
}
