using UnityEngine;
using Cursed.AI;

namespace Cursed.Character 
{ 
    [RequireComponent(typeof(AiController))]
    public class AiInputController : MonoBehaviour, IInputController
    {
        private AiController _aiController = null;
        private CharacterMovement _move = null;

        #region IInputController
        public float x { get; private set; }
        public float y { get; private set; }
        public BoolBuffer Jump { get; private set; }
        public bool HoldJump { get; private set; }
        public BoolBuffer Dash { get; private set; }
        public bool HoldRightTrigger { get; private set; }
        public bool Attack_1 { get; private set; }
        public bool Attack_2 { get; private set; }
        #endregion

        [SerializeField] private FloatReference _jumpInputBufferTimer;
        [SerializeField] private FloatReference _dashInputBufferTimer;

        private Vector2 _input = Vector2.zero;
        private bool _jump = false;
        private bool _wasJumping = false;
        private bool _dash = false;
        private bool _wasDashing = false;
        private bool _attack1 = false;
        private bool _attack2 = false;

        private void Awake()
        {
            _aiController = GetComponent<AiController>();
            _move = GetComponent<CharacterMovement>();
        }

        private void Start()
        {
            _input = Vector2.zero;
            _jump = false;
            _wasJumping = false;
            _wasDashing = false;
            _dash = false;
            _attack1 = false;
            _attack2 = false;

            Jump = new BoolBuffer(_jumpInputBufferTimer);
            Dash = new BoolBuffer(_dashInputBufferTimer);

            CursedDebugger.Instance.Add("Input", () => _input.ToString());
        }

        private void Update()
        {
            //Retrieve value from AiController
            _input = new Vector2(x, y);
            _jump = false;
            _aiController.GetInputs(ref _input, ref _jump, ref _dash, ref _attack1, ref _attack2);

            x = _input.x;
            y = _input.y;

            //Jump
            if (_jump)
            {
                Jump.Trigger();
                _wasJumping = true;
            }
            else if (_wasJumping)
            {
                _wasJumping = _move.IsJumping;
            }

            HoldJump = _jump;

            //Dash
            if (_dash && !_wasDashing)
            {
                Dash.Trigger();
                _wasDashing = true;
            }
            else if (_wasDashing)
            {
                _wasDashing = _move.IsDashing;
            }

            //Attacks
            Attack_1 = _attack1;
            Attack_2 = _attack2;
        }
    }
}
