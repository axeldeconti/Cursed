using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureStats : MonoBehaviour
    {
        public CreatureStats_SO baseStats;

        private Dictionary<CreatureStat, float> _statModifier;

        private IntReference _currentEnergy;
        private FloatReference _currentMoveSpeedInAir;
        private FloatReference _currentLoseStaminaAmount;
        private IntReference _currentMaxHealth;
        private FloatReference _currentMoveSpeedChaseAndComeBack;
        private FloatReference _currentGainStaminaAmount;
        private FloatReference _currentFrequencyLoseStamina;
        private FloatReference _currentFrequencyGainStamina;


        #region Initializer

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            //Init modifiers dico
            _statModifier = new Dictionary<CreatureStat, float>();
            for (int i = 0; i < Enum.GetNames(typeof(CreatureStat)).Length; i++)
            {
                _statModifier.Add((CreatureStat)i, 0f);
            }

        }

        private void Initialize()
        {
            //Init current stats
            _currentEnergy = baseStats.Energy;
            _currentMoveSpeedInAir = baseStats.MoveSpeedInAir;
            _currentLoseStaminaAmount = baseStats.LoseStaminaAmount;
            _currentMaxHealth = baseStats.MaxHealth;
            _currentMoveSpeedChaseAndComeBack = baseStats.MoveSpeedChaseAndComeBack;
            _currentGainStaminaAmount = baseStats.GainStaminaAmount;
            _currentFrequencyLoseStamina = baseStats.FrequencyLoseStamina;
            _currentFrequencyGainStamina = baseStats.FrequencyGainStamina;
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
                case CreatureStat.MaxHealth:
                    _currentMaxHealth.Value += (int)amount;
                    break;
                case CreatureStat.Energy:
                    _currentEnergy.Value += (int)amount;
                    break;
                case CreatureStat.MoveSpeedInAir:
                    _currentMoveSpeedInAir.Value += amount;
                    break;
                case CreatureStat.MoveSpeedChaseAndComeBack:
                    _currentMoveSpeedChaseAndComeBack.Value += amount;
                    break;
                case CreatureStat.LoseStaminaAmount:
                    _currentLoseStaminaAmount.Value += amount;
                    break;
                case CreatureStat.GainStaminaAmount:
                    _currentGainStaminaAmount.Value += amount;
                    break;
                case CreatureStat.FrequencyLoseStamina:
                    _currentFrequencyLoseStamina.Value += amount;
                    break;
                case CreatureStat.FrequencyGainStamina:
                    _currentFrequencyGainStamina.Value += amount;
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

        public int CurrentMaxHealth
        {
            get => _currentMaxHealth;
            set => _currentMaxHealth.Value = value;
        }
        public int CurrentEnergy => _currentEnergy;
        public float CurrentMoveSpeedInAir => _currentMoveSpeedInAir;
        public float CurrentMoveSpeedChaseAndComeBack => _currentMoveSpeedChaseAndComeBack;
        public float CurrentLoseStaminaAmount => _currentLoseStaminaAmount;
        public float CurrentGainStaminaAmount => _currentGainStaminaAmount;
        public float CurrentFrequencyLoseStamina => _currentFrequencyLoseStamina;
        public float CurrentFrequencyGainStamina => _currentFrequencyGainStamina;
        #endregion
    }
}