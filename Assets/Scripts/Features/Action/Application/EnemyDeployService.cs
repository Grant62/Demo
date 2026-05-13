using System.Collections.Generic;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using GameTools;
using JKFrame;
using Services.ExcelTool;
using UnityEngine;

namespace Features.Action.Application
{
    public static class EnemyDeployService
    {
        private static EnemyDeployConfig _config;

        public static void LoadConfig()
        {
            _config = ResSystem.LoadAsset<EnemyDeployConfig>("EnemyDeployConfig");
            if (_config == null)
                Debug.LogError("未找到敌人配置: EnemyDeployConfig");
        }

        public static bool TryGetDeployData(out List<EnemyInfo> enemies)
        {
            enemies = new List<EnemyInfo>();

            if (_config == null)
            {
                LoadConfig();
                if (_config == null) return false;
            }

            EnemyInfoContainer container = BinaryDataMgr.Ins.GetTable<EnemyInfoContainer>();
            if (container == null) return false;

            foreach (int id in _config.enemyIds)
            {
                if (container.DataDic.TryGetValue(id, out EnemyInfo enemy))
                    enemies.Add(enemy);
            }

            return enemies.Count > 0;
        }
    }
}