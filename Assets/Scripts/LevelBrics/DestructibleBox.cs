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

        public void OnAttack(GameObject attacker, Attack attack)
        {
            CreateDestroyEffect(attacker.transform);
            Destroy(this.gameObject);
        }

        private GameObject CreateDestroyEffect(Transform attacker)
        {
            GameObject go = Instantiate(_destructionEffect, this.transform.position, Quaternion.identity);
            Vector2 direction = this.transform.position - attacker.GetChild(0).position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            go.transform.rotation = Quaternion.Euler(rotation.eulerAngles);
            return go;
        }
    }
}
