﻿using System.Collections;
using UnityEngine;
using Cursed.Character;

namespace Cursed.Creature
{
    public class CreatureJoystickDirection : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _distanceToPlayer = 4f;

        [Header("Referencies")]
        [SerializeField] private GameObject TargetObject;

        private GameObject _target;
        private Vector2 _direction;
        private CreatureManager _creature;
        private CreatureInputController _input;
        private CollisionHandler _playerCollision;

        private void Awake()
        {
            _creature = GetComponent<CreatureManager>();
            _input = GetComponent<CreatureInputController>();
            _playerCollision = GameObject.FindGameObjectWithTag("Player").GetComponent<CollisionHandler>();
        }

        private void Update()
        {
            if (_creature.CurrentState == CreatureState.OnCharacter)
            {
                float mag = Mathf.Clamp01(new Vector2(Input.GetAxis("HorizontalRight"), Input.GetAxis("VerticalRight")).magnitude);
                if (mag > .9f)
                {
                    _direction = Vector2.right * Input.GetAxisRaw("HorizontalRight") + Vector2.up * Input.GetAxisRaw("VerticalRight");
                    float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

                    if (_playerCollision.OnGround)
                    {
                        if (angle < -90f)
                            _direction = -Vector2.right;
                        else if (angle < 0f && angle >= -90f)
                            _direction = Vector2.right;
                    }
                }

                if (_direction != Vector2.zero && _target == null && !_creature.Recall)
                {
                    _target = Instantiate(TargetObject, this.transform.position, Quaternion.identity, this.transform);
                    _target.GetComponent<CreatureJoystickLine>().LerpSize(false);
                    UpdateTargetPosition(_direction);
                }
                /*else if (_direction == Vector2.zero && _target != null)
                    _target.GetComponent<CreatureJoystickLine>().LerpSize(true);*/


                if (_target != null)

                {
                    if (_direction != Vector2.zero)
                    {
                        _target.GetComponent<CreatureJoystickLine>().LerpSize(false);
                        UpdateTargetRotation(_direction);
                    }
                }
                /*else if (!_input.Holding)
                    Destroy(_target);*/
            }
            else
            {
                Destroy(_target);
            }
        }

        private void UpdateTargetPosition(Vector2 dir)
        {
            _target.transform.position = new Vector3(this.transform.position.x + dir.x, this.transform.position.y + dir.y, this.transform.position.z);
        }

        private void UpdateTargetRotation(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            _target.transform.rotation = Quaternion.Euler(rotation.eulerAngles);
        }

        public Vector3 Direction
        {
            get => _direction;
            set => _direction = value;
        }
        public Transform Target => _target.transform;
    }
}