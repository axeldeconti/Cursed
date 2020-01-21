using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Cursed.Character
{
    [RequireComponent(typeof(CollisionHandler))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(AnimationHandler))]
    public class CharacterMovement : MonoBehaviour
    {
        private CollisionHandler _coll = null;
        private Rigidbody2D _rb = null;
        private AnimationHandler _anim = null;
        private IInputController _input = null;

        [SerializeField] private CharacterState _state = CharacterState.Idle;

        [Space]
        [Header("Stats")]
        [SerializeField] private FloatReference _runSpeed;
        [SerializeField] private FloatReference _airControl;
        [SerializeField] private FloatReference _wallClimbMultiplySpeed;
        [SerializeField] private FloatReference _wallSlideSpeed;

        [Space]
        [Header("Dash")]
        [SerializeField] private FloatReference _dashDistance;
        [SerializeField] private FloatReference _dashTime;
        [SerializeField] private FloatReference _dashCooldown;
        [SerializeField] private FloatReference _dashInvincibilityFrame;
        
        
        //[Range(0, 1)]
        //[SerializeField] private float _horizontalDampingWhenStopping;
        //[Range(0, 1)]
        //[SerializeField] private float _horizontalDampingWhenTurning;
        //[Range(0, 1)]
        //[SerializeField] private float _horizontalDampingBasic;

        [Space]
        [Header("Jump")]
        [SerializeField] private JumpData _normalJump = null;
        [SerializeField] private JumpData _lightJump = null;
        [SerializeField] private JumpData _doubleJump = null;
        [SerializeField] private JumpData _wallJump = null;
        [SerializeField] private JumpData _fastFall = null;
        [Range(0, 5)]
        [SerializeField] private float _wallJumpLerp;
        [SerializeField] private FloatReference _coyoteTime;
        [SerializeField] private FloatReference _jumpBuffer;

        [Space]
        [Header("Booleans")]
        [SerializeField] private bool _canMove = true;
        [SerializeField] private bool _wallGrab = false;
        [SerializeField] private bool _wallJumped = false;
        [SerializeField] private bool _wallSlide = false;
        [SerializeField] private bool _wallRun = false;
        [SerializeField] private bool _isDashing = false;
        [SerializeField] private bool _hasDoubleJumped = false;
        [SerializeField] private bool _isJumping = false;

        [Space]
        [Header("Unlocks")]
        [SerializeField] private bool _dashUnlock = false;
        [SerializeField] private bool _doubleJumpUnlock = false;

        [Space]
        private bool _groundTouch;
        private bool _hasDashed;
        private bool _invincibilityFrame;
        private bool _canEvenJump;
        private bool _jumpWasPressed;
        private int _side;

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
        }
        
        private void Update()
        {
            //Set and Get input
            float x = _input.x;
            float y = _input.y;

            UpdateWalk(x, y);
            UpdateBools();
            UpdateGravity();
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

            //_side = 1;

            if(_jumpWasPressed)
            {
                Jump(_normalJump);
                _canEvenJump = false;
            }
        }

        #region Dash

        /// <summary>
        /// Dash in the direction in parameter
        /// </summary>
        private void Dash(float x)
        {
            _hasDashed = true;

            //Reset the velocity
            UpdateVelocity(0f, 0f);

            //Apply new velocity
            //Vector2 dir = new Vector2(x, y);
            //_rb.velocity += dir.normalized * (_dashDistance / _dashTime);

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
            _wallJumped = true;
            _isDashing = true;
            _invincibilityFrame = true;

            yield return new WaitForSeconds(_dashInvincibilityFrame);

            //Reset values
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
        private void Walk(Vector2 dir)
        {
            if (!_wallJumped)
            {
                //Walk
                UpdateVelocity(dir.x * _runSpeed, _rb.velocity.y);
            }
            else
            {
                //-----Rien à faire ici-----
                //Apply x velocity during a wall jump
                Vector2 v = Vector2.Lerp(_currentVelocity, (new Vector2(dir.x / 2, _currentVelocity.y)), _wallJumpLerp * Time.deltaTime);
                UpdateVelocity(v.x, v.y);
            }
        }

        #region Jump

        /// <summary>
        /// Handle jump
        /// </summary>
        private void Jump(JumpData jump)
        {
            _isJumping = true;

            //Apply jump velocity
            UpdateVelocity(_currentVelocity.x, jump.InitialVelocity(_runSpeed));
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

            Jump(_wallJump);

        }

        /// <summary>
        /// Reste variable for jump
        /// </summary>
        private void ResetIsJumping() => _isJumping = false;

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
        /// Handle the walk
        /// </summary>
        private void UpdateWalk(float x, float y)
        {
            //Return if can't move
            if (!_canMove)
                return;

            //Don't need to walk if wall grabbing
            if (_wallGrab)
                return;

            Vector2 dir = new Vector2(x, y);
            Walk(dir);
        }

        /// <summary>
        /// Handle the different jumps
        /// </summary>
        private void UpdateJump()
        {
            if (!_input.Jump)
                return;

            _jumpWasPressed = true;
            StartCoroutine(RememberJumpTime(_jumpBuffer));

            //If on ground, jump
            if (_coll.OnGround || _canEvenJump)
            {
                _canEvenJump = false;
                Jump(_normalJump);
            }

            //If in air, double jump
            if (!_coll.OnWall && !_coll.OnGround && !_hasDoubleJumped && _doubleJumpUnlock && !_canEvenJump)
            {
                _hasDoubleJumped = true;
                Jump(_doubleJump);
                Debug.Log("Double Jump");
            }

            //If on wall, wall jump
            if (_coll.OnWall && !_coll.OnGround)
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
                    else if (!_input.HoldJump)
                    {
                        //Light jump
                        _currentGravity = _lightJump.Gravity(_runSpeed);
                    }
                }
            }
            else if(_currentVelocity.y <= 0.1f)//Going downward
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
            if (!_input.Dash && _hasDashed && !_groundTouch && !_dashUnlock)
                return;

            //if (x != 0)
            //    Dash(x, 0);

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
                if ((x > .2f || x < .2f) && !_isJumping)
                    UpdateVelocity(_currentVelocity.x, 0);

                //Wall climb
                if (y > .1f)
                {
                    _wallRun = true;

                    //Apply new velocity
                    if (!_isJumping)
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
                //_betterJump.enabled = true;
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
                _hasDoubleJumped = false;

            //Just touch ground
            if (_coll.OnGround && !_groundTouch)
                GroundTouch();

            //Just leave ground
            if (!_coll.OnGround && _groundTouch)
            {
                _groundTouch = false;
                StartCoroutine(CoyoteTimeForJump(_coyoteTime));
            }
        }

        /// <summary>
        /// Update the current velocity and the rigidBody one
        /// </summary>
        private void UpdateVelocity(float Vx, float Vy)
        {
            _currentVelocity = new Vector2(Vx, Vy);
            _rb.velocity = new Vector2(Vx, Vy);
        }

        /// <summary>
        /// Update the state of the character
        /// </summary>
        private void UpdateState()
        {

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
        public float XSpeed => _currentVelocity.x;
        public float YSpeed => _currentVelocity.y;
        public bool OnGroundTouch => _groundTouch;
        public bool IsGrabing => _wallGrab;
        public bool IsWallRun => _wallRun;

        public CharacterState State => _state;

        #endregion
    }
}