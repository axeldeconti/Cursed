﻿using UnityEngine;

namespace Cursed.Traps
{
    public enum LaserType { laser, multiLaserVertical, multilaserHorizontal }

    public class LaserBeam : Trap
    {
        public LaserType _laserType;

        [Header("Data")]
        [SerializeField] private Transform _start = null;
        [SerializeField] private Transform _end = null;
        [SerializeField] private ParticleSystem _groundParticles;
        [SerializeField] private ParticleSystem _laserParticles;

        [SerializeField] private FloatReference _colliderSize = null;

        private BoxCollider2D _coll = null;
        private Animator _animator;

        private void Awake()
        {
            if(!_animator)
                _animator = GetComponent<Animator>();
        }

        public void UpdateCollider()
        {
            if (!_coll)
                _coll = GetComponent<BoxCollider2D>();

            if (_laserType == LaserType.multiLaserVertical || _laserType == LaserType.laser)
            {
                _coll.offset = new Vector2(0, (_end.localPosition.y + _start.localPosition.y) / 2);
                _coll.size = new Vector2(_colliderSize, _end.localPosition.y - _start.localPosition.y);
            }
            if (_laserType == LaserType.multilaserHorizontal)
            {
                _coll.offset = new Vector2((_end.localPosition.x + _start.localPosition.x) / 2, 0);
                _coll.size = new Vector2(_end.localPosition.x - _start.localPosition.x, _colliderSize);
            }

        }

        public void ActiveLaser()
        {
            _animator.SetTrigger("Active");
            _groundParticles.Play();
            _laserParticles.Play();

            UpdateCollider();
        }

        public void DeActiveLaser()
        {
            _animator.SetTrigger("Deactive");

            if (!_coll)
                _coll = GetComponent<BoxCollider2D>();

            _coll.size = new Vector2(0, 0);
        }

        public float ColliderSize => _colliderSize;
        public Transform End => _end;
    }
}