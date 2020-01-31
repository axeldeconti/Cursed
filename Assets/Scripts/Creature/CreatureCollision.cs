using UnityEngine;
using Cursed.Character;

namespace Cursed.Creature
{
    public class CreatureCollision : MonoBehaviour
    {
        [Header ("Referencies")]
        public GameObject CreatureOnCharacter;
        public GameObject CreatureOnWall;
        private CreatureManager _creatureManager;

        void Start()
        {
            _creatureManager = GetComponentInParent<CreatureManager>();
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                _creatureManager.CurrentState = CreatureState.OnComeBack;
                GameObject go = Instantiate(CreatureOnWall, this.transform.position, Quaternion.identity, collider.transform);
            }

            if (collider.transform.GetComponent<CharacterMovement>())
            {
                if (collider.gameObject.CompareTag("Player"))
                    _creatureManager.CurrentState = CreatureState.OnCharacter;
                else if (collider.gameObject.CompareTag("Enemy"))
                    _creatureManager.CurrentState = CreatureState.OnEnemy;

                Instantiate(CreatureOnCharacter, collider.transform.position + new Vector3(0f, 0f, 0f), Quaternion.identity, collider.transform);
            }
        }
    }
}