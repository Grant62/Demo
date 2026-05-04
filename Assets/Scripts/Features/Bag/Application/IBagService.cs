using System.Collections.Generic;
using Features.Bag.Domain;

namespace Features.Bag.Application
{
    public interface IBagService
    {
        IReadOnlyList<BagItemData> BagItems { get; }
        IReadOnlyDictionary<EquipmentSlotType, BagItemData> Equipped { get; }

    void Equip(int bagIndex);
    void Unequip(EquipmentSlotType slotType);
    void EquipFromEquipment(EquipmentSlotType sourceSlot, EquipmentSlotType targetSlot);
    void AddItem(BagItemData item);
    void RemoveItem(int bagIndex, int count);
    void SwapBagItems(int indexA, int indexB);
    bool CanEquip(int bagIndex);
    }
}
