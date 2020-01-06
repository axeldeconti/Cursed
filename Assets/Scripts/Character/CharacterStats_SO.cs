using UnityEngine;
using Cursed.Item;

namespace Cursed.Character
{
    //[CreateAssetMenu(fileName = "NewStats", menuName = "Character/Stats")]
    public class CharacterStats_SO : ScriptableObject
    {
        #region Fields

        [SerializeField] private IntReference _maxHealth;
        [SerializeField] private float _speed = 0;
        [SerializeField] private float _runSpeed = 0;
        [SerializeField] private float _wallSpeed = 0;
        [SerializeField] private float _jumpForce = 0;
        [SerializeField] private float _weight = 0;

        #endregion

        #region Getters

        public int MaxHealth => _maxHealth;
        public float Speed => _speed;
        public float RunSpeed => _runSpeed;
        public float WallSpeed => _wallSpeed;
        public float Weight => _weight;

        #endregion
    }
}