using UnityEngine;

namespace Cursed.Character
{
    public class PlayerInputController : MonoBehaviour, IInputController
    {
        public float x { get; private set; }
        public float y { get; private set; }
        public float xRaw { get; private set; }
        public float yRaw { get; private set; }
        public bool Jump { get; private set; }
        public bool Dash { get; private set; }
        public bool Grab { get; private set; }

        private void Update()
        {
            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");
            xRaw = Input.GetAxisRaw("Horizontal");
            yRaw = Input.GetAxisRaw("Vertical");
            Jump = Input.GetButton("Jump");
            Dash = Input.GetButton("Dash&Grab");
            Grab = Input.GetButton("Dash&Grab");
        }
    }
}