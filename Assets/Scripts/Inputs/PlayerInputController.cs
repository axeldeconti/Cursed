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
        public bool Attack { get; private set; }

        private bool _hasDashed = false;

        private void Start() => _hasDashed = false;

        private void Update()
        {
            // Input for player movements
            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");
            xRaw = Input.GetAxisRaw("Horizontal");
            yRaw = Input.GetAxisRaw("Vertical");

            // Input for player abilities
            Jump = Input.GetButtonDown("Jump");
            Grab = Input.GetAxis("Dash&Grab") > 0.5f ? true : false;

            if (Input.GetAxis("Dash&Grab") > 0.5f && !Dash && !_hasDashed)
            {
                Dash = true;
                _hasDashed = true;
            }
            else
                Dash = false;

            if (Input.GetAxis("Dash&Grab") < 0.5f)
                _hasDashed = false;

            // Input for player attack
            Attack = Input.GetButtonDown("Attack");

        }
    }
}