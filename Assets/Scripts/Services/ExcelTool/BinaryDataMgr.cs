using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace Services.ExcelTool
{
    public class BinaryDataMgr
    {
        public static readonly string DataBinaryPath = Application.streamingAssetsPath + "/Binary/";
        private readonly Dictionary<string, object> _tableDic = new();
        private static readonly string SavePath = Application.persistentDataPath + "/Data/";
        public static BinaryDataMgr Ins { get; } = new();

        static BinaryDataMgr()
        {
            Ins.InitData();
        }

        private bool _isInit;
        private BinaryDataMgr() { }

        public void InitData()
        {
            if (_isInit)
                return;
            _isInit = true;

            // 自动发现并加载所有带 BinaryTable 特性的容器类
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in types)
            {
                BinaryTableAttribute attr = type.GetCustomAttribute<BinaryTableAttribute>();
                if (attr != null)
                {
                    try
                    {
                        LoadTable(type, attr.DataType);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"加载表格 {type.Name} 失败: {ex.Message}");
                    }
                }
            }
        }

        public void LoadTable<T, K>()
        {
            LoadTable(typeof(T), typeof(K));
        }

        public void LoadTable(Type containerType, Type dataType)
        {
            string filePath = DataBinaryPath + dataType.Name + ".txt";
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"表格文件不存在: {filePath}");
                return;
            }

            using FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            int index = 0;
            int count = BitConverter.ToInt32(bytes, index);
            index += 4;
            int keyNameLength = BitConverter.ToInt32(bytes, index);
            index += 4;
            string keyName = Encoding.UTF8.GetString(bytes, index, keyNameLength);
            index += keyNameLength;
            object containerObj = Activator.CreateInstance(containerType);
            FieldInfo[] infos = dataType.GetFields();
            for (int i = 0; i < count; i++)
            {
                object dataObj = Activator.CreateInstance(dataType);
                foreach (FieldInfo info in infos)
                    if (info.FieldType == typeof(int))
                    {
                        info.SetValue(dataObj, BitConverter.ToInt32(bytes, index));
                        index += 4;
                    }
                    else if (info.FieldType == typeof(float))
                    {
                        info.SetValue(dataObj, BitConverter.ToSingle(bytes, index));
                        index += 4;
                    }
                    else if (info.FieldType == typeof(bool))
                    {
                        info.SetValue(dataObj, BitConverter.ToBoolean(bytes, index));
                        index += 1;
                    }
                    else if (info.FieldType == typeof(string))
                    {
                        int length = BitConverter.ToInt32(bytes, index);
                        index += 4;
                        info.SetValue(dataObj, Encoding.UTF8.GetString(bytes, index, length));
                        index += length;
                    }

                object dicObject = containerType.GetField("DataDic").GetValue(containerObj);
                MethodInfo mInfo = dicObject.GetType().GetMethod("Add");
                object keyValue = infos[0].GetValue(dataObj);
                mInfo.Invoke(dicObject, new[] { keyValue, dataObj });
            }

            _tableDic.Add(containerType.Name, containerObj);
        }

        public T GetTable<T>() where T : class
        {
            string tableName = typeof(T).Name;
            if (_tableDic.TryGetValue(tableName, out object value))
                return value as T;
            return null;
        }

        public void Save(object obj, string fileName)
        {
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);

            using FileStream fs = new(SavePath + fileName + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryFormatter bf = new();
            bf.Serialize(fs, obj);
            fs.Close();
            Debug.Log($"游戏数据{obj}保存成功！位置在{SavePath + fileName + ".txt"}");
        }

        public T Load<T>(string fileName) where T : class
        {
            if (!File.Exists(SavePath + fileName + ".txt"))
                return null;

            using FileStream fs = File.Open(SavePath + fileName + ".txt", FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new();
            T obj = bf.Deserialize(fs) as T;
            fs.Close();
            return obj;
        }
    }
}