using System.Collections.Generic;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces
{
    public interface ISaveService
    {
        void Save(InventoryState state);
        InventoryState Load();
        bool HasSavedData();
    }

    public interface IInventoryService
    {
        InventoryState State { get; }
        bool AddItem(ItemInstance item);
        bool RemoveItemFromSlot(int slotIndex);
        bool MoveItem(int fromSlot, int toSlot);
        bool AddAmmo(string ammoDefinitionId, int amount, out int slotsUsed);
        List<InventorySlot> GetUnlockedNonEmptySlots();
        bool UnlockSlot(int slotIndex);
        float GetTotalWeight();
        void SaveState();
    }

    public interface ICoinService
    {
        int Coins { get; }
        void AddCoins(int amount);
        bool SpendCoins(int amount);
    }

    public interface IItemFactory
    {
        ItemInstance CreateItem(string definitionId);
        ItemInstance CreateAmmo(AmmoType ammoType, int amount);
    }

    public interface IShootService
    {
        bool TryShoot();
    }
}