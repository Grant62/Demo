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

        public static BuildingConfig[] BuildingConfigs { get; } =
        {
            new()
            {
                buildingType = TownBuildingType.行政大厅,
                displayName = "行政大厅",
                goldCost = 1100,
                spaceCost = 5
            },
            new()
            {
                buildingType = TownBuildingType.步兵营,
                displayName = "步兵营",
                goldCost = 1600,
                spaceCost = 4
            },
            new()
            {
                buildingType = TownBuildingType.哨兵所,
                displayName = "哨兵所",
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

            if (!ResourceManager.TrySpendGold(config.goldCost))
            {
                return false;
            }

            UsedSpace += config.spaceCost;
            return true;
        }
    }
}