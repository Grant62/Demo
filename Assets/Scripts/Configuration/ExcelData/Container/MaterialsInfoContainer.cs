using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(MaterialsInfo))]
    public class MaterialsInfoContainer
    {
        public Dictionary<int, MaterialsInfo> DataDic = new();
    }
}