using System;
using System.Collections.Generic;

namespace fs
{
    /// <summary>
    /// 逻辑锁，保证多线程执行的结果，返回给逻辑层后，同一时刻只有一个线程执行
    /// 1.网络消息
    /// 2.数据库查询结果
    /// 3.控制台cmd命令
    /// 4.其他涉及到多线程与主线程交互的
    /// @author hannibal
    /// @time 2016-7-28
    /// </summary>
    public class ThreadScheduler : Singleton<ThreadScheduler>
    {
        private Object m_LogicLock = new Object();
        private List<System.Action> m_AppendList = new List<Action>();
        private List<System.Action> m_RunList = new List<Action>();

        public Object LogicLock
        {
            get { return m_LogicLock; }
        }

        public void PushAction(System.Action action)
        {
            lock(m_LogicLock)
            {
                m_AppendList.Add(action);
            }
        }

        public void ProcessAction()
        {
            var runList = m_AppendList;
            if (runList.Count > 0)
            {
                lock (m_LogicLock)
                {
                    m_AppendList = m_RunList;
                    m_RunList = runList;
                }

                foreach (var action in runList)
                {
                    if (action != null) action();
                }
                runList.Clear();
            }
        }
    }
}
