using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Character
{
    [RequireComponent(typeof(HealthManager))]
    public class CharacterStats : MonoBehaviour
    {
        public CharacterStats_SO baseStats;
        public CharacterInventory charInv;

        private HealthManager _healthMgr;
        private Dictionary<Stat, float> _statModifier;

        #region Initializer

        private void Start()
        {
            _healthMgr = GetComponent<HealthManager>();

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
                case Stat.Damage:
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

        public int GetDamage()
        {
            //Change to return current damages
            return baseStats.BaseDamage;
        }

        public float GetStatModifier(Stat stat)
        {
            return _statModifier[stat];
        }

        #endregion
    }
}