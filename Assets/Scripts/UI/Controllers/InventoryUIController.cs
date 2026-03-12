using UI.Adapters;
using UI.Popups;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Controllers
{
    public class InventoryUIController : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button shootButton;
        [SerializeField] private Button addAmmoButton;
        [SerializeField] private Button addItemButton;
        [SerializeField] private Button removeItemButton;
        [SerializeField] private Button addCoinsButton;

        [Header("Views")]
        [SerializeField] private InfoPanelView infoPanelView;
        [SerializeField] private ItemPopupView itemPopupView;
        [SerializeField] private InventoryGridAdapter gridAdapter;

        private GameInstaller _installer;

        private void Awake()
        {
            _installer = GameInstaller.Instance;

            if (_installer == null) { Debug.LogError("[InventoryUIController] GameInstaller.Instance is NULL! Is GameInstaller in the scene?"); return; }
            if (_installer.ItemRegistry == null) Debug.LogError("[InventoryUIController] ItemRegistry is NULL! Assign it in GameInstaller inspector.");
            if (_installer.InventoryConfig == null) Debug.LogError("[InventoryUIController] InventoryConfig is NULL! Assign it in GameInstaller inspector.");
            if (gridAdapter == null) Debug.LogError("[InventoryUIController] gridAdapter is NULL! Assign it in Inspector.");

            gridAdapter.Setup(
                _installer.ItemRegistry,
                _installer.InventoryConfig,
                OnSlotClicked,
                OnSlotUnlockClicked,
                OnSlotDropped
            );
        }

        private void Start()
        {
            if (_installer == null) return;

            shootButton.onClick.AddListener(OnShoot);
            addAmmoButton.onClick.AddListener(OnAddAmmo);
            addItemButton.onClick.AddListener(OnAddItem);
            removeItemButton.onClick.AddListener(OnRemoveItem);
            addCoinsButton.onClick.AddListener(OnAddCoins);

            var slots = _installer.InventoryService.State.slots;
            Debug.Log($"[InventoryUIController] Loading {slots.Count} slots into grid.");

            int unlockedCount = 0;
            foreach (var s in slots) if (s.isUnlocked) unlockedCount++;
            Debug.Log($"[InventoryUIController] Unlocked slots: {unlockedCount}, Locked: {slots.Count - unlockedCount}");

            gridAdapter.SetSlots(slots);
            RefreshInfoPanel();
        }

        private void OnShoot() { _installer.UseCases.Shoot(); RefreshUI(); }
        private void OnAddAmmo() { _installer.UseCases.AddAmmo(); RefreshUI(); }
        private void OnAddItem() { _installer.UseCases.AddRandomItem(); RefreshUI(); }
        private void OnRemoveItem() { _installer.UseCases.RemoveRandomItem(); RefreshUI(); }
        private void OnAddCoins() { _installer.UseCases.AddCoins(); RefreshInfoPanel(); }

        private void OnSlotClicked(int slotIndex)
        {
            var slot = _installer.InventoryService.State.slots[slotIndex];
            if (slot.IsEmpty) return;
            var def = _installer.ItemRegistry.GetById(slot.item.definitionId);
            if (def != null) itemPopupView.Show(slot.item, def);
        }

        private void OnSlotUnlockClicked(int slotIndex)
        {
            bool unlocked = _installer.UseCases.TryUnlockSlot(slotIndex);
            if (unlocked) RefreshUI();
        }

        private void OnSlotDropped(int fromSlot, int toSlot)
        {
            _installer.InventoryService.MoveItem(fromSlot, toSlot);
            RefreshUI();
        }

        private void RefreshUI()
        {
            gridAdapter.RefreshSlots(_installer.InventoryService.State.slots);
            RefreshInfoPanel();
        }

        private void RefreshInfoPanel()
        {
            infoPanelView.UpdateInfo(
                _installer.CoinService.Coins,
                _installer.InventoryService.GetTotalWeight()
            );
        }
    }
}