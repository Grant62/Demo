using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Configuration.ExcelData.Container;
using Features.Town.Domain;
using Features.Town.Infrastructure;
using JKFrame;
using Services.ExcelTool;
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
        private RecruitSaveData _saveData;
        private readonly List<EntryComponent> _entryComponents = new();
        private readonly List<ResultItemComponent> _resultItems = new();

        private static readonly Dictionary<int, TownBuildingType> BuildingTypeMap = new()
        {
            { 2, TownBuildingType.战友团 },
            { 3, TownBuildingType.游侠箭阁 }
        };

        public override void OnShow()
        {
            _townSceneObj = FindObjectOfType<TownSceneObj>();
            if (_townSceneObj == null)
            {
                Debug.LogError("场景中未找到 TownSceneObj 组件");
            }

            _closeBtn.onClick.AddListener(OnCloseClick);
            LoadSaveData();
            GenerateEntries();
            RestoreResults();
        }

        public override void OnClose()
        {
            _closeBtn.onClick.RemoveAllListeners();
            ClearEntries();
            ClearResults();
        }

        private void LoadSaveData()
        {
            _saveData = SaveSystem.LoadSetting<RecruitSaveData>("RecruitData");
            if (_saveData == null)
            {
                _saveData = new RecruitSaveData();
            }
        }

        private void SaveData()
        {
            SaveSystem.SaveSetting(_saveData, "RecruitData");
        }

        private void RestoreResults()
        {
            OccupationInfoContainer container = BinaryDataMgr.Ins.GetTable<OccupationInfoContainer>();
            if (container == null)
            {
                return;
            }

            foreach (int occupationId in _saveData.occupationIds)
            {
                if (container.DataDic.TryGetValue(occupationId, out OccupationInfo info))
                {
                    GameObject resultObj = Instantiate(_resultPrefab, _resultsContainer);
                    ResultItemComponent resultComp = resultObj.GetComponent<ResultItemComponent>();
                    if (resultComp != null)
                    {
                        resultComp.Init(info);
                        _resultItems.Add(resultComp);
                    }
                }
            }
        }

        private void GenerateEntries()
        {
            ArchitectureInfoContainer container = BinaryDataMgr.Ins.GetTable<ArchitectureInfoContainer>();
            if (container == null)
            {
                return;
            }

            foreach (KeyValuePair<int, ArchitectureInfo> pair in container.DataDic)
            {
                ArchitectureInfo building = pair.Value;

                if (string.IsNullOrEmpty(building.ResAddress))
                {
                    continue;
                }

                if (!BuildingTypeMap.TryGetValue(building.Id, out TownBuildingType buildingType)
                    || _townSceneObj == null
                    || !_townSceneObj.IsBuildingActivated(buildingType))
                {
                    continue;
                }

                if (RecruitService.GetOccupations(building.Id, 1).Count == 0)
                {
                    continue;
                }

                GameObject entryObj = Instantiate(_entryPrefab, _entriesContainer);
                EntryComponent entryComp = entryObj.GetComponent<EntryComponent>();
                if (entryComp != null)
                {
                    entryComp.Init(building.Id, building.Name, building.RecruitmentFee, building.ResAddress, _itemPrefab, OnRecruitResult);
                    _entryComponents.Add(entryComp);
                }
            }
        }

        private void OnRecruitResult(OccupationInfo info)
        {
            _saveData.occupationIds.Add(info.Id);
            SaveData();

            GameObject resultObj = Instantiate(_resultPrefab, _resultsContainer);
            ResultItemComponent resultComp = resultObj.GetComponent<ResultItemComponent>();
            if (resultComp != null)
            {
                resultComp.Init(info);
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
