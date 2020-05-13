using UnityEngine;

namespace Cursed.Props
{
    public class CameraRotation : MonoBehaviour
    {
        [SerializeField] private float _minSpeed;
        [SerializeField] private float _maxSpeed;
        private float _currentSpeed;

        [SerializeField] private float _angleOffset = 50f;
        private float _initialZAngle = -75f;

        private void Awake()
        {
            _currentSpeed = Random.Range(_minSpeed, _maxSpeed);
        }

        private void Update()
        {
            /* angles.z = Mathf.PingPong(Time.time * _currentSpeed, _endAngles.z) - _beginAngles.z;
             transform.eulerAngles = angles;*/

            float angle = Mathf.Sin(Time.time * _currentSpeed) * _angleOffset; //tweak this to change frequency
            transform.rotation = Quaternion.AngleAxis(_initialZAngle - angle, Vector3.forward);
        }
    }
}
