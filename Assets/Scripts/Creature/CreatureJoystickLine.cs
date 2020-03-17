using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureJoystickLine : MonoBehaviour
    {
        [SerializeField] private float _lerpSpeed = 2f;

        private bool _launchLerp;
        private bool _typeOfLerp;

        private ParticleSystem _particles;

        private void Awake()
        {
            transform.localScale = new Vector3(0f, 1f);
            _particles = GetComponentInChildren<ParticleSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_launchLerp)
                LaunchLerp(_typeOfLerp);
        }

        public void LerpSize(bool reverse)
        {
            _launchLerp = true;
            _typeOfLerp = reverse;
        }

        private void LaunchLerp(bool reverse)
        {
            if (!reverse)
            {
                if (transform.localScale.x < 1)
                {
                    _particles.Play();
                    Vector3 currentScale = new Vector3(Mathf.MoveTowards(transform.localScale.x, 1, _lerpSpeed * Time.deltaTime), transform.localScale.y, transform.localScale.z);
                    transform.localScale = currentScale;
                }
                else
                    _launchLerp = false;
            }
            else
            {
                if (transform.localScale.x > 0)
                {
                    _particles.Stop();
                    Vector3 currentScale = new Vector3(Mathf.MoveTowards(transform.localScale.x, 0, _lerpSpeed * Time.deltaTime), transform.localScale.y, transform.localScale.z);
                    transform.localScale = currentScale;
                }
                else
                    _launchLerp = false;
            }
        }
    }
}