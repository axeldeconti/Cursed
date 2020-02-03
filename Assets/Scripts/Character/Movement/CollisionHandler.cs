using System;
using UnityEngine;

namespace Cursed.Character
{
    public class CollisionHandler : MonoBehaviour
    {
        [Space]
        [Header("Layers")]
        [SerializeField] private LayerMask _groundLayer;

        [Space]
        [Header("Booleans")]
        [SerializeField] private bool _onGround = false;
        [SerializeField] private bool _onWall = false;
        [SerializeField] private bool _onRightWall = false;
        [SerializeField] private bool _onLeftWall = false;
        private bool _lastGrounded = false;
        private bool _lastWalled = false;

        [Space]
        [SerializeField] private int _wallSide;

        [Space]
        [Header("Collision")]
        [SerializeField] private float _collisionRadius = 0.25f;
        [SerializeField] private Vector2 _bottomOffset, _rightOffset, _leftOffset;
        private Color _debugCollisionColor = Color.red;

        public Action OnGrounded;
        public Action OnWalled;

        private void FixedUpdate()
        {
            //Grounded if there is something of ground layer beneath
            _onGround = Physics2D.OverlapCircle((Vector2)transform.position + _bottomOffset, _collisionRadius, _groundLayer);
            if (OnGround && !_lastGrounded)
                OnGrounded?.Invoke();
            _lastGrounded = _onGround;

            //On a wall if there is something of ground layer on the right or on the left
            _onWall = Physics2D.OverlapCircle((Vector2)transform.position + _rightOffset, _collisionRadius, _groundLayer) || Physics2D.OverlapCircle((Vector2)transform.position + _leftOffset, _collisionRadius, _groundLayer);
            if (OnWall && !_lastWalled)
                OnWalled?.Invoke();
            _lastWalled = _onWall;

            if (_onWall)
            {
                //Witch wall 
                _onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + _rightOffset, _collisionRadius, _groundLayer);
                _onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + _leftOffset, _collisionRadius, _groundLayer);
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
            Gizmos.color = _debugCollisionColor;

            var positions = new Vector2[] { _bottomOffset, _rightOffset, _leftOffset };

            Gizmos.DrawWireSphere((Vector2)transform.position + _bottomOffset, _collisionRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + _rightOffset, _collisionRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + _leftOffset, _collisionRadius);
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