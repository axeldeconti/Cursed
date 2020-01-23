using UnityEngine;

namespace Cursed.Creature
{
    //[CreateAssetMenu(fileName = "NewStats", menuName = "Creature/Stats")]
    public class CreatureStats_SO : ScriptableObject
    {
        #region Fields

        [SerializeField] private IntReference _maxHealth;
        [SerializeField] private IntReference _energy;
        [SerializeField] private FloatReference _moveSpeedInAir;
        [SerializeField] private FloatReference _moveSpeedChaseAndComeBack;
        [SerializeField] private FloatReference _loseStaminaAmount;
        [SerializeField] private FloatReference _gainStaminaAmount;
        [SerializeField] private FloatReference _frequencyLoseStamina;
        [SerializeField] private FloatReference _frequencyGainStamina;

        #endregion

        #region Getters

        public IntReference MaxHealth => _maxHealth;
        public IntReference Energy => _energy;
        public FloatReference MoveSpeedInAir => _moveSpeedInAir;
        public FloatReference MoveSpeedChaseAndComeBack => _moveSpeedChaseAndComeBack;
        public FloatReference LoseStaminaAmount => _loseStaminaAmount;
        public FloatReference GainStaminaAmount => _gainStaminaAmount;
        public FloatReference FrequencyLoseStamina => _frequencyLoseStamina;
        public FloatReference FrequencyGainStamina => _frequencyGainStamina;

        #endregion
    }
}