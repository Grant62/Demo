using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(MonsterConfigInfo))]
    public class MonsterConfigInfoContainer
    {
        public Dictionary<int, MonsterConfigInfo> DataDic = new();
    }
}