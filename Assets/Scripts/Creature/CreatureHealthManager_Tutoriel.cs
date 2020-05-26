using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureHealthManager_Tutoriel : CreatureHealthManager
    {
        protected override void Start()
        {
            if (_stats != null)
                _maxHealth.Value = _stats.CurrentMaxHealth;

            UpdateCurrentHealth(_maxHealth);
        }
    }
}
