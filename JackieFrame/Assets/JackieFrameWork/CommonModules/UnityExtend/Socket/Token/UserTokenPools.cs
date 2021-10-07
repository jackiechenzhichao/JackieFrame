using System;
using System.Collections.Generic;
using System.Text;

namespace fs
{
    /// <summary>
    /// token对象池
    /// @author hannibal
    /// @time 2016-5-23
    /// </summary>
    public class UserTokenPools
    {
        private static int m_total_new_count = 0;
        private static int m_total_remove_count = 0;
        private List<UserToken> m_pools = new List<UserToken>();
        private object m_lock_obj = new object();

        public UserToken Spawn()
        {
            UserToken obj = null;
            lock (m_lock_obj)
            {
                if (m_pools.Count == 0)
                {
                    System.Threading.Interlocked.Increment(ref m_total_new_count);
                    obj = new UserToken();
                }
                else
                {
                    obj = m_pools[m_pools.Count - 1];
                    m_pools.RemoveAt(m_pools.Count - 1);
                }
            }
            return obj;
        }
        public void Despawn(UserToken obj)
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
            st.AppendLine("UserTokenPools使用情况:");
            st.AppendLine("New次数:" + m_total_new_count + " 空闲数量:" + m_total_remove_count);
            if (is_print) Debuger.Log(st.ToString());
            return st.ToString();
        }
    }
}