using UnityEngine;

namespace Cursed.VisualEffect
{
    [CreateAssetMenu(fileName = "New ShakeData", menuName = "VisualEffect/Shake Data")]
    public class ShakeData : ScriptableObject
    {
        [SerializeField] private float _amplitudeGain = 0f;
        [SerializeField] private float _frequencyGain = 0f;
        [SerializeField] private float _duration = 0f;

        public float AmplitudeGain => _amplitudeGain;
        public float FrequenceGain => _frequencyGain;
        public float Duration => _duration;
    }
}
