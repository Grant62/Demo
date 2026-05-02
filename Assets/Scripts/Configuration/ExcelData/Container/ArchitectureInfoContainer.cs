using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(ArchitectureInfo))]
    public class ArchitectureInfoContainer
    {
        public Dictionary<int, ArchitectureInfo> DataDic = new();
    }
}