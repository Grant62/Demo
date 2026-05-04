using System;
using System.Collections.Generic;
using Features.Bag.Application;
using Features.Bag.Domain;
using Features.Bag.Infrastructure;
using JKFrame;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EventSystem = JKFrame.EventSystem;

namespace Features.Bag.UI
{
    [UIWindowDataAttribute(typeof(BagPanelWin), true, "BagPanel", 1)]
    public class BagPanelWin : UI_WindowBase
    {
        [SerializeField] private SlotComponent[] _equipmentSlots;
        [SerializeField] private ItemComponent[] _bagItemSlots;
        [SerializeField] private DragGhostController _dragGhostPrefab;
        [SerializeField] private Button _closeBtn;

        private IBagService _bagService;
        private DragGhostController _currentGhost;
        private object _dragSource;
        private int _dragSourceBagIndex = -1;
        private EquipmentSlotType _dragSourceSlotType;

        private static readonly Dictionary<ItemQuality, string> QualityBgMap = new()
        {
            { ItemQuality.None, "BGempty" },
            { ItemQuality.Green, "BGgreen" },
            { ItemQuality.Orange, "BGorange" },
            { ItemQuality.Purple, "BGpurple" }
        };

        public override void OnShow()
        {
            _bagService = new BagService();
            _closeBtn.onClick.AddListener(OnCloseClick);

            SetupSlots();
            SetupBagItems();
            RegisterDragEvents();

            EventSystem.AddEventListener("BagChanged", UpdateAllDisplay);
            UpdateAllDisplay();
        }

        public override void OnClose()
        {
            CleanupDragEvents();
            EventSystem.RemoveEventListener("BagChanged", UpdateAllDisplay);
            _closeBtn.onClick.RemoveListener(OnCloseClick);

            if (_currentGhost != null)
            {
                Destroy(_currentGhost.gameObject);
                _currentGhost = null;
            }
        }

        private void OnCloseClick()
        {
            UISystem.Close<BagPanelWin>();
        }

        private void SetupSlots()
        {
            EquipmentSlotType[] types = (EquipmentSlotType[])Enum.GetValues(typeof(EquipmentSlotType));
            for (int i = 0; i < _equipmentSlots.Length && i < types.Length; i++)
            {
                _equipmentSlots[i].Setup(types[i]);
            }
        }

        private void SetupBagItems()
        {
            for (int i = 0; i < _bagItemSlots.Length; i++)
            {
                _bagItemSlots[i].Initialize(i);
            }
        }

        private void RegisterDragEvents()
        {
            for (int i = 0; i < _equipmentSlots.Length; i++)
            {
                SlotComponent slot = _equipmentSlots[i];
                slot.OnBeginDrag(OnSlotBeginDrag, slot);
                slot.OnDrag(OnSlotDrag, slot);
                slot.OnEndDrag(OnSlotEndDrag, slot);
                slot.OnClick(OnSlotClick, slot);
            }

            for (int i = 0; i < _bagItemSlots.Length; i++)
            {
                ItemComponent item = _bagItemSlots[i];
                item.OnBeginDrag(OnItemBeginDrag, item);
                item.OnDrag(OnItemDrag, item);
                item.OnEndDrag(OnItemEndDrag, item);
                item.OnClick(OnItemClick, item);
            }
        }

        private void CleanupDragEvents()
        {
            for (int i = 0; i < _equipmentSlots.Length; i++)
            {
                SlotComponent slot = _equipmentSlots[i];
                slot.RemoveAllListener();
            }

            for (int i = 0; i < _bagItemSlots.Length; i++)
            {
                ItemComponent item = _bagItemSlots[i];
                item.RemoveAllListener();
            }
        }

        private void UpdateAllDisplay()
        {
            for (int i = 0; i < _equipmentSlots.Length; i++)
            {
                UpdateSlotDisplay(_equipmentSlots[i]);
            }

            for (int i = 0; i < _bagItemSlots.Length; i++)
            {
                UpdateItemDisplay(_bagItemSlots[i]);
            }
        }

        private static Sprite LoadSpriteSafe(string address)
        {
            if (string.IsNullOrEmpty(address)) return null;
            return ResSystem.LoadAsset<Sprite>(address);
        }

        private void UpdateSlotDisplay(SlotComponent slot)
        {
            _bagService.Equipped.TryGetValue(slot.SlotType, out BagItemData item);
            Sprite iconSprite = item != null ? LoadSpriteSafe(item.iconAddress) : null;
            Sprite qualitySprite = item != null ? LoadSpriteSafe(item.qualityFrameAddress) : null;
            slot.SetItem(item, iconSprite, qualitySprite);
        }

        private void UpdateItemDisplay(ItemComponent itemComponent)
        {
            int index = itemComponent.BagIndex;
            BagItemData item = null;
            if (index >= 0 && index < _bagService.BagItems.Count)
            {
                item = _bagService.BagItems[index];
            }

            Sprite iconSprite = item != null ? LoadSpriteSafe(item.iconAddress) : null;
            Sprite qualitySprite = item != null ? LoadSpriteSafe(item.qualityFrameAddress) : null;
            itemComponent.SetItem(item, iconSprite, qualitySprite);
        }

        private void OnSlotClick(PointerEventData eventData, SlotComponent slot)
        {
            _bagService.Equipped.TryGetValue(slot.SlotType, out BagItemData item);
            if (item == null) return;

            _bagService.Unequip(slot.SlotType);
        }

        private void OnItemClick(PointerEventData eventData, ItemComponent item)
        {
            if (item.ItemData == null) return;

            _bagService.Equip(item.BagIndex);
        }

        private void OnSlotBeginDrag(PointerEventData eventData, SlotComponent slot)
        {
            _bagService.Equipped.TryGetValue(slot.SlotType, out BagItemData item);
            if (item == null) return;

            _dragSource = slot;
            _dragSourceSlotType = slot.SlotType;
            CreateGhost(item);
        }

        private void OnSlotDrag(PointerEventData eventData, SlotComponent slot)
        {
            if (_currentGhost != null)
            {
                _currentGhost.FollowPointer(eventData);
                HighlightTarget(eventData);
            }
        }

        private void OnSlotEndDrag(PointerEventData eventData, SlotComponent slot)
        {
            if (_currentGhost == null) return;

            ClearHighlights();
            SlotComponent targetSlot = FindDropTarget<SlotComponent>(eventData);
            if (targetSlot != null && targetSlot != slot)
            {
                _bagService.EquipFromEquipment(_dragSourceSlotType, targetSlot.SlotType);
            }
            else
            {
                ItemComponent targetItem = FindDropTarget<ItemComponent>(eventData);
                if (targetItem != null)
                {
                    _bagService.Unequip(_dragSourceSlotType);
                }
            }

            DestroyGhost();
            _dragSource = null;
        }

        private void OnItemBeginDrag(PointerEventData eventData, ItemComponent item)
        {
            if (item.ItemData == null) return;

            _dragSource = item;
            _dragSourceBagIndex = item.BagIndex;
            CreateGhost(item.ItemData);
        }

        private void OnItemDrag(PointerEventData eventData, ItemComponent item)
        {
            if (_currentGhost != null)
            {
                _currentGhost.FollowPointer(eventData);
                HighlightTarget(eventData);
            }
        }

        private void OnItemEndDrag(PointerEventData eventData, ItemComponent item)
        {
            if (_currentGhost == null) return;

            ClearHighlights();
            SlotComponent targetSlot = FindDropTarget<SlotComponent>(eventData);
            if (targetSlot != null)
            {
                _bagService.Equip(_dragSourceBagIndex);
            }
            else
            {
                ItemComponent targetItem = FindDropTarget<ItemComponent>(eventData);
                if (targetItem != null && targetItem != item)
                {
                    _bagService.SwapBagItems(_dragSourceBagIndex, targetItem.BagIndex);
                }
            }

            DestroyGhost();
            _dragSource = null;
            _dragSourceBagIndex = -1;
        }

        private void CreateGhost(BagItemData item)
        {
            if (_currentGhost == null)
            {
                _currentGhost = Instantiate(_dragGhostPrefab);
            }

            Sprite iconSprite = LoadSpriteSafe(item.iconAddress);
            Sprite qualitySprite = LoadSpriteSafe(item.qualityFrameAddress);
            _currentGhost.Show(iconSprite, qualitySprite, Input.mousePosition);
        }

        private void DestroyGhost()
        {
            if (_currentGhost != null)
            {
                _currentGhost.Hide();
            }
        }

        private void HighlightTarget(PointerEventData eventData)
        {
            SlotComponent slot = FindDropTarget<SlotComponent>(eventData);
            if (slot != null)
            {
                slot.SetHighlight(true);
                return;
            }

            ItemComponent item = FindDropTarget<ItemComponent>(eventData);
            if (item != null)
            {
                item.SetHighlight(true);
            }
        }

        private void ClearHighlights()
        {
            for (int i = 0; i < _equipmentSlots.Length; i++)
            {
                _equipmentSlots[i].SetHighlight(false);
            }

            for (int i = 0; i < _bagItemSlots.Length; i++)
            {
                _bagItemSlots[i].SetHighlight(false);
            }
        }

        private T FindDropTarget<T>(PointerEventData eventData) where T : Component
        {
            List<RaycastResult> results = new();
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);

            for (int i = 0; i < results.Count; i++)
            {
                T component = results[i].gameObject.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }

            return null;
        }
    }
}