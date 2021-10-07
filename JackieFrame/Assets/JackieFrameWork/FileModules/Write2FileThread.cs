using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using JsonFx.Json;
using System.Runtime.InteropServices;

namespace fs
{
    struct WriteItem
    {
        public string path;
        public object value;
        public bool append;//是否追加文件
        public bool register;//是否写入注册表
        public WriteItem(string _path, object _value, bool _append, bool _reg)
        {
            path = _path; value = _value;append = _append; register = _reg;
        }
    }
    /// <summary>
    /// 多线程写到文件
    /// 1.写入的目标文件是文本文件
    /// 2.TODO:需要支持断电保护
    /// @author hannibal
    /// @time 2019-7-2
    /// </summary>
    public class Write2FileThread : DnotMonoSingleton<Write2FileThread>
    {
#if UNITY_ANDROID
        [DllImport("NativeWriteFile")]
        public static extern int FX_Native_WriteBytes(string path, string data, int length);
#endif
        private bool m_Disposed = false;
        private Thread m_TaskThread;
        private System.Object m_ThreadListLock = new System.Object();
        private List<WriteItem> m_AppendList = new List<WriteItem>();
        private List<WriteItem> m_RunList = new List<WriteItem>();

        protected override void Awake()
        {
            base.Awake();

            m_TaskThread = new Thread(new ThreadStart(Tick));
            m_TaskThread.Priority = System.Threading.ThreadPriority.Lowest;
            m_TaskThread.Start();
        }

        void OnDestroy()
        {
            ProcessWrite();
            m_Disposed = true;
            if (m_TaskThread != null)
            {
                m_TaskThread.Join();
                m_TaskThread = null;
            }
        }

        public void Push(string path, string data)
        {
            lock (m_ThreadListLock)
            {
                m_AppendList.Add(new WriteItem(path, data, false, false));
            }
        }
        public void Push(string path, string data, bool append)
        {
            lock (m_ThreadListLock)
            {
                m_AppendList.Add(new WriteItem(path, data, append, false));
            }
        }
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="obj"></param>
        /// <param name="register">是否写入注册表</param>
        public void Push(string path, object obj, bool register = false)
        {
            lock (m_ThreadListLock)
            {
                m_AppendList.Add(new WriteItem(path, obj, false, register));
            }
        }

        private void Tick()
        {
            while (m_Disposed == false)
            {
                ProcessWrite();
                Thread.Sleep(33);
            }
        }

        public void ProcessWrite()
        {
            var runList = m_AppendList;
            if (runList.Count > 0)
            {
                lock (m_ThreadListLock)
                {
                    m_AppendList = m_RunList;
                    m_RunList = runList;
                }

                WriteItem t;
                WriteItem[] temp = runList.ToArray();
                for (int i = 0; i < temp.Length; ++i)
                {
                    t = temp[i];

                    //创建目录
                    string path = t.path;
                    string folder = Path.GetDirectoryName(path);
                    if (!Directory.Exists(folder))
                        FileUtils.CreateDirectory(folder);
                    //内容
                    string data = string.Empty;
                    if (t.value is string)
                    {
                        data = t.value as string;
                    }
                    else if(t.value is object)
                    {
                        try
                        {
                            data = JsonWriter.Serialize(t.value);
                        }
                        catch(System.Exception e)
                        {
                            Debuger.LogException(e);
                            data = string.Empty;
                        }
                    }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                    Write2Windows(path, data, t.append, t.register);
#elif UNITY_ANDROID
                    Write2Android(path, data, t.append);
#else
                    Debuger.LogError("未支持的平台，写入错误:" + path);
#endif
                }
                runList.Clear();
            }
        }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        /// <summary>
        /// 写入到windows
        /// </summary>
        private void Write2Windows(string path, string data, bool append, bool register)
        {
            //Debuger.Log(string.Format("请求写文件path:{0}, register:{1}, data:{2}", path, register, data));
            if (register)
            {
                string file_name = Path.GetFileNameWithoutExtension(path);
                MyPlayerPrefs.SetString(file_name, data);
            }
            else
            {
                FileUtils.WriteFileText(path, data, append);
            }
        }
#endif

#if UNITY_ANDROID
        /// <summary>
        /// 写入安卓
        /// </summary>
        private void Write2Android(string path, string data, bool append)
        {
            try
            {
                int result = FX_Native_WriteBytes(path, data, data.Length);
                if (result < 0)
                {
                    Debuger.LogError("写入文件错误:" + result);
                }
            }
            catch (Exception e)
            {
                Debuger.LogException(e);
            }
        }
#endif
    }
}