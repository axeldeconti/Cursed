using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureStaminaManager : MonoBehaviour
    {
        [Header ("Settings")]
        [SerializeField] private IntReference _maxStamina;
        [SerializeField] private IntReference _currentStamina;

        private CreatureStats _stats;
        private CreatureManager _manager;

        [Header ("Events")]
        public IntEvent OnCurrentStaminaUpdate;
        public IntEvent OnMaxStaminaUpdate;

        private float _loseStaminaTimer;

        private float _gainStaminaTimer;

        private void Awake()
        {
            _stats = GetComponent<CreatureStats>();
            _manager = GetComponent<CreatureManager>();
        }

        private void Start()
        {
            if (_stats != null)
                _maxStamina.Value = _stats.CurrentEnergy;

            _loseStaminaTimer = _stats.CurrentFrequencyLoseStamina;
            _gainStaminaTimer = _stats.CurrentFrequencyGainStamina;
            UpdateCurrentStamina(_maxStamina);
        }

        private void Update()
        {
            if(_manager.CurrentState != CreatureState.OnCharacter)
            {
                _gainStaminaTimer = _stats.CurrentFrequencyGainStamina;
                _loseStaminaTimer -= Time.deltaTime;
                if(_loseStaminaTimer < 0)
                {
                    AddCurrentStamina(-(int)_stats.CurrentLoseStaminaAmount);
                    _loseStaminaTimer = _stats.CurrentFrequencyLoseStamina;
                }
            }

            else
            {
                _loseStaminaTimer = _stats.CurrentFrequencyLoseStamina;
                _gainStaminaTimer -= Time.deltaTime;
                if(_gainStaminaTimer < 0)
                {
                    AddCurrentStamina((int)_stats.CurrentGainStaminaAmount);
                    _gainStaminaTimer = _stats.CurrentFrequencyGainStamina;
                }
            }
        }

        public void UpdateCurrentStamina(int amount)
        {
            _currentStamina.Value = amount;
            OnCurrentStaminaUpdate?.Raise(_currentStamina);
        }

        public void UpdateMaxStamina(int amount)
        {
            _maxStamina.Value = amount;
            OnMaxStaminaUpdate?.Raise(_maxStamina);
        }

        public void AddCurrentStamina(int amount)
        {
            _currentStamina.Value += amount;

            if (_currentStamina < 0)
            {
                _currentStamina.Value = 0;
                _manager.CurrentState = CreatureState.OnComeBack;
            }

            if (_currentStamina >= _maxStamina)
                _currentStamina.Value = _maxStamina.Value;

            OnCurrentStaminaUpdate?.Raise(_currentStamina);
        }

        public void AddMaxStamina(int amount)
        {
            _maxStamina.Value += amount;
            OnMaxStaminaUpdate?.Raise(_maxStamina);

            AddCurrentStamina(amount);
        }

    }
}
