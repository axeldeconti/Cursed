﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureStats : MonoBehaviour
    {
        public CreatureStats_SO baseStats;

        private Dictionary<CreatureStat, float> _statModifier;

        private int _currentEnergy;
        private float _currentMoveSpeed;
        private float _currentDrainSpeed;
        private int _currentMaxHealth;


        #region Initializer

        private void Start()
        {
            //Init modifiers dico
            _statModifier = new Dictionary<CreatureStat, float>();
            for (int i = 0; i < Enum.GetNames(typeof(CreatureStat)).Length; i++)
            {
                _statModifier.Add((CreatureStat)i, 0f);
            }

            Initialize();
        }

        private void Initialize()
        {
            //Init current stats
            _currentEnergy = baseStats.Energy;
            _currentMoveSpeed = baseStats.MoveSpeed;
            _currentDrainSpeed = baseStats.DrainSpeed;
            _currentMaxHealth = baseStats.MaxHealth;
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
                    _currentEnergy += (int)amount;
                    break;
                case CreatureStat.MoveSpeed:
                    _currentMoveSpeed += amount;
                    break;
                case CreatureStat.DrainSpeed:
                    _currentDrainSpeed += amount;
                    break;
                case CreatureStat.MaxHealth:
                    _currentMaxHealth += (int)amount;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Getters & Setters

        public float GetStatModifier(CreatureStat stat)
        {
            return _statModifier[stat];
        }

        public float CurrentEnergy => _currentEnergy;
        public float CurrentMoveSpeed => _currentMoveSpeed;
        public float CurrentDrainSpeed => _currentDrainSpeed;
        public float CurrentMaxHealth => _currentMaxHealth;

        #endregion
    }
}