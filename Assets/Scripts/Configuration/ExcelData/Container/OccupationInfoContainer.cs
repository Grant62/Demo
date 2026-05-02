using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(OccupationInfo))]
    public class OccupationInfoContainer
    {
        public Dictionary<int, OccupationInfo> DataDic = new();
    }
}