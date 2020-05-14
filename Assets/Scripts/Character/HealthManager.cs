using Cursed.Combat;
using Cursed.Utilities;
using Cursed.VisualEffect;
using System;
using UnityEngine.Experimental.Rendering.Universal;
using System.Collections;
using UnityEngine;

namespace Cursed.Character
{
    public class HealthManager : MonoBehaviour, IAttackable
    {
        private CharacterMovement _move = null;

        [SerializeField] private IntReference _maxHealth;
        [SerializeField] private FloatReference _invincibleTime;
        [SerializeField] private FloatReference _zoomDuration;
        [SerializeField] private FloatReference _slowMotionDuration;
        [SerializeField] private VibrationData_SO _takeDamageVibration;
        [SerializeField] private VibrationData_SO _divekickTouchVibration;

        [Space]
        [Header("Head light")]
        [SerializeField] private Light2D _headLight = null;
        [SerializeField] private Gradient _lightGradient = null;

        private CharacterStats _stats = null;
        protected int _currentHealth = 0;
        private bool _isInvincible = false;
        private float _timeInvincibleLeft = 0f;

        [Space]
        public IntEvent onHealthUpdate;
        public IntEvent onMaxHealthUpdate;
        public VoidEvent onDeath;
        public VoidEvent addEnemy;

        private VfxHandler _vfx = null;
        private SFXHandler _sfx = null;
        private InvincibilityAnimation _invAnim;

        public Action<int> onEnemyHealthUpdate;

        [Space]
        [Header("Stats Camera Shake")]
        [SerializeField] private ShakeData _shakeCombo3 = null;
        [SerializeField] private ShakeData _shakeCritic = null;
        [SerializeField] private ShakeDataEvent _onCamShake = null;

        #region Initalizer

        private void Awake()
        {
            _move = GetComponent<CharacterMovement>();
        }

        public virtual void Start()
        {
            _vfx = GetComponent<VfxHandler>();
            _sfx = GetComponent<SFXHandler>();
            _invAnim = GetComponent<InvincibilityAnimation>();

            //Set to an eventual base number
            _stats = GetComponent<CharacterStats>();
            if (_stats != null)
                _maxHealth.Value = _stats.BaseStats.MaxHealth;

            UpdateCurrentHealth(_maxHealth);

            _isInvincible = false;
            _timeInvincibleLeft = 0f;

            addEnemy?.Raise();
        }

        #endregion

        private void Update()
        {
            if (_timeInvincibleLeft > 0f)
            {
                _timeInvincibleLeft -= Time.deltaTime;
            }
            else if (_isInvincible)
            {
                //End invincibility
                _isInvincible = false;
            }
        }

        #region Modifiers

        public virtual void OnAttack(GameObject attacker, Attack attack)
        {
            if (_isInvincible || IsInvicibleMovement())
            {
                //Do something if invincible
            }
            else
            {
                //Update health
                UpdateCurrentHealth(_currentHealth - attack.Damage);

                //Apply the effect of the attack
                if (attack.Effect != null && _stats != null)
                    attack.Effect.Invoke(_stats);


                if (attacker != null)
                    Debug.Log(gameObject.name + " got attacked by " + attacker.name + " and did " + attack.Damage + " damages");

                //Play sound, vfx and animation
                if (gameObject.tag.Equals("Player"))
                    _sfx.LowHealth();

                CharacterAttackManager atkMgr = attacker.GetComponent<CharacterAttackManager>();

                if (gameObject.tag.Equals("Player"))
                {
                    if (!attacker.tag.Equals("Creature"))
                    {
                        // Player take damage
                        _sfx.PlayerDamageSFX();
                        _vfx.FlashScreenDmgPlayer();

                        ControllerVibration.Instance.StartVibration(_takeDamageVibration);
                        _invAnim.LaunchAnimation();

                        //Become invincible
                        StartInvincibility(_invincibleTime);
                    }
                }

                if (gameObject.tag.Equals("Enemy"))
                {
                    if ((!attacker.tag.Equals("Player")) && (!attacker.tag.Equals("Creature")))
                        _sfx.FirstEnemyDamageSFX();

                    if (attacker.tag.Equals("Player"))
                    {
                        if (atkMgr)
                        {
                            _sfx.FirstEnemyDamageSFX();
                            _vfx.TouchImpact(transform.position, atkMgr.GetVfxTouchImpact());

                            if(!atkMgr.IsDiveKicking)
                                _vfx.AttackEffect(transform.position, attacker);
                            if(atkMgr.IsDiveKicking)
                                ControllerVibration.Instance.StartVibration(_divekickTouchVibration);

                            //Do something is critical
                            if (attack.IsCritical && atkMgr.Combo != 3)
                            {
                                _vfx.CriticalEffect(transform.position, attacker);
                                _onCamShake?.Raise(_shakeCritic);
                            }

                            //Do something for Combo 2
                            if (atkMgr.Combo == 2)
                            {
                                _sfx.SecondEnemyDamageSFX();
                            }

                            //Do something for Combo 3
                            if (atkMgr.Combo == 3)
                            {
                                _vfx.Combo3(transform.position, atkMgr.GetVfxCombo3(), attacker);
                                _sfx.ThirdEnemyDamageSFX();
                                _onCamShake?.Raise(_shakeCombo3);
                            }

                            //Blood effect
                            int _varRndBlood = UnityEngine.Random.Range(0, 3);
                            if(!atkMgr.IsDiveKicking && _varRndBlood == 0)
                            {
                                _vfx.BloodParticle(transform.position, attacker);
                                _vfx.BloodProjection(transform.position, attacker);
                            }
                        }
                    }
                }
            }
        }

        public void UpdateCurrentHealth(int health)
        {
            //Check if dead or not
            if (health <= 0)
            {
                Die();
            }
            else
            {
                _currentHealth = health;
                _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);

                _headLight.color = _lightGradient.Evaluate(1 - (float)_currentHealth / (float)_maxHealth);
                onHealthUpdate?.Raise(_currentHealth);
                onEnemyHealthUpdate?.Invoke(_currentHealth);

            }
        }

        public void AddCurrentHealth(int amount)
        {
            _currentHealth += amount;

            _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);

            _headLight.color = _lightGradient.Evaluate(1 - (float)_currentHealth / (float)_maxHealth);


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

            if (amount >= 0)
                AddCurrentHealth(amount);
            else
                AddCurrentHealth(0);
        }

        public void ApplyDot(float damagePerSecond, float duration)
        {
            StartCoroutine(ApplyDotCoroutine(damagePerSecond, duration));
        }

        private IEnumerator ApplyDotCoroutine(float damagePerSecond, float duration)
        {
            //Apply dot
            float timeLeft = duration;

            while (timeLeft > 0)
            {
                UpdateCurrentHealth((int)((float)_currentHealth - damagePerSecond));
                yield return new WaitForSeconds(1f);
            }

            //End dot
        }

        #endregion

        #region Invincibility

        public void StartInvincibility(float time)
        {
            if (time > _timeInvincibleLeft)
                _timeInvincibleLeft = time;

            _isInvincible = true;
        }

        private bool IsInvicibleMovement()
        {
            if (_move)
                return _move.IsInvincible;

            return false;
        }

        #endregion

        #region Death

        private void Die()
        {
            Debug.Log(gameObject.name + " is dead :(");

            if (gameObject.tag.Equals("Player"))
            {
                _sfx.PlayerDeathSFX();
                _vfx.DeathEffect(transform.position);
            }
            if (gameObject.tag.Equals("Enemy"))
            {
                _sfx.EnemyDeathSFX();
                _vfx.DeathEffect(transform.position);
                _vfx.BloodExplosion(transform.position);
                _vfx.AndroidPartExplosion(transform.position);

                if (_zoomDuration != null)
                {
                    CameraZoomController.Instance.Zoom(CameraZoomController.Instance._maxZoomKill, CameraZoomController.Instance._zoomInKillSpeed);
                    StartCoroutine(DoUnZoom(_zoomDuration.Value));
                }

                if (_slowMotionDuration != null)
                {
                    SlowMotion.Instance.Freeze(_slowMotionDuration);
                }

                Destroy(gameObject);
            }

            onDeath?.Raise();
        }

        private IEnumerator DoUnZoom(float _duration)
        {
            yield return new WaitForSecondsRealtime(_duration);
            CameraZoomController.Instance.Zoom(CameraZoomController.Instance._initialZoom, CameraZoomController.Instance._zoomOutKillSpeed);
        }

        #endregion

        #region Getters

        public float CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public bool IsInvincible { get => _isInvincible; set => _isInvincible = value; }

        #endregion
    }
}