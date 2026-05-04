using System.Collections.Generic;
using Features.Bag.Domain;
using JKFrame;
using UnityEngine;

namespace Features.Bag.Application
{
    public class BagService : IBagService
    {
        private readonly BagState _state;

        public IReadOnlyList<BagItemData> BagItems { get => _state.bagItems; }

        public IReadOnlyDictionary<EquipmentSlotType, BagItemData> Equipped { get => _state.equipped; }

        public BagService()
        {
            _state = new BagState();
            for (int i = 0; i < 24; i++)
            {
                _state.bagItems.Add(null);
            }
        }

        public bool CanEquip(int bagIndex)
        {
            BagItemData item = GetBagItem(bagIndex);
            return item != null && item.count > 0;
        }

        public void Equip(int bagIndex)
        {
            BagItemData item = GetBagItem(bagIndex);
            if (item == null || item.count <= 0)
            {
                Debug.LogWarning("装备失败：物品无效");
                return;
            }

            _state.equipped.TryGetValue(item.slotType, out BagItemData oldItem);

            if (oldItem != null)
            {
                Unequip(item.slotType);
            }

            RemoveItem(bagIndex, 1);
            BagItemData equippedItem = new()
            {
                id = item.id,
                itemName = item.itemName,
                iconAddress = item.iconAddress,
                qualityFrameAddress = item.qualityFrameAddress,
                quality = item.quality,
                count = 1,
                maxStack = item.maxStack,
                slotType = item.slotType
            };
            _state.equipped[item.slotType] = equippedItem;

            EventSystem.EventTrigger("BagChanged");
        }

        public void Unequip(EquipmentSlotType slotType)
        {
            if (!_state.equipped.TryGetValue(slotType, out BagItemData item))
            {
                return;
            }

            _state.equipped.Remove(slotType);

            AddItem(new BagItemData
            {
                id = item.id,
                itemName = item.itemName,
                iconAddress = item.iconAddress,
                qualityFrameAddress = item.qualityFrameAddress,
                quality = item.quality,
                count = 1,
                maxStack = item.maxStack,
                slotType = item.slotType
            });

            EventSystem.EventTrigger("BagChanged");
        }

        public void AddItem(BagItemData item)
        {
            for (int i = 0; i < _state.bagItems.Count; i++)
            {
                BagItemData existing = _state.bagItems[i];
                if (existing != null && existing.id == item.id && existing.count < existing.maxStack)
                {
                    int space = existing.maxStack - existing.count;
                    int toAdd = Mathf.Min(item.count, space);
                    existing.count += toAdd;
                    item.count -= toAdd;
                    if (item.count <= 0) return;
                }
            }

            for (int i = 0; i < _state.bagItems.Count; i++)
            {
                if (_state.bagItems[i] == null)
                {
                    BagItemData newItem = new()
                    {
                        id = item.id,
                        itemName = item.itemName,
                        iconAddress = item.iconAddress,
                        qualityFrameAddress = item.qualityFrameAddress,
                        quality = item.quality,
                        count = item.count,
                        maxStack = item.maxStack,
                        slotType = item.slotType
                    };
                    _state.bagItems[i] = newItem;
                    return;
                }
            }

            Debug.LogWarning("背包已满，无法添加物品");
        }

        public void RemoveItem(int bagIndex, int count)
        {
            BagItemData item = GetBagItem(bagIndex);
            if (item == null) return;

            item.count -= count;
            if (item.count <= 0)
            {
                _state.bagItems[bagIndex] = null;
            }
        }

        public void EquipFromEquipment(EquipmentSlotType sourceSlot, EquipmentSlotType targetSlot)
        {
            if (!_state.equipped.TryGetValue(sourceSlot, out BagItemData sourceItem))
            {
                return;
            }

            Unequip(sourceSlot);
            _state.equipped.TryGetValue(targetSlot, out BagItemData targetItem);
            if (targetItem != null)
            {
                Unequip(targetSlot);
            }

            _state.equipped[targetSlot] = sourceItem;

            EventSystem.EventTrigger("BagChanged");
        }

        public void SwapBagItems(int indexA, int indexB)
        {
            if (indexA < 0 || indexA >= _state.bagItems.Count ||
                indexB < 0 || indexB >= _state.bagItems.Count)
            {
                return;
            }

            BagItemData temp = _state.bagItems[indexA];
            _state.bagItems[indexA] = _state.bagItems[indexB];
            _state.bagItems[indexB] = temp;

            EventSystem.EventTrigger("BagChanged");
        }

        private BagItemData GetBagItem(int bagIndex)
        {
            if (bagIndex < 0 || bagIndex >= _state.bagItems.Count) return null;
            return _state.bagItems[bagIndex];
        }
    }
}