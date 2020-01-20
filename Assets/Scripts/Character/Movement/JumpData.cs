using UnityEngine;

namespace Cursed.Character
{
    //[CreateAssetMenu(fileName = "New JumpData", menuName = "Character/Jump Data")]
    public class JumpData : ScriptableObject
    {
        [SerializeField] private float _height = 0f;
        [SerializeField] private float _halfDistance = 0f;

        public float InitialVelocity(float Vx)
        {
            float v = (2 * _height * Vx) / _halfDistance;

            return v;
        }

        public float Gravity(float Vx)
        {
            float g = (2 * _height * Vx * Vx) / (_halfDistance * _halfDistance);

            return g;
        }
    }
}