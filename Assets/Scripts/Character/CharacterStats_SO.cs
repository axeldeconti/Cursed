using UnityEngine;
using Cursed.Item;

namespace Cursed.Character
{
    [CreateAssetMenu(fileName = "NewStats", menuName = "Character/Stats", order = 1)]
    public class CharacterStats_SO : ScriptableObject
    {
        #region Fields
        public bool setManually = false;
        public bool saveDataOnClose = false;

        public ItemPickUp weapon { get; private set; }
        public ItemPickUp item01 { get; private set; }
        public ItemPickUp item02 { get; private set; }

        public int maxHealth = 0;
        public int currentHealth = 0;

        public int baseDamage = 0;
        public int currentDamage = 0;
        #endregion

        #region Stat Increasers
        public void ApplyHealth(int healthAmount)
        {
            if ((currentHealth + healthAmount) > maxHealth)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth += healthAmount;
            }
        }

        public void EquipWeapon(ItemPickUp weaponPickUp, CharacterInventory charInventory, GameObject weaponSlot)
        {
            Rigidbody newWeapon;

            weapon = weaponPickUp;
            charInventory.inventoryDisplaySlots[2].sprite = weaponPickUp.itemDefinition.itemIcon;
            newWeapon = Instantiate(weaponPickUp.itemDefinition.weaponSlotObject.WeaponPrefab, weaponSlot.transform);
            currentDamage = baseDamage + weapon.itemDefinition.itemAmount;
        }
        #endregion

        #region Stat Reducers
        public void TakeDamage(int amount)
        {
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                Death();
            }
        }

        public bool UnEquipWeapon(ItemPickUp weaponPickUp, CharacterInventory charInventory, GameObject weaponSlot)
        {
            bool previousWeaponSame = false;

            if (weapon != null)
            {
                if (weapon == weaponPickUp)
                {
                    previousWeaponSame = true;
                }
                charInventory.inventoryDisplaySlots[2].sprite = null;
                DestroyObject(weaponSlot.transform.GetChild(0).gameObject);
                weapon = null;
                currentDamage = baseDamage;
            }

            return previousWeaponSame;
        }
        #endregion

        #region Character Level Up and Death
        private void Death()
        {
            Debug.Log("You are dead, too bad :(");
            //Call to Game Manager for Death State to trigger respawn
            //Dispaly the Death visualization
        }
        #endregion
    }
}