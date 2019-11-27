using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureStats : MonoBehaviour
    {
        public CreatureStats_SO baseStats;

        private Dictionary<CreatureStat, float> _statModifier;

        #region Initializer

        private void Start()
        {
            //Init modifiers dico
            _statModifier = new Dictionary<CreatureStat, float>();
            for (int i = 0; i < Enum.GetNames(typeof(CreatureStat)).Length; i++)
            {
                _statModifier.Add((CreatureStat)i, 0f);
            }
        }

        #endregion

        #region Stat Modifer

        public void ModifyStat(CreatureStat stat, float amount)
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
                case CreatureStat.Energy:
                    break;
                case CreatureStat.MoveSpeed:
                    break;
                case CreatureStat.DrainSpeed:
                    break;
                case CreatureStat.MaxHealth:
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Getters

        public float GetStatModifier(CreatureStat stat)
        {
            return _statModifier[stat];
        }

        #endregion
    }
}