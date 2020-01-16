using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureMovement : MonoBehaviour
    {
        public Transform playerPosition;

        private CreatureStats _creatureStats;
        private Rigidbody2D _rb;
        private CreatureManager _creatureManager;
        private CreatureSearching _creatureSearching;
        private int _direction;

        private void Start()
        {
            //Init referencies
            _creatureStats = GetComponent<CreatureStats>();
            _rb = GetComponent<Rigidbody2D>();
            _creatureManager = GetComponent<CreatureManager>();
            _creatureSearching = GetComponent<CreatureSearching>();
        }

        private void Update()
        {
            if(_creatureManager.CurrentState == CreatureState.OnComeBack) 
                MoveToTarget(playerPosition.position);
            if(_creatureManager.CurrentState == CreatureState.Moving) 
                MoveToDirection(_direction);
            if(_creatureManager.CurrentState == CreatureState.Chasing)
                MoveToTarget(_creatureSearching.Enemy);
        }

        public void MoveToDirection(int direction)
        {
            _rb.velocity = new Vector2(direction * _creatureStats.CurrentMoveSpeedInAir, _rb.velocity.y);
            //transform.position += (Vector3.right * _creatureStats.CurrentMoveSpeed * direction);
        }

        public void MoveToTarget(Vector3 target)
        {
            transform.position = Vector3.MoveTowards(this.transform.position, target, _creatureStats.CurrentMoveSpeedChaseAndComeBack * Time.deltaTime);
        }

        // GETTERS & SETTERS
        public int Direction 
        {
            get => _direction;
            set => _direction = value;
        }
    }
}
