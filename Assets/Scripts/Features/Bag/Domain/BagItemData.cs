using System;

namespace Features.Bag.Domain
{
    [Serializable]
    public class BagItemData
    {
        public int id;
        public string itemName;
        public string iconAddress;
        public string qualityFrameAddress;
        public ItemQuality quality;
        public int count;
        public int maxStack = 99;
        public EquipmentSlotType slotType;
    }
}
