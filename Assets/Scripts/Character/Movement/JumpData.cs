using UnityEngine;

namespace Cursed.Character
{
    [CreateAssetMenu(fileName = "New JumpData", menuName = "Character/Jump Data")]
    public class JumpData : ScriptableObject
    {
        [SerializeField] private float _height = 0f;
        [SerializeField] private float _distance = 0f;

        public float InitialVelocity(float Vx)
        {
            float v = (2 * _height * 100 * Vx) / (_distance * 100 / 2);

            return v;
        }

        public float Gravity(float Vx)
        {
            float g = (2 * _height * 100 * Vx * Vx) / ((_distance * 100 / 2) * (_distance * 100 / 2));

            return g;
        }

        public float Height => _height;
        public float Distance => _distance;
    }
}