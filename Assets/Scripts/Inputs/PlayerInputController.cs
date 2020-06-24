using Cursed.Managers;
using UnityEngine;

namespace Cursed.Character
{
    public class PlayerInputController : MonoBehaviour, IInputController
    {
        private CharacterAttackManager _atk = null;

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

        private ControlerManager _controlerManager;

        private void Start()
        {
            _atk = GetComponent<CharacterAttackManager>();
            _controlerManager = ControlerManager.Instance;

            _hasDashed = false;

            Jump = new BoolBuffer(_jumpInputBufferTimer);
            Dash = new BoolBuffer(_dashInputBufferTimer);
        }

        private void Update()
        {
            //Update input buffers
            Jump.Update(Time.deltaTime);
            Dash.Update(Time.deltaTime);

            //Input for world interaction
            WorldInteraction = Input.GetButtonDown("WorldInteraction");

            #region XBOX CONTROLS

            if (_controlerManager._ControlerType == ControlerManager.ControlerType.XBOX || _controlerManager._ControlerType == ControlerManager.ControlerType.None)
            {
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

                if (Attack_1 || Attack_2)
                    _atk.CheckForFlipWhenAttack(x);
            }

            #endregion

            #region PS4 CONTROLS

            else if (_controlerManager._ControlerType == ControlerManager.ControlerType.PS4)
            {
                //Input for player movements
                x = Input.GetAxisRaw("Horizontal_PS4") >= _joystickDeadzone ? 1 : Input.GetAxisRaw("Horizontal_PS4") <= -_joystickDeadzone ? -1 : 0;
                y = Input.GetAxisRaw("Vertical_PS4") >= _joystickDeadzone ? 1 : Input.GetAxisRaw("Vertical_PS4") <= -_joystickDeadzone ? -1 : 0;

                //Input for player abilities
                if (Input.GetButtonDown("Jump_PS4"))
                    Jump.Trigger();

                HoldJump = Input.GetButton("Jump_PS4");

                HoldRightTrigger = Input.GetAxis("RightTrigger_PS4") > 0.5f ? true : false;

                if (Input.GetAxis("RightTrigger_PS4") < 0.5f && _hasDashed)
                    _hasDashed = false;

                if (Input.GetAxis("RightTrigger_PS4") > 0.5f && !Dash.Value && !_hasDashed)
                {
                    Dash.Trigger();
                    _hasDashed = true;
                }

                //Input for player attack
                Attack_1 = Input.GetButtonDown("Attack_1_PS4");
                Attack_2 = Input.GetButtonDown("Attack_2_PS4");

                if (Attack_1 || Attack_2)
                    _atk.CheckForFlipWhenAttack(x);
            }

            #endregion


        }
    }
}