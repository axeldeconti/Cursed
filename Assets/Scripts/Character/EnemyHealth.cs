using Cursed.Combat;
using Cursed.Creature;
using UnityEngine;

namespace Cursed.Character
{
    public class EnemyHealth : HealthManager
    {
        [Header("Creature")]
        [SerializeField] private IntReference _minCreatureAmountHealth;
        public bool _canBeAttackable { get; private set; }

        public override void Start()
        {
            _canBeAttackable = true;
            base.Start();
        }

        public override void OnAttack(GameObject attacker, Attack attack)
        {
            if (attacker.tag.Equals("Creature"))
            {
                if (_currentHealth > _minCreatureAmountHealth)
                    UpdateCurrentHealth(_currentHealth - attack.Damage);
                else
                {
                    _canBeAttackable = false;
                    attacker.GetComponent<CreatureManager>().CurrentState = CreatureState.OnComeBack;

                    // LAUNCH SFX & VFX
                    AkSoundEngine.PostEvent("Play_Creature_EndDrain", gameObject);
                }
            }
            else
            {
                base.OnAttack(attacker, attack);
            }
        }

        public override void Die()
        {
            base.Die();
        }
    }
}

