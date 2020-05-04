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
        public bool HoldRightTrigger { get; private set; }
        public bool Attack_1 { get; private set; }
        public bool Attack_2 { get; private set; }
        public bool WorldInteraction { get; private set; }

        private bool _hasDashed = false;

        [SerializeField] private FloatReference _jumpInputBufferTimer;
        [SerializeField] private FloatReference _dashInputBufferTimer;

        private void Start()
        {
            _hasDashed = false;

            Jump = new BoolBuffer(_jumpInputBufferTimer);
            Dash = new BoolBuffer(_dashInputBufferTimer);
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

            HoldRightTrigger = Input.GetAxis("RightTrigger") > 0.5f ? true : false;

            if (Input.GetAxis("RightTrigger") < 0.5f && _hasDashed)
                _hasDashed = false;

            if (Input.GetAxis("RightTrigger") > 0.5f && !Dash.Value && !_hasDashed)
            {
                Dash.Trigger();
                _hasDashed = true;
            }

            //Input for player attack
            Attack_1 = Input.GetButtonDown("Attack_1");
            Attack_2 = Input.GetButtonDown("Attack_2");

            //Input for world interaction
            WorldInteraction = Input.GetButtonDown("WorldInteraction");

        }
    }
}