#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;

namespace JKFrame
{
    public static class LogEditorJump
    {
        [OnOpenAsset(0)]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            string stackTrace = GetStackTrace();

            //非JKLog日志则跳出
            if (string.IsNullOrEmpty(stackTrace) || !stackTrace.Contains("JK.Log.JKLog")) return false;
            //正则表达式匹配
            string pattern = @"\(at Assets(/.+\.cs):(\d+)\)";
            Match match = Regex.Match(stackTrace, pattern);
            while (match.Success)
            {
                string path = match.Groups[1].Value;
                string l = match.Groups[2].Value;
                if (!path.Contains("Log.cs"))
                {
                    string fullPath = Application.dataPath + path;
                    if (int.TryParse(l, out int row))
                    {
                        InternalEditorUtility.OpenFileAtLineExternal(fullPath, row);
                        return true;
                    }
                }

                match = match.NextMatch();
            }

            return false;
        }

        /// <summary>
        ///     获取当前日志窗口选中的日志的堆栈信息
        /// </summary>
        /// <returns>堆栈文本</returns>
        private static string GetStackTrace()
        {
            Type consoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            FieldInfo consoleWindowFieldInfo = consoleWindowType.GetField("ms_ConsoleWindow",
                BindingFlags.Static | BindingFlags.NonPublic);

            if (consoleWindowFieldInfo != null)
            {
                EditorWindow consoleWindow = consoleWindowFieldInfo.GetValue(null) as EditorWindow;

                if (consoleWindow != EditorWindow.focusedWindow) return null;

                FieldInfo activeTextFieldInfo = consoleWindowType.GetField(
                    "m_ActiveText",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (activeTextFieldInfo != null) return activeTextFieldInfo.GetValue(consoleWindow).ToString();
            }

            return null;
        }
    }
}
#endif