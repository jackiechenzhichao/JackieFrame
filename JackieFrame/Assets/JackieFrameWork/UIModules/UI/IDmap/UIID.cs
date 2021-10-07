using UnityEngine;
using System.Collections;

namespace fs
{
    /// <summary>
    /// ui
    /// @author hannibal
    /// @time 2016-9-21
    /// </summary>
    public class UIID
    {
        public static int DEFAULT_WIDTH = 750;  //标准界面大小
        public static int DEFAULT_HEIGHT = 1334;
        public static int MAX_WIDTH = 1080;  //最大界面大小
        public static int MAX_HEIGHT = 1920;

        public static float ScreenScaleX = 1;    //界面缩放
        public static float ScreenScaleY = 1;
        public static float InvScreenScaleX = 1;
        public static float InvScreenScaleY = 1;

        /*不同节点下的sortLayer层数*/
        public const int OrderLyaerInterval = 1000;

    }

    /// <summary>
    /// ui事件
    /// </summary>
    public class UIEventID
    {
        public const string SHOW_GUI            = "SHOW_GUI";           //显示界面
        public const string CLOSE_GUI           = "CLOSE_GUI";          //关闭界面通知事件
        public const string REQUEST_CLOSE_GUI   = "REQUEST_CLOSE_GUI";  //请求关闭界面
    }

    /// <summary>
    /// 界面layer
    /// </summary>
    public enum eUILayer
    {
        TOOLS,  //主城主界面、战斗主界面等需要一直处于最下层的界面
        APP,    //普通界面
        Upper,  //比APP高一层的界面
        TOP,    //需要在上层表现，如弹出式确认对话框等
        MASK,   //遮罩
        MAX,
    }

    /// <summary>
    /// 界面id
    /// </summary>
    public enum eInternalUIID
    {
        ID_ALERT = 1,           // 弹出框

        ID_MAX,
    }
}