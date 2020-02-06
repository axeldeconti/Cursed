using UnityEngine;

namespace Cursed.Character 
{ 

    public class AiInputController : MonoBehaviour, IInputController
    {
        public float x { get; private set; }
        public float y { get; private set; }
        public BoolBuffer Jump { get; private set; }
        public bool HoldJump { get; private set; }
        public BoolBuffer Dash { get; private set; }
        public bool HoldRightTrigger { get; private set; }
        public bool Attack { get; private set; }

    }



}
