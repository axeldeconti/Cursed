﻿using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureVfxHandler : MonoBehaviour
    {
        [Header("VFX Movement")]
        [SerializeField] private GameObject _vfxMoveParticle;
        [SerializeField] private GameObject _vfxTrailParticle;

        public void CreatureMoveParticle()
        {
            Instantiate(_vfxMoveParticle, transform.position, Quaternion.identity, transform);
        }
        public GameObject CreatureTrailParticle()
        {
            GameObject particle = Instantiate(_vfxTrailParticle, transform.position, Quaternion.identity, transform);
            return particle;
        }
    }
}