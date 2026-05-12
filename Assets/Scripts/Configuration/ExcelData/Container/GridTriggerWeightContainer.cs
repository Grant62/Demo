using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(GridTriggerWeight))]
    public class GridTriggerWeightContainer
    {
        public Dictionary<int, GridTriggerWeight> DataDic = new();
    }
}