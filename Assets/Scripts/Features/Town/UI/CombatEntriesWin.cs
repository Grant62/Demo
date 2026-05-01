using System.Collections.Generic;
using Features.Town.Domain;
using Features.Town.Infrastructure;
using JKFrame;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Town.UI
{
    [UIWindowDataAttribute(typeof(CombatEntriesWin), false, "CombatEntriesWin", 1)]
    public class CombatEntriesWin : UI_WindowBase
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private RectTransform _entriesContainer;
        [SerializeField] private RectTransform _resultsContainer;
        [SerializeField] private GameObject _entryPrefab;
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private GameObject _resultPrefab;

        private TownSceneObj _townSceneObj;
        private readonly List<EntryComponent> _entryComponents = new();
        private readonly List<ResultItemComponent> _resultItems = new();

        private static readonly (UnlockSource source, string title)[] AllEntries =
        {
            (UnlockSource.步兵营, "步兵营"),
            (UnlockSource.哨兵所, "哨兵所"),
            (UnlockSource.教堂, "教堂"),
            (UnlockSource.盗贼工会, "盗贼工会"),
            (UnlockSource.狂战士营地, "狂战士营地"),
            (UnlockSource.修道院, "修道院"),
            (UnlockSource.魔法师协会, "魔法师协会")
        };

        public override void OnShow()
        {
            _townSceneObj = FindObjectOfType<TownSceneObj>();
            if (_townSceneObj == null)
            {
                Debug.LogError("场景中未找到 TownSceneObj 组件");
            }

            _closeBtn.onClick.AddListener(OnCloseClick);
            GenerateEntries();
        }

        public override void OnClose()
        {
            _closeBtn.onClick.RemoveAllListeners();
            ClearEntries();
            ClearResults();
        }

        private void GenerateEntries()
        {
            foreach ((UnlockSource source, string title) in AllEntries)
            {
                if (!IsBuildingUnlocked(source))
                {
                    continue;
                }

                if (RecruitService.GetPool(source).Count == 0)
                {
                    continue;
                }

                GameObject entryObj = Instantiate(_entryPrefab, _entriesContainer);
                EntryComponent entryComp = entryObj.GetComponent<EntryComponent>();
                if (entryComp != null)
                {
                    entryComp.Init(source, title, _itemPrefab, OnRecruitResult);
                    _entryComponents.Add(entryComp);
                }
            }
        }

        private bool IsBuildingUnlocked(UnlockSource source)
        {
            if (_townSceneObj == null)
            {
                return false;
            }

            switch (source)
            {
                case UnlockSource.步兵营:
                    return _townSceneObj.IsBuildingActivated(TownBuildingType.步兵营);
                case UnlockSource.哨兵所:
                    return _townSceneObj.IsBuildingActivated(TownBuildingType.哨兵所);
                default:
                    return false;
            }
        }

        private void OnRecruitResult(RecruitEntry entry)
        {
            GameObject resultObj = Instantiate(_resultPrefab, _resultsContainer);
            ResultItemComponent resultComp = resultObj.GetComponent<ResultItemComponent>();
            if (resultComp != null)
            {
                resultComp.Init(entry);
                _resultItems.Add(resultComp);
            }
        }

        private void ClearEntries()
        {
            foreach (EntryComponent comp in _entryComponents)
            {
                comp.Clear();
                if (comp != null)
                {
                    Destroy(comp.gameObject);
                }
            }

            _entryComponents.Clear();
        }

        private void ClearResults()
        {
            foreach (ResultItemComponent comp in _resultItems)
            {
                if (comp != null)
                {
                    Destroy(comp.gameObject);
                }
            }

            _resultItems.Clear();
        }

        private void OnCloseClick()
        {
            UISystem.Close<CombatEntriesWin>();
        }
    }
}