using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

namespace fs
{
    public enum eLog2Screen
    {
        None = 0x00,
        GUI = 0x01,
        All = 0x03,
    }

    /// <summary>
    /// log写入文件
    /// @author hannibal
    /// @time 2016-11-8
    /// </summary>
    public class Log2FileScript : MonoBehaviour
    {
        public bool m_EnableLog = true;
        public bool m_EnableWarning = true;
        public bool m_EnableError = true;
        public bool m_EnableExceptioin = true;
        public bool m_MouseActiveGUI = true;
        public eLog2Screen m_Log2Screen = eLog2Screen.None;
        public string m_Log2Path = "";
        private Log2FileThread m_Log2File = null;
        private eLog2Screen m_OldLog2Screen;


        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void StartOnUnity()
        {
            Log2FileScript instance = FindObjectOfType<Log2FileScript>();
            if (instance == null)
            {
                string instanceName = typeof(Log2FileScript).Name;
                GameObject instanceGO = GameObject.Find(instanceName);
                if (instanceGO == null)
                    instanceGO = new GameObject(instanceName);
                instance = instanceGO.AddComponent<Log2FileScript>();
                DontDestroyOnLoad(instanceGO);
            }
        }

        void Awake()
        {
            StartOnUnity();

            m_Log2File = new Log2FileThread();
            m_Log2File.Setup(GetLog2Path());
            Application.logMessageReceivedThreaded += OnReceivedLog;
            Debuger.MsgFun += OnReceivedLog;

            if(m_Log2Screen != eLog2Screen.None)
                gameObject.AddComponent<Log2ScreenScript>();
            m_OldLog2Screen = m_Log2Screen;

            this.RemoveOldFile();
        }

        void OnDestroy()
        {
            if (m_Log2File != null)
            {
                m_Log2File.Process();
                m_Log2File.Dispose();
                m_Log2File = null;
            }
            Application.logMessageReceivedThreaded -= OnReceivedLog;
            Debuger.MsgFun -= OnReceivedLog;
        }

        private bool tmp_draging = false;
        private Vector3 tmp_start_pos;
        void Update()
        {
            //运行中改变了日志输出方式
            if(m_OldLog2Screen != m_Log2Screen)
            {
                Log2ScreenScript script = gameObject.GetComponent<Log2ScreenScript>();
                if (m_Log2Screen != eLog2Screen.None)
                {
                    if(script == null) gameObject.AddComponent<Log2ScreenScript>();
                }
                else
                {
                    if (script != null) GameObject.Destroy(script);
                }  

                m_OldLog2Screen = m_Log2Screen;
            }

            //开启日志显示
            if (m_MouseActiveGUI)
            {
                if (!tmp_draging && Input.GetMouseButtonDown(0))
                {
                    tmp_draging = true;
                    tmp_start_pos = Input.mousePosition;
                }
                if (tmp_draging && Input.GetMouseButtonUp(0))
                {
                    tmp_draging = false;
                    if (Vector2.Distance(Input.mousePosition, tmp_start_pos) >= 100)
                    {
                        if (m_Log2Screen == eLog2Screen.None) m_Log2Screen = eLog2Screen.GUI;
                        else m_Log2Screen = eLog2Screen.None;
                    }
                }
            }
        }
        void OnReceivedLog(string logString, string stackTrace, LogType type)
        {
            if (m_Log2File != null)
            {
                switch (type)
                {
                    case LogType.Assert: break;
                    case LogType.Log: if (m_EnableLog) m_Log2File.Push(logString); break;
                    case LogType.Warning: if (m_EnableWarning) m_Log2File.Push(logString); break;
                    case LogType.Error: if (m_EnableError) m_Log2File.Push(logString); break;
                    case LogType.Exception: if (m_EnableExceptioin) m_Log2File.Push(logString); break;
                }
                if(m_Log2Screen == eLog2Screen.All)
                    Debuger.LogGUI(logString);
            }
            if (type == LogType.Error || type == LogType.Exception)
            {
                HandleLog(stackTrace);
                if (m_Log2Screen == eLog2Screen.All)
                    Debuger.LogGUI(stackTrace);
            }
        }
        void OnReceivedLog(string logString)
        {
            m_Log2File.Push(logString);
        }

        void HandleLog(params object[] objs)
        {
            if (m_Log2File != null)
            {
                string text = "";
                for (int i = 0; i < objs.Length; ++i)
                {
                    if (i == 0)
                    {
                        text += objs[i].ToString();
                    }
                    else
                    {
                        text += ", " + objs[i].ToString();
                    }
                }
                m_Log2File.Push(text);
            }
        }
        /// <summary>
        /// 写入路径
        /// </summary>
        string GetLog2Path()
        {
            if (string.IsNullOrEmpty(m_Log2Path)) m_Log2Path = "Log";
            if (Application.platform == RuntimePlatform.WindowsEditor)
                return Application.streamingAssetsPath + "/../../" + m_Log2Path;
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
                return Application.streamingAssetsPath + "/" + m_Log2Path;
            else
                return Application.persistentDataPath + "/" + m_Log2Path;
        }
        
        /// <summary>
        /// 移除过期日志文件：日志文件保留一天
        /// </summary>
        private void RemoveOldFile()
        {
            try
            {
                string file_name = System.DateTime.Now.ToString("yyyyMMdd");

                string dir_path = GetLog2Path();
                DirectoryInfo dir = new DirectoryInfo(dir_path);
                FileInfo[] all_files = dir.GetFiles();
                foreach (FileInfo d in all_files)
                {
                    if (!d.Name.StartsWith(file_name))
                        File.Delete(d.FullName);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}