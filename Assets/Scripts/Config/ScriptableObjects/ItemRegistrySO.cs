using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Config.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ItemRegistry", menuName = "Inventory/Item Registry")]
    public class ItemRegistrySO : ScriptableObject
    {
        public List<ItemDefinitionSO> items;

        public ItemDefinitionSO GetById(string id) =>
            items.FirstOrDefault(x => x.itemId == id);

        public List<ItemDefinitionSO> GetByType(Domain.Enums.ItemType type) =>
            items.Where(x => x.itemType == type).ToList();
    }
}
