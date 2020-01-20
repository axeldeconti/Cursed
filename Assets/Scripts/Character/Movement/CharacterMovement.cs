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

        [SerializeField] private CharacterState _state = CharacterState.Idle;
        [SerializeField] private IInputController _input = null;

        [Space]
        [Header("Stats")]
        [SerializeField] private FloatReference _speed;
        [SerializeField] private FloatReference _gravity;
        [Range (0,1)]
        [SerializeField] private float _airControl;
        [SerializeField] private FloatReference _jumpForce;
        [SerializeField] private FloatReference _wallClimbMultiplySpeed;
        [SerializeField] private FloatReference _wallSlideSpeed;
        [Range(0, 5)]
        [SerializeField] private float _wallJumpLerp;
        [SerializeField] private FloatReference _dashSpeed;
        [SerializeField] private FloatReference _dashCooldown;
        [SerializeField] private FloatReference _dashInvincibilityFrame;
        [SerializeField] private FloatReference _coyoteTimeForJump;
        [SerializeField] private FloatReference _bufferJump;
        [Range(0, 1)]
        [SerializeField] private float _horizontalDampingWhenStopping;
        [Range(0, 1)]
        [SerializeField] private float _horizontalDampingWhenTurning;
        [Range(0, 1)]
        [SerializeField] private float _horizontalDampingBasic;

        [Space]
        [Header("Booleans")]
        [SerializeField] private bool _canMove = true;
        [SerializeField] private bool _wallGrab = false;
        [SerializeField] private bool _wallJumped = false;
        [SerializeField] private bool _wallSlide = false;
        [SerializeField] private bool _wallRun = false;
        [SerializeField] private bool _isDashing = false;
        [SerializeField] private bool _doubleJump = false;
        [SerializeField] private bool _isJumping = false;

        [Space]
        [Header("Unlocks")]
        [SerializeField] private bool _dashUnlock;
        [SerializeField] private bool _doubleJumpUnlock;

        [Space]
        private bool _groundTouch;
        private bool _hasDashed;
        private bool _invincibilityFrame;
        private bool _canEvenJump;
        private bool _jumpWasPressed;

        [Space]
        [SerializeField] private int _side;

        void Start()
        {
            _coll = GetComponent<CollisionHandler>();
            _rb = GetComponent<Rigidbody2D>();
            _betterJump = GetComponent<BetterJumping>();
            _anim = GetComponentInChildren<AnimationHandler>();
            _input = GetComponent<IInputController>();
            _coll.OnGrounded += ResetIsJumping;

            _groundTouch = true;
        }
        
        private void Update()
        {
            //Set and Get input
            float x = _input.x;
            float y = _input.y;

            //Is Grounded
            if (_coll.OnGround)
            {
                //Get direction of the movement and Walk in that direction
                /*
                if (Mathf.Abs(_input.x) < 0.01f)
                    x *= Mathf.Pow(1f - _horizontalDampingWhenStopping, Time.deltaTime * _speed);
                else if (Mathf.Sign(_input.x) != Mathf.Sign(x))
                    x *= Mathf.Pow(1f - _horizontalDampingWhenTurning, Time.deltaTime * _speed);
                else
                    x *= Mathf.Pow(1f - _horizontalDampingBasic, Time.deltaTime * _speed);
                */
                Vector2 dir = new Vector2(x, y);
                Walk(dir);
            }
            else
            {
                //Get direction of the movement if is on air and apply air control
                /*
                if (Mathf.Abs(_input.x) < 0.01f)
                    x *= Mathf.Pow(1f - _horizontalDampingWhenStopping, Time.deltaTime * (_speed / _airControl));
                else if (Mathf.Sign(_input.x) != Mathf.Sign(x))
                    x *= Mathf.Pow(1f - _horizontalDampingWhenTurning, Time.deltaTime * (_speed / _airControl));
                else
                    x *= Mathf.Pow(1f - _horizontalDampingBasic, Time.deltaTime * (_speed / _airControl));
                */
                Vector2 dir = new Vector2(x, y);
                Walk(dir);
            }

            UpdateBools();
            UpdateJump();
            UpdateDash(x);
            UpdateWallGrab(x, y);
            UpdateFlip(x, y);
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
            _canEvenJump = true;

            _side = 1;

            if(_jumpWasPressed)
            {
                Jump(Vector2.up);
                _canEvenJump = false;
            }
        }

        #region Dash

        /// <summary>
        /// Dash in the direction in parameter
        /// </summary>
        private void Dash(float x, float y)
        {
            _hasDashed = true;

            //Reset the velocity
            _rb.velocity = Vector2.zero;

            //Apply new velocity
            Vector2 dir = new Vector2(x, y);
            _rb.velocity += dir.normalized * _dashSpeed;

            //Start dash coroutine
            StartCoroutine(DashWait());
        }

        /// <summary>
        /// Start cooldown dash and invicibility frame
        /// </summary>
        private IEnumerator DashWait()
        {
            //Start the ground dash coroutine
            StartCoroutine(GroundDash());

            //Change the value of the rigidbody drag
            DOVirtual.Float(14, 0, .2f, RigidbodyDrag);

            //Set values for the dash
            _betterJump.enabled = false;
            _wallJumped = true;
            _isDashing = true;
            _invincibilityFrame = true;

            yield return new WaitForSeconds(_dashInvincibilityFrame);

            //Reset values
            _betterJump.enabled = true;
            _wallJumped = false;
            _isDashing = false;
            _invincibilityFrame = false;
        }

        /// <summary>
        /// Use to delay a new dash when grounded
        /// </summary>
        private IEnumerator GroundDash()
        {
            yield return new WaitForSeconds(_dashCooldown);
            if (_coll.OnGround)
                _hasDashed = false;
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
            if ((_rb.velocity.x > 0 && _coll.OnRightWall) || (_rb.velocity.x < 0 && _coll.OnLeftWall))
            {
                pushingWall = true;
            }
            float push = pushingWall ? 0 : _rb.velocity.x;

            //Apply new velocity
            _rb.velocity = new Vector2(push, -_wallSlideSpeed);
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
                _rb.velocity = Vector2.Lerp(_rb.velocity, (new Vector2(dir.x / 2, _rb.velocity.y)), _wallJumpLerp * Time.deltaTime);
            }
        }

        #region Jump

        /// <summary>
        /// Handle jump
        /// </summary>
        private void Jump(Vector2 dir)
        {
            _isJumping = true;

            //Apply jump velocity
            _rb.velocity = new Vector2(_rb.velocity.x, 1);
            _rb.velocity += dir * _jumpForce;
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
                _anim.Flip(_side);
            }

            //Disable movement input
            StopCoroutine(DisableMovement(0));
            StartCoroutine(DisableMovement(.1f));

            //Jump in the right direction
            Vector2 wallDir = _coll.OnRightWall ? Vector2.left : Vector2.right;
            Vector2 dir = Vector2.up / 1f + wallDir;

            Jump(dir);

        }

        /// <summary>
        /// Reste variable for jump
        /// </summary>
        private void ResetIsJumping()
        {
            _isJumping = false;
        }

        /// <summary>
        /// Coyote Time
        /// </summary>
        private IEnumerator CoyoteTimeForJump(float time)
        {
            yield return new WaitForSeconds(time);
            _canEvenJump = false;
        }

        /// <summary>
        /// Input buffer jump
        /// </summary>
        private IEnumerator RememberJumpTime(float time)
        {
            yield return new WaitForSeconds(time);
            _jumpWasPressed = false;
        }

        #endregion

        #region Updates

        /// <summary>
        /// Handle the different jumps
        /// </summary>
        private void UpdateJump()
        {
            if (!_input.Jump)
                return;

            _jumpWasPressed = true;
            StartCoroutine(RememberJumpTime(_bufferJump));

            //If on ground, jump
            if (_coll.OnGround || _canEvenJump)
            {
                _canEvenJump = false;
                _betterJump.fallMultiplier = 3f;
                _betterJump.lowJumpMultiplier = 8f;
                Jump(Vector2.up);
            }

            //If in air, double jump
            if (!_coll.OnWall && !_coll.OnGround && !_doubleJump && _doubleJumpUnlock && !_canEvenJump)
            {
                _doubleJump = true;
                _betterJump.fallMultiplier = 3f;
                _betterJump.lowJumpMultiplier = 8f;
                Jump(Vector2.up);
            }

            //If on wall, wall jump
            if (_coll.OnWall && !_coll.OnGround)
            {
                _betterJump.fallMultiplier = 0f;
                _betterJump.lowJumpMultiplier = 0f;
                WallJump();
            }
        }

        /// <summary>
        /// Handle the dash
        /// </summary>
        private void UpdateDash(float x)
        {
            if (!_input.Dash && _hasDashed && !_groundTouch && !_dashUnlock)
                return;

            if (x != 0)
                Dash(x, 0);

            if (_invincibilityFrame)
                Debug.Log("Invincibility Frame");
        }

        /// <summary>
        /// Handle the wall grab
        /// </summary>
        private void UpdateWallGrab(float x, float y)
        {
            if (_wallGrab && !_isDashing)
            {
                //Set gravity to zero
                _rb.gravityScale = 0;

                if ((x > .2f || x < .2f) && !_isJumping)
                    _rb.velocity = new Vector2(_rb.velocity.x, 0);

                //Wall climb
                if (y > .1f)
                {
                    _wallRun = true;

                    //Apply new velocity
                    if (!_isJumping)
                        _rb.velocity = new Vector2(_rb.velocity.x, y * (_speed * _wallClimbMultiplySpeed));
                }
                else //Slide on wall
                {
                    _wallRun = false;
                    _wallSlide = true;
                    SlideOnWall();
                }
            }
            else
            {
                //Reset gravity
                _rb.gravityScale = _gravity;
            }
        }

        /// <summary>
        /// Update the flip side
        /// </summary>
        private void UpdateFlip(float x, float y)
        {
            //Return if the character can't flip 
            if (_wallGrab || _wallSlide || !_canMove)
                return;

            if (x > 0)
            {
                _side = 1;
                _anim.Flip(_side);

                if (_coll.OnRightWall)
                {
                    Vector2 dir = new Vector2(0, y);
                    Walk(dir);
                }
            }
            if (x < 0)
            {
                _side = -1;
                _anim.Flip(_side);

                if (_coll.OnLeftWall)
                {
                    Vector2 dir = new Vector2(0, y);
                    Walk(dir);
                }
            }
        }

        /// <summary>
        /// Update all boolean values
        /// </summary>
        private void UpdateBools()
        {
            //If is on ground, reset values
            if (_coll.OnGround && !_isDashing)
            {
                _wallJumped = false;
                _betterJump.enabled = true;
            }

            //If on wall and input Grab hold, wall grab
            if (_coll.OnWall && _input.Grab && _canMove)
            {
                _wallGrab = true;
                _wallSlide = false;
                ResetIsJumping();
            }

            //Reset wall grab and fall
            if (!_input.Grab || !_coll.OnWall || !_canMove)
            {
                _wallGrab = false;
                _wallSlide = false;
            }

            //Reset wall slide
            if (!_coll.OnWall || _coll.OnGround)
                _wallSlide = false;

            //Reset double jump
            if (_coll.OnWall || _coll.OnGround)
                _doubleJump = false;

            //Just touch ground
            if (_coll.OnGround && !_groundTouch)
                GroundTouch();

            //Just leave ground
            if (!_coll.OnGround && _groundTouch)
            {
                _groundTouch = false;
                StartCoroutine(CoyoteTimeForJump(_coyoteTimeForJump));
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
        public bool IsJumping => _isJumping;
        public float XSpeed => _rb.velocity.x;
        public float YSpeed => _rb.velocity.y;
        public bool OnGroundTouch => _groundTouch;
        public bool IsGrabing => _wallGrab;
        public bool IsWallRun => _wallRun;

        public CharacterState State => _state;

        #endregion
    }
}