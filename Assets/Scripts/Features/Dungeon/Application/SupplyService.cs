using System.Text;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;
using UnityEngine;

namespace Features.Dungeon.Application
{
    public static class SupplyService
    {
        public static void DrawRewards(int supplyId)
        {
            SupplyInfoContainer container = BinaryDataMgr.Ins.GetTable<SupplyInfoContainer>();
            if (container == null)
            {
                Debug.LogError($"SupplyInfoContainer 未加载");
                return;
            }

            if (!container.DataDic.TryGetValue(supplyId, out SupplyInfo supply))
            {
                Debug.LogError($"未找到 SupplyInfo，ID: {supplyId}");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[物资抽取] {supply.Name} (ID: {supplyId})");

            TryAppendItem(sb, supply.Item1, supply.Item1Num);
            TryAppendItem(sb, supply.Item2, supply.Item2Num);
            TryAppendItem(sb, supply.Item3, supply.Item3Num);
            TryAppendItem(sb, supply.Item4, supply.Item4Num);
            TryAppendItem(sb, supply.Item5, supply.Item5Num);
            TryAppendItem(sb, supply.Item6, supply.Item6Num);

            Debug.Log(sb.ToString().TrimEnd());
        }

        private static void TryAppendItem(StringBuilder sb, int itemId, string itemNumStr)
        {
            if (itemId <= 0 || string.IsNullOrEmpty(itemNumStr))
                return;

            int quantity = ParseQuantity(itemNumStr);
            sb.AppendLine($"  → 物品 {itemId} x {quantity}");
        }

        private static int ParseQuantity(string numStr)
        {
            if (int.TryParse(numStr, out int fixedNum))
                return fixedNum;

            char[] separators = new[] { '-', ',' };
            string[] parts = numStr.Split(separators);
            if (parts.Length == 2
                && int.TryParse(parts[0], out int min)
                && int.TryParse(parts[1], out int max))
            {
                return Random.Range(min, max + 1);
            }

            Debug.LogWarning($"无法解析数量: {numStr}，使用默认值 1");
            return 1;
        }
    }
}
