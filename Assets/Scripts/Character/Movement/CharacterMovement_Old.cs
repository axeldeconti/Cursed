﻿using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Cursed.Character
{
    [RequireComponent(typeof(CollisionHandler))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(AnimationHandler))]
    public class CharacterMovement_Old : MonoBehaviour
    {
        private CollisionHandler _coll = null;
        private Rigidbody2D _rb = null;
        private AnimationHandler _anim = null;
        private IInputController _input = null;

        [SerializeField] private CharacterMovementState _state = CharacterMovementState.Idle;

        [Space]
        [Header("Stats")]
        [SerializeField] private FloatReference _runSpeed;
        [SerializeField] private FloatReference _airControl;
        [SerializeField] private FloatReference _wallClimbMultiplySpeed;
        [SerializeField] private FloatReference _wallSlideSpeed;
        [SerializeField] private FloatReference _walkInertia;

        [Space]
        [Header("Dash")]
        [SerializeField] private FloatReference _dashDistance;
        [SerializeField] private FloatReference _dashTime;
        [SerializeField] private FloatReference _dashCooldown;

        [Space]
        [Header("Jump")]
        [SerializeField] private JumpData _normalJump = null;
        [SerializeField] private JumpData _lightJump = null;
        [SerializeField] private JumpData _doubleJump = null;
        [SerializeField] private JumpData _wallJump = null;
        [SerializeField] private JumpData _fastFall = null;
        [SerializeField] private FloatReference _coyoteTime;

        [Space]
        [Header("Booleans")]
        [SerializeField] private bool _canMove = true;
        [SerializeField] private bool _isJumping = false;
        [SerializeField] private bool _hasDoubleJumped = false;
        [SerializeField] private bool _hasWallJumped = false;
        [SerializeField] private bool _wallGrab = false;
        [SerializeField] private bool _wallSlide = false;
        [SerializeField] private bool _wallRun = false;
        [SerializeField] private bool _isDashing = false;
        [SerializeField] private bool _canDash = false;
        [SerializeField] private bool _isInvincible = false;

        [Space]
        [Header("Unlocks")]
        [SerializeField] private bool _dashUnlock = false;
        [SerializeField] private bool _doubleJumpUnlock = false;

        [Space]
        private bool _groundTouch;
        private bool _hasDashed;
        private bool _canStillJump;
        private bool _jumpWasPressed;
        private int _side;
        private Vector2 _vel = Vector2.zero;
        private float _timeToNextDash = 0f;
        private bool _isCoyoteTime;

        [Space]
        private float _currentGravity = 0f;
        private Vector2 _currentVelocity = Vector2.zero;

        void Start()
        {
            _coll = GetComponent<CollisionHandler>();
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponentInChildren<AnimationHandler>();
            _input = GetComponent<IInputController>();
            _coll.OnGrounded += ResetIsJumping;

            _groundTouch = true;
            _canStillJump = true;

            CursedDebugger.Instance.Add("State", () => _state.ToString());
            CursedDebugger.Instance.Add("Can still jump", () => _canStillJump.ToString());
        }

        private void Update()
        {
            //Get input
            float x = _input.x;
            float y = _input.y;

            UpdateBools();
            UpdateWallGrab(x, y);
            UpdateJump();
            UpdateDash(x);
            UpdateFlip(Mathf.Abs(x) <= .1f ? _currentVelocity.x : x);

            //Set current velocity
            _currentVelocity = _rb.velocity;

            UpdateState();
        }

        private void FixedUpdate()
        {
            //Get input
            float x = _input.x;
            float y = _input.y;

            UpdateGravity();
            UpdateWalk(x);
            UpdateAirControl(x);

            //Clamp y velocity to not to fall to fast
            UpdateVelocity(_rb.velocity.x, Mathf.Clamp(_rb.velocity.y, -60f, 60f));
        }

        /// <summary>
        /// Call when just touch ground
        /// </summary>
        private void GroundTouch()
        {
            //Reset grounded values
            _groundTouch = true;
            _hasDashed = false;
            _isDashing = false;
            _canStillJump = true;
            _hasDoubleJumped = false;
            _hasWallJumped = false;
            _canStillJump = true;

            StopCoroutine("CoyoteTime");
            _isCoyoteTime = false;
        }

        #region Dash

        /// <summary>
        /// Dash in the direction in parameter
        /// </summary>
        private IEnumerator Dash(float x)
        {
            //Reset the velocity
            UpdateVelocity(0f, 0f);

            //Set bools
            _isDashing = true;
            _isInvincible = true;

            float dashTimer = _dashTime;
            float deltaDist = _side * _dashDistance * 10 * (1 / (float)GameSettings.FRAME_RATE) / _dashTime;
            float newX = 0f;

            while(dashTimer >= 0)
            {
                if (!CheckIfCanDash())
                    break;

                newX = transform.position.x + deltaDist;
                transform.position = new Vector2(newX, transform.position.y);
                dashTimer -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            //Reset bools
            _isDashing = false;
            _isInvincible = false;

            //Set dash cooldown
            _timeToNextDash = Time.time + _dashCooldown;
        }

        private bool CheckIfCanDash()
        {
            int i = Physics2D.RaycastAll(new Vector2(0f, 1.5f) + (Vector2)transform.position, _side * Vector2.right, 3f, LayerMask.GetMask("Ground")).Length;

            bool canDash = i == 0;

            return canDash;
        }

        #endregion

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
            if ((_currentVelocity.x > 0 && _coll.OnRightWall) || (_currentVelocity.x < 0 && _coll.OnLeftWall))
            {
                pushingWall = true;
            }
            float push = pushingWall ? 0 : _currentVelocity.x;

            //Apply new velocity
            UpdateVelocity(push, -_wallSlideSpeed);
        }

        /// <summary>
        /// Handle the walk
        /// </summary>
        private void Walk(float x)
        {
            //Walk
            Vector2 v = Vector2.SmoothDamp(_currentVelocity, new Vector2(x * _runSpeed, _rb.velocity.y), ref _vel, _walkInertia);
            UpdateVelocity(v.x, _rb.velocity.y);
        }

        #region Jump

        /// <summary>
        /// Handle jump
        /// </summary>
        private void Jump(JumpData jump)
        {
            _isJumping = true;
            _input.Jump.Reset();

            //Apply jump velocity
            UpdateVelocity(_currentVelocity.x, jump.InitialVelocity(_runSpeed));
        }

        /// <summary>
        /// Call to do a wall jump
        /// </summary>
        private void WallJump()
        {
            _hasWallJumped = true;
            _canStillJump = false;

            //Disable movement input
            StopCoroutine(DisableMovement(0));
            StartCoroutine(DisableMovement(.1f));

            Jump(_wallJump);
            UpdateVelocity(-_side * _runSpeed, _rb.velocity.y);
        }

        /// <summary>
        /// Reste variable for jump
        /// </summary>
        private void ResetIsJumping()
        {
            _isJumping = false;
            _hasWallJumped = false;
            _hasDoubleJumped = false;
        }

        /// <summary>
        /// Coyote Time
        /// </summary>
        private IEnumerator CoyoteTime(float time)
        {
            _isCoyoteTime = true;

            float timer = time;

            while(_isCoyoteTime || (timer >= 0f))
            {
                timer -= Time.deltaTime;
                yield return null;
            }
            
            _canStillJump = false;
            _isCoyoteTime = false;
        }

        #endregion

        #region Updates

        /// <summary>
        /// Handle the walk
        /// </summary>
        private void UpdateWalk(float x)
        {
            //Return if can't move
            if (!_canMove)
                return;

            //Don't need to walk if wall grabbing of in the air
            if (_wallGrab || !_coll.OnGround)
                return;

            Walk(x);
        }

        private void UpdateAirControl(float x)
        {
            //No air control if on ground or on the wall
            if (_coll.OnGround || _wallGrab || _wallRun)
                return;

            //Apply x velocity during a wall jump
            //Wall jump air control
            float X = Mathf.Clamp(_currentVelocity.x + x * _runSpeed, -_runSpeed * .9f, _runSpeed * .9f) ;
            Vector2 v = Vector2.Lerp(_currentVelocity, new Vector2(X, _currentVelocity.y), _airControl * Time.deltaTime);
            UpdateVelocity(v.x, _rb.velocity.y);
        }

        /// <summary>
        /// Handle the different jumps
        /// </summary>
        private void UpdateJump()
        {
            if (!_input.Jump.Value)
                return;

            if (_isDashing)
                return;

            _jumpWasPressed = true;

            //If in air, double jump
            if (!_coll.OnWall && !_coll.OnGround && !_hasDoubleJumped && _doubleJumpUnlock && !_canStillJump)
            {
                _hasDoubleJumped = true;
                _hasWallJumped = false;
                Jump(_doubleJump);
            }

            //If on ground, jump
            if ((_coll.OnGround || _canStillJump) && !_wallGrab)
            {
                _canStillJump = false;
                StopCoroutine("CoyoteTime");
                _isCoyoteTime = false;
                Jump(_normalJump);
            }

            //If on wall, wall jump
            if (_coll.OnWall && !_coll.OnGround && _wallGrab)
                WallJump();
        }

        /// <summary>
        /// Update the gravity depending on what state is the character
        /// </summary>
        private void UpdateGravity()
        {
            //Set to default to Normal jump
            _currentGravity = _normalJump.Gravity(_runSpeed);

            //Going upward
            if (_currentVelocity.y >= 0.1f)
            {
                if (_isJumping)
                {
                    if (_hasDoubleJumped)
                    {
                        //Double jump
                        _currentGravity = _doubleJump.Gravity(_runSpeed);
                    }
                    else if (_hasWallJumped)
                    {
                        //Wall jump
                        _currentGravity = _wallJump.Gravity(_runSpeed);
                    }
                    
                    if (!_input.HoldJump)
                    {
                        //Light jump
                        _currentGravity = _lightJump.Gravity(_runSpeed);
                    }
                }
            }
            else if (_currentVelocity.y <= 0.1f)//Going downward
            {
                //Fast fall
                _currentGravity = _fastFall.Gravity(_runSpeed);
            }

            ////Wall climb
            //if (_wallGrab && !_isDashing)
            //    _currentGravity = 0f;

            _rb.gravityScale = _currentGravity;
        }

        /// <summary>
        /// Handle the dash
        /// </summary>
        private void UpdateDash(float x)
        {
            _canDash = !_isDashing && (Time.time >= _timeToNextDash);

            if (!CheckIfCanDash())
                return;

            if (!_input.Dash.Value || !_canDash || !_groundTouch || !_dashUnlock)
                return;

            if (x != 0)
                StartCoroutine(Dash(x));
        }

        /// <summary>
        /// Handle the wall grab
        /// </summary>
        private void UpdateWallGrab(float x, float y)
        {
            if (_wallGrab && !_isDashing && !_isJumping)
            {
                if (x > .2f || x < .2f)
                    UpdateVelocity(_currentVelocity.x, 0);

                if ((_side == 1 && _coll.OnRightWall) || (_side == -1 && !_coll.OnRightWall))
                    return;

                //Wall climb
                if (y > .1f)
                {
                    _wallRun = true;

                    //Apply new velocity
                    UpdateVelocity(_currentVelocity.x, y * (_runSpeed * _wallClimbMultiplySpeed));
                }
                else //Slide on wall
                {
                    _wallRun = false;
                    _wallSlide = true;
                    SlideOnWall();
                }
            }
        }

        /// <summary>
        /// Update the flip side
        /// </summary>
        private void UpdateFlip(float x)
        {
            //Return if the character can't flip 
            if (_wallGrab || _wallSlide || !_canMove)
                return;

            if (x > .1f)
            {
                _side = 1;
                _anim.Flip(_side);

                if (_coll.OnRightWall)
                    Walk(0);
            }
            if (x < -.1f)
            {
                _side = -1;
                _anim.Flip(_side);

                if (_coll.OnLeftWall)
                    Walk(0);
            }
        }

        /// <summary>
        /// Update all boolean values
        /// </summary>
        private void UpdateBools()
        {
            //If on wall and input Grab hold, wall grab
            if (_coll.OnWall && _input.HoldRightTrigger && _canMove)
            {
                _wallGrab = true;
                _wallSlide = false;
                ResetIsJumping();
            }

            //Reset wall grab and fall
            if (!_input.HoldRightTrigger || !_coll.OnWall || !_canMove)
            {
                _wallGrab = false;
                _wallSlide = false;
            }

            //Reset wall slide
            if (!_coll.OnWall || _coll.OnGround)
                _wallSlide = false;

            //reset wall run if not on wall or not input grab
            if (_wallRun && (!_coll.OnWall || !_input.HoldRightTrigger))
                _wallRun = false;

            //Reset double jump
            if (_coll.OnWall || _coll.OnGround)
                _hasDoubleJumped = false;

            //Just touch ground
            if (_coll.OnGround && !_groundTouch)
                GroundTouch();

            //Just leave ground
            if (!_coll.OnGround && _groundTouch)
            {
                _groundTouch = false;

                //Start coyote time if falling from a cliff
                if(!_isJumping)
                    StartCoroutine(CoyoteTime(_coyoteTime));
            }

            //Reset can still jump when on the ground
            if (_coll.OnGround && !_canStillJump && !_isJumping)
            {
                _canStillJump = true;
            }
        }

        /// <summary>
        /// Update the current velocity and the rigidBody one
        /// </summary>
        private void UpdateVelocity(float Vx, float Vy)
        {
            _rb.velocity = new Vector2(Vx, Vy);
        }

        /// <summary>
        /// Update the state of the character
        /// </summary>
        private void UpdateState()
        {
            _state = CharacterMovementState.Idle;

            if (Mathf.Abs(_currentVelocity.x) >= .1)
                _state = CharacterMovementState.Run;

            if (IsJumping)
                _state = CharacterMovementState.Jump;

            if (IsDashing)
                _state = CharacterMovementState.Dash;

            if (_currentVelocity.y <= -.1f)
                _state = CharacterMovementState.Fall;

            if (_wallGrab)
            {
                if (_currentVelocity.y > 0f)
                    _state = CharacterMovementState.WallRun;
                else
                    _state = CharacterMovementState.WallSlide;
            } 
        }

        #endregion

        /// <summary>
        /// Disable the movement input for the duration in parameter
        /// </summary>
        private IEnumerator DisableMovement(float time)
        {
            _canMove = false;
            yield return new WaitForSeconds(time);
            _canMove = true;
        }

        #region Getters & Setters

        public bool WallGrab => _wallGrab;
        public bool WallSlide => _wallSlide;
        public bool CanMove => _canMove;
        public bool IsDashing => _isDashing;
        public bool IsJumping => _isJumping;
        public float XSpeed => _currentVelocity.x;
        public float YSpeed => _currentVelocity.y;
        public bool OnGroundTouch => _groundTouch;
        public bool IsGrabing => _wallGrab;
        public bool IsWallRun => _wallRun;
        public bool IsInvincible => _isInvincible;
        public int Side => _side;

        public CharacterMovementState State => _state;

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(0, 1.5f, 0f) + transform.position, new Vector3(_side * 3f, 1.5f, 0f) + transform.position);
        }
    }
}