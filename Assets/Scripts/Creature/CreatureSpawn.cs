using UnityEngine;
using Cursed.Character;

namespace Cursed.Creature
{
    public class CreatureSpawn : MonoBehaviour
    {
        public GameObject Creature;
        private CreatureInputController _creatureInputController;
        private CreatureMovement _creatureMovement;
        private CharacterMovement _characterMovement;
        private CreatureManager _creatureManager;

        void Start()
        {
            _creatureInputController = GetComponent<CreatureInputController>();
            _creatureMovement = GetComponent<CreatureMovement>();
            _creatureManager = GetComponent<CreatureManager>();
            _characterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
        }

        public void Spawn()
        {
            Debug.Log("Spawn");
            
        }
    }
}
