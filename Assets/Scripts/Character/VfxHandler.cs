using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Character
{
    public class VfxHandler : MonoBehaviour
    {
        public GameObject _vfxRun;
        public GameObject _vfxJump;
        public GameObject _vfxDoubleJump;
        public GameObject _vfxFall;

        public void SpawnVfx (GameObject vfx, Vector3 position)
        {
            Instantiate(vfx, position, Quaternion.identity);
        }

        public void RunVfx ()
        {
            Instantiate(_vfxRun, transform.position, Quaternion.identity);
        }

        public void FlipRun (int _flipVfx)
        {
            ParticleSystemRenderer particleRenderer = _vfxRun.GetComponent<ParticleSystemRenderer>();
            particleRenderer.flip = new Vector3(_flipVfx, 0, 0);
        }
    }
}