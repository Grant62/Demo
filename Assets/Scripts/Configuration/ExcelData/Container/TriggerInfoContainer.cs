using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(TriggerInfo))]
    public class TriggerInfoContainer
    {
        public Dictionary<int, TriggerInfo> DataDic = new();
    }
}