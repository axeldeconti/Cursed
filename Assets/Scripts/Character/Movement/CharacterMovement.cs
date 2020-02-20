using System.Collections;
using UnityEngine;
using Cursed.VisualEffect;

namespace Cursed.Character
{
    [RequireComponent(typeof(CollisionHandler))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(AnimationHandler))]
    [RequireComponent(typeof(HealthManager))]
    [RequireComponent(typeof(CharacterAttackManager))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class CharacterMovement : MonoBehaviour
    {
        private CollisionHandler _coll = null;
        private Rigidbody2D _rb = null;
        private AnimationHandler _anim = null;
        private VfxHandler _vfx = null;       
        private HealthManager _healthManager = null;
        private CharacterAttackManager _attackManager = null;
        private CapsuleCollider2D _capsuleCollider = null;
        private IInputController _input = null;

        [SerializeField] private CharacterMovementState _state = CharacterMovementState.Idle;
        [SerializeField] private bool _showDebug = true;

        [Space]
        [Header("Stats Camera Shake")]
        [SerializeField] private ShakeData _shakeDash = null;
        [SerializeField] private ShakeDataEvent _onCamShake = null;

        [Space]
        [Header("Stats")]
        [SerializeField] private FloatReference _runSpeed;
        [SerializeField] private FloatReference _airControl;
        [SerializeField] private FloatReference _rationRunAirSpeed;
        [SerializeField] private FloatReference _wallClimbMultiplySpeed;
        [SerializeField] private FloatReference _wallSlideSpeed;
        [SerializeField] private FloatReference _walkInertia;
        [SerializeField] private FloatReference _dashInvincibilityTime;

        [Space]
        [Header("Dash")]
        [SerializeField] private FloatReference _dashDistance;
        [SerializeField] private FloatReference _dashTime;
        [SerializeField] private FloatReference _dashCooldown;
        [SerializeField] private Vector2 _upFrontRaycastOffset = Vector2.zero;
        [SerializeField] private Vector2 _upBackRaycastOffset = Vector2.zero;

        [Space]
        [Header("Jump")]
        [SerializeField] private JumpData _normalJump = null;
        [SerializeField] private JumpData _lightJump = null;
        [SerializeField] private JumpData _doubleJump = null;
        [SerializeField] private JumpData _dashJump = null;
        [SerializeField] private JumpData _wallJump = null;
        [SerializeField] private JumpData _fastFall = null;
        [SerializeField] private FloatReference _coyoteTime;

        [Space]
        [Header("Dive kick")]
        [SerializeField] private Vector2 _diveKickDirection = Vector2.zero;
        [SerializeField] private FloatReference _diveKickSpeed = null;

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
        [SerializeField] private bool _isDiveKicking = false;
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
        private bool _wasOnWall;
        private int _side;
        private Vector2 _vel = Vector2.zero;
        private float _timeToNextDash = 0f;
        private bool _isCoyoteTime;
        private float _lastX;

        [Space]
        private float _currentGravity = 0f;
        private Vector2 _currentVelocity = Vector2.zero;

        [Space]
        private GameObject _refDashSpeedVfx;
        private GameObject _refDashDustVfx;
        private GameObject _refWallSlideSparkVfx;
        private GameObject _refWallSlideDustVfx;

        private void Start()
        {
            _coll = GetComponent<CollisionHandler>();
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<AnimationHandler>();
            _anim = GetComponentInChildren<AnimationHandler>();
            _vfx = GetComponent<VfxHandler>();
            _healthManager = GetComponent<HealthManager>();
            _attackManager = GetComponent<CharacterAttackManager>();
            _capsuleCollider = GetComponent<CapsuleCollider2D>();
            _input = GetComponent<IInputController>();
            _coll.OnGrounded += ResetIsJumping;
            _coll.OnWalled += ResetIsJumping;

            _groundTouch = true;
            _canStillJump = true;
            _wasOnWall = false;
            _side = 1;

            if (_showDebug)
            {
                CursedDebugger.Instance.Add("State", () => _state.ToString());
            }
        }

        private void Update()
        {
            //Get input
            float x = _isDiveKicking ? _lastX : _input.x;
            float y = _input.y;

            UpdateBools();
            UpdateWallGrab(x, y);
            UpdateJump();
            UpdateDash(x);
            UpdateDiveKick();
            UpdateFlip(Mathf.Abs(x) <= .1f ? _currentVelocity.x : x);

            //Set current velocity
            _currentVelocity = _rb.velocity;

            UpdateState();

            _lastX = x;
        }

        private void FixedUpdate()
        {
            //Get input
            float x = _isDiveKicking ? _lastX : _input.x;
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

            _vfx.SpawnVfx(_vfx.VfxFall, transform.position);
            Destroy(_refWallSlideSparkVfx);
            Destroy(_refWallSlideDustVfx);

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

            if (_healthManager)
                _healthManager.StartInvincibility(_dashInvincibilityTime);

            //Change capsule collider values
            Vector2 capsuleOffset = _capsuleCollider.offset;
            Vector2 capsuleSize = _capsuleCollider.size;
            _capsuleCollider.offset = new Vector2(_capsuleCollider.offset.x, 0.16f);
            _capsuleCollider.size = new Vector2(_capsuleCollider.size.x, 0.31f);

            //Create variables for the dash
            float dashTimer = _dashTime;
            float deltaDist = 0;
            float newX = 0f;
            bool forceToContinu = false;
            int side = _side;

            while (dashTimer >= 0 || forceToContinu)
            {
                if (!CheckIfCanDash())
                    break;

                deltaDist = side * _dashDistance * 10 * (1 / (float)GameManager.FPS) / _dashTime;
                newX = transform.position.x + deltaDist;
                transform.position = new Vector2(newX, transform.position.y);
                dashTimer -= Time.deltaTime;
                UpdateForceToContinu(ref forceToContinu);
                yield return new WaitForEndOfFrame();
            }

            //Reset values
            StartCoroutine("ResetValuesOnAfterDash");
            _capsuleCollider.offset = capsuleOffset;
            _capsuleCollider.size = capsuleSize;

            //Set dash cooldown
            _timeToNextDash = Time.time + _dashCooldown;
            Destroy(_refDashSpeedVfx);
            Destroy(_refDashDustVfx);
        }

        private void UpdateForceToContinu(ref bool forceToContinu)
        {
            int i = Physics2D.RaycastAll(_upFrontRaycastOffset + (Vector2)transform.position, Vector2.up, 3f, LayerMask.GetMask("Ground")).Length;
            int j = Physics2D.RaycastAll(_upBackRaycastOffset + (Vector2)transform.position, Vector2.up, 3f, LayerMask.GetMask("Ground")).Length;

            forceToContinu = (i + j) != 0;
        }

        private bool CheckIfCanDash()
        {
            int i = Physics2D.RaycastAll(new Vector2(0f, 1.5f) + (Vector2)transform.position,Vector2.right, 3f, LayerMask.GetMask("Ground")).Length;

            bool canDash = i == 0;

            return canDash || _isJumping;
        }

        private IEnumerator ResetValuesOnAfterDash()
        {
            _isDashing = false;
            _isInvincible = false;

            int frames = 2;
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }

            if (_coll.OnGround && _isJumping)
                _isJumping = false;
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

            while(_isCoyoteTime && (timer >= 0f))
            {
                timer -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
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

            //Don't need to walk if wall grabbing or in the air of dashing
            if (_wallGrab || !_coll.OnGround || _isDashing)
                return;

            //Don't need to walk if wall attacking
            if (_attackManager.IsAttacking)
                return;

            Walk(x);           
        }

        /// <summary>
        /// Handle the air control
        /// </summary>
        private void UpdateAirControl(float x)
        {
            //No air control if on ground or on the wall
            if (_coll.OnGround || _wallGrab || _wallRun)
                return;

            //Apply x velocity during a wall jump
            //Wall jump air control
            float clamp = (_isJumping && _rb.velocity.y > .1f) ? _runSpeed * _rationRunAirSpeed : _runSpeed;
            float X = Mathf.Clamp(_currentVelocity.x + x * _runSpeed, -clamp, clamp);
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

            if (_attackManager.IsAttacking)
                return;

            _jumpWasPressed = true;

            //If in air, double jump
            if (!CheckForWallGrab() && !_coll.OnGround && !_hasDoubleJumped && _doubleJumpUnlock && !_canStillJump && !_wasOnWall)
            {
                _hasDoubleJumped = true;
                _hasWallJumped = false;
                UpdateVelocity(0f, -_rb.velocity.y);
                Jump(_doubleJump);
                _vfx.SpawnVfx(_vfx.VfxDoubleJump, transform.position);
            }

            //If on ground, jump
            if (_coll.OnGround || _canStillJump)
            {
                _canStillJump = false;
                StopCoroutine("CoyoteTime");
                _isCoyoteTime = false;

                if (_isDashing)
                {
                    Jump(_dashJump);
                }
                else
                {
                    Jump(_normalJump);
                }
            }

            //If on wall, wall jump
            if ((_coll.OnWall && !_coll.OnGround && _wallGrab) || _wasOnWall)
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

                //No gravity  when dive kicking
                if (_isDiveKicking)
                    _currentGravity = 0;
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

            if (_attackManager.IsAttacking)
                return;

            if (!_input.Dash.Value || !_canDash || !_groundTouch || !_dashUnlock)
                return;

            float dir = x > 0.2f ? x : _side;

            if (dir != 0)
            {
                StartCoroutine(Dash(dir));
                _onCamShake?.Raise(_shakeDash);
                _refDashSpeedVfx = _vfx.DashSpeedVfx();
                _refDashDustVfx = _vfx.DashDustVfx();
            }
        }

        /// <summary>
        /// Handle the wall grab
        /// </summary>
        private void UpdateWallGrab(float x, float y)
        {
            if (_wallGrab && !_isDashing && CheckIfWallGrabDuringJump())
            {
                if (x > .2f || x < .2f)
                    UpdateVelocity(_currentVelocity.x, 0);

                if ((_side == 1 && _coll.OnRightWall) || (_side == -1 && !_coll.OnRightWall))
                    return;

                //Wall run
                if (_input.HoldRightTrigger)
                {
                    _wallRun = true;

                    if (_isJumping)
                        ResetIsJumping();

                    //Apply new velocity
                    UpdateVelocity(_currentVelocity.x, _runSpeed * _wallClimbMultiplySpeed);
                    Destroy(_refWallSlideSparkVfx);
                    Destroy(_refWallSlideDustVfx);
                }
                else if (!_coll.OnGround) //Slide on wall
                {
                    _wallRun = false;
                    _wallSlide = true;
                    SlideOnWall();

                    if (_refWallSlideSparkVfx == null)
                        _refWallSlideSparkVfx = _vfx.WallSlideSparkVfx();

                    if (_refWallSlideDustVfx == null)
                        _refWallSlideDustVfx = _vfx.WallSlideDustVfx();
                }
            }
        }

        /// <summary>
        /// Update the flip side
        /// </summary>
        private void UpdateFlip(float x)
        {
            //Return if the character can't flip 
            if (_wallGrab || _wallSlide || !_canMove || _isDashing)
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

        private void UpdateDiveKick()
        {
            if(_attackManager.IsDiveKicking && !_isDiveKicking)
            {
                //Just dive kick
                UpdateVelocity(_diveKickDirection.x * _side * _diveKickSpeed, _diveKickDirection.y * _diveKickSpeed);
            }

            _isDiveKicking = _attackManager.IsDiveKicking;
        }

        /// <summary>
        /// Update all boolean values
        /// </summary>
        private void UpdateBools()
        {
            //If on wall and input Grab hold, wall grab
            if (_coll.OnWall && CheckForWallGrab() && _canMove && CheckIfWallGrabDuringJump())
            {
                _wallGrab = true;
                _wallSlide = false;
            }

            //Reset wall grab and fall
            if ((!CheckForWallGrab() || !_coll.OnWall || !_canMove) && (_wallGrab || _wallSlide))
            {
                _wallGrab = false;
                _wallSlide = false;
                _wasOnWall = true;
                StartCoroutine("ResetWasOnWall");
                Destroy(_refWallSlideSparkVfx);
                Destroy(_refWallSlideDustVfx);
            }

            //Reset wall slide
            if (!_coll.OnWall || _coll.OnGround)
                _wallSlide = false;

            //Reset wall run if not on wall or not input grab
            if (_wallRun && (!_coll.OnWall || !CheckForWallGrab()))
                _wallRun = false;

            //Reset double jump
            //if (_coll.OnWall || _coll.OnGround)
            //    _hasDoubleJumped = false;

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
                _canStillJump = true;
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

            if (_wallGrab && !_coll.OnGround)
            {
                if (_currentVelocity.y > 0f)
                    _state = CharacterMovementState.WallRun;
                else
                    _state = CharacterMovementState.WallSlide;
            } 
        }

        #endregion

        /// <summary>
        /// Check if wall grabbing with the joystick
        /// </summary>
        private bool CheckForWallGrab()
        {
            return (_input.x < -.1f && _coll.OnRightWall && _side < -.1f) || (_input.x > .1f && _coll.OnLeftWall && _side > .1f);
        }

        private bool CheckIfWallGrabDuringJump()
        {
            if (_isJumping)
            {
                if (_rb.velocity.y > .1f)
                    return false;
                else
                    return true;
            }

            return true;
        }

        private IEnumerator ResetWasOnWall()
        {
            yield return new WaitForSeconds(.1f);

            _wasOnWall = false;
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

        private void OnDrawGizmos()
        {
            if (!_showDebug)
                return;

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine((Vector2)transform.position + _upFrontRaycastOffset, (Vector2)transform.position + _upFrontRaycastOffset + Vector2.up * 3);
            Gizmos.DrawLine((Vector2)transform.position + _upBackRaycastOffset, (Vector2)transform.position + _upBackRaycastOffset + Vector2.up * 3);
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
    }
}