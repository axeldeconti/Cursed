using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Character
{
    public class EnemyRegister : MonoBehaviour
    {
        [SerializeField] private VoidEvent _registerEnemy = null;

        private void Start()
        {
            _registerEnemy.Raise();
        }
    }
}