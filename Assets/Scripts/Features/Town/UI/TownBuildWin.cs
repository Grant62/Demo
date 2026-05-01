using System.Collections.Generic;
using Features.Town.Application;
using Features.Town.Domain;
using Features.Town.Infrastructure;
using JKFrame;
using Main.Resource;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Town.UI
{
    [UIWindowDataAttribute(typeof(TownBuildWin), false, "TownBuildWin", 1)]
    public class TownBuildWin : UI_WindowBase
    {
        [SerializeField] private TMP_Text _costText;
        [SerializeField] private Button _closeBtn;

        private TownSceneObj _townSceneObj;
        private readonly List<BuildItemComponent> _buildItems = new();

        public override void OnShow()
        {
            _townSceneObj = FindObjectOfType<TownSceneObj>();
            if (_townSceneObj == null)
            {
                Debug.LogError("场景中未找到 TownSceneObj 组件");
            }

            _buildItems.Clear();
            _buildItems.AddRange(GetComponentsInChildren<BuildItemComponent>(true));

            for (int i = 0; i < _buildItems.Count && i < TownBuildManager.BuildingConfigs.Length; i++)
            {
                BuildingConfig config = TownBuildManager.BuildingConfigs[i];
                TownBuildingType type = config.buildingType;
                _buildItems[i].Init(config, () => OnBuildClick(type));
            }

            _closeBtn.onClick.AddListener(OnCloseClick);
            EventSystem.AddEventListener("GoldChanged", RefreshUI);
            RefreshUI();
        }

        public override void OnClose()
        {
            _closeBtn.onClick.RemoveListener(OnCloseClick);
            EventSystem.RemoveEventListener("GoldChanged", RefreshUI);
            _buildItems.Clear();
        }

        private void OnCloseClick()
        {
            UISystem.Close<TownBuildWin>();
        }

        private void RefreshUI()
        {
            _costText.text = $"空间容量：{TownBuildManager.UsedSpace} / {TownBuildManager.MaxSpace}费";

            for (int i = 0; i < _buildItems.Count && i < TownBuildManager.BuildingConfigs.Length; i++)
            {
                BuildingConfig config = TownBuildManager.BuildingConfigs[i];
                bool alreadyBuilt = _townSceneObj != null && _townSceneObj.IsBuildingActivated(config.buildingType);
                bool canAfford = ResourceManager.Gold >= config.goldCost;
                bool hasSpace = TownBuildManager.UsedSpace + config.spaceCost <= TownBuildManager.MaxSpace;
                _buildItems[i].SetInteractable(!alreadyBuilt && canAfford && hasSpace);
            }
        }

        private void OnBuildClick(TownBuildingType type)
        {
            if (_townSceneObj != null && _townSceneObj.IsBuildingActivated(type))
            {
                return;
            }

            foreach (BuildingConfig config in TownBuildManager.BuildingConfigs)
            {
                if (config.buildingType == type && TownBuildManager.TryBuild(config))
                {
                    _townSceneObj?.ActivateBuilding(type);
                    RefreshUI();
                    return;
                }
            }
        }
    }
}