using UnityEngine;

namespace Cursed.Utilities
{
    [CreateAssetMenu(fileName = "NewVibration", menuName = "VibrationData")]
    public class VibrationData_SO : ScriptableObject
    {
        [SerializeField] private float _vibrationTime;
        [SerializeField] private float _leftMotorIntensity;
        [SerializeField] private float _rightMotorIntensity;

        #region GETTERS
        public float VibrationTime => _vibrationTime;
        public float LeftMotorIntensity => _leftMotorIntensity;
        public float RightMotorIntensity => _rightMotorIntensity;

        #endregion
    }
}
