using System;
using System.Collections.Generic;
using UnityEngine;

namespace fs
{
    /// <summary>
    /// 日志写入屏幕
    /// @author hannibal
    /// @time 2019-8-26
    /// </summary>
    public class Log2ScreenScript : MonoBehaviour
    {
        private bool m_is_show = false;
        private GUIStyle m_gui_style = new GUIStyle();
        private Queue<string> m_list_msg = new Queue<string>();

        void OnEnable()
        {
            m_gui_style.fontSize = 32;
            m_gui_style.normal.textColor = new Color(1, 0f, 0.5f, 1.0f);
            Debuger.Log2ScreenFun = OnReceivedLog;
        }
        void OnDisable()
        {
            Debuger.Log2ScreenFun = null;
        }

        void OnReceivedLog(string msg)
        {
            m_is_show = true;

            if (m_list_msg.Count >= 15)
            {
                m_list_msg.Dequeue();
            }


            m_list_msg.Enqueue(msg);
        }
        
        void OnGUI()
        {
            if (m_is_show)
            {
                string msg = string.Join("\n", m_list_msg.ToArray());
                GUILayout.Label(msg, m_gui_style);
            }
        }
    }
}
