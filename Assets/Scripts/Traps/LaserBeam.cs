using Cursed.Character;
using Cursed.Tutoriel;
using System.Collections;
using UnityEngine;

namespace Cursed.Traps
{
    public enum LaserType { Laser, MultiLaserVertical, MultilaserHorizontal }

    public class LaserBeam : Trap
    {
        public LaserType _laserType;
        [Header("Knock Back")]
        [SerializeField] private Vector2 _knockBackForce = new Vector2(17f, 15f);

        [Header("Activate")]
        [SerializeField] private bool _isActive = true;
        private TutorielBox _dashTutoriel;

        [Header("Data")]
        [SerializeField] private Transform _start = null;
        [SerializeField] private Transform _end = null;
        [SerializeField] private ParticleSystem _groundParticles;
        [SerializeField] private ParticleSystem _laserParticles;
        [SerializeField] private float _timeBeforeReactivation = 1f;


        [SerializeField] private FloatReference _colliderSize = null;

        private BoxCollider2D _coll = null;
        private Animator _animator;

        private void Awake()
        {
            if (!_animator)
                _animator = GetComponent<Animator>();

            if (!_isActive)
            {
                DeActiveLaser();
                foreach (TutorielBox box in FindObjectsOfType<TutorielBox>())
                {
                    if (box.TypeOfTutoriel == TutorielType.Dash)
                        _dashTutoriel = box;
                }
                if (_dashTutoriel != null)
                    _dashTutoriel.SpellUnlock += (value) => ActiveLaser(false);
            }
        }

        protected override void InflinctDamage(Component[] attackables)
        {
            base.InflinctDamage(attackables);

            // KNOCBACK CHARACTERS
            for (int i = 0; i < attackables.Length; i++)
            {
                attackables[i].GetComponent<CharacterMovement>().Knockback(_knockBackForce, .3f, gameObject);
            }
        }

        public void UpdateCollider()
        {
            if (!_coll)
                _coll = GetComponent<BoxCollider2D>();

            if (_laserType == LaserType.MultiLaserVertical || _laserType == LaserType.Laser)
            {
                _coll.offset = new Vector2(0, (_end.localPosition.y + _start.localPosition.y) / 2);
                _coll.size = new Vector2(_colliderSize, _end.localPosition.y - _start.localPosition.y);
            }
            if (_laserType == LaserType.MultilaserHorizontal)
            {
                _coll.offset = new Vector2((_end.localPosition.x + _start.localPosition.x) / 2, 0);
                _coll.size = new Vector2(_end.localPosition.x - _start.localPosition.x, _colliderSize);
            }
        }

        public void ActiveLaser(bool delay = true)
        {
            if (delay)
                StartCoroutine("WaitForActive");
            else
            {
                _animator.SetBool("Deactive", false);
                _animator.SetBool("Active", true);
                _groundParticles.Play();
                _laserParticles.Play();

                UpdateCollider();
            }
        }

        public void DeActiveLaser()
        {
            StopCoroutine("WaitForActive");
            _animator.SetBool("Deactive", true);
            _animator.SetBool("Active", false);

            if (!_coll)
                _coll = GetComponent<BoxCollider2D>();

            _coll.size = new Vector2(0, 0);
        }

        IEnumerator WaitForActive()
        {
            yield return new WaitForSeconds(_timeBeforeReactivation);
            _animator.SetBool("Deactive", false);
            _animator.SetBool("Active", true);
            _groundParticles.Play();
            _laserParticles.Play();

            UpdateCollider();
        }

        public float ColliderSize => _colliderSize;
        public Transform End => _end;
    }
}