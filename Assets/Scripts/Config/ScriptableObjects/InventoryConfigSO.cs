using UnityEngine;

namespace Config.ScriptableObjects
{
    [CreateAssetMenu(fileName = "InventoryConfig", menuName = "Inventory/Inventory Config")]
    public class InventoryConfigSO : ScriptableObject
    {
        public int totalSlots = 30;
        public int defaultUnlockedSlots = 15;
        public int slotUnlockCost = 50;
        public int addCoinsAmount = 50;
        public int addAmmoAmount = 30;
    }
}
