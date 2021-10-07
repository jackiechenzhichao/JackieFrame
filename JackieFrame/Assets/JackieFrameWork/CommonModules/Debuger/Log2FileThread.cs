using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace fs
{
    /// <summary>
    /// 多线程写日志到文件
    /// @author hannibal
    /// @time 2017-4-7
    /// </summary>
    public class Log2FileThread
    {
        private bool m_Enable = false;
        private string m_OutPath = "";

        private bool m_Disposed = false;
        private Thread m_TaskThread;
        private System.Object m_ThreadListLock = new System.Object();
        private List<string> m_AppendList = new List<string>();
        private List<string> m_RunList = new List<string>();

        public void Setup(string root_path)
        {
            m_Enable = true;
            if (!Directory.Exists(root_path))
            {
                try
                {
                    Directory.CreateDirectory(root_path);
                }
                catch (System.Exception)
                {
                    m_Enable = false;
                }
            }
            if (m_Enable)
            {
                string file_name = System.DateTime.Now.ToString("yyyyMMdd_HH_mm_ss");
                m_OutPath = root_path + "/" + file_name + ".txt";
                try
                {
                    using (StreamWriter writer = new StreamWriter(m_OutPath, false, Encoding.UTF8))
                    {
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning("无写入权限:" + e.ToString());
                    m_Enable = false;
                }
            }

            if (m_Enable)
            {
                Debug.Log("日志文件写入路径:" + m_OutPath);
                m_TaskThread = new Thread(new ThreadStart(Tick));
                m_TaskThread.Priority = System.Threading.ThreadPriority.Lowest;
                m_TaskThread.Start();
            }
        }

        public void Dispose()
        {
            m_Disposed = true;
            if (m_TaskThread != null)
            {
                try
                {
                    m_TaskThread.Join();
                }
                catch
                {
                }
                m_TaskThread = null;
            }
        }

        private void Tick()
        {
            while (m_Disposed == false && m_Enable)
            {
                Process();
                Thread.Sleep(64);
            }
        }

        public void Push(string msg)
        {
            lock (m_ThreadListLock)
            {
                m_AppendList.Add(msg);
            }
        }

        public void Process()
        {
            var runList = m_AppendList;
            if (runList.Count > 0)
            {
                lock (m_ThreadListLock)
                {
                    m_AppendList = m_RunList;
                    m_RunList = runList;
                }

                string[] temp = runList.ToArray();
                using (StreamWriter writer = new StreamWriter(m_OutPath, true, Encoding.UTF8))
                {
                    string t;
                    for (int i = 0; i < temp.Length; ++i)
                    {
                        t = temp[i];
                        writer.WriteLine(t);
                        if (m_Disposed) break;
                    }
                }
                runList.Clear();
            }
        }
    }
}