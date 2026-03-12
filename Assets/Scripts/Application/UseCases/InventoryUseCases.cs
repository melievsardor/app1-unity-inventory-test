using Config.ScriptableObjects;
using Domain.Enums;
using Domain.Interfaces;
using UnityEngine;

namespace Application.UseCases
{
    public class InventoryUseCases
    {
        private readonly IInventoryService _inventory;
        private readonly ICoinService _coins;
        private readonly IItemFactory _factory;
        private readonly IShootService _shoot;
        private readonly ItemRegistrySO _registry;
        private readonly InventoryConfigSO _config;

        public InventoryUseCases(
            IInventoryService inventory,
            ICoinService coins,
            IItemFactory factory,
            IShootService shoot,
            ItemRegistrySO registry,
            InventoryConfigSO config)
        {
            _inventory = inventory;
            _coins = coins;
            _factory = factory;
            _shoot = shoot;
            _registry = registry;
            _config = config;
        }

        public void Shoot() => _shoot.TryShoot();

        public void AddAmmo()
        {
            var allAmmo = _registry.GetByType(ItemType.Ammo);

            var pistolDef = allAmmo.Find(x => x.ammoType == AmmoType.Pistol);
            var rifleDef = allAmmo.Find(x => x.ammoType == AmmoType.Rifle);

            if (pistolDef == null)
                Debug.LogError("[AddAmmo] Pistol ammo definition not found in ItemRegistry");
            else
                AddAmmoOfType(pistolDef);

            if (rifleDef == null)
                Debug.LogError("[AddAmmo] Rifle ammo definition not found in ItemRegistry");
            else
                AddAmmoOfType(rifleDef);
        }

        private void AddAmmoOfType(ItemDefinitionSO def)
        {
            bool added = _inventory.AddAmmo(def.itemId, _config.addAmmoAmount, out int _);
            if (!added)
                Debug.LogError($"[AddAmmo] No free slots for {def.displayName}");
        }

        public void AddRandomItem()
        {
            var types = new[] { ItemType.Weapon, ItemType.Head, ItemType.Torso };
            var type = types[Random.Range(0, types.Length)];
            var defs = _registry.GetByType(type);

            if (defs.Count == 0)
            {
                Debug.LogError($"[AddItem] No definitions for type {type} in ItemRegistry");
                return;
            }

            var def = defs[Random.Range(0, defs.Count)];
            var item = _factory.CreateItem(def.itemId);
            bool added = _inventory.AddItem(item);

            if (added)
            {
                int slotIndex = _inventory.State.slots.FindIndex(s => s.item == item);
                Debug.Log($"[AddItem] '{def.displayName}' added to slot {slotIndex}");
            }
            else
            {
                Debug.LogError("[AddItem] No free unlocked slots");
            }
        }

        public void RemoveRandomItem()
        {
            var occupied = _inventory.GetUnlockedNonEmptySlots();
            if (occupied.Count == 0)
            {
                Debug.LogError("[RemoveItem] All slots are empty");
                return;
            }

            var slot = occupied[Random.Range(0, occupied.Count)];
            var def = _registry.GetById(slot.item.definitionId);
            int idx = slot.index;
            _inventory.RemoveItemFromSlot(idx);
            Debug.Log($"[RemoveItem] '{def?.displayName ?? "Unknown"}' removed from slot {idx}");
        }

        public void AddCoins()
        {
            _coins.AddCoins(_config.addCoinsAmount);
            Debug.Log($"[Coins] +{_config.addCoinsAmount}. Total: {_coins.Coins}");
        }

        public bool TryUnlockSlot(int slotIndex)
        {
            if (!_coins.SpendCoins(_config.slotUnlockCost))
            {
                Debug.LogError($"[Unlock] Not enough coins. Need {_config.slotUnlockCost}, have {_coins.Coins}");
                return false;
            }
            bool result = _inventory.UnlockSlot(slotIndex);
            if (result) Debug.Log($"[Unlock] Slot {slotIndex} unlocked");
            return result;
        }
    }
}