using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Item
{
    public class Inventory : MonoBehaviour
    {
        private IStorage weaponStorage = null;
        private IStorage implantStorage = null;
        private IStorage itemStorage = null;

        public void TryStore(PickUp_SO pickUp)
        {
            PickUp_SO pickUpLeft = null;

            if ((Cursed.Combat.AttackDefinition)pickUp)
                pickUpLeft = weaponStorage.Store(pickUp);

            //if(pickUpLeft)
            //   Spawn a new item
        }
    }
}