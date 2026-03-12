using System.Collections.Generic;
using System.Linq;
using Config.ScriptableObjects;
using Domain.Enums;
using Domain.Interfaces;
using UnityEngine;

namespace Application.Services
{
    public class ShootService : IShootService
    {
        private readonly IInventoryService _inventoryService;
        private readonly ItemRegistrySO _registry;

        public ShootService(IInventoryService inventoryService, ItemRegistrySO registry)
        {
            _inventoryService = inventoryService;
            _registry = registry;
        }

        public bool TryShoot()
        {
            var slots = _inventoryService.State.slots;

            var weaponSlots = slots
                .Where(s => s.isUnlocked && !s.IsEmpty &&
                    _registry.GetById(s.item.definitionId)?.itemType == ItemType.Weapon)
                .ToList();

            if (weaponSlots.Count == 0)
            {
                Debug.LogError("[Shoot] No weapons in inventory");
                return false;
            }

            Shuffle(weaponSlots);

            foreach (var weaponSlot in weaponSlots)
            {
                var weaponDef = _registry.GetById(weaponSlot.item.definitionId);
                if (weaponDef == null) continue;

                var ammoSlot = slots.FirstOrDefault(s =>
                    s.isUnlocked && !s.IsEmpty &&
                    _registry.GetById(s.item.definitionId)?.itemType == ItemType.Ammo &&
                    _registry.GetById(s.item.definitionId)?.ammoType == weaponDef.requiredAmmoType);

                if (ammoSlot == null) continue;

                var ammoDef = _registry.GetById(ammoSlot.item.definitionId);

                ammoSlot.item.stackCount--;
                if (ammoSlot.item.stackCount <= 0)
                    ammoSlot.item = null;

                _inventoryService.SaveState();

                Debug.Log($"Shot fired from {weaponDef.displayName} using {ammoDef.displayName}. Damage: {weaponDef.damage}");
                return true;
            }

            Debug.LogError("[Shoot] No matching ammo found for any weapon in inventory");
            return false;
        }

        private void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}