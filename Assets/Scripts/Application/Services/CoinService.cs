using Domain.Interfaces;

namespace Application.Services
{
    public class CoinService : ICoinService
    {
        private readonly IInventoryService _inventoryService;

        public int Coins => _inventoryService.State.coins;

        public CoinService(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public void AddCoins(int amount)
        {
            _inventoryService.State.coins += amount;
            _inventoryService.SaveState();
        }

        public bool SpendCoins(int amount)
        {
            if (_inventoryService.State.coins < amount) return false;
            _inventoryService.State.coins -= amount;
            _inventoryService.SaveState();
            return true;
        }
    }
}