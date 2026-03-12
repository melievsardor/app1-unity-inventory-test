using Config.ScriptableObjects;
using Domain.Entities;
using Domain.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public class ItemPopupView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI typeText;
        [SerializeField] private TextMeshProUGUI weightText;
        [SerializeField] private TextMeshProUGUI statText;
        [SerializeField] private TextMeshProUGUI stackText;
        [SerializeField] private Image iconImage;
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            closeButton.onClick.AddListener(Hide);
            panel.SetActive(false);
        }

        public void Show(ItemInstance item, ItemDefinitionSO def)
        {
            nameText.text = def.displayName;
            typeText.text = $"Type: {def.itemType}";
            weightText.text = $"Weight: {def.weight} kg";
            stackText.text = $"Stack: {item.stackCount} / {def.maxStackSize}";
            iconImage.sprite = def.icon;

            if (def.itemType == ItemType.Weapon)
                statText.text = $"Damage: {def.damage}";
            else if (def.itemType == ItemType.Torso || def.itemType == ItemType.Head)
                statText.text = $"Armor: +{def.armorValue}";
            else if (def.itemType == ItemType.Ammo)
                statText.text = $"Ammo Type: {def.ammoType}";
            else
                statText.text = string.Empty;

            panel.SetActive(true);
        }

        public void Hide() => panel.SetActive(false);
    }
}
