using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(PropInfo))]
    public class PropInfoContainer
    {
        public Dictionary<int, PropInfo> DataDic = new();
    }
}