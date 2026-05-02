using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(SpellInfo))]
    public class SpellInfoContainer
    {
        public Dictionary<int, SpellInfo> DataDic = new();
    }
}