using System;
using UnityEngine;

namespace Cursed.Character
{
    public class CollisionHandler : MonoBehaviour
    {

        [Header("Layers")]
        [SerializeField] private LayerMask _groundLayer;

        [Space]

        [SerializeField] private bool _onGround;
        [SerializeField] private bool _onWall;
        [SerializeField] private bool _onRightWall;
        [SerializeField] private bool _onLeftWall;
        [SerializeField] private int _wallSide;

        [Space]
        [Header("Collision")]
        public float collisionRadius = 0.25f;
        public Vector2 bottomOffset, rightOffset, leftOffset;
        private Color debugCollisionColor = Color.red;
        public Action OnGrounded;

        void Update()
        {
            //Grounded if there is something of ground layer beneath
            _onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, _groundLayer);
            if (OnGround)
                OnGrounded?.Invoke();

            //On a wall if there is something of ground layer on the right or on the left
            _onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, _groundLayer) || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, _groundLayer);

            if (_onWall)
            {
                //Witch wall 
                _onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, _groundLayer);
                _onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, _groundLayer);
            }
            else
            {
                _onRightWall = false;
                _onLeftWall = false;
            }

            _wallSide = _onRightWall ? -1 : 1;
        }

        void OnDrawGizmos()
        {
            //Draw red circles at the collision locations
            Gizmos.color = Color.red;

            var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

            Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
        }

        #region Getters & Setters 

        public bool OnGround => _onGround;
        public bool OnWall => _onWall;
        public bool OnRightWall => _onRightWall;
        public bool OnLeftWall => _onLeftWall;
        public int WallSide => _wallSide;

        #endregion
    }
}