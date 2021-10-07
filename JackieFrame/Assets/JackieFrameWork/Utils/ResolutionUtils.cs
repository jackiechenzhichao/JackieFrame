using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;

namespace fs
{
    /// <summary>
    /// 分辨率工具类
    /// @author hannibal
    /// @time 2018-1-1
    /// </summary>
    public class ResolutionUtils
    {
        public static int DeviceWidth = 1;
        public static int DeviceHeight = 1;

#if UNITY_STANDALONE_WIN
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
#endif
        /// <summary>
        /// 设置分辨率
        /// </summary>
        /// <param name="default_screen_w">设置分辨率宽</param>
        /// <param name="default_screen_h">设置分辨率高</param>
        /// <param name="max_screen_h">最大分辨率</param>
        public static void SetResolution(int default_screen_w, int default_screen_h, int max_screen_w, int max_screen_h)
        {
            DeviceWidth = Screen.width;
            DeviceHeight = Screen.height;

#if UNITY_EDITOR
            UIID.DEFAULT_WIDTH = default_screen_w;
            UIID.DEFAULT_HEIGHT = default_screen_h;
#elif UNITY_STANDALONE_WIN
            Debuger.Log("默认设备分辨率:" + Screen.width + "*" + Screen.height);
            UIID.DEFAULT_WIDTH = default_screen_w;
            UIID.DEFAULT_HEIGHT = default_screen_h;
            SetWindowLong(GetForegroundWindow(), 0, 0);
            SetWindowPos(GetForegroundWindow(), 300, 100, 0, 507, 900, 0);
            Screen.SetResolution(507, 900, false);
            Debuger.Log("最终分辨率:" + Screen.width + "*" + Screen.height);
#else
            SetResolutionImpl(default_screen_w, default_screen_h, max_screen_w, max_screen_h);
#endif
        }
        /// <summary>
        /// 设置分辨率
        /// </summary>
        private static void SetResolutionImpl(int default_screen_w, int default_screen_h, int max_screen_w, int max_screen_h)
        {
            UIID.DEFAULT_WIDTH = default_screen_w;
            UIID.DEFAULT_HEIGHT = default_screen_h;

            float screen_w = Screen.width;
            float screen_h = Screen.height;
            Debuger.Log("默认设备分辨率:" + UIID.DEFAULT_WIDTH + "*" + UIID.DEFAULT_HEIGHT);
            Debuger.Log("当前设备分辨率:" + Screen.width + "*" + Screen.height);

            float w = screen_w;
            float h = screen_h;
            if (w > max_screen_w || h > max_screen_h)
            {
                if (w > max_screen_w)
                {//横屏
                    w = max_screen_w;
                    h = w * screen_h / screen_w;
                }
                else
                {//竖屏
                    h = max_screen_h;
                    w = screen_w * h / screen_h;
                }
            }
            Debuger.Log("请求设置分辨率:" + (int)w + "*" + (int)h);
            Screen.SetResolution((int)w, (int)h, false);
        }
    }
}