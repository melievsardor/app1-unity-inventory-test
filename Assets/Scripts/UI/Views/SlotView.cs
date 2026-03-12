using Config.ScriptableObjects;
using Domain.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Views
{
    public class SlotView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI stackCountText;
        [SerializeField] private GameObject lockOverlay;
        [SerializeField] private Button lockButton;
        [SerializeField] private TextMeshProUGUI lockPriceText;
        [SerializeField] private Image backgroundImage;

        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f);

        private InventorySlot _slot;
        private System.Action<int> _onClicked;
        private System.Action<int> _onUnlockClicked;
        private System.Action<int, int> _onDropped;

        private static SlotView _draggingSlot;
        private static GameObject _dragGhost;
        private Canvas _rootCanvas;

        private void Awake()
        {
            _rootCanvas = GetComponentInParent<Canvas>()?.rootCanvas;
            if (lockButton != null)
                lockButton.onClick.AddListener(OnUnlockButtonClicked);
        }

        public void Bind(
            InventorySlot slot,
            ItemRegistrySO registry,
            InventoryConfigSO config,
            System.Action<int> onClicked,
            System.Action<int> onUnlockClicked,
            System.Action<int, int> onDropped)
        {
            if (slot == null)
            {
                Debug.LogError("[SlotView] Bind called with null slot!");
                return;
            }

            _slot = slot;
            _onClicked = onClicked;
            _onUnlockClicked = onUnlockClicked;
            _onDropped = onDropped;

            if (!slot.isUnlocked)
            {
                SetLocked(config.slotUnlockCost);
                return;
            }

            SetUnlocked();

            if (slot.IsEmpty)
            {
                SetEmpty();
                return;
            }

            if (registry == null)
            {
                Debug.LogError($"[SlotView] Slot {slot.index} has item '{slot.item.definitionId}' but registry is NULL!");
                SetEmpty();
                return;
            }

            var def = registry.GetById(slot.item.definitionId);
            if (def == null)
            {
                Debug.LogError($"[SlotView] ItemDefinition NOT FOUND for id='{slot.item.definitionId}'.");
                SetEmpty();
                return;
            }

            if (iconImage != null)
            {
                iconImage.sprite = def.icon;
                iconImage.enabled = true;
                iconImage.color = def.icon != null ? Color.white : new Color(1, 1, 1, 0.2f);
            }

            bool showStack = def.maxStackSize > 1 && slot.item.stackCount > 1;
            if (stackCountText != null)
            {
                stackCountText.gameObject.SetActive(showStack);
                if (showStack)
                    stackCountText.text = slot.item.stackCount.ToString();
            }
        }

        public void ResetState()
        {
            if (iconImage != null) { iconImage.sprite = null; iconImage.enabled = false; }
            if (stackCountText != null) stackCountText.gameObject.SetActive(false);
            if (lockOverlay != null) lockOverlay.SetActive(false);
            if (lockButton != null) lockButton.gameObject.SetActive(false);
            if (lockPriceText != null) lockPriceText.gameObject.SetActive(false);
            if (backgroundImage != null) backgroundImage.color = normalColor;
        }

        private void SetLocked(int price)
        {
            if (backgroundImage != null) backgroundImage.color = lockedColor;
            if (lockOverlay != null) lockOverlay.SetActive(true);
            if (lockButton != null) lockButton.gameObject.SetActive(true);
            if (lockPriceText != null)
            {
                lockPriceText.gameObject.SetActive(true);
                lockPriceText.text = $"Buy({price})";
            }
            if (iconImage != null) iconImage.enabled = false;
            if (stackCountText != null) stackCountText.gameObject.SetActive(false);
        }

        private void SetUnlocked()
        {
            if (backgroundImage != null) backgroundImage.color = normalColor;
            if (lockOverlay != null) lockOverlay.SetActive(false);
            if (lockButton != null) lockButton.gameObject.SetActive(false);
            if (lockPriceText != null) lockPriceText.gameObject.SetActive(false);
        }

        private void SetEmpty()
        {
            if (iconImage != null) iconImage.enabled = false;
            if (stackCountText != null) stackCountText.gameObject.SetActive(false);
        }

        private void OnUnlockButtonClicked()
        {
            if (_slot != null)
                _onUnlockClicked?.Invoke(_slot.index);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.dragging) return;
            if (_slot == null || !_slot.isUnlocked || _slot.IsEmpty) return;
            _onClicked?.Invoke(_slot.index);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_slot == null || !_slot.isUnlocked || _slot.IsEmpty)
            {
                eventData.pointerDrag = null;
                return;
            }

            _draggingSlot = this;
            if (_rootCanvas == null) _rootCanvas = GetComponentInParent<Canvas>()?.rootCanvas;
            if (_rootCanvas == null) return;

            _dragGhost = new GameObject("DragGhost");
            _dragGhost.transform.SetParent(_rootCanvas.transform, false);
            _dragGhost.transform.SetAsLastSibling();

            var img = _dragGhost.AddComponent<Image>();
            img.sprite = iconImage != null ? iconImage.sprite : null;
            img.raycastTarget = false;
            img.preserveAspect = true;

            var rt = _dragGhost.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(80, 80);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rootCanvas.GetComponent<RectTransform>(),
                eventData.position,
                _rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _rootCanvas.worldCamera,
                out Vector2 localPos);
            rt.localPosition = localPos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragGhost == null || _rootCanvas == null) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rootCanvas.GetComponent<RectTransform>(),
                eventData.position,
                _rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _rootCanvas.worldCamera,
                out Vector2 localPos);
            _dragGhost.GetComponent<RectTransform>().localPosition = localPos;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_dragGhost != null) { Destroy(_dragGhost); _dragGhost = null; }
            _draggingSlot = null;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (_draggingSlot == null || _draggingSlot == this || _slot == null) return;
            if (!_slot.isUnlocked) return;
            _onDropped?.Invoke(_draggingSlot._slot.index, _slot.index);
        }
    }
}