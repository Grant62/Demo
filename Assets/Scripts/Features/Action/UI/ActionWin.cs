using System;
using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Features.Action.Application;
using Features.Bag.UI;
using Features.Battle.Domain;
using JKFrame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        [Header("敌人面板")]
        [SerializeField] private Transform _enemyBoard;

        [Header("右侧按钮")]
        [SerializeField] private Button _endTurnBtn;
        [SerializeField] private Button _mapBtn;
        [SerializeField] private Button _equipBtn;
        [SerializeField] private TMP_Text _turnNumText;

        [Header("预制体模板")]
        [SerializeField] private QuickSlot _spellSlotPrefab;
        [SerializeField] private QuickSlot _itemSlotPrefab;
        [SerializeField] private MercenaryController mercenaryControllerPrefab;
        [SerializeField] private Enemy _enemyPrefab;

        private readonly List<QuickSlot> _spellSlots = new();
        private readonly List<QuickSlot> _itemSlots = new();
        private readonly List<MercenaryController> _mercenaryCards = new();
        private readonly List<Enemy> _enemies = new();

        public event Action<int> OnSpellSlotClicked;
        public event Action<int> OnItemSlotClicked;
        public event Action<MercenaryController, int> OnMercenaryClicked;
        public event System.Action OnEndTurnClicked;
        public event System.Action OnMapClicked;

        public override void OnShow()
        {
            _endTurnBtn.onClick.AddListener(() => OnEndTurnClicked?.Invoke());
            _mapBtn.onClick.AddListener(() => OnMapClicked?.Invoke());
            _equipBtn.onClick.AddListener(() => UISystem.Show<BagWin>());

            if (_mercenaryBoard.childCount == 0)
                LoadMercenaries();
            if (_itemGrid.childCount == 0)
                LoadItems();
            if (_spellGrid.childCount == 0)
                LoadSpells();
            if (_enemyBoard.childCount == 0)
                LoadEnemies();
        }

        public override void OnClose()
        {
            _endTurnBtn.onClick.RemoveAllListeners();
            _mapBtn.onClick.RemoveAllListeners();
            _equipBtn.onClick.RemoveAllListeners();
        }

        #region 玩家状态
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
        #endregion

        #region 魔法快捷栏
        public void CreateSpellSlots(int count)
        {
            ClearSpellSlots();
            for (int i = 0; i < count; i++)
            {
                QuickSlot slot = Instantiate(_spellSlotPrefab, _spellGrid);
                slot.Index = i;
                slot.OnClick += idx => OnSpellSlotClicked?.Invoke(idx);
                _spellSlots.Add(slot);
            }
        }

        public void ClearSpellSlots()
        {
            for (int i = _spellSlots.Count - 1; i >= 0; i--)
                Destroy(_spellSlots[i].gameObject);
            _spellSlots.Clear();
        }

        public void SetSpellSlot(int index, Sprite icon)
        {
            _spellSlots[index].Setup(icon, index);
        }

        public void LoadSpells()
        {
            if (!SpellDeployService.TryGetDeployData(out List<SpellInfo> spells)) return;
            ClearSpellSlots();
            for (int i = 0; i < spells.Count; i++)
            {
                int cost = spells[i].Cost;
                QuickSlot slot = Instantiate(_spellSlotPrefab, _spellGrid);
                slot.Index = i;
                int idx = i;
                slot.OnClick += _ =>
                {
                    if (MpSystem.TrySpend(cost))
                        UpdateMP(MpSystem.CurrentMP, MpSystem.MaxMP);
                    OnSpellSlotClicked?.Invoke(idx);
                };
                Sprite icon = ResSystem.LoadAsset<Sprite>(spells[i].IconAdd);
                slot.SetupWithMpCost(icon, cost, i);
                _spellSlots.Add(slot);
            }
        }
        #endregion

        #region 道具快捷栏
        public void LoadItems()
        {
            if (!PropDeployService.TryGetDeployData(out List<(PropInfo item, int quantity)> items)) return;
            ClearItemSlots();
            for (int i = 0; i < items.Count; i++)
            {
                QuickSlot slot = Instantiate(_itemSlotPrefab, _itemGrid);
                slot.Index = i;
                int idx = i;
                slot.OnClick += _ => OnItemSlotClicked?.Invoke(idx);
                Sprite icon = ResSystem.LoadAsset<Sprite>(items[i].item.IconAdd);
                slot.SetupWithAmount(icon, items[i].quantity, i);
                _itemSlots.Add(slot);
            }
        }

        public void ClearItemSlots()
        {
            for (int i = _itemSlots.Count - 1; i >= 0; i--)
                Destroy(_itemSlots[i].gameObject);
            _itemSlots.Clear();
        }
        #endregion

        #region 雇佣兵
        public void CreateMercenaryCards(IReadOnlyList<OccupationInfo> occupations, IReadOnlyList<EntryInfo> entries)
        {
            ClearMercenaryCards();
            for (int i = 0; i < occupations.Count; i++)
            {
                MercenaryController controller = Instantiate(mercenaryControllerPrefab, _mercenaryBoard);
                controller.Index = i;
                controller.Initialize(occupations[i], entries[i]);
                controller.OnClick += (c, idx) => OnMercenaryClicked?.Invoke(c, idx);
                _mercenaryCards.Add(controller);
            }
        }

        public void LoadMercenaries()
        {
            if (MercenaryDeployService.TryGetDeployData(out List<OccupationInfo> occupations, out List<EntryInfo> entries))
                CreateMercenaryCards(occupations, entries);
        }

        public void ClearMercenaryCards()
        {
            for (int i = _mercenaryCards.Count - 1; i >= 0; i--)
                Destroy(_mercenaryCards[i].gameObject);
            _mercenaryCards.Clear();
        }

        public void RefreshAllMercenaryUI()
        {
            foreach (MercenaryController card in _mercenaryCards)
                card.UpdateUI();
        }
        #endregion

        #region 敌人
        public void LoadEnemies()
        {
            if (!EnemyDeployService.TryGetDeployData(out List<EnemyInfo> list)) return;
            LoadEnemies(list);
        }

        public void LoadEnemies(IReadOnlyList<EnemyInfo> enemies)
        {
            ClearEnemies();
            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy enemy = Instantiate(_enemyPrefab, _enemyBoard);
                enemy.Setup(enemies[i], enemies[i].InitATK, enemies[i].InitHP);
                _enemies.Add(enemy);
            }
        }

        public void ClearEnemies()
        {
            for (int i = _enemies.Count - 1; i >= 0; i--)
                Destroy(_enemies[i].gameObject);
            _enemies.Clear();
        }

        public void UpdateEnemyHP(int index, int current, int max)
        {
            _enemies[index].UpdateHP(current, max);
        }
        #endregion
    }
}