using UnityEngine;
using Cursed.Props;

namespace Cursed.Character
{
    public class EnemyRegister : MonoBehaviour
    {
        [SerializeField] private VoidEvent _registerEnemy = null;
        public CellInfo _currentCell { get; private set; }

        private void Start()
        {
            _registerEnemy.Raise();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.GetComponent<CellInfo>())
            {
                _currentCell = other.GetComponent<CellInfo>();
            }
        }
    }
}