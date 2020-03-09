using System.Collections;
using UnityEngine;

namespace Cursed.Character
{
    public class VfxHandler : MonoBehaviour
    {
        [Header("VFX Movement")]
        [SerializeField] private GameObject _vfxRun;
        [SerializeField] private GameObject _vfxWallRun;
        [SerializeField] private GameObject _vfxJump;
        [SerializeField] private GameObject _vfxDoubleJump;
        [SerializeField] private GameObject _vfxFall;
        [SerializeField] private GameObject _vfxWallSlideSpark;
        [SerializeField] private GameObject _vfxWallSlideDust;
        [SerializeField] private GameObject _vfxDashSpeed;
        [SerializeField] private GameObject _vfxDashDust;
        [SerializeField] private GameObject _vfxTrailDivekick;
        [Space]
        [Header("VFX Attack")]
        [SerializeField] private GameObject _vfxTouchImpactSword1;
        [SerializeField] private GameObject _vfxTouchImpactSword2;
        [SerializeField] private GameObject _vfxTouchImpactSword3;
        [SerializeField] private GameObject _vfxTouchImpactSword4;
        [SerializeField] private GameObject _vfxTouchImpactSword5;
        [SerializeField] private GameObject _vfxTouchImpactAxe1;
        [SerializeField] private GameObject _vfxTouchImpactAxe2;
        [SerializeField] private GameObject _vfxTouchImpactAxe3;
        [SerializeField] private GameObject _vfxTouchImpactAxe4;
        [SerializeField] private GameObject _vfxTouchImpactAxe5;

        private CollisionHandler _coll;
        private CharacterMovement _move;

        private void Awake()
        {
            _coll = GetComponent<CollisionHandler>();
            _move = GetComponent<CharacterMovement>();
        }

        public GameObject SpawnVfx(GameObject vfx, Vector3 position)
        {
            return Instantiate(vfx, position, Quaternion.identity);
        }

        public void RunVfx()
        {
            Vector3 VfxPosition = transform.position;
            int side = _move.Side == 1 ? 0 : 1;

            if (side == 0)
                VfxPosition += new Vector3(-1.05f, 0.05f, 0f);
            else
                VfxPosition += new Vector3(1.05f, 0.05f, 0f);

            ParticleSystemRenderer particle = Instantiate(_vfxRun, VfxPosition, Quaternion.identity).GetComponent<ParticleSystemRenderer>();
            particle.flip = new Vector3(side, 0, 0);

        }

        public void WallRunVfx()
        {
            Vector3 VfxPosition = transform.position;
            int side = _coll.OnLeftWall ? 0 : 1;

            if (side == 0)
                VfxPosition += new Vector3(1.05f, 0.05f, 0f);
            else
                VfxPosition += new Vector3(-1.05f, 0.05f, 0f);

            ParticleSystemRenderer particle = Instantiate(_vfxWallRun, VfxPosition, Quaternion.identity).GetComponent<ParticleSystemRenderer>();
            particle.flip = new Vector3(0, side, 0);
        }

        public void JumpVfx()
        {
            ParticleSystem particle = Instantiate(_vfxJump, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            if (_move.XSpeed > 0.2)
                particle.startRotation = 0.8f;
            else if (_move.XSpeed < -0.2)
                particle.startRotation = -0.8f;
            else
                particle.startRotation = 0f;
        }

        public GameObject WallSlideSparkVfx()
        {
            Vector3 VfxPosition = transform.position;
            int side = _coll.OnLeftWall ? -20 : 20;

            if (side == -20)
                VfxPosition += new Vector3(0.9f, 4.9f, 0f);
            else
                VfxPosition += new Vector3(-0.9f, 4.9f, 0f);

            GameObject particle = Instantiate(_vfxWallSlideSpark, VfxPosition, Quaternion.identity, transform);
            ParticleSystem.ShapeModule shapeParticle = particle.GetComponent<ParticleSystem>().shape;
            shapeParticle.rotation = new Vector3(0, side, 0);

            return particle;
        }

        public GameObject WallSlideDustVfx()
        {
            Vector3 VfxPosition = transform.position;
            int side = _coll.OnLeftWall ? 0 : 1;

            if (side == 0)
                VfxPosition += new Vector3(-0.4f, 2.8f, 0f);
            else
                VfxPosition += new Vector3(0.4f, 2.8f, 0f);

            GameObject particle = Instantiate(_vfxWallSlideDust, VfxPosition, Quaternion.identity, transform);
            ParticleSystemRenderer rendererParticle = particle.GetComponent<ParticleSystemRenderer>();
            rendererParticle.flip = new Vector3(side, 0, 0);

            return particle;
        }

        public GameObject DashSpeedVfx()
        {
            Vector3 VfxPosition = transform.position;
            int side = _move.Side == 1 ? 0 : 180;

            if (side == 0)
                VfxPosition += new Vector3(-1f, 1f, 0f);
            else
                VfxPosition += new Vector3(1f, 1f, 0f);

            GameObject particle = Instantiate(_vfxDashSpeed, VfxPosition, Quaternion.identity, transform);
            ParticleSystem.ShapeModule shapeParticle = particle.GetComponent<ParticleSystem>().shape;
            shapeParticle.rotation = new Vector3(side, -90, 0);

            return particle;
        }

        public GameObject DashDustVfx()
        {
            Vector3 VfxPosition = transform.position;
            int side = _move.Side == 1 ? 0 : 1;

            if (side == 0)
                VfxPosition += new Vector3(0f, 1.5f, 0f);
            else
                VfxPosition += new Vector3(0f, 1.5f, 0f);

            GameObject particle = Instantiate(_vfxDashDust, VfxPosition, Quaternion.identity, transform);
            ParticleSystemRenderer rendererParticle = particle.GetComponent<ParticleSystemRenderer>();
            rendererParticle.flip = new Vector3(side, 0, 0);

            return particle;
        }

        public GameObject TrailDivekickEffect()
        {
            Vector3 VfxPosition = transform.position;
            VfxPosition += new Vector3(0f, 3f, 0f);
            GameObject particle = Instantiate(_vfxTrailDivekick, VfxPosition, Quaternion.identity, transform);
            return particle;
        }

        public void TouchImpactSwordVfx (Vector3 VfxPosition)
        {
            int randParticle = Random.Range(0,5);

            if (randParticle == 0)
                Instantiate(_vfxTouchImpactSword1, VfxPosition += new Vector3 (0,3,0), Quaternion.identity);

            if (randParticle == 1)
                Instantiate(_vfxTouchImpactSword2, VfxPosition += new Vector3(0, 3, 0), Quaternion.identity);

            if (randParticle == 2)
                Instantiate(_vfxTouchImpactSword3, VfxPosition += new Vector3(0, 3, 0), Quaternion.identity);

            if (randParticle == 3)
                Instantiate(_vfxTouchImpactSword4, VfxPosition += new Vector3(0, 3, 0), Quaternion.identity);

            if (randParticle == 4)
                Instantiate(_vfxTouchImpactSword5, VfxPosition += new Vector3(0, 3, 0), Quaternion.identity);
        }

        public void TouchImpactAxeVfx(Vector3 VfxPosition)
        {
            int randParticle = Random.Range(0, 5);

            if (randParticle == 0)
                Instantiate(_vfxTouchImpactAxe1, VfxPosition += new Vector3(0, 3, 0), Quaternion.identity);

            if (randParticle == 1)
                Instantiate(_vfxTouchImpactAxe2, VfxPosition += new Vector3(0, 3, 0), Quaternion.identity);

            if (randParticle == 2)
                Instantiate(_vfxTouchImpactAxe3, VfxPosition += new Vector3(0, 3, 0), Quaternion.identity);

            if (randParticle == 3)
                Instantiate(_vfxTouchImpactAxe4, VfxPosition += new Vector3(0, 3, 0), Quaternion.identity);

            if (randParticle == 4)
                Instantiate(_vfxTouchImpactAxe5, VfxPosition += new Vector3(0, 3, 0), Quaternion.identity);
        }

        #region Getters & Setters
        public GameObject VfxDoubleJump => _vfxDoubleJump;
        public GameObject VfxFall => _vfxFall;
        #endregion
    }
}