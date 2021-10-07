using System;
using System.Collections.Generic;
using UnityEngine;

namespace fs
{
    /// <summary>
    /// 平台类型
    /// </summary>
    public class PlatformUtils
    {
        private static uint m_PlatType = 0; //当前平台类型
        public static uint PlatType
        {
            get { return m_PlatType; }
        }
        
        static PlatformUtils()
        {
            //平台
#if UNITY_EDITOR
            UnionPlatType(ePlatformType.Editor);
#endif
#if UNITY_STANDALONE_WIN
            UnionPlatType(ePlatformType.Windows);
            Debuger.Log("当前平台:Windows");
#endif
#if UNITY_STANDALONE_OSX
            UnionPlatType(ePlatformType.OSX);
            Debuger.Log("当前平台:OSX");
#endif
#if UNITY_ANDROID
            UnionPlatType(ePlatformType.Android);
            Debuger.Log("当前平台:Android");
#endif
#if UNITY_IOS
            UnionPlatType(ePlatformType.iOS);
            Debuger.Log("当前平台:iOS");
#endif
        }
        
        public static void UnionPlatType(ePlatformType type)
        {
            IntUtils.InsertFlag(ref m_PlatType, (uint)type);
        }
        public static bool IsPlatType(ePlatformType type)
        {
            return IntUtils.HasFlag(m_PlatType, (uint)type);
        }

        public string GetPlatformName(ePlatformType type) 
        {
            switch(type)
            {
                case ePlatformType.Editor: return "editor";
                case ePlatformType.Windows: return "windows";
                case ePlatformType.OSX: return "osx";
                case ePlatformType.Android: return "android";
                case ePlatformType.iOS: return "ios";
                case ePlatformType.WebGL: return "webgl";
                default: return "";
            }
        }
    }

    /// <summary>
    /// 平台类型
    /// </summary>
    public enum ePlatformType
    {
        Editor = 1,
        Windows = 2,
        OSX = 4,
        Android = 8,
        iOS = 16,
        WebGL = 32,
    }
}

/**
UNITY_EDITOR	    编辑器调用
UNITY_EDITOR_OSX	专门为Mac OS(包括Universal，PPC和Intelarchitectures)平台的定义
UNITY_EDITOR_WIN	Windows操作系统
UNITY_STANDALONE_LINUX	Linux的独立应用程序
UNITY_STANDALONE_OSX	用于专门为Mac OS X
UNITY_STANDALONE_WIN	用于专门为Windows独立应用程序
UNITY_STANDALONE	独立的平台(Mac,Windows或Linux)
UNITY_WII	        Wii游戏平台
UNITY_IOS	        iPhone平台
UNITY_ANDROID	    Android平台
UNITY_PS4	        用于运行PlayStation 4
UNITY_XBOXONE	    Xbox One  
UNITY_WEBGL	        WebGL
UNITY_FACEBOOK	    Facebook平台
UNITY_EDITOR_WIN	Windows上编辑器
UNITY_EDITOR_OSX	Mac OS X上编辑器
*/
