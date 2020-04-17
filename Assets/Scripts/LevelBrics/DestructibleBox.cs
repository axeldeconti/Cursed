using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cursed.Combat;
using Cursed.Character;

namespace Cursed.Combat
{
    public class DestructibleBox : MonoBehaviour, IAttackable
    {
        [SerializeField] private GameObject _destructionEffect;
        [SerializeField] private GameObject _destructionImpact;
        [SerializeField] private GameObject _destructionImpactDivekick;

        public void OnAttack(GameObject attacker, Attack attack)
        {
            CreateImpactEffect(attacker.transform);
            CreateDestroyEffect(attacker.transform);
            Destroy(this.gameObject);
        }

        private GameObject CreateImpactEffect(Transform attacker)
        {
            if(attacker.GetComponent<CharacterMovement>().IsDiveKicking)
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
