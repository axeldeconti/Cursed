using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureStaminaManager : MonoBehaviour
    {
        [SerializeField] private IntReference _maxStamina;
        [SerializeField] private IntReference _currentStamina;

        private CreatureStats _stats;
        private CreatureManager _manager;

        public IntEvent OnCurrentStaminaUpdate;
        public IntEvent OnMaxStaminaUpdate;

        private float _frequencyLoseStaminaTimer = .5f;
        private float _loseStaminaTimer;

        private float _frequencyGainStaminaTimer = .5f;
        private float _gainStaminaTimer;

        private void Awake()
        {
            _stats = GetComponent<CreatureStats>();
            _manager = GetComponent<CreatureManager>();
        }

        private void Start()
        {
            if (_stats != null)
                _maxStamina = _stats.baseStats.Energy;
            _loseStaminaTimer = _frequencyLoseStaminaTimer;
            _gainStaminaTimer = _frequencyGainStaminaTimer;
            UpdateCurrentStamina(_maxStamina);
        }

        private void Update()
        {
            if(_manager.CurrentState != CreatureState.OnCharacter)
            {
                _gainStaminaTimer = _frequencyGainStaminaTimer;
                _loseStaminaTimer -= Time.deltaTime;
                if(_loseStaminaTimer < 0)
                {
                    AddCurrentStamina(-5);
                    _loseStaminaTimer = _frequencyLoseStaminaTimer;
                }
            }

            else
            {
                _loseStaminaTimer = _frequencyLoseStaminaTimer;
                _gainStaminaTimer -= Time.deltaTime;
                if(_gainStaminaTimer < 0)
                {
                    AddCurrentStamina(5);
                    _gainStaminaTimer = _frequencyGainStaminaTimer;
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
