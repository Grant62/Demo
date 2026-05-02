using Main.Resource;
using UnityEditor;
using UnityEngine;

namespace Editor.GameTool
{
    public class SaveTool
    {
        [MenuItem("游戏工具/删除存档")]
        private static void DeleteSave()
        {
            if (!EditorUtility.DisplayDialog("删除存档", "确定要删除所有存档数据吗？", "确定", "取消"))
            {
                return;
            }

            GameSaveData.Delete();
            Debug.Log("存档已删除");
        }
    }
}
