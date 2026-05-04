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
        [SerializeField] private Transform _entriesRoot;

        private TownSceneObj _townSceneObj;
        private readonly List<BuildItemComponent> _buildItems = new();

        public override void OnShow()
        {
            _townSceneObj = FindObjectOfType<TownSceneObj>();
            if (_townSceneObj == null)
            {
                Debug.LogError("场景中未找到 TownSceneObj 组件");
            }

            // 清除上次打开时动态创建的子物体
            for (int i = _entriesRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(_entriesRoot.GetChild(i).gameObject);
            }

            _buildItems.Clear();

            // 动态加载 BuildItem
            for (int i = 0; i < TownBuildManager.BuildingConfigs.Length; i++)
            {
                BuildingConfig config = TownBuildManager.BuildingConfigs[i];
                TownBuildingType type = config.buildingType;
                BuildItemComponent item = ResSystem.InstantiateGameObject<BuildItemComponent>("BuildItem", _entriesRoot);
                item.Init(config, () => OnBuildClick(type));
                _buildItems.Add(item);
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
            _costText.text = $"蓝图：{TownBuildManager.Blueprints}    空间容量：{TownBuildManager.UsedSpace} / {TownBuildManager.MaxSpace}费";

            for (int i = 0; i < _buildItems.Count && i < TownBuildManager.BuildingConfigs.Length; i++)
            {
                BuildingConfig config = TownBuildManager.BuildingConfigs[i];
                bool alreadyBuilt = _townSceneObj != null && _townSceneObj.IsBuildingActivated(config.buildingType);
                bool canAfford = ResourceManager.Gold >= config.goldCost;
                bool hasSpace = TownBuildManager.UsedSpace + config.spaceCost <= TownBuildManager.MaxSpace;
                bool hasBlueprint = TownBuildManager.Blueprints > 0;
                _buildItems[i].SetInteractable(!alreadyBuilt && canAfford && hasSpace && hasBlueprint);
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