using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Cursed.Utilities
{
    public class LightIntensityPingPong : MonoBehaviour
    {
        [SerializeField] private float _minIntensity = 2.5f;
        [SerializeField] private float _maxIntensity = 5f;
        [SerializeField] private float _speed = 2f;

        private Light2D _light;
        private float _currentIntensity;

        private void Awake()
        {
            _light = GetComponent<Light2D>();
            _currentIntensity = _minIntensity;
        }

        private void Update()
        {
            _currentIntensity = Mathf.PingPong(Time.time, _maxIntensity - _minIntensity) + _minIntensity;
            _light.intensity = _currentIntensity;
        }
    }
}
