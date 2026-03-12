using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config.ScriptableObjects;
using Domain.Entities;
using Domain.Interfaces;
using UnityEngine;

namespace Application.Services
{
    public class InventoryService : IInventoryService
    {
        public InventoryState State { get; private set; }

        private readonly ISaveService _saveService;
        private readonly ItemRegistrySO _registry;

        public InventoryService(ISaveService saveService, ItemRegistrySO registry)
        {
            _saveService = saveService;
            _registry = registry;
            State = _saveService.Load();
        }

        public bool AddItem(ItemInstance item)
        {
            var slot = State.slots.FirstOrDefault(s => s.isUnlocked && s.IsEmpty);
            if (slot == null) return false;
            slot.item = item;
            Save();
            return true;
        }

        public bool RemoveItemFromSlot(int slotIndex)
        {
            var slot = State.slots[slotIndex];
            if (slot.IsEmpty) return false;
            slot.item = null;
            Save();
            return true;
        }

        public bool MoveItem(int fromSlot, int toSlot)
        {
            var from = State.slots[fromSlot];
            var to = State.slots[toSlot];

            if (!to.isUnlocked) return false;

            if (!from.IsEmpty && !to.IsEmpty)
            {
                var fromDef = _registry.GetById(from.item.definitionId);
                var toDef = _registry.GetById(to.item.definitionId);

                if (fromDef != null && toDef != null &&
                    from.item.definitionId == to.item.definitionId &&
                    EffectiveMaxStack(fromDef) > 1)
                {
                    int space = EffectiveMaxStack(toDef) - to.item.stackCount;
                    int transfer = System.Math.Min(space, from.item.stackCount);
                    to.item.stackCount += transfer;
                    from.item.stackCount -= transfer;
                    if (from.item.stackCount <= 0) from.item = null;
                    Save();
                    return true;
                }

                var temp = from.item;
                from.item = to.item;
                to.item = temp;
                Save();
                return true;
            }

            to.item = from.item;
            from.item = null;
            Save();
            return true;
        }

        public bool AddAmmo(string ammoDefinitionId, int amount, out int slotsUsed)
        {
            slotsUsed = 0;
            var def = _registry.GetById(ammoDefinitionId);
            if (def == null) return false;

            int maxStack = EffectiveMaxStack(def);
            int remaining = amount;
            var usedSlotIndices = new List<int>();

            var existingStacks = State.slots
                .Where(s => s.isUnlocked && !s.IsEmpty && s.item.definitionId == ammoDefinitionId)
                .OrderBy(s => s.index)
                .ToList();

            foreach (var slot in existingStacks)
            {
                if (remaining <= 0) break;
                int space = maxStack - slot.item.stackCount;
                if (space <= 0) continue;
                int fill = System.Math.Min(space, remaining);
                slot.item.stackCount += fill;
                remaining -= fill;
                if (!usedSlotIndices.Contains(slot.index))
                    usedSlotIndices.Add(slot.index);
            }

            while (remaining > 0)
            {
                var freeSlot = State.slots.FirstOrDefault(s => s.isUnlocked && s.IsEmpty);
                if (freeSlot == null) break;
                int stackAmount = System.Math.Min(maxStack, remaining);
                freeSlot.item = new ItemInstance(def, stackAmount);
                remaining -= stackAmount;
                slotsUsed++;
                usedSlotIndices.Add(freeSlot.index);
            }

            if (usedSlotIndices.Count > 0)
            {
                var sb = new StringBuilder();
                sb.Append($"[AddAmmo] {def.displayName}: {amount - remaining} added to slots [");
                sb.Append(string.Join(", ", usedSlotIndices));
                sb.Append("]");
                if (remaining > 0)
                    sb.Append($". {remaining} could not fit (no free slots)");
                Debug.Log(sb.ToString());
            }

            Save();
            return usedSlotIndices.Count > 0;
        }

        public bool UnlockSlot(int slotIndex)
        {
            var slot = State.slots[slotIndex];
            if (slot.isUnlocked) return false;
            slot.isUnlocked = true;
            Save();
            return true;
        }

        public List<InventorySlot> GetUnlockedNonEmptySlots() =>
            State.slots.Where(s => s.isUnlocked && !s.IsEmpty).ToList();

        public float GetTotalWeight()
        {
            float total = 0f;
            foreach (var slot in State.slots)
            {
                if (slot.IsEmpty) continue;
                var def = _registry.GetById(slot.item.definitionId);
                if (def != null)
                    total += def.weight * slot.item.stackCount;
            }
            return total;
        }

        public void SaveState() => Save();

        private int EffectiveMaxStack(ItemDefinitionSO def) =>
            def.maxStackSize > 0 ? def.maxStackSize : 1;

        private void Save() => _saveService.Save(State);
    }
}