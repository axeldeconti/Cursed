using UnityEngine;

namespace Cursed.Traps
{
    public class ParticleLaserSize : MonoBehaviour
    {
        private ParticleSystem _particles;
        [SerializeField] private Transform _end;
        [SerializeField] private bool _horizontal;

        private void Awake()
        {
            _particles = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            if(!_horizontal)
                _particles.startLifetime = (_end.localPosition.y) / 100; 
            else
                _particles.startLifetime = (_end.localPosition.x) / 100;

        }
    }
}
