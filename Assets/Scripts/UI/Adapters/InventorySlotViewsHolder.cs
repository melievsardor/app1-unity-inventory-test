using System;
using Config.ScriptableObjects;
using Com.ForbiddenByte.OSA.CustomAdapters.GridView;
using UI.Views;
using UnityEngine;

namespace UI.Adapters
{
    public class InventorySlotViewsHolder : CellViewsHolder
    {
        private SlotView _slotView;

        public override void CollectViews()
        {
            base.CollectViews();

            _slotView = views.GetComponent<SlotView>();

            if (_slotView == null)
                _slotView = root.GetComponentInChildren<SlotView>(true);

            if (_slotView == null)
                Debug.LogError("[InventorySlotViewsHolder] SlotView component NOT FOUND! Make sure SlotView script is on the Views child or anywhere inside the cell prefab.");
        }

        public void Bind(
            InventorySlotModel model,
            ItemRegistrySO registry,
            InventoryConfigSO config,
            Action<int> onClicked,
            Action<int> onUnlockClicked,
            Action<int, int> onDropped)
        {
            if (_slotView == null)
            {
                Debug.LogError($"[InventorySlotViewsHolder] _slotView is null at index {ItemIndex}, cannot bind.");
                return;
            }
            _slotView.Bind(model.SlotData, registry, config, onClicked, onUnlockClicked, onDropped);
        }

        public void ResetView()
        {
            _slotView?.ResetState();
        }
    }
}