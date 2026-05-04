using System;
using System.Collections.Generic;

namespace Features.Bag.Domain
{
    [Serializable]
    public class BagState
    {
        public List<BagItemData> bagItems = new(24);
        public Dictionary<EquipmentSlotType, BagItemData> equipped = new();
    }
}