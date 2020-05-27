using System.Collections;
using UnityEngine;
using Cursed.Character;

namespace Cursed.AI
{
    [RequireComponent(typeof(AiController))]
    public class AiInputController : MonoBehaviour, IInputController
    {
        private AiController _aiController = null;
        private CharacterMovement _move = null;
        private CharacterAttackManager _atk = null;

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
        [SerializeField] private FloatReference _jumpTimeForceInput;
        [SerializeField] private IntReference _aiUpdateFrame;

        [Header("Debug")]
        [SerializeField] private bool _debug = false;

        private AIData _data = null;
        private bool _wasJumping = false;
        private bool _wasDashing = false;
        private bool _forceJump = false;

        private bool _updateAI = true;
        private int _currentAiUpdateFrame = 0;
        private AIData _previousData = null;

        private void Awake()
        {
            _aiController = GetComponent<AiController>();
            _move = GetComponent<CharacterMovement>();
            _atk = GetComponent<CharacterAttackManager>();
        }

        private void Start()
        {
            _data = new AIData();
            _wasJumping = false;
            _wasDashing = false;
            _forceJump = false;

            _updateAI = true;
            _currentAiUpdateFrame = 0;
            _previousData = new AIData();

            Jump = new BoolBuffer(_jumpInputBufferTimer);
            Dash = new BoolBuffer(_dashInputBufferTimer);

            if (_debug)
            {
                CursedDebugger.Instance.Add("Input", () => _data.input.ToString());
            }
        }

        private void Update()
        {
            //Update input buffers
            Jump.Update(Time.deltaTime);
            Dash.Update(Time.deltaTime);

            //Reset data
            _data.Reset();

            //Update AI when needed
            if (_updateAI)
            {
                _updateAI = false;
                _currentAiUpdateFrame = 0;

                //Retrieve value from AiController
                _aiController.GetInputs(ref _data);

                _previousData = _data;
            }
            else
            {
                _currentAiUpdateFrame++;
                if(_currentAiUpdateFrame >= _aiUpdateFrame)
                    _updateAI = true;

                _data.input = _previousData.input;
                return;
            }

            x = _data.input.x;
            y = _data.input.y;

            //Jump
            if (_data.jump)
            {
                Jump.Trigger();
                _wasJumping = true;

                StopCoroutine(JumpCoroutine());
                StartCoroutine(JumpCoroutine());
            }
            else if (_wasJumping)
            {
                _wasJumping = _move.IsJumping;
            }

            HoldJump = _forceJump ? true : _data.jump;

            //Dash
            if (_data.dash && !_wasDashing)
            {
                Dash.Trigger();
                _wasDashing = true;
            }
            else if (_wasDashing)
            {
                _wasDashing = _move.IsDashing;
            }

            //Attacks
            Attack_1 = _data.attack1;
            Attack_2 = _data.attack2;

            if (Attack_1 || Attack_2)
                _atk.CheckForFlipWhenAttack(x);
        }

        private IEnumerator JumpCoroutine()
        {
            _forceJump = true;
            yield return new WaitForSeconds(_jumpTimeForceInput);
            _forceJump = false;
        }
    }

    public class AIData
    {
        public Vector2 input;
        public bool jump;
        public bool dash;
        public bool attack1;
        public bool attack2;

        public AIData() => Reset();

        public void Reset()
        {
            input = Vector2.zero;
            jump = false;
            dash = false;
            attack1 = false;
            attack2 = false;
        }
    }
}
