using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(SupplySetInfo))]
    public class SupplySetInfoContainer
    {
        public Dictionary<int, SupplySetInfo> DataDic = new();
    }
}