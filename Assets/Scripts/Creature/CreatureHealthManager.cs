using UnityEngine;
using Cursed.Combat;
using Cursed.Character;

namespace Cursed.Creature
{
    public class CreatureHealthManager : MonoBehaviour
    {
        [SerializeField] private IntReference _maxHealth;

        [SerializeField] private IntReference _currentHealth;
        private CreatureStats _stats;
        private CreatureManager _creatureManager;
        private CreatureSearching _creatureSearching;

        public IntEvent onHealthUpdate;
        public IntEvent onMaxHealthUpdate;

        public CreatureAttack playerAttack;
        public CreatureAttack enemyAttack;

        private float _currentTimer;
        private bool _alreadyOnPlayer;

        private void Awake()
        {
            _stats = GetComponent<CreatureStats>();
            _creatureManager = GetComponent<CreatureManager>();
            _creatureSearching = GetComponent<CreatureSearching>();
        }

        private void Start()
        {
            if(_stats != null)
                _maxHealth.Value = _stats.baseStats.MaxHealth;

            UpdateCurrentHealth(0);
        }

        private void Update()
        {
            if (_creatureManager.CurrentState == CreatureState.OnEnemy)
                LaunchTimer();

            else if(_creatureManager.CurrentState == CreatureState.OnCharacter)
            {
                if (!_alreadyOnPlayer)
                {
                    GiveHealthToPlayer();
                    _alreadyOnPlayer = true;
                }
                LaunchTimer();
            }

            else
                ResetTimer();
        }

        public void UpdateCurrentHealth(int health)
        {
            _currentHealth.Value = health;
            if (onHealthUpdate != null)
                onHealthUpdate.Raise(_currentHealth);
        }

        public void AddCurrentHealth(int amount)
        {
            _currentHealth.Value += amount;

            if (_currentHealth < 0)
                _currentHealth.Value = 0;

            if (_currentHealth >= _maxHealth)
            {
                _currentHealth.Value = _maxHealth.Value;
                _creatureManager.CurrentState = CreatureState.OnComeBack;
            }

            if (onHealthUpdate != null)
                onHealthUpdate.Raise(_currentHealth);
        }

        public void AddMaxHealth(int amount)
        {
            _maxHealth.Value += amount;

            if (_maxHealth < 0)
                _maxHealth.Value = 0;

            if (onMaxHealthUpdate != null)
                onMaxHealthUpdate.Raise(_maxHealth);

            UpdateCurrentHealth(_currentHealth + amount);
        }

        private void ResetTimer() => _currentTimer = 0f;

        private void LaunchTimer()
        {
            _currentTimer += Time.deltaTime;
            TakeLifeToTarget();
        }

        private void TakeLifeToTarget()
        {
            switch(_creatureManager.CurrentState)
            {
                case CreatureState.OnCharacter:
                    if (_currentTimer >= playerAttack.TimeBetweenAttack)
                    {
                        playerAttack.InflictDamage(this.gameObject, GameObject.FindGameObjectWithTag("Player"));
                        ResetTimer();
                    }
                    break;

                case CreatureState.OnEnemy:
                    if (_currentTimer >= enemyAttack.TimeBetweenAttack)
                    {
                        _alreadyOnPlayer = false;
                        Attack attack = enemyAttack.InflictDamage(this.gameObject, _creatureSearching.Enemy.gameObject);
                        AddCurrentHealth(attack.Damage);
                        ResetTimer();
                    }
                    break;

                default:
                    break;
            }
        }

        private void GiveHealthToPlayer()
        {
            HealthManager playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthManager>();
            playerHealth.AddCurrentHealth(_currentHealth);
            UpdateCurrentHealth(0);
        }
    }
}
