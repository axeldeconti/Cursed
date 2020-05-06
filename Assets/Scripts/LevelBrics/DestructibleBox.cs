﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cursed.Combat;
using Cursed.Character;
using Cursed.VisualEffect;

namespace Cursed.Combat
{
    public class DestructibleBox : MonoBehaviour, IAttackable
    {
        private int _wallLife = 3;
        [SerializeField] private GameObject _destructionEffect;
        [SerializeField] private GameObject _destructionImpact;
        [SerializeField] private GameObject _destructionImpactDivekick;

        [Space]
        [Header("Stats Camera Shake")]
        [SerializeField] private ShakeData _shakeDestructibleWall = null;
        [SerializeField] private ShakeDataEvent _onCamShake = null;

        private void Update()
        {
            if(_wallLife == 3)
            {
                //sprite 3hp
            }

            if (_wallLife == 2)
            {
                //sprite 2hp
            }

            if (_wallLife == 1)
            {
                //sprite 1hp
            }
        }

        public void OnAttack(GameObject attacker, Attack attack)
        {
            if(attacker.GetComponent<CharacterMovement>().IsDiveKicking)
            {
                _wallLife -= 3;
                CreateImpactEffect(attacker.transform);
            }
            else
            {
                _wallLife -= 1;
                CreateImpactEffect(attacker.transform);
            }

            if (_wallLife <= 0)
            {
                CreateDestroyEffect(attacker.transform);
                _onCamShake?.Raise(_shakeDestructibleWall);
                Destroy(this.gameObject);
            }
        }

        private GameObject CreateImpactEffect(Transform attacker)
        {
            if (attacker.GetComponent<CharacterMovement>().IsDiveKicking)
            {
                GameObject go = Instantiate(_destructionImpactDivekick, this.transform.position, Quaternion.identity);
                return go;
            }
            else
            {
                if (attacker.GetComponent<CharacterMovement>().Side == -1)
                {
                    GameObject go = Instantiate(_destructionImpact, this.transform.position, Quaternion.identity);
                    ParticleSystemRenderer rendererParticle = go.GetComponent<ParticleSystemRenderer>();
                    rendererParticle.flip = new Vector3(0, 0, 0);
                    rendererParticle.pivot = new Vector3(-0.5f, 0, 0);
                    return go;
                }
                else
                {
                    GameObject go = Instantiate(_destructionImpact, this.transform.position, Quaternion.identity);
                    ParticleSystemRenderer rendererParticle = go.GetComponent<ParticleSystemRenderer>();
                    rendererParticle.flip = new Vector3(1, 0, 0);
                    rendererParticle.pivot = new Vector3(0.5f, 0, 0);
                    return go;
                }
            }
        }
        private GameObject CreateDestroyEffect(Transform attacker)
        {
            GameObject go = Instantiate(_destructionEffect, this.transform.position, Quaternion.identity);
            AkSoundEngine.PostEvent("Play_DestructibleWall", gameObject);
            Vector2 direction = this.transform.position - attacker.GetChild(0).position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            go.transform.rotation = Quaternion.Euler(rotation.eulerAngles);
            return go;
        }
    }
}
