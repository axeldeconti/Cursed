using UnityEngine;
using Cursed.AI;

namespace Cursed.Character 
{ 
    [RequireComponent(typeof(AiController))]
    public class AiInputController : MonoBehaviour, IInputController
    {
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

        private AiController _aiController = null;

        private Vector3 _velocity = Vector3.zero;

        private void Awake()
        {
            _aiController = GetComponent<AiController>();
        }

        private void Start()
        {
            _velocity = Vector3.zero;
        }

        private void Update()
        {
            Vector2 input = new Vector2(x, y);
            bool jump = Jump.Value;
            _aiController.GetInput(ref _velocity, ref input, ref jump);
        }
    }
}
