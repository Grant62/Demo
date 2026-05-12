using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(SupplyInfo))]
    public class SupplyInfoContainer
    {
        public Dictionary<int, SupplyInfo> DataDic = new();
    }
}