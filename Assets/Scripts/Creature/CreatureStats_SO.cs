using UnityEngine;

namespace Cursed.Creature
{
    //[CreateAssetMenu(fileName = "NewStats", menuName = "Creature/Stats")]
    public class CreatureStats_SO : ScriptableObject
    {
        #region Fields

        [SerializeField] private int _energy = 0;
        [SerializeField] private float _moveSpeed = 0;
        [SerializeField] private float _drainSpeed = 0;
        [SerializeField] private int _maxHealth = 0;

        #endregion

        #region Getters

        public int Energy => _energy;
        public float MoveSpeed => _moveSpeed;
        public float DrainSpeed => _drainSpeed;
        public int MaxHealth => _maxHealth;

        #endregion
    }
}