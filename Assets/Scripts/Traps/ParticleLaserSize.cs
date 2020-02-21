using UnityEngine;

namespace Cursed.Traps
{
    public class ParticleLaserSize : MonoBehaviour
    {
        private ParticleSystem _particles;
        [SerializeField] private Transform _end;

        private void Awake()
        {
            _particles = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            _particles.startLifetime = (_end.localPosition.y) / 100; 
        }
    }
}
