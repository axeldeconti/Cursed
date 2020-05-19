using System.Collections;
using UnityEngine;
using Cinemachine;

namespace Cursed.VisualEffect
{
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class CameraShake : MonoBehaviour
    {
        private CinemachineImpulseSource _impulseSource;

        private void Awake()
        {
            _impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        public void Shake (ShakeData shake)
        {
            Vector2 v = new Vector2(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f)).normalized;

            _impulseSource.m_ImpulseDefinition.m_AmplitudeGain = shake.AmplitudeGain;
            _impulseSource.m_ImpulseDefinition.m_FrequencyGain = shake.FrequenceGain;

            _impulseSource.GenerateImpulse(v);
        }
    }
}