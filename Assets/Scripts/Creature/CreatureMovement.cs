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
        private bool _moveToPlayer;
        private bool _moveInTheAir;
        private bool _onPlayer;
        private int _direction;

        private void Start()
        {
            //Init referencies
            _creatureStats = GetComponent<CreatureStats>();
            _rb = GetComponent<Rigidbody2D>();
            _creatureManager = GetComponent<CreatureManager>();
        }

        private void Update()
        {
            if(_moveToPlayer) MoveToTarget(playerPosition.position);
            if(_moveInTheAir) MoveToDirection(_direction);
        }

        public void MoveToDirection(int direction)
        {
            _rb.velocity = new Vector2(direction * _creatureStats.CurrentMoveSpeed, _rb.velocity.y);
        }

        public void MoveToTarget(Vector3 target)
        {
            transform.position = Vector3.MoveTowards(this.transform.position, target, _creatureStats.CurrentMoveSpeed * Time.deltaTime);
        }

        // GETTERS & SETTERS
        public bool MoveToPlayer
        {
            get => _moveToPlayer;
            set => _moveToPlayer = value;
        }
        public int Direction 
        {
            get => _direction;
            set => _direction = value;
        }
        public bool MoveInTheAir
        {
            get => _moveInTheAir;
            set => _moveInTheAir = value;
        }
    }
}
