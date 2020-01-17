using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureSearching : MonoBehaviour
    {
        public float Radius = 4f;
        private Transform _enemyHit;
        private CreatureManager _creatureManager;

        void Start()
        {
            _creatureManager = GetComponent<CreatureManager>();
        }
        void Update()
        {
            if(_creatureManager.CurrentState == CreatureState.Moving)
            {
                RaycastHit2D[] obj = Physics2D.CircleCastAll(transform.position, Radius, new Vector2(0f,0f));
                foreach (RaycastHit2D hit in obj)
                {
                    Debug.Log(hit.collider.gameObject.name);
                    if(hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        _enemyHit = hit.collider.transform;
                        _creatureManager.CurrentState = CreatureState.Chasing;
                    }
                }
            }
        }

        public Transform Enemy 
        {
            get => _enemyHit;
            set => _enemyHit = value;
        }
    }
}
