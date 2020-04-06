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
        [SerializeField] private GameObject _destructionEffectDown;

        public void OnAttack(GameObject attacker, Attack attack)
        {
            CreateDestroyEffect(attacker.transform);
            Destroy(this.gameObject);
        }

        private GameObject CreateDestroyEffect(Transform attacker)
        {
            if(attacker.GetComponent<CharacterMovement>().IsDiveKicking)
            {
                GameObject particle = Instantiate(_destructionEffectDown, this.transform.position, Quaternion.identity);
                return particle;
            }
            else 
            {
                int side = attacker.GetComponent<CharacterMovement>().Side == 1 ? 0 : 1;
                GameObject particle = Instantiate(_destructionEffect, this.transform.position, Quaternion.identity);
                if (side == 0)
                {
                    ParticleSystemRenderer rendererParticle = particle.GetComponent<ParticleSystemRenderer>();
                    rendererParticle.flip = new Vector3(1, 0, 0);
                    rendererParticle.pivot = new Vector3(0.5f, 0, 0);
                }
                else
                {
                    ParticleSystemRenderer rendererParticle = particle.GetComponent<ParticleSystemRenderer>();
                    rendererParticle.flip = new Vector3(0, 0, 0);
                    rendererParticle.pivot = new Vector3(-0.5f, 0, 0);
                }
                return particle;
            }
        }
    }
}
