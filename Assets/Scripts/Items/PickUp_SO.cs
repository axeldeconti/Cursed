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

        #region Getters & Setters

        public string Name { get => _name; set => _name = value; }
        public PickUpType Type { get => _type; set => _type = value; }
        public int SpawnChanceWeight { get => _spawnChanceWeight; set => _spawnChanceWeight = value; }

        #endregion
    }
}