using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Character
{
    public class EnemyRegister : MonoBehaviour
    {
        [SerializeField] private VoidEvent _registerEnemy = null;
        private CellInfo _currentCell;

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