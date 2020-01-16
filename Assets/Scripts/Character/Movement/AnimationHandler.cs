using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Character
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(CollisionHandler))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class AnimationHandler : MonoBehaviour
    {
        private Animator _anim;
        private CharacterMovement _move;
        private CollisionHandler _coll;
        private SpriteRenderer _renderer;

        private static readonly int _hasIsMovingOnX = Animator.StringToHash("MoveSpeedX");
        private static readonly int _hasIsMovingOnY = Animator.StringToHash("MoveSpeedY");
        private static readonly int _hasIsJumping = Animator.StringToHash("IsJumping");
        private static readonly int _hasGroundTouch = Animator.StringToHash("GroundTouch");
        private static readonly int _hasIsDashing = Animator.StringToHash("IsDashing");

        void Start()
        {
            _anim = GetComponent<Animator>();
            _coll = GetComponentInParent<CollisionHandler>();
            _move = GetComponentInParent<CharacterMovement>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            //Update every anim variables
            _anim.SetFloat(_hasIsMovingOnX, Mathf.Abs(_move.XSpeed));
            _anim.SetFloat(_hasIsMovingOnY, Mathf.Clamp(_move.YSpeed, -15, 15));
            _anim.SetBool(_hasGroundTouch, _move.OnGroundTouch);
            _anim.SetBool(_hasIsJumping, _move.IsJumping);
            _anim.SetBool(_hasIsDashing, _move.IsDashing);
        }

        /// <summary>
        /// Set the animator variable for the movement
        /// </summary>
        public void SetHorizontalMovement(float x, float y, float yVel)
        {
            _anim.SetFloat("HorizontalAxis", x);
            _anim.SetFloat("VerticalAxis", y);
            _anim.SetFloat("VerticalVelocity", yVel);
        }

        /// <summary>
        /// Flip the renderer on the X axis
        /// </summary>
        public void Flip(int side)
        {
            if (_move.WallGrab || _move.WallSlide)
            {
                if (side == -1 && _renderer.flipX)
                    return;

                if (side == 1 && !_renderer.flipX)
                    return;
            }

            bool state = (side == 1) ? false : true;
            _renderer.flipX = state;
        }

        public SpriteRenderer Renderer => _renderer;
    }
}