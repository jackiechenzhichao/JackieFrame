using UnityEngine;
using System.Collections;

namespace fs
{
    /// <summary>
    /// 定时器
    /// @author hannibal
    /// @time 2016-3-7
    /// </summary>
    public sealed class IntervalTime
    {
        private bool m_active = false;
        private float m_interval_time;
        private float m_now_time;

        public IntervalTime()
        {
        }
        /// <summary>
        /// 初始化定时器
        /// </summary>
        /// <param name="interval">触发间隔</param>
        /// <param name="start">是否第一帧开始执行</param>
        public void Init(float interval, bool first_frame = false)
        {
            m_active = true;
            m_now_time = 0.0f;
            m_interval_time = interval;
            if (first_frame) m_now_time = m_interval_time;
        }

        public void Reset()
        {
            m_now_time = 0.0f;
        }

        public bool Update(float elapse_time)
        {
            if (!m_active) return false;
            m_now_time += elapse_time;
            if (m_now_time >= m_interval_time)
            {
                m_now_time -= m_interval_time;
                return true;
            }
            return false;
        }
    }
    /// <summary>
    /// 每几帧执行一次
    /// </summary>
    public sealed class LogicIntervalTime
    {
        private bool m_active = false;
        private uint m_interval_frame = 0;
        private uint m_now_frame = 0;

        public void Init(uint interval, bool start = false)
        {
            m_active = true;
            m_interval_frame = interval;
            if (start) m_now_frame = m_interval_frame;
        }

        public void Reset()
        {
            m_now_frame = 0;
        }

        public bool Update(uint elapse_frame)
        {
            if (!m_active) return false;
            m_now_frame += elapse_frame;
            if (m_now_frame >= m_interval_frame)
            {
                m_now_frame -= m_interval_frame;
                return true;
            }
            return false;
        }
    }
}