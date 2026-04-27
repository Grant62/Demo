using System;

namespace Services.ExcelTool
{
    /// <summary>
    ///     标记二进制数据表容器类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BinaryTableAttribute : Attribute
    {
        /// <summary>
        ///     数据类类型
        /// </summary>
        public Type DataType { get; set; }
    }
}