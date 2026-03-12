using System;

namespace Domain.Entities
{
    [Serializable]
    public class InventorySlot
    {
        public int index;
        public bool isUnlocked;
        public ItemInstance item;

        public bool IsEmpty => item == null;

        public InventorySlot() { }

        public InventorySlot(int index, bool isUnlocked)
        {
            this.index = index;
            this.isUnlocked = isUnlocked;
        }
    }
}
