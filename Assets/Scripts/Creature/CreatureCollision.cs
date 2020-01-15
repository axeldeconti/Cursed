using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureCollision : MonoBehaviour
    {
        private CreatureManager _creatureManager;

        void Start()
        {
            _creatureManager = GetComponentInParent<CreatureManager>();
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.gameObject.CompareTag("Player"))
            {
                _creatureManager.CurrentState = CreatureState.OnCharacter;
            }
        }
    }
}