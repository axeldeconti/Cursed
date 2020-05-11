using System.Collections;
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
        private SpriteRenderer _spriteR;
        private int _spriteVersion = 0;
        [SerializeField] private Sprite[] _spriteArray;
        [SerializeField] private GameObject _destructionEffect;
        [SerializeField] private GameObject _destructionImpact;
        [SerializeField] private GameObject _destructionImpactDivekick;

        [Space]
        [Header("Stats Camera Shake")]
        [SerializeField] private ShakeData _shakeDestructibleWall = null;
        [SerializeField] private ShakeDataEvent _onCamShake = null;

        private void Start()
        {
            _spriteR = gameObject.GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if(_wallLife == 3)
            {
                //sprite 1hp
                _spriteVersion = 0;
                _spriteR.sprite = _spriteArray[_spriteVersion];
            }

            if (_wallLife == 2)
            {
                //sprite 2hp
                _spriteVersion = 1;
                _spriteR.sprite = _spriteArray[_spriteVersion];
            }

            if (_wallLife == 1)
            {
                //sprite 1hp
                _spriteVersion = 2;
                _spriteR.sprite = _spriteArray[_spriteVersion];
            }
        }

        public void OnAttack(GameObject attacker, Attack attack)
        {
            if(attacker.GetComponent<CharacterMovement>().IsDiveKicking)
            {
                _wallLife -= 3;
                AkSoundEngine.PostEvent("Play_DestructibleWall", gameObject);
                CreateImpactEffect(attacker.transform);
                CreateDestroyEffect(attacker.transform);
            }
            else
            {
                _wallLife -= 1;
                AkSoundEngine.PostEvent("Play_DestructibleWall", gameObject);
                CreateImpactEffect(attacker.transform);
                CreateDestroyEffect(attacker.transform);
            }

            if (_wallLife <= 0)
            {
                _onCamShake?.Raise(_shakeDestructibleWall);
                Destroy(this.gameObject);
            }
        }

        private GameObject CreateImpactEffect(Transform attacker)
        {
            if (attacker.GetComponent<CharacterMovement>().Side == -1)
            {
                GameObject go = Instantiate(_destructionImpact, this.transform.position + new Vector3(-1, 0, 0), Quaternion.identity);
                ParticleSystemRenderer rendererParticle = go.GetComponent<ParticleSystemRenderer>();
                rendererParticle.flip = new Vector3(0, 0, 0);
                rendererParticle.pivot = new Vector3(-0.5f, 0, 0);
                return go;
            }
            else
            {
                GameObject go = Instantiate(_destructionImpact, this.transform.position + new Vector3(1, 0, 0), Quaternion.identity);
                ParticleSystemRenderer rendererParticle = go.GetComponent<ParticleSystemRenderer>();
                rendererParticle.flip = new Vector3(1, 0, 0);
                rendererParticle.pivot = new Vector3(0.5f, 0, 0);
                return go;
            }
        }
        private GameObject CreateDestroyEffect(Transform attacker)
        {
            if (attacker.GetComponent<CharacterMovement>().Side == -1)
            {
                GameObject go = Instantiate(_destructionEffect, this.transform.position + new Vector3(-3, 0, 0), Quaternion.identity);
                Vector2 direction = this.transform.position - attacker.GetChild(0).position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                go.transform.rotation = Quaternion.Euler(rotation.eulerAngles);
                return go;
            }
            else
            {
                GameObject go = Instantiate(_destructionEffect, this.transform.position + new Vector3(3, 0, 0), Quaternion.identity);
                Vector2 direction = this.transform.position - attacker.GetChild(0).position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                go.transform.rotation = Quaternion.Euler(rotation.eulerAngles);
                return go;
            }
                
        }
    }
}
