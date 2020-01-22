using UnityEngine;

namespace Cursed.Creature
{
    //[CreateAssetMenu(fileName = "NewStats", menuName = "Creature/Stats")]
    public class CreatureStats_SO : ScriptableObject
    {
        #region Fields

        [SerializeField] private int _energy = 0;
        [SerializeField] private float _moveSpeedInAir = 0;
        [SerializeField] private float _drainSpeed = 0;
        [SerializeField] private int _maxHealth = 0;
        [SerializeField] private float _moveSpeedChaseAndComeBack = 0;

        #endregion

        #region Getters

        public int Energy => _energy;
        public float MoveSpeedInAir => _moveSpeedInAir;
        public float DrainSpeed => _drainSpeed;
        public int MaxHealth => _maxHealth;
        public float MoveSpeedChaseAndComeBack => _moveSpeedChaseAndComeBack;

        #endregion
    }
}