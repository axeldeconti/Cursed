using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Item
{
    public class PickUp_SO : ScriptableObject
    {
        #region Fields

        [Header("Pick Up")]
        [SerializeField] private string _name = "New Item";
        [SerializeField] private PickUpType _type = PickUpType.Weapon;
        [SerializeField] private int _spawnChanceWeight = 0;

        #endregion

        #region Getters

        public string Name => _name;
        public PickUpType Type => _type;
        public int SpawnChanceWeight => _spawnChanceWeight;

        #endregion
    }
}