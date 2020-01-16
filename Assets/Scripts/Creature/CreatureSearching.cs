using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureSearching : MonoBehaviour
    {
        public float Radius = 4f;
        private Vector3 _enemyHit;
        private CreatureManager _creatureManager;

        void Start()
        {
            _creatureManager = GetComponent<CreatureManager>();
        }
        void Update()
        {
            if(_creatureManager.CurrentState == CreatureState.Moving)
            {
                RaycastHit2D[] obj = Physics2D.CircleCastAll(transform.position, Radius, new Vector3(0f, 0f, 0f));
                foreach(RaycastHit2D hit in obj)
                {
                    if(hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        _enemyHit = hit.collider.transform.position;
                        GetComponent<CreatureManager>().CurrentState = CreatureState.Chasing;
                    }
                }
            }
        }

        public Vector3 Enemy 
        {
            get => _enemyHit;
            set => _enemyHit = value;
        }
    }
}
