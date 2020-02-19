using UnityEngine;

namespace Cursed.Character
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(CollisionHandler))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CharacterAttackManager))]
    public class AnimationHandler : MonoBehaviour
    {
        private Animator _anim = null;
        private CharacterMovement _move = null;
        private CollisionHandler _coll = null;
        private SpriteRenderer _renderer = null;
        private CharacterAttackManager _atttack = null;

        private static readonly int _moveSpeedX = Animator.StringToHash("MoveSpeedX");
        private static readonly int _moveSpeedY = Animator.StringToHash("MoveSpeedY");
        private static readonly int _jumpVelocity = Animator.StringToHash("JumpVelocity");
        private static readonly int _isJumping = Animator.StringToHash("IsJumping");
        private static readonly int _groundTouch = Animator.StringToHash("GroundTouch");
        private static readonly int _wallTouch = Animator.StringToHash("WallTouch");
        private static readonly int _isDashing = Animator.StringToHash("IsDashing");
        private static readonly int _isGrabing = Animator.StringToHash("GrabWall");
        private static readonly int _isWallRun = Animator.StringToHash("WallRun");
        private static readonly int _isWallSliding = Animator.StringToHash("IsWallSliding");
        private static readonly int _decelerationTrigger = Animator.StringToHash("DecelerationTrigger");
        private static readonly int _isAttacking = Animator.StringToHash("IsAttacking");
        private static readonly int _weaponType = Animator.StringToHash("WeaponType");

        private void Start()
        {
            _anim = GetComponent<Animator>();
            _move = GetComponentInParent<CharacterMovement>();
            _coll = GetComponentInParent<CollisionHandler>();
            _renderer = GetComponent<SpriteRenderer>();
            _atttack = GetComponent<CharacterAttackManager>();

            GetComponent<CollisionHandler>().OnGrounded += () => { _anim.ResetTrigger(_decelerationTrigger); };
        }

        private void Update()
        {
            //Update every anim variables
            _anim.SetFloat(_moveSpeedY, _move.YSpeed);
            _anim.SetFloat(_jumpVelocity, Mathf.Clamp(_move.YSpeed, -15, 15));
            _anim.SetBool(_groundTouch, _coll.OnGround);
            _anim.SetBool(_wallTouch, _coll.OnWall);
            _anim.SetBool(_isGrabing, _move.IsGrabing);
            _anim.SetBool(_isWallRun, _move.IsWallRun);
            _anim.SetBool(_isJumping, _move.IsJumping);
            _anim.SetBool(_isDashing, _move.IsDashing);
            _anim.SetBool(_isWallSliding, _move.WallSlide);
            _anim.SetBool(_isAttacking, _atttack.IsAttacking);

            if(_move.IsDashing)
                _anim.SetFloat(_moveSpeedX, 20f);
            else
                _anim.SetFloat(_moveSpeedX, Mathf.Abs(_move.XSpeed));
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

            bool prevState = _renderer.flipX;
            bool state = (side == 1) ? false : true;
            _renderer.flipX = state;

            if(state == !prevState && Mathf.Abs(_move.XSpeed) > 4)
                _anim.SetTrigger(_decelerationTrigger);
        }

        /// <summary>
        /// Start the attack animation with the correct weapon
        /// </summary>
        /// <param name="weapon">Weapon name to attack with</param>
        public void LaunchAttack(int weaponType)
        {
            _anim.SetFloat(_weaponType, weaponType);
        }
    }
}