using UnityEngine;
using Cursed.Item;

namespace Cursed.Character
{
    public class InventoryEntry
    {
        public SS_ItemPickUp invEntry;
        public int stackSize;
        public int inventorySlot;
        public int hotBarSlot;
        public Sprite hbSprite;

        public InventoryEntry(int stackSize, SS_ItemPickUp invEntry, Sprite hbSprite)
        {
            this.invEntry = invEntry;

            this.stackSize = stackSize;
            this.hotBarSlot = 0;
            this.inventorySlot = 0;
            this.hbSprite = hbSprite;
        }
    }
}