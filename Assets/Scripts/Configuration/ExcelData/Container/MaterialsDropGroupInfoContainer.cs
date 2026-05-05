using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(MaterialsDropGroupInfo))]
    public class MaterialsDropGroupInfoContainer
    {
        public Dictionary<int, MaterialsDropGroupInfo> DataDic = new();
    }
}