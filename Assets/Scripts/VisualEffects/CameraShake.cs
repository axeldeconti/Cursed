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
            Debug.Log("Shake");

            Vector2 v = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

            _impulseSource.m_ImpulseDefinition.m_AmplitudeGain = shake.AmplitudeGain;
            _impulseSource.m_ImpulseDefinition.m_FrequencyGain = shake.FrequenceGain;

            _impulseSource.GenerateImpulse(v);
        }
    }
}