using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(TriggerEventRandomConfig))]
    public class TriggerEventRandomConfigContainer
    {
        public Dictionary<int, TriggerEventRandomConfig> DataDic = new();
    }
}