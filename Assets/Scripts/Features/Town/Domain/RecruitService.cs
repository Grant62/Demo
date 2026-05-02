using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Configuration.ExcelData.Container;
using Services.ExcelTool;
using UnityEngine;

namespace Features.Town.Domain
{
    public static class RecruitService
    {
        private static Dictionary<int, int[]> _buildingOccupationMap;

        private static void EnsureMapping()
        {
            if (_buildingOccupationMap != null)
            {
                return;
            }

            _buildingOccupationMap = new Dictionary<int, int[]>
            {
                { 2, new[] { 1, 2, 3, 4 } },
                { 3, new[] { 5, 6 } }
            };
        }

        private static OccupationInfoContainer GetContainer()
        {
            return BinaryDataMgr.Ins.GetTable<OccupationInfoContainer>();
        }

        public static List<OccupationInfo> GetOccupations(int buildingId, int level)
        {
            EnsureMapping();
            OccupationInfoContainer container = GetContainer();
            if (container == null)
            {
                return new List<OccupationInfo>();
            }

            if (!_buildingOccupationMap.TryGetValue(buildingId, out int[] occupationIds))
            {
                return new List<OccupationInfo>();
            }

            int count = Mathf.Min(level, occupationIds.Length);
            List<OccupationInfo> result = new();
            for (int i = 0; i < count; i++)
            {
                if (container.DataDic.TryGetValue(occupationIds[i], out OccupationInfo info))
                {
                    result.Add(info);
                }
            }

            return result;
        }

        public static OccupationInfo DrawFrom(int buildingId, int level)
        {
            List<OccupationInfo> available = GetOccupations(buildingId, level);
            if (available.Count == 0)
            {
                Debug.LogWarning($"招募池为空 buildingId={buildingId}, level={level}");
                return null;
            }

            return available[Random.Range(0, available.Count)];
        }
    }
}
