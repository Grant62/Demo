using System.Collections.Generic;
using System.Text;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;
using UnityEngine;

namespace Features.Dungeon.Application
{
    public static class CombatService
    {
        public static void SpawnEnemyGroup(int triggerId)
        {
            EnemyGroupInfoContainer groupContainer = BinaryDataMgr.Ins.GetTable<EnemyGroupInfoContainer>();
            if (groupContainer == null)
            {
                Debug.LogError($"EnemyGroupInfoContainer 未加载");
                return;
            }

            if (!groupContainer.DataDic.TryGetValue(triggerId, out EnemyGroupInfo group))
            {
                Debug.LogError($"未找到 EnemyGroupInfo，TriggerId: {triggerId}");
                return;
            }

            string[] enemyIds = group.EnemySet.Split(',');
            Dictionary<int, EnemyInfo> enemyCache = new();
            EnemyInfoContainer infoContainer = BinaryDataMgr.Ins.GetTable<EnemyInfoContainer>();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[遭遇战] 触发ID: {triggerId} | {group.TriggerType}");

            foreach (string idStr in enemyIds)
            {
                if (!int.TryParse(idStr.Trim(), out int enemyId))
                    continue;

                if (!enemyCache.TryGetValue(enemyId, out EnemyInfo enemy))
                {
                    if (infoContainer == null || !infoContainer.DataDic.TryGetValue(enemyId, out enemy))
                    {
                        sb.AppendLine($"  → 未知敌人 ID: {enemyId}");
                        continue;
                    }
                    enemyCache[enemyId] = enemy;
                }

                sb.AppendLine($"  → {enemy.Name}({enemy.Type}) | ATK:{enemy.InitATK} HP:{enemy.InitHP} 物防:{enemy.PhyDef} 法防:{enemy.MagDef}");
            }

            Debug.Log(sb.ToString().TrimEnd());
        }
    }
}
