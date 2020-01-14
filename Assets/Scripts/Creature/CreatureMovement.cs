using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureMovement : MonoBehaviour
    {
        private CreatureStats _creatureStats;
        private Rigidbody2D _rb;

        private void Start()
        {
            //Init referencies
            _creatureStats = GetComponent<CreatureStats>();
            _rb = GetComponent<Rigidbody2D>();
        }

        public void MoveToDirection(Vector2 direction)
        {
            _rb.velocity = new Vector2(direction.x * _creatureStats.CurrentMoveSpeed, _rb.velocity.y);
        }
    }
}
