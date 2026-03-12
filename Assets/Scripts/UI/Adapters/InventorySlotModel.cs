using Domain.Entities;

namespace UI.Adapters
{
    public class InventorySlotModel
    {
        public InventorySlot SlotData;

        public InventorySlotModel(InventorySlot slot)
        {
            SlotData = slot;
        }
    }
}
