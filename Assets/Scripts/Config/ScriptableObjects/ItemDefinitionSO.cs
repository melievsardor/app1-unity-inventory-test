using UnityEngine;
using Domain.Enums;

namespace Config.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ItemDefinition", menuName = "Inventory/Item Definition")]
    public class ItemDefinitionSO : ScriptableObject
    {
        public string itemId;
        public string displayName;
        public ItemType itemType;
        public float weight;
        public int maxStackSize = 1;
        public Sprite icon;

        [Header("Weapon")]
        public int damage;
        public AmmoType requiredAmmoType;

        [Header("Armor")]
        public int armorValue;

        [Header("Ammo")]
        public AmmoType ammoType;
    }
}
