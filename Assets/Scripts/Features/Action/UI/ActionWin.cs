using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Features.Action.Domain;
using Features.Bag.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JKFrame;

namespace Features.Action.UI
{
    [UIWindowDataAttribute(typeof(ActionWin), true, "ActionWin", 1)]
    public class ActionWin : UI_WindowBase
    {
        [Header("玩家状态条")]
        [SerializeField] private BarWidget _hpBar;
        [SerializeField] private BarWidget _mpBar;
        [SerializeField] private BarWidget _sanBar;

        [Header("行动点")]
        [SerializeField] private TMP_Text _actionPointText;

        [Header("快捷栏容器")]
        [SerializeField] private Transform _spellGrid;
        [SerializeField] private Transform _itemGrid;

        [Header("雇佣兵面板")]
        [SerializeField] private Transform _mercenaryBoard;

        [Header("右侧按钮")]
        [SerializeField] private Button _endTurnBtn;
        [SerializeField] private Button _mapBtn;
        [SerializeField] private Button _equipBtn;
        [SerializeField] private TMP_Text _turnNumText;

        [Header("预制体模板")]
        [SerializeField] private QuickSlot _spellSlotPrefab;
        [SerializeField] private QuickSlot _itemSlotPrefab;
        [SerializeField] private MercenaryCard _mercenaryCardPrefab;

        private readonly List<QuickSlot> _spellSlots = new();
        private readonly List<QuickSlot> _itemSlots = new();
        private readonly List<MercenaryCard> _mercenaryCards = new();

        public event System.Action<int> OnSpellSlotClicked;
        public event System.Action<int> OnItemSlotClicked;
        public event System.Action<MercenaryCard, int> OnMercenaryClicked;
        public event System.Action OnEndTurnClicked;
        public event System.Action OnMapClicked;
        public override void OnShow()
        {
            _endTurnBtn.onClick.AddListener(() => OnEndTurnClicked.Invoke());
            _mapBtn.onClick.AddListener(() => OnMapClicked.Invoke());
            _equipBtn.onClick.AddListener(() => UISystem.Show<BagPanelWin>());
        }

        public override void OnClose()
        {
            _endTurnBtn.onClick.RemoveAllListeners();
            _mapBtn.onClick.RemoveAllListeners();
            _equipBtn.onClick.RemoveAllListeners();
        }

        public void UpdateHP(int current, int max)
        {
            _hpBar.UpdateValue(current, max);
        }

        public void UpdateMP(int current, int max)
        {
            _mpBar.UpdateValue(current, max);
        }

        public void UpdateSan(int current, int max)
        {
            _sanBar.UpdateValue(current, max);
        }

        public void UpdateActionPoint(int current)
        {
            _actionPointText.text = current.ToString();
        }

        public void UpdateTurnNum(int num)
        {
            _turnNumText.text = $"TURN {num}";
        }

        public void CreateSpellSlots(int count)
        {
            ClearSpellSlots();
            for (int i = 0; i < count; i++)
            {
                QuickSlot slot = Instantiate(_spellSlotPrefab, _spellGrid);
                slot.Index = i;
                slot.OnClick += idx => OnSpellSlotClicked.Invoke(idx);
                _spellSlots.Add(slot);
            }
        }

        public void ClearSpellSlots()
        {
            for (int i = _spellSlots.Count - 1; i >= 0; i--)
            {
                Destroy(_spellSlots[i].gameObject);
            }
            _spellSlots.Clear();
        }

        public void SetSpellSlot(int index, Sprite icon)
        {
            _spellSlots[index].Setup(icon, index);
        }

        public void CreateItemSlots(int count)
        {
            ClearItemSlots();
            for (int i = 0; i < count; i++)
            {
                QuickSlot slot = Instantiate(_itemSlotPrefab, _itemGrid);
                slot.Index = i;
                slot.OnClick += idx => OnItemSlotClicked.Invoke(idx);
                _itemSlots.Add(slot);
            }
        }

        public void ClearItemSlots()
        {
            for (int i = _itemSlots.Count - 1; i >= 0; i--)
            {
                Destroy(_itemSlots[i].gameObject);
            }
            _itemSlots.Clear();
        }

        public void SetItemSlot(int index, Sprite icon)
        {
            _itemSlots[index].Setup(icon, index);
        }

        public void CreateMercenaryCards(IReadOnlyList<OccupationInfo> occupations, IReadOnlyList<EntryInfo> entries)
        {
            ClearMercenaryCards();
            for (int i = 0; i < occupations.Count; i++)
            {
                MercenaryCard card = Instantiate(_mercenaryCardPrefab, _mercenaryBoard);
                card.Index = i;
                EntryInfo entry = occupations[i].EntryId > 0
                    ? FindEntry(entries, occupations[i].EntryId)
                    : null;
                card.Initialize(occupations[i], entry);
                card.OnClick += (c, idx) => OnMercenaryClicked.Invoke(c, idx);
                _mercenaryCards.Add(card);
            }
        }

        public void ClearMercenaryCards()
        {
            for (int i = _mercenaryCards.Count - 1; i >= 0; i--)
            {
                Destroy(_mercenaryCards[i].gameObject);
            }
            _mercenaryCards.Clear();
        }

        public void RefreshAllMercenaryUI()
        {
            foreach (MercenaryCard card in _mercenaryCards)
            {
                card.UpdateUI();
            }
        }

        private static EntryInfo FindEntry(IReadOnlyList<EntryInfo> entries, int entryId)
        {
            foreach (EntryInfo e in entries)
            {
                if (e.Id == entryId) return e;
            }
            return null;
        }
    }
}
