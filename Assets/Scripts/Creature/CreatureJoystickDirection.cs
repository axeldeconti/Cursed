using System.Collections;
using UnityEngine;

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

        private void Awake()
        {
            _creature = GetComponent<CreatureManager>();
            _input = GetComponent<CreatureInputController>();
        }

        private void Update()
        {
            if (_creature.CurrentState == CreatureState.OnCharacter)
            {
                _direction = Vector2.right * Input.GetAxisRaw("HorizontalRight") + Vector2.up * Input.GetAxisRaw("VerticalRight");
                if (_direction != Vector2.zero && _target == null && _input.ButtonTriggered)
                {
                    _target = Instantiate(TargetObject, this.transform.position, Quaternion.identity, this.transform);
                    _target.GetComponent<CreatureJoystickLine>().LerpSize(false);
                    UpdateTargetPosition(_direction);
                }
                else if (_direction == Vector2.zero && _target != null)
                    _target.GetComponent<CreatureJoystickLine>().LerpSize(true);


                if (_target != null && _input.ButtonTriggered)
                {
                    if (_direction != Vector2.zero)
                    {
                        _target.GetComponent<CreatureJoystickLine>().LerpSize(false);
                        UpdateTargetRotation(_direction);
                    }
                }
            }
            else
                Destroy(_target);
        }

        private void UpdateTargetPosition(Vector2 dir)
        {
            _target.transform.position = new Vector3(this.transform.position.x + dir.x, this.transform.position.y + dir.y, this.transform.position.z);
        }

        private void UpdateTargetRotation(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _target.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        public Vector2 Direction => _direction;
        public Transform Target => _target.transform;
    }
}
