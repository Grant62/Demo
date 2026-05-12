using System.Collections.Generic;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using GameTools;
using JKFrame;
using Services.ExcelTool;
using UnityEngine;

namespace Features.Action.Application
{
    public static class PropDeployService
    {
        private static PropDeployConfig _config;

        public static void LoadConfig()
        {
            _config = ResSystem.LoadAsset<PropDeployConfig>("PropDeployConfig");
            if (_config == null)
                Debug.LogError("未找到道具配置: PropDeployConfig");
        }

        public static bool TryGetDeployData(out List<(PropInfo item, int quantity)> items)
        {
            items = new List<(PropInfo, int)>();

            if (_config == null)
            {
                LoadConfig();
                if (_config == null) return false;
            }

            PropInfoContainer container = BinaryDataMgr.Ins.GetTable<PropInfoContainer>();
            if (container == null) return false;

            foreach (PropSlotConfig slot in _config.slots)
            {
                if (int.TryParse(slot.itemId, out int id) && container.DataDic.TryGetValue(id, out PropInfo prop))
                    items.Add((prop, slot.quantity));
            }

            return items.Count > 0;
        }
    }
}