using System;
using System.Collections.Generic;
using UnityEngine;
using Cursed.Item;

namespace Cursed.Character
{
    [RequireComponent(typeof(HealthManager)), RequireComponent(typeof(Inventory))]
    public class CharacterStats : MonoBehaviour
    {
        [SerializeField] private CharacterStats_SO _baseStats = null;

        private HealthManager _healthMgr  = null;
        private Inventory _inventory = null;
        private Dictionary<Stat, float> _statModifier = null;

        #region Initializer

        private void Start()
        {
            _healthMgr = GetComponent<HealthManager>();
            _inventory = GetComponent<Inventory>();

            //Init modifiers dico
            _statModifier = new Dictionary<Stat, float>();
            for (int i = 0; i < Enum.GetNames(typeof(Stat)).Length; i++)
            {
                _statModifier.Add((Stat)i, 0f);
            }
        }

        #endregion

        #region Stat Modifer

        public void ModifyStat(Stat stat, float amount)
        {
            if (!_statModifier.ContainsKey(stat))
            {
                Debug.LogError("Stat not in modifier dictionary.");
                return;
            }
            else
                _statModifier[stat] += amount;

            //Allows to do specific action depending on the stat changing
            switch (stat)
            {
                case Stat.Health:
                    _healthMgr.AddMaxHealth((int)amount);
                    break;
                case Stat.FixedDamage:
                    break;
                case Stat.DotDamage:
                    break;
                case Stat.Speed:
                    break;
                case Stat.RunSpeed:
                    break;
                case Stat.WallSpeed:
                    break;
                case Stat.JumpForce:
                    break;
                case Stat.Weight:
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Getters

        public CharacterStats_SO BaseStats => _baseStats;

        public float GetStatModifier(Stat stat)
        {
            return _statModifier[stat];
        }

        #endregion
    }
}