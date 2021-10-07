using System;
using System.Collections.Generic;
using System.Text;

namespace fs
{
    /// <summary>
    /// buff对象池
    /// @author hannibal
    /// @time 2016-5-23
    /// </summary>
    public class SendRecvBufferPools
    {
        private static int m_total_new_count = 0;
        private static int m_total_remove_count = 0;
        private List<SendRecvBuffer> m_pools = new List<SendRecvBuffer>();
        private object m_lock_obj = new object();

        public SendRecvBuffer Spawn()
        {
            SendRecvBuffer obj = null;
            lock (m_lock_obj)
            {
                if (m_pools.Count == 0)
                {
                    System.Threading.Interlocked.Increment(ref m_total_new_count);
                    obj = new SendRecvBuffer();
                }
                else
                {
                    obj = m_pools[m_pools.Count - 1];
                    m_pools.RemoveAt(m_pools.Count - 1);
                }
            }
            return obj;
        }
        public void Despawn(SendRecvBuffer obj)
        {
            if (obj == null) return;
            lock (m_lock_obj)
            {
                m_pools.Add(obj);
            }
            System.Threading.Interlocked.Increment(ref m_total_remove_count);
        }
        public static string ToString(bool is_print)
        {
            StringBuilder st = new StringBuilder();
            st.AppendLine("SendRecvBufferPools使用情况:");
            st.AppendLine("New次数:" + m_total_new_count);
            st.AppendLine("空闲数量:" + m_total_remove_count);
            if (is_print) Debuger.Log(st.ToString());
            return st.ToString();
        }
    }
}