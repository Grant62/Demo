using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(RoomConfigInfo))]
    public class RoomConfigInfoContainer
    {
        public Dictionary<int, RoomConfigInfo> DataDic = new();
    }
}