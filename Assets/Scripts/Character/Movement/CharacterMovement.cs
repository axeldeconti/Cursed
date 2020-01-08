using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Cursed.Character
{
    [RequireComponent(typeof(CollisionHandler))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BetterJumping))]
    public class CharacterMovement : MonoBehaviour
    {
        private CollisionHandler _coll;
        private Rigidbody2D _rb;
        private BetterJumping _betterJump;
        private AnimationHandler _anim;

        [Space]
        [Header("Stats")]
        [SerializeField] private FloatReference _speed;
        [SerializeField] private FloatReference _jumpForce;
        [SerializeField] private FloatReference _slideSpeed;
        [SerializeField] private FloatReference _wallJumpLerp;
        [SerializeField] [Range(0, 1)] private float _wallJumpImpulse;
        [SerializeField] private FloatReference _dashSpeed;

        [Space]
        [Header("Booleans")]
        [SerializeField] private bool _canMove;
        [SerializeField] private bool _wallGrab;
        [SerializeField] private bool _wallJumped;
        [SerializeField] private bool _wallSlide;
        [SerializeField] private bool _isDashing;

        [Space]

        private bool _groundTouch;
        private bool _hasDashed;

        [SerializeField] private int _side;

        [Space]
        [Header("VFX")]
        [SerializeField] private ParticleSystem _dashParticle;
        [SerializeField] private ParticleSystem _jumpParticle;
        [SerializeField] private ParticleSystem _wallJumpParticle;

        [SerializeField] private IInputController _input = null;

        void Start()
        {
            _coll = GetComponent<CollisionHandler>();
            _rb = GetComponent<Rigidbody2D>();
            _betterJump = GetComponent<BetterJumping>();
            _anim = GetComponentInChildren<AnimationHandler>();
            _input = GetComponent<IInputController>();
        }

        void Update()
        {
            //Get input - to put in an input manager
            float x = _input.x;
            float y = _input.y;
            float xRaw = _input.xRaw;
            float yRaw = _input.yRaw;

            //Get direction of the movement
            Vector2 dir = new Vector2(x, y);

            //Walk in that direction
            Walk(dir);

            //Jump
            if (_input.Jump == true)
            {
                //If on ground, jump
                if (_coll.OnGround)
                    Jump(Vector2.up, false);

                //If on wall, wall jump
                if (_coll.OnWall && !_coll.OnGround)
                    WallJump();
            }

            //If is on ground, reset values
            if (_coll.OnGround && !_isDashing)
            {
                _wallJumped = false;
                _betterJump.enabled = true;
            }

            //Dash
            if (_input.Dash == true && !_hasDashed && _groundTouch)
            {
                if (xRaw != 0 || yRaw != 0)
                    Dash(xRaw, 0);
            }

            //If on wall and input Grab hold, wall grab
            if (_coll.OnWall && _input.Grab == true && _canMove)
            {
                if (_side != _coll.WallSide)
                _wallGrab = true;
                _wallSlide = false;
            }

            //Wall grab handler
            if (_wallGrab && !_isDashing)
            {
                //Set gravity to zero
                _rb.gravityScale = 0;

                //??????
                if (x > .2f || x < .2f)
                    _rb.velocity = new Vector2(_rb.velocity.x, 0);

                if(y > .1f)
                {
                    //Apply new velocity
                    _rb.velocity = new Vector2(_rb.velocity.x, y * (_speed * .5f));
                }
                else
                {
                    _wallSlide = true;
                    SlideOnWall();
                }            
            }
            else
            {
                //Reset gravity
                _rb.gravityScale = 3;
            }

            //Reset wall grab and fall
            if (_input.Grab == false || !_coll.OnWall || !_canMove)
            {
                _wallGrab = false;
                _wallSlide = false;
            }

            //Reset wall slide
            if (!_coll.OnWall || _coll.OnGround)
            {
                _wallSlide = false;
            }

            //Just touch ground
            if (_coll.OnGround && !_groundTouch)
            {
                GroundTouch();
                _groundTouch = true;
            }

            //Just leave ground
            if (!_coll.OnGround && _groundTouch)
            {
                _groundTouch = false;
            }

            //Return if the character can't flip 
            if (_wallGrab || _wallSlide || !_canMove)
                return;

            //Handle the flip side
            if (x > 0)
            {
                _side = 1;
            }
            if (x < 0)
            {
                _side = -1;
            }
        }

        /// <summary>
        /// Call when just touch ground
        /// </summary>
        void GroundTouch()
        {
            //Reset grounded values
            _groundTouch = true;
            _hasDashed = false;
            _isDashing = false;

            _side = 1;
        }

        //Dash in the direction in parameter
        private void Dash(float x, float y)
        {
            _hasDashed = true;

            //End all tweens
            Camera.main.transform.DOComplete();

            //Reset the velocity
            _rb.velocity = Vector2.zero;

            //Apply new velocity
            Vector2 dir = new Vector2(x, y);
            _rb.velocity += dir.normalized * _dashSpeed;

            //Start dash coroutine
            StartCoroutine(DashWait());
        }

        private IEnumerator DashWait()
        {

            //Start the ground dash coroutine
            StartCoroutine(GroundDash());

            //Change the value of the rigidbody drag
            DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

            //Set values for the dash
            _rb.gravityScale = 0;
            _betterJump.enabled = false;
            _wallJumped = true;
            _isDashing = true;

            yield return new WaitForSeconds(.3f);

            //Reset values
            _rb.gravityScale = 3;
            _betterJump.enabled = true;
            _wallJumped = false;
            _isDashing = false;
        }

        /// <summary>
        /// Use to delay a new dash when grounded
        /// </summary>
        private IEnumerator GroundDash()
        {
            yield return new WaitForSeconds(.15f);
            if (_coll.OnGround)
                _hasDashed = false;
        }

        /// <summary>
        /// Call to do a wall jump
        /// </summary>
        private void WallJump()
        {
            _wallJumped = true;

            //Flip the character to face the wall
            if ((_side == 1 && _coll.OnRightWall) || (_side == -1 && !_coll.OnRightWall))
            {
                _side *= -1;
            }

            //Disable movement input
            StopCoroutine(DisableMovement(0));
            StartCoroutine(DisableMovement(.1f));

            //Jump in the right direction
            Vector2 wallDir = _coll.OnRightWall ? Vector2.left : Vector2.right;
            Jump((Vector2.up / 1f + wallDir / _wallJumpImpulse), true);
        }

        /// <summary>
        /// Call to slide on a wall
        /// </summary>
        private void SlideOnWall()
        {
            //Return if cannot move
            if (!_canMove)
                return;

            //If pushing against the wall, set x velocity to 0
            bool pushingWall = false;
            if ((_rb.velocity.x > 0 && _coll.OnRightWall) || (_rb.velocity.x < 0 && _coll.OnLeftWall))
            {
                pushingWall = true;
            }
            float push = pushingWall ? 0 : _rb.velocity.x;

            //Apply new velocity
            _rb.velocity = new Vector2(push, -_slideSpeed);
        }

        /// <summary>
        /// Handle the walk
        /// </summary>
        private void Walk(Vector2 dir)
        {
            //Return if can't move
            if (!_canMove)
                return;

            //Don't need to walk if wall grabbing
            if (_wallGrab)
                return;

            if (!_wallJumped)
            {
                //Walk
                _rb.velocity = new Vector2(dir.x * _speed, _rb.velocity.y);
            }
            else
            {
                //Apply x velocity during a wall jump
                _rb.velocity = Vector2.Lerp(_rb.velocity, (new Vector2(dir.x * _speed, _rb.velocity.y)), _wallJumpLerp * Time.deltaTime);
            }
        }

        /// <summary>
        /// Handle jump
        /// </summary>
        private void Jump(Vector2 dir, bool wall)
        {
            //Apply jump velocity
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _rb.velocity += dir * _jumpForce;
        }

        /// <summary>
        /// Disable the movement input for the duration in parameter
        /// </summary>
        private IEnumerator DisableMovement(float time)
        {
            _canMove = false;
            yield return new WaitForSeconds(time);
            _canMove = true;
        }

        /// <summary>
        /// Change the drag parameter of the rigidbody
        /// </summary>
        private void RigidbodyDrag(float x)
        {
            _rb.drag = x;
        }

        #region Getters & Setters

        public bool WallGrab => _wallGrab;
        public bool WallSlide => _wallSlide;
        public bool CanMove => _canMove;
        public bool IsDashing => _isDashing;

        #endregion
    }
}