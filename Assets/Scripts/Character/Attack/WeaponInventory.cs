using UnityEngine;
using Cursed.Combat;

namespace Cursed.Character
{
    [RequireComponent(typeof(IInputController))]
    [RequireComponent(typeof(CharacterAttackManager))]
    public class WeaponInventory : MonoBehaviour
    {
        private IInputController _input = null;
        private CharacterAttackManager _attackManager = null;

        [SerializeField] private Weapon _weapon1 = null;
        [SerializeField] private Weapon _weapon2 = null;

        private void Awake()
        {
            _input = GetComponent<IInputController>();
            _attackManager = GetComponent<CharacterAttackManager>();
        }

        #region Store

        public void AddWeapon(Weapon weapon)
        {

        }

        public Weapon RemoveWeapon(int weaponIndex)
        {
            return _weapon1;
        }

        #endregion

        public Weapon GetWeapon(int weaponNb) { return weaponNb == 1 ? _weapon1 : _weapon2; }
    }
}