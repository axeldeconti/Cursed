using UnityEngine;

namespace Cursed.Character
{
    public class VfxHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _vfxRun;
        [SerializeField] private GameObject _vfxWallRun;
        [SerializeField] private GameObject _vfxJump;
        [SerializeField] private GameObject _vfxDoubleJump;
        [SerializeField] private GameObject _vfxFall;

        private CollisionHandler _coll;
        private CharacterMovement _move;

        private void Awake()
        {
            _coll = GetComponent<CollisionHandler>();
            _move = GetComponent<CharacterMovement>();
        }

        public void SpawnVfx(GameObject vfx, Vector3 position)
        {
            Instantiate(vfx, position, Quaternion.identity);
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

        #region Getters & Setters
        public GameObject VfxDoubleJump => _vfxDoubleJump;
        public GameObject VfxFall => _vfxFall;
        #endregion
    }
}