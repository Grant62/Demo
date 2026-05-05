using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(MaterialsDropGroupConfigInfo))]
    public class MaterialsDropGroupConfigInfoContainer
    {
        public Dictionary<int, MaterialsDropGroupConfigInfo> DataDic = new();
    }
}