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
            _anim.SetBool("onGround", _coll.OnGround);
            _anim.SetBool("onWall", _coll.OnWall);
            _anim.SetBool("onRightWall", _coll.OnRightWall);
            _anim.SetBool("wallGrab", _move.WallGrab);
            _anim.SetBool("wallSlide", _move.WallSlide);
            _anim.SetBool("canMove", _move.CanMove);
            _anim.SetBool("isDashing", _move.IsDashing);
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
        /// Trigger the animator with the string in parameter
        /// </summary>
        public void SetTrigger(string trigger)
        {
            _anim.SetTrigger(trigger);
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