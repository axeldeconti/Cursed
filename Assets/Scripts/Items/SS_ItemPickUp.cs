using UnityEngine;
using Cursed.Character;

namespace Cursed.Item
{
    public class SS_ItemPickUp : MonoBehaviour
    {
        public SS_ItemPickUps_SO itemDefinition;

        public CharacterStats charStats;
        CharacterInventory charInventory;

        GameObject foundStats;

        #region Constructors
        public SS_ItemPickUp()
        {
            charInventory = CharacterInventory.instance;
        }
        #endregion

        void Start()
        {
            foundStats = GameObject.FindGameObjectWithTag("Player");
            charStats = foundStats.GetComponent<CharacterStats>();
        }

        void StoreItem()
        {
            charInventory.StoreItem(this);
        }

        public void UseItem()
        {
            switch (itemDefinition.itemType)
            {
                case ItemTypeDefinitions.HEALTH:
                    //charStats.ApplyHealth(itemDefinition.itemAmount);
                    break;
                case ItemTypeDefinitions.WEAPON:
                    //charStats.ChangeWeapon(this);
                    break;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                if (itemDefinition.isStorable)
                {
                    StoreItem();
                }
                else
                {
                    UseItem();
                }
            }
        }
    }
}