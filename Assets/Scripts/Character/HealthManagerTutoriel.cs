using Cursed.Combat;
using Cursed.VisualEffect;
using UnityEngine;

namespace Cursed.Character
{
    public class HealthManagerTutoriel : HealthManager
    {
        [Header("Tutoriel")]
        [SerializeField] private IntReference _minHealthAmount;

        public override void OnAttack(GameObject attacker, Attack attack)
        {
            if (_currentHealth <= _minHealthAmount)
            {
                _currentHealth = _minHealthAmount;

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
            else
                base.OnAttack(attacker, attack);

        }
    }
}
