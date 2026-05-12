using System.Collections.Generic;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using GameTools;
using JKFrame;
using Services.ExcelTool;
using UnityEngine;

namespace Features.Action.Application
{
    public static class MercenaryDeployService
    {
        private static MercenaryDeployConfig _config;

        public static void LoadConfig()
        {
            _config = ResSystem.LoadAsset<MercenaryDeployConfig>("MercenaryDeployConfig");
            if (_config == null)
                Debug.LogError("未找到部署配置: MercenaryDeployConfig");
        }

        public static bool TryGetDeployData(out List<OccupationInfo> occupations, out List<EntryInfo> entries)
        {
            occupations = new List<OccupationInfo>();
            entries = new List<EntryInfo>();

            if (_config == null)
            {
                LoadConfig();
                if (_config == null) return false;
            }

            OccupationInfoContainer occContainer = BinaryDataMgr.Ins.GetTable<OccupationInfoContainer>();
            EntryInfoContainer entryContainer = BinaryDataMgr.Ins.GetTable<EntryInfoContainer>();
            if (occContainer == null || entryContainer == null) return false;

            foreach (int occId in _config.occupationIds)
            {
                if (!occContainer.DataDic.TryGetValue(occId, out OccupationInfo occupation)) continue;
                occupations.Add(occupation);
                entries.Add(occupation.EntryId > 0 && entryContainer.DataDic.TryGetValue(occupation.EntryId, out EntryInfo entry) ? entry : null);
            }

            return occupations.Count > 0;
        }
    }
}