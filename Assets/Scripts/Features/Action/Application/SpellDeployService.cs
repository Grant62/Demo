using System.Collections.Generic;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using GameTools;
using JKFrame;
using Services.ExcelTool;
using UnityEngine;

namespace Features.Action.Application
{
    public static class SpellDeployService
    {
        private static SpellDeployConfig _config;

        public static void LoadConfig()
        {
            _config = ResSystem.LoadAsset<SpellDeployConfig>("SpellDeployConfig");
            if (_config == null)
                Debug.LogError("未找到法术配置: SpellDeployConfig");
        }

        public static bool TryGetDeployData(out List<SpellInfo> spells)
        {
            spells = new List<SpellInfo>();

            if (_config == null)
            {
                LoadConfig();
                if (_config == null) return false;
            }

            SpellInfoContainer container = BinaryDataMgr.Ins.GetTable<SpellInfoContainer>();
            if (container == null) return false;

            foreach (int id in _config.spellIds)
            {
                if (container.DataDic.TryGetValue(id, out SpellInfo spell))
                    spells.Add(spell);
            }

            return spells.Count > 0;
        }
    }
}