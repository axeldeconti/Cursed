using System.Collections;
using UnityEngine;
using Cursed.Character;

namespace Cursed.Creature
{
    public class CreatureJoystickDirection : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _minLeftAngle = -150;
        [SerializeField] private int _minRightAngle = -30;

        [Header("Referencies")]
        [SerializeField] private GameObject _targetLine;

        private GameObject _target;
        private Vector2 _direction;
        private CreatureManager _creature;
        private CreatureInputController _input;
        private CollisionHandler _playerCollision;  
        private Transform _origin;

        private void Awake()
        {
            _creature = GetComponent<CreatureManager>();
            _input = GetComponent<CreatureInputController>();
            _origin = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);
            _playerCollision = GameObject.FindGameObjectWithTag("Player").GetComponent<CollisionHandler>();
        }

        private void Update()
        {
            UpdateDirection();
        }

        private void UpdateDirection()
        {
            if (GameManager.Instance.State == GameManager.GameState.Pause)
                return;

            if (_creature.CurrentState == CreatureState.OnCharacter)
            {
                float mag = Mathf.Clamp01(new Vector2(Input.GetAxis("HorizontalRight"), Input.GetAxis("VerticalRight")).magnitude);
                if (mag > .85f)
                {
                    _direction = Vector2.right * Input.GetAxisRaw("HorizontalRight") + Vector2.up * Input.GetAxisRaw("VerticalRight");
                    float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

                    if (_playerCollision.OnGround)
                    {
                        if (angle < -90f && angle > _minLeftAngle)
                            _direction = new Vector2(Mathf.Cos(_minLeftAngle * Mathf.Deg2Rad), Mathf.Sin(_minLeftAngle * Mathf.Deg2Rad));
                        else if (angle < _minRightAngle && angle >= -90f)
                            _direction = new Vector2(Mathf.Cos(_minRightAngle * Mathf.Deg2Rad), Mathf.Sin(_minRightAngle * Mathf.Deg2Rad));
                    }
                }
                else
                {
                    _direction = Vector2.zero;
                    if (_target != null)
                    {
                        if (_target.GetComponent<CreatureJoystickLine>() != null)
                            _target.GetComponent<CreatureJoystickLine>().LerpSize(true);
                        else
                            Destroy(_target);
                    }
                }


                if (_direction != Vector2.zero && _target == null)
                {
                    if(_origin != null)
                        _target = Instantiate(_targetLine, _origin.position, Quaternion.identity, this.transform);
                    else
                        _origin = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);


                    if (_target.GetComponent<CreatureJoystickLine>() != null) _target.GetComponent<CreatureJoystickLine>().LerpSize(false);
                    //UpdateTargetPosition(_direction);
                }


                if (_target != null)
                {
                    if (_direction != Vector2.zero)
                    {
                        if (_target.GetComponent<CreatureJoystickLine>() != null) _target.GetComponent<CreatureJoystickLine>().LerpSize(false);
                        _target.transform.position = _origin.position;
                        UpdateTargetRotation(_direction);
                    }
                }
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
