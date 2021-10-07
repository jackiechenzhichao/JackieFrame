using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace fs
{
    /// <summary>
    /// 编辑器工具类
    /// @author hannibal
    /// @time 2018-11-1
    /// </summary>
    public class EditorUtils
    {
        /// <summary>
        /// 清除日志
        /// </summary>
        public static void ClearLog()
        {
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(ActiveEditorTracker));
            var type = assembly.GetType("UnityEditorInternal.LogEntries");
            if (type == null)
            {
                type = assembly.GetType("UnityEditor.LogEntries");
            }
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
#if UNITY_EDITOR
        [DllImport("user32")]
        static extern int GetSystemMetrics(int nIndex);
#endif
        public static Rect GetWindowRect(int x, int y, int width, int height)
        {
#if UNITY_EDITOR
            int w = GetSystemMetrics(0);
            int h = GetSystemMetrics(1);
            x = (w - width) >> 1;
            y = (h - height) >> 1;
#endif
            return new Rect(x, y, width, height);
        }
    }
}