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

        private static readonly int _hasIsMoving = Animator.StringToHash("IsMoving");
        private static readonly int _hasIsDashing = Animator.StringToHash("IsDashing");
        private static readonly int _hasIsJumping = Animator.StringToHash("IsJumping");
        private static readonly int _hasIsGrabing = Animator.StringToHash("IsGrabing");
        private static readonly int _hasIsClimbing = Animator.StringToHash("IsClimbing");
        private static readonly int _hasIsSliding = Animator.StringToHash("IsSliding");
        private static readonly int _hasIsAttacking = Animator.StringToHash("IsAttacking");
        private static readonly int _hasOnRightWall = Animator.StringToHash("OnRightWall");
        private static readonly int _hasCanMove = Animator.StringToHash("CanMove");

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
            _anim.SetBool(_hasIsMoving, _coll.OnGround);
            _anim.SetBool(_hasIsClimbing, _coll.OnWall);
            _anim.SetBool(_hasIsGrabing, _move.WallGrab);
            _anim.SetBool(_hasIsSliding, _move.WallSlide);
            _anim.SetBool(_hasCanMove, _move.CanMove);
            _anim.SetBool(_hasOnRightWall, _coll.OnRightWall);

            _anim.SetTrigger(_hasIsDashing);
            _anim.SetTrigger(_hasIsJumping);

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