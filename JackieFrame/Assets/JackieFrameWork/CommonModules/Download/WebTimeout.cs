using System;
using System.Collections.Generic;
using System.Configuration;
using System.Timers;

namespace fs
{
    /// <summary>
    /// 下载超时判断
    /// </summary>
    public class WebTimeout
    {
        private uint m_timer_id = 0;
        /// <summary>
        /// 是否超时
        /// </summary>
        private bool m_is_timeout = false;
        /// <summary>
        /// 超时次数
        /// </summary>
        private int m_timeout_count = 0;
        /// <summary>
        /// 容许的最大超时次数
        /// </summary>
        private const int TIMEOUT_COUNT = 3;
        private object m_lock_obj = new object();
        /// <summary>
        /// 超时回调
        /// </summary>
        private System.Action m_timeout_callback = null;

        /// <summary>
        /// 开始计时
        /// </summary>
        /// <param name="timeout">超时时间(s)</param>
        /// <param name="callback">超时回调</param>
        public void Start(int timeout, System.Action callback)
        {
            this.Stop();

            m_is_timeout = false;
            m_timeout_count = 0;
            m_timeout_callback = callback;
            
            m_timer_id = Timer.instance.AddLoop(timeout, OnTimer);
        }
        public void Stop()
        {
            if(m_timer_id != 0)
            {
                Timer.instance.RemoveTimer(m_timer_id);
                m_timer_id = 0;
            }
        }
        /// <summary>
        /// 收到数据时触发，标记网络正常
        /// </summary>
        public void Reset()
        {
            lock(m_lock_obj)
            {
                m_is_timeout = false;
                m_timeout_count = 0;
            }
        }
        private void OnTimer()
        {
            if(m_is_timeout)
            {
                ++m_timeout_count;
                Debuger.Log("检测到下载超时次数:" + m_timeout_count);
                if (m_timeout_count >= TIMEOUT_COUNT)
                {
                    Debuger.Log("判定超时");
                    this.Stop();
                    if (m_timeout_callback != null) m_timeout_callback();
                }
            }
            lock (m_lock_obj)
            {
                m_is_timeout = true;
            }
        }
    }
}
