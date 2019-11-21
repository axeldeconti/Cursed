using UnityEngine;
using Cursed.Item;
using Cursed.Combat;

namespace Cursed.Character
{
    public class CharacterStats : MonoBehaviour
    {
        public CharacterStats_SO characterDefinition;
        public CharacterInventory charInv;
        public GameObject characterWeaponSlot;

        #region Constructors
        public CharacterStats()
        {
            charInv = CharacterInventory.instance;
        }
        #endregion

        #region Initializations
        void Start()
        {
            if (!characterDefinition.setManually)
            {
                characterDefinition.maxHealth = 100;
                characterDefinition.currentHealth = 50;

                characterDefinition.baseDamage = 2;
                characterDefinition.currentDamage = characterDefinition.baseDamage;
            }
        }
        #endregion

        #region Stat Increasers
        public void ApplyHealth(int healthAmount)
        {
            characterDefinition.ApplyHealth(healthAmount);
        }
        #endregion

        #region Stat Reducers
        public void TakeDamage(int amount)
        {
            characterDefinition.TakeDamage(amount);
        }
        #endregion

        #region Weapon and Armor Change
        public void ChangeWeapon(ItemPickUp weaponPickUp)
        {
            if (!characterDefinition.UnEquipWeapon(weaponPickUp, charInv, characterWeaponSlot))
            {
                characterDefinition.EquipWeapon(weaponPickUp, charInv, characterWeaponSlot);
            }
        }
        #endregion

        #region Getters
        public int GetHealth()
        {
            return characterDefinition.currentHealth;
        }

        public Weapon GetCurrentWeapon()
        {
            if (characterDefinition.weapon != null)
            {
                return characterDefinition.weapon.itemDefinition.weaponSlotObject;
            }
            else
            {
                return null;
            }
        }

        public int GetDamage()
        {
            return characterDefinition.currentDamage;
        }
        #endregion
    }
}