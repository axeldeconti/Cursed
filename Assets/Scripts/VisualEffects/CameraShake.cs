using System.Collections;
using UnityEngine;
using Cinemachine;

namespace Cursed.VisualEffect
{
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class CameraShake : MonoBehaviour
    {
        private CinemachineImpulseSource _impulseSource;

        private int _randomVec;
        [SerializeField] FloatReference _amplitudeGain;
        [SerializeField] FloatReference _frequencyGain;

        private void Awake()
        {
            _impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.G))
            {
                Shake();
            }
        }

        private void Shake ()
        {
            Debug.Log("Shake");
           
            _randomVec = Random.Range(0, 4);
            _impulseSource.m_ImpulseDefinition.m_AmplitudeGain = _amplitudeGain;
            _impulseSource.m_ImpulseDefinition.m_FrequencyGain = _frequencyGain;

            if (_randomVec == 0)
                _impulseSource.GenerateImpulse(Vector3.up);

            else if (_randomVec == 1)
                _impulseSource.GenerateImpulse(Vector3.right);

            else if (_randomVec == 2)
                _impulseSource.GenerateImpulse(Vector3.down);

            else
                _impulseSource.GenerateImpulse(Vector3.left);
        }
    }
}