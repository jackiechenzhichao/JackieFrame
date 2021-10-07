using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

/// <summary>
/// 等级
/// </summary>
public enum eLogLevel
{
    LV_DEBUG = 0,
    LV_INFO,
    LV_WARNING,
    LV_ERROR,
    LV_EXCEPTION,
    LV_MAX,
}
/// <summary>
/// 输出目标
/// </summary>
public enum eLogOutputType
{
    Console = 1,
    File = 2,
    Screen = 4,
    All = 7,
}

/// 日志输出
/// @author hannibal
/// @time 2019-7-3
/// </summary>
public sealed class Debuger
{
    public static eLogLevel LogLv = eLogLevel.LV_DEBUG;
    public static int FrameCount { set; private get; }

    public static uint LogOutput = (uint)eLogOutputType.All;
    /// <summary>
    /// 输出到屏幕
    /// </summary>
    public static System.Action<string> Log2ScreenFun = null;
    /// <summary>
    /// 输出到文件
    /// </summary>
    /// <param name="msg"></param>
    public delegate void RegistFunction(string msg);
    public static event RegistFunction MsgFun = null;

    /// <summary>
    /// 断言
    /// </summary>
    /// <param name="condition">条件</param>
    /// <param name="info">输出信息</param>
    public static void Assert(bool condition, string info = "fatal error")
    {
        if (condition)
            return;
        LogError(info);
    }
    /// <summary>
    /// 普通日志
    /// </summary>
    public static void Log(string msg)
    {
        if (LogLv > eLogLevel.LV_INFO) return;

        string log = string.Format("[info]{0} frame:{1}", msg, FrameCount);
        if ((LogOutput & (uint)eLogOutputType.Console) != 0)
            UnityEngine.Debug.Log(log);
        if ((LogOutput & (uint)eLogOutputType.File) != 0)
            if (MsgFun != null) MsgFun(log);
    }
    /// <summary>
    /// 普通日志-橘色
    /// </summary>
    public static void LogOrange(string msg)
    {
        if (LogLv > eLogLevel.LV_INFO) return;

        string log = string.Format("<color=orange>[info]</color>{0} frame:{1}", msg, FrameCount);
        if ((LogOutput & (uint)eLogOutputType.Console) != 0)
            UnityEngine.Debug.Log(log);
        if ((LogOutput & (uint)eLogOutputType.File) != 0)
            if (MsgFun != null) MsgFun(log);
    }
    /// <summary>
    /// 普通日志-颜色
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="color">颜色</param>
    public static void LogColor(string msg, string color)
    {
        if (LogLv > eLogLevel.LV_INFO) return;

        string log = string.Format("<color={0}>[info]</color>{1} frame:{2}", color, msg, FrameCount);
        if ((LogOutput & (uint)eLogOutputType.Console) != 0)
            UnityEngine.Debug.Log(log);
        if ((LogOutput & (uint)eLogOutputType.File) != 0)
            if (MsgFun != null) MsgFun(log);
    }
    /// <summary>
    /// 警告
    /// </summary>
    public static void LogWarning(string msg)
    {
        if (LogLv > eLogLevel.LV_WARNING) return;

        string log = string.Format("<color=yellow>[warning]</color>{0} frame:{1}", msg, FrameCount);
        if ((LogOutput & (uint)eLogOutputType.Console) != 0)
            UnityEngine.Debug.LogWarning(log);
        if ((LogOutput & (uint)eLogOutputType.File) != 0)
            if (MsgFun != null) MsgFun(log);
    }
    /// <summary>
    /// 错误
    /// </summary>
    public static void LogError(string msg)
    {
        if (LogLv > eLogLevel.LV_ERROR) return;

        string log = string.Format("<color=red>[error]</color>{0} frame:{1}", msg.ToString(), FrameCount);
        if ((LogOutput & (uint)eLogOutputType.Console) != 0)
            UnityEngine.Debug.LogError(log);
        if ((LogOutput & (uint)eLogOutputType.File) != 0)
            if (MsgFun != null) MsgFun(log);
    }
    /// <summary>
    /// 抛出异常
    /// </summary>
    public static void LogException(string msg)
    {
        if (LogLv > eLogLevel.LV_EXCEPTION) return;

        LogError(msg);
    }
    /// <summary>
    /// 抛出异常
    /// </summary>
    public static void LogException(Exception e)
    {
        if (LogLv > eLogLevel.LV_EXCEPTION) return;

        LogError(e.ToString());
    }

    /// <summary>
    /// 输出到屏幕
    /// </summary>
    public static void GUI(string msg)
    {
        Log(msg);
        if (Log2ScreenFun != null) Log2ScreenFun(msg);
    }
    /// <summary>
    /// 输出到屏幕
    /// </summary>
    internal static void LogGUI(string msg)
    {
        if (Log2ScreenFun != null) Log2ScreenFun(msg);
    }
}
