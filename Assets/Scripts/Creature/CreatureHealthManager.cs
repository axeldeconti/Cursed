using Cursed.Character;
using Cursed.Combat;
using System.Collections;
using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureHealthManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private IntReference _maxHealth;

        [SerializeField] private IntReference _currentHealth;
        [SerializeField] private CreatureStats _stats;
        [SerializeField] private CreatureManager _creatureManager;
        [SerializeField] private CreatureSearching _creatureSearching;

        [Header("Events")]
        [SerializeField] private IntEvent onHealthUpdate;
        [SerializeField] private IntEvent onMaxHealthUpdate;
        [SerializeField] private VoidEvent onHeal;
        [SerializeField] private VoidEvent onPlayer;

        [Header("Attacks")]
        [SerializeField] private bool _canAttackPlayer = false;
        [SerializeField] private CreatureAttack _playerAttack;
        [SerializeField] private CreatureAttack _enemyAttack;

        private float _currentTimer;
        private bool _alreadyOnPlayer;
        private bool _giveHealth;


        private void Awake()
        {
            _stats = GetComponent<CreatureStats>();
            _creatureManager = GetComponent<CreatureManager>();
            _creatureSearching = GetComponent<CreatureSearching>();
        }

        private void Start()
        {
            if (_stats != null)
                _maxHealth.Value = _stats.CurrentMaxHealth;

            UpdateCurrentHealth(0);
        }

        private void Update()
        {
            if (_creatureManager.CurrentState == CreatureState.OnEnemy)
                LaunchTimer();

            else if (_creatureManager.CurrentState == CreatureState.OnCharacter)
            {
                if (!_alreadyOnPlayer && _currentHealth.Value > 0)
                {
                    GiveHealthToPlayer();
                    _alreadyOnPlayer = true;
                }

                if (!_giveHealth)
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

            onHealthUpdate?.Raise(_currentHealth);
        }

        public void AddMaxHealth(int amount)
        {
            _maxHealth.Value += amount;

            if (_maxHealth < 0)
                _maxHealth.Value = 0;

            if (onMaxHealthUpdate != null)
                onMaxHealthUpdate.Raise(_maxHealth);

            onHealthUpdate?.Raise(_currentHealth);
        }

        private void ResetTimer() => _currentTimer = 0f;

        private void LaunchTimer()
        {
            _currentTimer += Time.deltaTime;
            TakeLifeToTarget();
        }

        private void TakeLifeToTarget()
        {
            switch (_creatureManager.CurrentState)
            {
                case CreatureState.OnCharacter:
                    if (_currentTimer >= _playerAttack.TimeBetweenAttack && _canAttackPlayer)
                    {
                        onPlayer?.Raise();
                        _playerAttack.InflictDamage(this.gameObject, GameObject.FindGameObjectWithTag("Player"));
                        ResetTimer();
                    }
                    break;

                case CreatureState.OnEnemy:
                    if (_currentTimer >= _enemyAttack.TimeBetweenAttack)
                    {
                        EnemyHealth enemyHealth = _creatureSearching.Enemy.gameObject.GetComponent<EnemyHealth>();
                        _alreadyOnPlayer = false;
                        Attack attack = _enemyAttack.InflictDamage(this.gameObject, enemyHealth.gameObject);
                        if (enemyHealth._canBeAttackable)
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
            onHeal.Raise();
            UpdateCurrentHealth(0);
            _giveHealth = true;
            StartCoroutine(WaitForUnHealth(2f));
        }

        private IEnumerator WaitForUnHealth(float timer)
        {
            yield return new WaitForSeconds(timer);
            _giveHealth = false;
        }

        public bool CanAttackPlayer { get => _canAttackPlayer; set => _canAttackPlayer = value; }
    }
}
