using UnityEngine;
using Cursed.AI;

namespace Cursed.Character 
{ 
    [RequireComponent(typeof(AiController))]
    public class AiInputController : MonoBehaviour, IInputController
    {
        private AiController _aiController = null;

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

        private void Awake()
        {
            _aiController = GetComponent<AiController>();
        }

        private void Start()
        {
            _input = Vector2.zero;

            Jump = new BoolBuffer(_jumpInputBufferTimer);
            Dash = new BoolBuffer(_dashInputBufferTimer);

            CursedDebugger.Instance.Add("Input", () => _input.ToString());
        }

        private void FixedUpdate()
        {
            //Retrieve value from AiController
            _input = new Vector2(x, y);
            _jump = false;
            _aiController.GetInput(ref _input, ref _jump);

            x = _input.x;
            y = _input.y;

            if (_jump)
                Jump.Trigger();
        }
    }
}
