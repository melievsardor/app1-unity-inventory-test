using System;
using Config.ScriptableObjects;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Infrastructure.Factories
{
    public class ItemFactory : IItemFactory
    {
        private readonly ItemRegistrySO _registry;

        public ItemFactory(ItemRegistrySO registry)
        {
            _registry = registry;
        }

        public ItemInstance CreateItem(string definitionId)
        {
            var def = _registry.GetById(definitionId);
            if (def == null) throw new Exception($"[ItemFactory] Item definition not found: {definitionId}");
            return new ItemInstance(def, 1);
        }

        public ItemInstance CreateAmmo(AmmoType ammoType, int amount)
        {
            var items = _registry.GetByType(ItemType.Ammo);
            var def = items.Find(x => x.ammoType == ammoType);
            if (def == null) throw new Exception($"[ItemFactory] Ammo definition not found for type: {ammoType}");

            var instance = new ItemInstance(def, 0);
            instance.stackCount = Math.Min(amount, def.maxStackSize);
            return instance;
        }
    }
}
