using System;
using Features.Town.Domain;
using Main.Resource;
using UnityEngine;

namespace Features.Town.Application
{
    [Serializable]
    public class BuildingConfig
    {
        public TownBuildingType buildingType;
        public string displayName;
        public int goldCost;
        public int spaceCost;
    }

    public static class TownBuildManager
    {
        public static int UsedSpace { get; private set; }

        public static int MaxSpace { get => 100; }

        public static int Blueprints { get; private set; } = 3;

        static TownBuildManager()
        {
            GameSaveData data = GameSaveData.Load();
            if (data != null)
            {
                Blueprints = data.blueprints;
                UsedSpace = data.usedSpace;
            }
        }

        public static BuildingConfig[] BuildingConfigs { get; } =
        {
            new()
            {
                buildingType = TownBuildingType.祖宅,
                displayName = "祖宅",
                goldCost = 1100,
                spaceCost = 5
            },
            new()
            {
                buildingType = TownBuildingType.战友团,
                displayName = "战友团",
                goldCost = 1600,
                spaceCost = 4
            },
            new()
            {
                buildingType = TownBuildingType.游侠箭阁,
                displayName = "游侠箭阁",
                goldCost = 1800,
                spaceCost = 5
            }
        };

        public static bool TryBuild(BuildingConfig config)
        {
            if (ResourceManager.Gold < config.goldCost)
            {
                Debug.LogWarning($"金币不足：需要 {config.goldCost}，当前 {ResourceManager.Gold}");
                return false;
            }

            if (UsedSpace + config.spaceCost > MaxSpace)
            {
                Debug.LogWarning($"空间不足：需要 {config.spaceCost}，剩余 {MaxSpace - UsedSpace}");
                return false;
            }

            if (Blueprints < 1)
            {
                Debug.LogWarning("蓝图不足");
                return false;
            }

            if (!ResourceManager.TrySpendGold(config.goldCost))
            {
                return false;
            }

            Blueprints -= 1;
            UsedSpace += config.spaceCost;

            GameSaveData saveData = new()
            {
                gold = ResourceManager.Gold,
                blueprints = Blueprints,
                usedSpace = UsedSpace
            };
            saveData.Save();

            return true;
        }
    }
}
