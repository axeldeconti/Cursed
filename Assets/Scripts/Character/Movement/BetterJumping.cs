using UnityEngine;

namespace Cursed.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BetterJumping : MonoBehaviour
    {
        private Rigidbody2D _rb2D;

        public float fallMultiplier;
        public float lowJumpMultiplier;

        void Start()
        {
            _rb2D = GetComponent<Rigidbody2D>();
            fallMultiplier = 0f;
            lowJumpMultiplier = 0f;
        }

        void Update()
        {
            if (_rb2D.velocity.y < 0)
            {
                _rb2D.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (_rb2D.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                _rb2D.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }
    }
}