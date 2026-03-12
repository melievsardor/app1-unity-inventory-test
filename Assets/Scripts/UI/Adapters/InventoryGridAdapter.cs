using System;
using System.Collections.Generic;
using Config.ScriptableObjects;
using Domain.Entities;
using Com.ForbiddenByte.OSA.CustomAdapters.GridView;
using Com.ForbiddenByte.OSA.DataHelpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Adapters
{
    public class InventoryGridAdapter : GridAdapter<GridParams, InventorySlotViewsHolder>
    {
        public SimpleDataHelper<InventorySlotModel> Data { get; private set; }

        private ItemRegistrySO _registry;
        private InventoryConfigSO _config;
        private Action<int> _onSlotClicked;
        private Action<int> _onSlotUnlockClicked;
        private Action<int, int> _onSlotDropped;

        private List<InventorySlot> _pendingSlots;
        private bool _initialized;

        public void Setup(
            ItemRegistrySO registry,
            InventoryConfigSO config,
            Action<int> onSlotClicked,
            Action<int> onSlotUnlockClicked,
            Action<int, int> onSlotDropped)
        {
            _registry = registry;
            _config = config;
            _onSlotClicked = onSlotClicked;
            _onSlotUnlockClicked = onSlotUnlockClicked;
            _onSlotDropped = onSlotDropped;

            if (_registry == null) Debug.LogError("[InventoryGridAdapter] Setup: registry is NULL!");
            if (_config == null) Debug.LogError("[InventoryGridAdapter] Setup: config is NULL!");
        }

        protected override void Start()
        {
            EnsureCellPrefabLayoutElement();

            Data = new SimpleDataHelper<InventorySlotModel>(this);
            base.Start();
            _initialized = true;

            Debug.Log("[InventoryGridAdapter] OSA initialized successfully.");

            if (_pendingSlots != null)
            {
                Debug.Log($"[InventoryGridAdapter] Applying {_pendingSlots.Count} pending slots.");
                SetSlotsInternal(_pendingSlots);
                _pendingSlots = null;
            }
        }

        protected override void UpdateCellViewsHolder(InventorySlotViewsHolder newOrRecycled)
        {
            var model = Data[newOrRecycled.ItemIndex];
            newOrRecycled.Bind(model, _registry, _config, _onSlotClicked, _onSlotUnlockClicked, _onSlotDropped);
        }

        protected override void OnBeforeRecycleOrDisableCellViewsHolder(InventorySlotViewsHolder inRecycleBinOrVisible, int newItemIndex)
        {
            inRecycleBinOrVisible.ResetView();
            base.OnBeforeRecycleOrDisableCellViewsHolder(inRecycleBinOrVisible, newItemIndex);
        }

        public void SetSlots(List<InventorySlot> slots)
        {
            if (!_initialized)
            {
                Debug.Log($"[InventoryGridAdapter] OSA not ready yet, queuing {slots.Count} slots.");
                _pendingSlots = slots;
                return;
            }
            SetSlotsInternal(slots);
        }

        public void RefreshSlots(List<InventorySlot> slots)
        {
            if (!_initialized)
            {
                _pendingSlots = slots;
                return;
            }

            if (Data.Count != slots.Count)
            {
                SetSlotsInternal(slots);
                return;
            }

            for (int i = 0; i < slots.Count; i++)
                Data[i].SlotData = slots[i];

            Data.NotifyListChangedExternally();
        }

        private void SetSlotsInternal(List<InventorySlot> slots)
        {
            Debug.Log($"[InventoryGridAdapter] SetSlotsInternal: {slots.Count} slots.");
            var models = new List<InventorySlotModel>(slots.Count);
            foreach (var slot in slots)
                models.Add(new InventorySlotModel(slot));
            Data.ResetItems(models);
        }

        private void EnsureCellPrefabLayoutElement()
        {
            var prefab = Parameters.Grid.CellPrefab;
            if (prefab == null)
            {
                Debug.LogError("[InventoryGridAdapter] Cell prefab is NULL! Assign it in GridParams on the OSA component.");
                return;
            }

            if (prefab.GetComponent<LayoutElement>() == null)
            {
                prefab.gameObject.AddComponent<LayoutElement>();
                Debug.Log("[InventoryGridAdapter] LayoutElement added to cell prefab automatically.");
            }
        }
    }
}