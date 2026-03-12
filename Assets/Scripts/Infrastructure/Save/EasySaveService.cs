using Domain.Entities;
using Domain.Interfaces;
using Config.ScriptableObjects;

namespace Infrastructure.Save
{
    public class EasySaveService : ISaveService
    {
        private const string SaveKey = "inventory_state";
        private readonly InventoryConfigSO _config;

        public EasySaveService(InventoryConfigSO config)
        {
            _config = config;
        }

        public void Save(InventoryState state)
        {
            ES3.Save(SaveKey, state);
        }

        public InventoryState Load()
        {
            if (!HasSavedData())
                return new InventoryState(_config.totalSlots, _config.defaultUnlockedSlots);

            return ES3.Load<InventoryState>(SaveKey);
        }

        public bool HasSavedData() => ES3.KeyExists(SaveKey);
    }
}
