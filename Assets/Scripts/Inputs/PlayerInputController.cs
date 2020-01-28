using UnityEngine;

namespace Cursed.Character
{
    public class PlayerInputController : MonoBehaviour, IInputController
    {
        [SerializeField] private FloatReference _joystickDeadzone;

        public float x { get; private set; }
        public float y { get; private set; }
        public BoolBuffer Jump { get; private set; }
        public bool HoldJump { get; private set; }
        public BoolBuffer Dash { get; private set; }
        public bool Grab { get; private set; }
        public bool Attack { get; private set; }

        private bool _hasDashed = false;

        [SerializeField] private FloatReference _jumpInputBufferTimer;
        [SerializeField] private FloatReference _dashInputBufferTimer;

        private void Start()
        {
            _hasDashed = false;

            Jump = new BoolBuffer(_jumpInputBufferTimer);
            Dash = new BoolBuffer(_dashInputBufferTimer);

            //CursedDebugger.Instance.Add("HasDashed", () => _hasDashed.ToString());
            //CursedDebugger.Instance.Add("Dash", () => Dash.ToString());
        }

        private void Update()
        {
            //Update input buffers
            Jump.Update(Time.deltaTime);
            Dash.Update(Time.deltaTime);

            //Input for player movements
            x = Input.GetAxisRaw("Horizontal") >= _joystickDeadzone ? 1 : Input.GetAxisRaw("Horizontal") <= -_joystickDeadzone ? -1 : 0;
            y = Input.GetAxisRaw("Vertical") >= _joystickDeadzone ? 1 : Input.GetAxisRaw("Vertical") <= -_joystickDeadzone ? -1 : 0;

            //Input for player abilities
            if (Input.GetButtonDown("Jump"))
                Jump.Trigger();

            HoldJump = Input.GetButton("Jump");
            Grab = Input.GetAxis("Dash&Grab") > 0.5f ? true : false;

            if (Input.GetAxis("Dash&Grab") < 0.5f && _hasDashed)
                _hasDashed = false;

            if (Input.GetAxis("Dash&Grab") > 0.5f && !Dash.Value && !_hasDashed)
            {
                Dash.Trigger();
                _hasDashed = true;
            }

            //Input for player attack
            Attack = Input.GetButtonDown("Attack");

        }
    }
}