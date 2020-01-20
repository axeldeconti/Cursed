using UnityEngine;

namespace Cursed.Creature
{
    //[CreateAssetMenu(fileName = "NewStats", menuName = "Creature/Stats")]
    public class CreatureStats_SO : ScriptableObject
    {
        #region Fields

        [SerializeField] private IntReference _energy;
        [SerializeField] private FloatReference _moveSpeedInAir;
        [SerializeField] private FloatReference _drainSpeed;
        [SerializeField] private IntReference _maxHealth;
        [SerializeField] private FloatReference _moveSpeedChaseAndComeBack;

        #endregion

        #region Getters

        public IntReference Energy => _energy;
        public FloatReference MoveSpeedInAir => _moveSpeedInAir;
        public FloatReference DrainSpeed => _drainSpeed;
        public IntReference MaxHealth => _maxHealth;
        public FloatReference MoveSpeedChaseAndComeBack => _moveSpeedChaseAndComeBack;

        #endregion
    }
}