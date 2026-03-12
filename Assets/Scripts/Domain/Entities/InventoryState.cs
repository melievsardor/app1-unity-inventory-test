using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    [Serializable]
    public class InventoryState
    {
        public int coins;
        public List<InventorySlot> slots;

        public InventoryState() { }

        public InventoryState(int totalSlots, int defaultUnlocked)
        {
            coins = 0;
            slots = new List<InventorySlot>();
            for (int i = 0; i < totalSlots; i++)
                slots.Add(new InventorySlot(i, i < defaultUnlocked));
        }
    }
}
