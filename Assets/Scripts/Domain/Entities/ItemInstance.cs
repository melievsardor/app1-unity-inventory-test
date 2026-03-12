using System;
using Config.ScriptableObjects;
using Domain.Enums;

namespace Domain.Entities
{
    [Serializable]
    public class ItemInstance
    {
        public string instanceId;
        public string definitionId;
        public int stackCount;

        public ItemInstance() { }

        public ItemInstance(ItemDefinitionSO definition, int count = 1)
        {
            instanceId = Guid.NewGuid().ToString();
            definitionId = definition.itemId;
            stackCount = count;
        }
    }
}
